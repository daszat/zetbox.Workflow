using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateActions
    {
        [Invocation]
        public static void ToString(State obj, MethodReturnEventArgs<string> e)
        {
            e.Result = string.Format("{0}, {1} - {2}", obj.StateDefinition, obj.EnteredOn, obj.LeftOn);
        }
    }
}
