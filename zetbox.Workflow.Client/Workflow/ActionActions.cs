
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
    public static class ActionActions
    {
        [Invocation]
        public static void NotifyCreated(Action obj)
        {
            obj.ParameterType = (ObjectClass)NamedObjects.Base.Classes.Zetbox.Basic.Workflow.ParameterizedActionDefinition.Find(obj.Context);
        }
    }
}
