
namespace Zetbox.Basic.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Zetbox.API;
    using Zetbox.API.Common;
    using Zetbox.App.Base;


    /// <summary>
    /// An action invocation prototype. Return false to skip a state change and log entry.
    /// </summary>
    /// <param name="action">the calling action</param>
    /// <param name="parameter">the action's definition</param>
    /// <param name="current">the current workflow state</param>
    /// <param name="identity">the current identity or null, if the identity cannot be resolved</param>
    /// <returns>False to skip a state change and log entry.</returns>
    public delegate bool ActionInvocationPrototype(Action action, ParameterizedActionDefinition parameter, State current, Identity identity);

    [Implementor]
    public class ActionActions
    {
        private static IIdentityResolver _idResolver;
        private static IInvocationExecutor _invocationExec;
        public ActionActions(IIdentityResolver idResolver, IInvocationExecutor invocationExec)
        {
            _idResolver = idResolver;
            _invocationExec = invocationExec;
        }

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
        public static void GetLabel(Action obj, MethodReturnEventArgs<string> e)
        {
            e.Result = string.IsNullOrEmpty(obj.Label) ? obj.Name : obj.Label;
        }

        [Invocation]
        public static void Execute(Action obj, ParameterizedActionDefinition paramedAction, State current)
        {
            if (current == null) throw new ArgumentException("current");
            var identity = _idResolver.GetCurrent();

            // call invocation
            if (_invocationExec.HasValidInvocation(obj))
            {
                var result = _invocationExec.CallInvocation<bool>(obj, typeof(ActionInvocationPrototype), obj, paramedAction, current, identity);
                if (result == false)
                {
                    return;
                }
            }

            // Add logfile entry
            current.Instance.AddLogEntry(obj.LogMessageFormat);

            // find and execute state change
            var stateChangeList = current.StateDefinition.StateChanges.Where(sc => sc.InvokedByActions.Contains(paramedAction)).ToList();
            if (stateChangeList.Count == 0)
            {
                // Action does not invoke a state change
            }
            else if (stateChangeList.Count > 1)
            {
                // this is an error
                throw new NotSupportedException(string.Format("Action {0} invokes more than one state change logic. This is not supported", obj));
            }
            else
            {
                var stateChange = stateChangeList.Single();
                stateChange.Execute(current);
            }
        }
    }
}
