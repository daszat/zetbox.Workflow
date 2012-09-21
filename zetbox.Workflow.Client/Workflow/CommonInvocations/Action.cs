
namespace zetbox.Workflow.Client.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;
using Zetbox.Client.Presentables;
    
    public class Action
    {
        private readonly IViewModelFactory _vmf;

        public Action(IViewModelFactory vmf)
        {
            _vmf = vmf;
        }

        public bool Forward(wf.Action action, wf.State current, Identity identity)
        {
            _vmf.ShowMessage("Hello from Action", "Hello");
            current.Instance.AddLogEntry("Was forwared to ");
            return true;
        }
    }
}
