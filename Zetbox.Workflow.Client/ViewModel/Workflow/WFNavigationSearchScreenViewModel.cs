namespace Zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox;
    using Zetbox.API;
    using Zetbox.App.Base;
    using Zetbox.App.GUI;
    using Zetbox.Client.Presentables;
    using Zetbox.Client.Presentables.GUI;
    using wf = Zetbox.Basic.Workflow;

    [ViewModelDescriptor]
    public class WFNavigationSearchScreenViewModel : NavigationSearchScreenViewModel
    {
        public new delegate WFNavigationSearchScreenViewModel Factory(IZetboxContext dataCtx, ViewModel parent, NavigationSearchScreen screen);

        public WFNavigationSearchScreenViewModel(IViewModelDependencies appCtx,
            IZetboxContext dataCtx, ViewModel parent, NavigationScreen screen)
            : base(appCtx, dataCtx, parent, screen)
        {
            base.Type = (ObjectClass)NamedObjects.Base.Classes.Zetbox.Basic.Workflow.State.Find(FrozenContext);
        }

        protected override Func<IQueryable> InitializeQueryFactory()
        {
            return () =>
            {
                if (CurrentIdentity != null)
                {
                    return DataContext.GetQuery<wf.State>();
                    // TODO: Add filter for responsibles
                    // Currently it's not possible to filter over a n:m relation
                }
                else
                {
                    return new wf.State[] { }.AsQueryable();
                }
            };
        }
    }
}
