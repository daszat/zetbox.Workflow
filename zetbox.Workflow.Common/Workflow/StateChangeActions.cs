using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateChangeActions
    {
        [Invocation]
        public static void ToString(StateChange obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void isValid_InvokedByActions(StateChange obj, PropertyIsValidEventArgs e)
        {
            // Check if actions invoke only one state change
            if (obj.StateDefinition != null)
            {
                var allOtherActions = obj.StateDefinition.StateChanges.Except(new[] { obj }).SelectMany(sc => sc.InvokedByActions);
                var actions = allOtherActions.Intersect(obj.InvokedByActions).ToList();
                e.IsValid = actions.Count == 0;
                e.Error = e.IsValid ? string.Empty : "A Action is invoking more than one state change. This state change is one of it. Actions found: " + string.Join(", ", actions.Select(a => a.Name).ToArray());
            }
        }

        [Invocation]
        public static void isValid_NextStates(StateChange obj, PropertyIsValidEventArgs e)
        {
            // Check if next states are member of the same workflow definition
            if (obj.StateDefinition != null)
            {
                var invalidNextStates = obj.NextStates.Where(ns => ns.WFDefinition != obj.StateDefinition.WFDefinition).ToList();
                e.IsValid = invalidNextStates.Count == 0;
                e.Error = e.IsValid ? string.Empty : "At least one next state does not belong to the same workflow. These states are: " + string.Join(", ", invalidNextStates.Select(ns => ns.Name).ToArray());
            }
        }

        [Invocation]
        public static void Execute(StateChange obj, Zetbox.Basic.Workflow.State current)
        {
            var ctx = obj.Context;
            var nextStates = obj.NextStates;
            // call invocation
            // nextStates = obj.ExecuteInvocation(...., nextStates);

            var stateEnds = true;
            if (nextStates == null || nextStates.Count == 0)
            {
                // workflow branch ends here
            }
            else
            {
                // change current state
                foreach (var stateDef in nextStates)
                {
                    if (stateDef == current.StateDefinition)
                    {
                        stateEnds = false;
                    }
                    else
                    {
                        var state = ctx.Create<State>();
                        state.Instance = current.Instance;
                        state.StateDefinition = stateDef;
                    }
                }
            }

            // set on left
            if (stateEnds)
            {
                current.LeftOn = DateTime.Now;
            }
        }
    }
}
