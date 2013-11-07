using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;
using Zetbox.App.Base;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateActions
    {
        internal static State CreateState(WFInstance instance, StateDefinition def)
        {
            var ctx = instance.Context;
            var state = ctx.Create<State>();
            state.Instance = instance;
            state.StateDefinition = def;
            foreach (var action in def.Actions.Where(a => a.IsOnEnterAction))
            {
                action.Action.Execute(action, state);
            }
            return state;
        }

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

            // Avoid multiple entries for same action on same state
            var entry = ctx.GetQuery<SchedulerEntry>().FirstOrDefault(e => e.State == obj && e.Action == action);
            if (entry == null)
            {
                entry = ctx.Create<SchedulerEntry>();
                entry.Action = action;
                obj.ScheduledActions.Add(entry);
            }
            entry.InvokeOn = invokeOn;
        }
    }
}
