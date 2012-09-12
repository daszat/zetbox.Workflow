using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zetbox.API;
using System.Text.RegularExpressions;

namespace Zetbox.Basic.Workflow
{
    [Implementor]
    public static class ActionActions
    {
        [Invocation]
        public static void ToString(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Name;
        }

        [Invocation]
        public static void GetName(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = obj.Module != null ? string.Format("Workflow.Actions.{0}.{1}", obj.Module.Namespace, Regex.Replace(obj.Name, "\\W", "_")) : null;
        }

        [Invocation]
        public static void Execute(Action obj, Zetbox.Basic.Workflow.State current)
        {
            // Add logfile entry
            // call invocation
        }
    }
}
