using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class WFDefinitionActions
    {
        [Invocation]
        public static void ToString(WFDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void GetName(WFDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? string.Format("Workflow.WFDefinitions.{0}.{1}", obj.Module.Namespace, obj.Name) : null;
        }
    }
}
