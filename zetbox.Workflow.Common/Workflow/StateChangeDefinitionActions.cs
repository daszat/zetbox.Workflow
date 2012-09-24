
namespace Zetbox.Basic.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using Zetbox.API.Common;
    using Zetbox.App.Base;

    /// <summary>
    /// An state change invocation prototype. If nothing changes, return StateChange.NextStates. If the state should end, return null or empty. If specific states should be entered, return them.
    /// </summary>
    /// <param name="stateChange">the calling state change</param>
    /// <param name="current">the current workflow state</param>
    /// <param name="identity">the current identity or null, if the identity cannot be resolved</param>
    /// <returns>A list of next states.</returns>
    public delegate List<StateDefinition> StateChangeInvocationPrototype(StateChangeDefinition stateChange, State current, Identity identity);

    [Implementor]
    public class StateChangeDefinitionActions
    {
        private static IIdentityResolver _idResolver;
        private static IInvocationExecutor _invocationExec;
        public StateChangeDefinitionActions(IIdentityResolver idResolver, IInvocationExecutor invocationExec)
        {
            _idResolver = idResolver;
            _invocationExec = invocationExec;
        }

        [Invocation]
        public static void ToString(StateChangeDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void isValid_InvokedByActions(StateChangeDefinition obj, PropertyIsValidEventArgs e)
        {
            // Check if actions invoke only one state change
            if (obj.StateDefinition != null)
            {
                var allOtherActions = obj.StateDefinition.StateChanges.Except(new[] { obj }).SelectMany(sc => sc.InvokedByActions);
                var actions = allOtherActions.Intersect(obj.InvokedByActions).ToList();
                e.IsValid = actions.Count == 0;
                e.Error = e.IsValid ? string.Empty : "A Action is invoking more than one state change. This state change is one of it. Actions found: " + string.Join(", ", actions.Select(a => a.Action.Name).ToArray());
            }
        }

        [Invocation]
        public static void isValid_NextStates(StateChangeDefinition obj, PropertyIsValidEventArgs e)
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
        public static void Execute(StateChangeDefinition obj, Zetbox.Basic.Workflow.State current)
        {
            var ctx = obj.Context;
            var identity = _idResolver.GetCurrent();

            var nextStates = obj.NextStates.ToList();
            // call invocation
            if (_invocationExec.HasValidInvocation(obj))
            {
                nextStates = _invocationExec.CallInvocation<List<StateDefinition>>(obj, typeof(StateChangeInvocationPrototype), obj, current, identity);
            }

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
                        StateActions.CreateState(current.Instance, stateDef);
                    }
                }
            }

            // set on left
            if (stateEnds)
            {
                current.LeftOn = DateTime.Now;
                current.Persons.Clear();
                current.Groups.Clear();
                current.ScheduledActions.Clear();
            }
        }
    }
}
