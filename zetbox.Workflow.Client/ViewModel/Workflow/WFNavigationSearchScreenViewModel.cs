namespace zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.API;
    using Zetbox.Client.Presentables;
    using Zetbox.Client.Presentables.GUI;
    using Zetbox.App.GUI;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox;
    using Zetbox.App.Base;
    
    [ViewModelDescriptor]
    public class WFNavigationSearchScreenViewModel : NavigationSearchScreenViewModel
    {
        public new delegate WFNavigationSearchScreenViewModel Factory(IZetboxContext dataCtx, ViewModel parent, NavigationSearchScreen screen);

        private readonly Func<IZetboxContext> _ctxFactory;

        public WFNavigationSearchScreenViewModel(IViewModelDependencies appCtx, Func<IZetboxContext> ctxFactory,
            IZetboxContext dataCtx, ViewModel parent, NavigationScreen screen)
            : base(appCtx, dataCtx, ctxFactory, parent, screen)
        {
            _ctxFactory = ctxFactory;
            base.Type = (ObjectClass)NamedObjects.Base.Classes.Zetbox.Basic.Workflow.State.Find(FrozenContext);
        }

        protected override Func<IQueryable> InitializeQueryFactory()
        {
            return () =>
            {
                if (CurrentIdentity != null)
                {
                    int id = CurrentIdentity.ID;
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
