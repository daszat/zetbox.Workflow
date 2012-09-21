using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateActions
    {
        [Invocation]
        public static void ToString(State obj, MethodReturnEventArgs<string> e)
        {
            e.Result = string.Format("{0}, {1} - {2}", obj.StateDefinition, obj.EnteredOn, obj.LeftOn);
        }

        [Invocation]
        public static void get_IsActive(State obj, PropertyGetterEventArgs<bool> e)
        {
            e.Result = obj.LeftOn == null;
        }

        [Invocation]
        public static void postSet_LeftOn(State obj, PropertyPostSetterEventArgs<DateTime?> e)
        {
            obj.Recalculate("IsActive");
            obj.Instance.Recalculate("IsActive");
        }

        [Invocation]
        public static void ScheduleAction(State obj, DateTime invokeOn, Zetbox.Basic.Workflow.ParameterizedActionDefinition action)
        {
            var ctx = obj.Context;
            var entry = ctx.Create<SchedulerEntry>();
            entry.Action = action;
            entry.InvokeOn = invokeOn;
            obj.ScheduledActions.Add(entry);
        }
    }
}
