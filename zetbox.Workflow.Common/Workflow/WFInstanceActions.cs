
namespace Zetbox.Basic.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using Zetbox.API.Common;

    [Implementor]
    public static class WFInstanceActions
    {
        private static IIdentityResolver _idResolver;
        public WFInstanceActions(IIdentityResolver idResolver)
        {
            _idResolver = idResolver;
        }

        [Invocation]
        public static void ToString(WFInstance obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Summary;
        }

        [Invocation]
        public static void get_IsActive(WFInstance obj, PropertyGetterEventArgs<bool> e)
        {
            e.Result = obj.Workflow != null && obj.States.Any(s => s.IsActive);
        }

        [Invocation]
        public static void preSet_Workflow(WFInstance obj, PropertyPreSetterEventArgs<Zetbox.Basic.Workflow.WFDefinition> e)
        {
            if (e.OldValue == null) return; // OK
            if (e.OldValue != e.NewValue) throw new NotSupportedException("Changing the workflow is not supported");
        }

        [Invocation]
        public static void Start(WFInstance obj, Zetbox.Basic.Workflow.WFDefinition workflow)
        {
            if (workflow != null)
            {
                var ctx = obj.Context;
                obj.Workflow = workflow;
                foreach (var stateDef in workflow.StateDefinitions.Where(s => s.IsStartState))
                {
                    var state = ctx.Create<State>();
                    state.Instance = obj;
                    state.StateDefinition = stateDef;
                }
                obj.Recalculate("IsActive");
            }
        }

        [Invocation]
        public static void StartCanExec(WFInstance obj, MethodReturnEventArgs<bool> e)
        {
            e.Result = obj.Workflow == null;
        }

        [Invocation]
        public static void StartCanExecReason(WFInstance obj, MethodReturnEventArgs<string> e)
        {
            e.Result = "A workflow was started already";
        }

        [Invocation]
        public static void Abort(WFInstance obj)
        {
        }

        [Invocation]
        public static void AddLogEntry(WFInstance obj, string formatString)
        {
            if (!string.IsNullOrEmpty(formatString))
            {
                var identity = _idResolver.GetCurrent();
                var ctx = obj.Context;

                var msg = formatString
                    .Replace("{User}", (identity ?? (object)string.Empty).ToString())
                    .Replace("{Date}", DateTime.Today.ToShortDateString())
                    .Replace("{Time}", DateTime.Now.ToShortTimeString());
                var logEntry = ctx.Create<LogEntry>();
                logEntry.Message = msg;
                logEntry.Identity = identity;
                obj.LogEntries.Add(logEntry);
            }
        }
    }
}
