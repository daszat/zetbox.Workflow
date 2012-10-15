
namespace zetbox.Workflow.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using System.Threading;
    using Zetbox.API.Utils;
    using Autofac;
    using Zetbox.Basic.Workflow;
    using Zetbox.API.Server;

    public class SchedulerService : IService
    {
        private static readonly double TIMER_DUE_TIME_SECONDS = 10;
        private static readonly double TIMER_PERIOD_MINUTES = 0.01;
        private static readonly int MAX_ITEMS_PER_BATCH = 100;

        private object _lock = new object();
        private bool _isRunning = false;
        private bool _isInShutdown = false;
        private Timer _timer;
        private readonly Autofac.ILifetimeScope _scope;

        public SchedulerService(Autofac.ILifetimeScope scope)
        {
            this._scope = scope;
        }

        public string Description
        {
            get { return "Service for the workflow scheduler."; }
        }

        public string DisplayName
        {
            get { return "Workflow Scheduler Service"; }
        }

        public void Start()
        {
            lock (_lock)
            {
                _isInShutdown = false;
            }
            if (_timer == null)
            {
                _timer = new Timer(new TimerCallback(OnTimer), null, TimeSpan.FromSeconds(TIMER_DUE_TIME_SECONDS), TimeSpan.FromMinutes(TIMER_PERIOD_MINUTES));
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _isInShutdown = true;
            }
            if (_timer != null)
            {
                using (var waiter = new AutoResetEvent(false))
                {
                    _timer.Dispose(waiter);
                    Logging.Log.Info("Waiting for workflow service to stop");
                    if (!waiter.WaitOne(10000))
                    {
                        Logging.Log.Warn("Workflow service took longer than 10s for shutdown.");
                    }
                }
                _timer = null;
            }
        }

        private void OnTimer(object state)
        {
            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
            }

            using (Logging.Server.DebugTraceMethodCall("WorkflowServer.OnTimer"))
            {
                try
                {
                    using (var localScope = _scope.BeginLifetimeScope())
                    {
                        while (true)
                        {
                            using (var queryCtx = localScope.Resolve<IZetboxServerContext>())
                            {
                                var now = DateTime.Now;
                                var nextSchedulerEntries = queryCtx.GetQuery<SchedulerEntry>()
                                        .Where(e => e.InvokeOn <= now)
                                        .Where(e => e.State.IsActive)
                                        .Take(MAX_ITEMS_PER_BATCH)
                                        .ToList();
                                if (nextSchedulerEntries.Count == 0) break;
                                foreach (var entry in nextSchedulerEntries)
                                {
                                    lock (_lock)
                                    {
                                        if (_isInShutdown) return;
                                    }

                                    using (var ctx = localScope.Resolve<IZetboxContext>())
                                    {
                                        try
                                        {
                                            var localEntry = ctx.Find<SchedulerEntry>(entry.ID);
                                            localEntry.Action.Action.Execute(localEntry.Action, localEntry.State);
                                            // Re-Schedule
                                            var scheduledActions = localEntry.State.StateDefinition.Actions
                                                .OfType<ScheduledActionDefinition>()
                                                .Where(ad => ad.IsRecurrent)
                                                .Where(ad => ad.InvokeAction == localEntry.Action)
                                                .ToList();
                                            foreach(var sAction in scheduledActions)
                                            {
                                                sAction.Action.Execute(sAction, localEntry.State);
                                            }
                                            ctx.Delete(localEntry);
                                            ctx.SubmitChanges();
                                        }
                                        catch (ConcurrencyException)
                                        {
                                            // This is one of the rare cases, where the exception may be ignored
                                            // continue and try again.
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Server.Error("Error during workflow scheduler service timer callback", ex);
                }
                finally
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                    }
                }
            }
        }
    }
}
