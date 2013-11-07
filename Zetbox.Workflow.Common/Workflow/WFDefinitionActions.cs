using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;
using System.Text.RegularExpressions;

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
        public static void ObjectIsValid(WFDefinition obj, ObjectIsValidEventArgs e)
        {
            // Check if start states are present
            if (obj.StateDefinitions.Count > 0)
            {
                e.IsValid = obj.StateDefinitions.Any(sd => sd.IsStartState);
                if (!e.IsValid) e.Errors.Add("No start state was defined");
            }
        }

        [Invocation]
        public static void GetName(WFDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? string.Format("Workflow.WFDefinitions.{0}.{1}", obj.Module.Namespace, Regex.Replace(obj.Name, "\\W", "_")) : null;
        }
    }
}
