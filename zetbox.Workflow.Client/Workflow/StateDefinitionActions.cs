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
        public static void NotifyDeleting(StateDefinition obj)
        {
            var ctx = obj.Context;
            if (obj.WFDefinition != null)
            {
                foreach (var stateChange in obj.WFDefinition.StateDefinitions.SelectMany(sd => sd.StateChanges))
                {
                    stateChange.NextStates.Remove(obj);
                }
            }
            obj.WFDefinition = null;
            obj.Actions.ToList().ForEach(a => ctx.Delete(a));
            obj.StateChanges.ToList().ForEach(s => ctx.Delete(s));            
        }
    }
}
