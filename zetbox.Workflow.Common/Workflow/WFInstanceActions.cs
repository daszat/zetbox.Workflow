using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class WFInstanceActions
    {
        [Invocation]
        public static void ToString(WFInstance obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Summary;
        }

        [Invocation]
        public static void postSet_Workflow(WFInstance obj, PropertyPostSetterEventArgs<Zetbox.Basic.Workflow.WFDefinition> e)
        {
            // Setup initial states
        }
    }
}
