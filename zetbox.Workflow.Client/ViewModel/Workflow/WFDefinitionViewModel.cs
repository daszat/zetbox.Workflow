
namespace zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;
    using zetbox.Workflow.Client.ViewModel.Workflow.Designer;

    [ViewModelDescriptor]
    public class WFDefinitionViewModel : DataObjectViewModel
    {
        public new delegate WFDefinitionViewModel Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public WFDefinitionViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.WFDefinition obj)
            : base(appCtx, dataCtx, parent, obj)
        {
            WFDefinition = obj;
        }

        public wf.WFDefinition WFDefinition { get; private set; }

        protected override List<PropertyGroupViewModel> CreatePropertyGroups()
        {
            var result = base.CreatePropertyGroups();

            result.Add(ViewModelFactory.CreateViewModel<SinglePropertyGroupViewModel.Factory>().Invoke(DataContext, this, "Designer", new[] { 
                ViewModelFactory.CreateViewModel<LabeledViewContainerViewModel.Factory>().Invoke(DataContext, this, "Workflow", "", 
                    ViewModelFactory.CreateViewModel<WFDefinitionDesigner.Factory>().Invoke(DataContext, this, WFDefinition))
            }));

            return result;
        }
    }
}
