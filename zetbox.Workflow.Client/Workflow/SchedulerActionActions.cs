
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
    using Zetbox.App.Extensions;

    [Implementor]
    public static class SchedulerActionActions
    {
        [Invocation]
        public static void NotifyCreated(SchedulerAction obj)
        {
            obj.ParameterType = (ObjectClass)NamedObjects.Base.Classes.Zetbox.Basic.Workflow.ScheduledActionDefinition.Find(obj.Context);
            obj.Implementor = typeof(zetbox.Workflow.Common.Workflow.CommonInvocations.Action).ToRef(obj.Context);
            obj.MemberName = "Schedule";
        }
    }
}
