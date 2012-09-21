
namespace zetbox.Workflow.Client.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;
    using Zetbox.Client.Presentables;
    using Zetbox;
    using Zetbox.API;

    public class Action
    {
        private readonly IViewModelFactory _vmf;
        private readonly IFrozenContext _frozenCtx;

        public Action(IViewModelFactory vmf, IFrozenContext frozenCtx)
        {
            _vmf = vmf;
            _frozenCtx = frozenCtx;
        }

        public bool Forward(wf.Action action, wf.State current, Identity identity)
        {
            var ctx = current.Context;
            var retVal = false;
            var dlg = _vmf.CreateViewModel<DataObjectSelectionTaskViewModel.Factory>()
                .Invoke(
                    ctx,
                    _vmf.GetWorkspace(ctx),
                    (ObjectClass)NamedObjects.Base.Classes.Zetbox.App.Base.Identity.Find(_frozenCtx),
                    null,
                    (lst) =>
                    {
                        foreach (Identity id in lst.Select(i => i.Object))
                        {
                            current.Instance.AddLogEntry("Forwared to " + id.ToString());
                            if (!current.Persons.Contains(id))
                            {
                                current.Persons.Add(id);
                            }
                        }
                        retVal = true;
                    },
                    null);
            _vmf.ShowDialog(dlg);
            return retVal;
        }
    }
}