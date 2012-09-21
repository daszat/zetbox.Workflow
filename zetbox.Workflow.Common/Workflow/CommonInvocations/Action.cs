
namespace zetbox.Workflow.Common.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;

    public static class Action
    {
        public static bool Schedule(wf.Action action, wf.ParameterizedActionDefinition parameter, wf.State current, Identity identity)
        {
            var ctx = current.Context;
            var sAction = (wf.SchedulerAction)action;
            var sParameter = (wf.ScheduledActionDefinition)parameter;
            return true;
        }
    }
}
