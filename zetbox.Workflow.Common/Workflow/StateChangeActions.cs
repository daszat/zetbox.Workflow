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
