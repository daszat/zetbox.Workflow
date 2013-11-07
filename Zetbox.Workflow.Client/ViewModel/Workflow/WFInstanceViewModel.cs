namespace Zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;

    [ViewModelDescriptor]
    public class WFInstanceViewModel : DataObjectViewModel
    {
        public new delegate WFInstanceViewModel Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public WFInstanceViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.WFInstance obj)
            : base(appCtx, dataCtx, parent, obj)
        {
            Instance = obj;
        }

        public wf.WFInstance Instance { get; private set; }

        public override string Name
        {
            get { return Instance.ToString(); }
        }
    }
}
