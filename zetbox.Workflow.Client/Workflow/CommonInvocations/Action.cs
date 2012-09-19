
namespace zetbox.Workflow.Client.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;
    
    public static class Action
    {
        public static bool Forward(wf.Action action, wf.State current, Identity identity)
        {
            var ctx = action.Context;
            var logEntry = ctx.Create<wf.LogEntry>();
            logEntry.Message = "Was forwared...";
            current.Instance.LogEntries.Add(logEntry);

            return true;
        }
    }
}
