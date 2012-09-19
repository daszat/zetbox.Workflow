
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
            current.Instance.AddLogEntry("Was forwared...");
            return true;
        }
    }
}
