namespace zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using Zetbox.Client.Presentables.ValueViewModels;
    using Zetbox.Client.Models;

    [ViewModelDescriptor]
    public class InvokeActionObjRefPropertyViewModel : ObjectReferenceViewModel
    {
        public new delegate InvokeActionObjRefPropertyViewModel Factory(IZetboxContext dataCtx, ViewModel parent, IValueModel mdl);

        public InvokeActionObjRefPropertyViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, IValueModel mdl)
            : base(appCtx, dataCtx, parent, mdl)
        {
        }

        protected override List<ViewModel> GetPossibleValues(out bool needMoreButton)
        {
            needMoreButton = false;
            var result = new List<ViewModel>();
            var propMdl = base.ObjectReferenceModel as ObjectReferencePropertyValueModel;
            if (propMdl != null)
            {
                var obj = (Zetbox.Basic.Workflow.ScheduledActionDefinition)propMdl.Object;
                result.AddRange(obj.StateDefinition.Actions.Select(i => DataObjectViewModel.Fetch(ViewModelFactory, DataContext, ViewModelFactory.GetWorkspace(DataContext), i)).Cast<ViewModel>());
            }
            return result;
        }
    }
}
