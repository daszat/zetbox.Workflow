namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;

    [ViewModelDescriptor]
    public class HiddenGraphElementViewModel : ViewModel
    {
        public new delegate HiddenGraphElementViewModel Factory(IZetboxContext dataCtx, ViewModel parent);

        public HiddenGraphElementViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent)
            : base(appCtx, dataCtx, parent)
        {
        }

        public override string Name
        {
            get { return "HiddenGraphElementViewModel"; }
        }
    }
}
