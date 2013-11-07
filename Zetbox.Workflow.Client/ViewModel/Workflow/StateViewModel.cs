namespace Zetbox.Workflow.Client.ViewModel.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox;

    [ViewModelDescriptor]
    public class StateViewModel : DataObjectViewModel
    {
        public new delegate StateViewModel Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public StateViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.State obj)
            : base(appCtx, dataCtx, parent, obj)
        {
            State = obj;
            dataCtx.IsElevatedModeChanged += new EventHandler(dataCtx_IsElevatedModeChanged);
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

        void dataCtx_IsElevatedModeChanged(object sender, EventArgs e)
        {
            base.commandsStore = null;
            OnPropertyChanged("Commands");
        }

        public override string Name
        {
            get { return State.ToString(); }
        }

        protected override System.Collections.ObjectModel.ObservableCollection<ICommandViewModel> CreateCommands()
        {
            var commands = base.CreateCommands();
            var isElevated = DataContext.IsElevatedMode;
            if (State.IsActive || isElevated)
            {
                foreach (var action in State.StateDefinition.Actions.Where(a => a.IsVisible))
                {
                    // avoid capturing the loop variable
                    var localAction = action;
                    var cmd = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        action.Action.GetLabel(),
                        action.Action.Description,
                        () => ExecuteAction(localAction),
                        null,
                        null);
                    // cmd.Icon = IconConverter.ToImage(action.Icon);
                    commands.Add(cmd);
                }

                if (isElevated)
                {
                    foreach (var action in State.StateDefinition.Actions.Where(a => a.IsVisible == false))
                    {
                        // avoid capturing the loop variable
                        var localAction = action;
                        var cmd = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                            action.Action.GetLabel(),
                            action.Action.Description,
                            () => ExecuteAction(localAction),
                            null,
                            null);
                        cmd.Icon = IconConverter.ToImage(NamedObjects.Gui.Icons.ZetboxBase.warning_png.Find(FrozenContext));
                        commands.Add(cmd);
                    }
                }
            }
            return commands;
        }

        public void ExecuteAction(wf.ParameterizedActionDefinition action)
        {
            action.Action.Execute(action, State);
        }
    }
}
