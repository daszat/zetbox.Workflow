
namespace Zetbox.Basic.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using Zetbox.API.Common;
    using Zetbox.App.Base;

    [Implementor]
    public class StateChangeDefinitionActions
    {
        [Invocation]
        public static void NotifyDeleting(StateChangeDefinition obj)
        {
            obj.InvokedByActions.Clear();
            obj.NextStates.Clear();
            obj.StateDefinition = null;
        }
    }
}
