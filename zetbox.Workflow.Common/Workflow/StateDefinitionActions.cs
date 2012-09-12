using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;
using System.Text.RegularExpressions;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateDefinitionActions
    {
        [Invocation]
        public static void ToString(StateDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void GetName(StateDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? string.Format("Workflow.StateDefinitions.{0}.{1}", obj.Module.Namespace, Regex.Replace(obj.Name, "\\W", "_")) : null;
        }
    }
}
