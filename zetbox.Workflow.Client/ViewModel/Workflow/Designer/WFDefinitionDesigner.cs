
namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;

    [ViewModelDescriptor]
    public class WFDefinitionDesigner : ViewModel
    {
        public new delegate WFDefinitionDesigner Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public WFDefinitionDesigner(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.WFDefinition obj)
            : base(appCtx, dataCtx, parent)
        {
            WFDefinition = obj;
        }

        public wf.WFDefinition WFDefinition { get; private set; }

        public override string Name
        {
            get { return WFDefinition.ToString(); }
        }
    }
}
