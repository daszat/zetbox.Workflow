using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;
using System.Text.RegularExpressions;
using Zetbox.API.Common;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public class ActionActions
    {
        private static IIdentityResolver _idResolver;
        public ActionActions(IIdentityResolver idResolver)
        {
            _idResolver = idResolver;
        }

        [Invocation]
        public static void ToString(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void GetName(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? string.Format("Workflow.Actions.{0}.{1}", obj.Module.Namespace, Regex.Replace(obj.Name, "\\W", "_")) : null;
        }

        [Invocation]
        public static void GetLabel(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = string.IsNullOrEmpty(obj.Label) ? obj.Name : obj.Label;
        }

        [Invocation]
        public static void Execute(Action obj, Zetbox.Basic.Workflow.State current)
        {
            if(current == null) throw new ArgumentException("current");
            var ctx = obj.Context;

            // Add logfile entry
            if (!string.IsNullOrEmpty(obj.LogMessageFormat))
            {
                var msg = obj.LogMessageFormat
                    .Replace("{User}", (string)(_idResolver.GetCurrent() ?? (object)string.Empty))
                    .Replace("{Date}", DateTime.Today.ToShortDateString())
                    .Replace("{Time}", DateTime.Now.ToShortTimeString());
                var logEntry = ctx.Create<LogEntry>();
                logEntry.Message = msg;
                current.Instance.LogEntries.Add(logEntry);
            }

            // call invocation
            // find and execute state change
            var stateChangeList = current.StateDefinition.StateChanges.Where(sc => sc.InvokedByActions.Contains(obj)).ToList();
            if (stateChangeList.Count == 0)
            {
                // Action does not invoke a state change
            }
            else if (stateChangeList.Count > 1)
            {
                // this is an error
                throw new NotSupportedException(string.Format("Action {0} invokes more than one state change logic. This is not supported", obj));
            }
            else
            {
                var stateChange = stateChangeList.Single();
                stateChange.Execute(current);
            }
        }
    }
}
