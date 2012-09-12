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
        public static void preSet_Workflow(WFInstance obj, PropertyPreSetterEventArgs<Zetbox.Basic.Workflow.WFDefinition> e)
        {
            // disallow changes
        }

        [Invocation]
        public static void Execute(StateChange obj, Zetbox.Basic.Workflow.State current)
        {
            // set on left
            // call invocation
            // change current state
        }
    }
}
