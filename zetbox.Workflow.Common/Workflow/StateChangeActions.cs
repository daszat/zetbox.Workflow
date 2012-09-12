using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class StateChangeActions
    {
        [Invocation]
        public static void ToString(StateChange obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }
    }
}
