
namespace Zetbox.Basic.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using System.Text.RegularExpressions;
    using Zetbox.API.Common;
    using Zetbox.App.Base;


    [Implementor]
    public class ParameterizedActionDefinitionActions
    {
        public ParameterizedActionDefinitionActions(IIdentityResolver idResolver, IInvocationExecutor invocationExec)
        {
        }

        [Invocation]
        public static void ToString(ParameterizedActionDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Action != null ? obj.Action.Name : "<missing action>";
        }

        [Invocation]
        public static void GetName(ParameterizedActionDefinition obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? 
                string.Format("Workflow.ParameterizedActionDefinition.{0}.{1}.{2}.{3}",
                    obj.StateDefinition.WFDefinition.Module.Namespace,
                    Regex.Replace(obj.StateDefinition.WFDefinition.Name, "\\W", "_"),
                    Regex.Replace(obj.StateDefinition.Name, "\\W", "_"),
                    Regex.Replace(obj.Action.Name, "\\W", "_")) : null;
        }
    }
}
