
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
        [Invocation]
        public static void NotifyDeleting(ParameterizedActionDefinition obj)
        {
            if (obj.StateDefinition != null)
            {
                obj.StateDefinition.StateChanges.ToList().ForEach(sc => sc.InvokedByActions.Remove(obj));
                foreach (var sa in obj.StateDefinition.Actions.OfType<ScheduledActionDefinition>().ToList())
                {
                    if (sa.InvokeAction == obj) 
                        sa.InvokeAction = null;
                }
            }
            obj.Action = null;
            obj.StateDefinition = null;            
        }
    }
}
