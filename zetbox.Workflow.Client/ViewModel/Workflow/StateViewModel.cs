namespace zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;

    [ViewModelDescriptor]
    public class StateViewModel : DataObjectViewModel
    {
        public new delegate StateViewModel Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public StateViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.State obj)
            : base(appCtx, dataCtx, parent, obj)
        {
            State = obj;
        }

        public wf.State State { get; private set; }
        public DataObjectViewModel InstanceViewModel
        {
            get
            {
                return DataObjectViewModel.Fetch(ViewModelFactory, DataContext, this, State.Instance);
            }
        }

        protected override void OnObjectPropertyChanged(string propName)
        {
            base.OnObjectPropertyChanged(propName);

            switch (propName)
            {
                case "IsActive":
                    base.commandsStore = null;
                    OnPropertyChanged("Commands");
                    break;
            }
        }

        public override string Name
        {
            get { return State.ToString(); }
        }

        protected override System.Collections.ObjectModel.ObservableCollection<ICommandViewModel> CreateCommands()
        {
            var commands = base.CreateCommands();
            if (State.IsActive)
            {
                foreach (var action in State.StateDefinition.Actions)
                {
                    // avoid capturing the loop variable
                    var localAction = action;
                    commands.Add(ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        action.GetLabel(),
                        action.Description,
                        () => ExecuteAction(localAction),
                        null,
                        null));
                }
            }
            return commands;
        }

        public void ExecuteAction(wf.Action action)
        {
            action.Execute(State);
        }
    }
}
