
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
    public class ScheduledActionDefinitionActions
    {
        [Invocation]
        public static void NotifyDeleting(ScheduledActionDefinition obj)
        {
            obj.InvokeAction = null;
        }
    }
}
