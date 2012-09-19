namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;

    // [ViewModelDescriptor] Used internal
    public class StateChangeGraphViewModel : ViewModel
    {
        public new delegate StateChangeGraphViewModel Factory(IZetboxContext dataCtx, ViewModel parent, wf.StateChange change);

        public StateChangeGraphViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.StateChange change)
            : base(appCtx, dataCtx, parent)
        {
            StateChange = change;
        }

        public wf.StateChange StateChange { get; private set; }

        public override string Name
        {
            get { return StateChange.ToString(); }
        }

        public override string ToString()
        {
            return Name;
        }

        public event EventHandler IsSelectedChanged;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnIsSelectedChanged();
                }
            }
        }

        protected void OnIsSelectedChanged()
        {
            var temp = IsSelectedChanged;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
            OnPropertyChanged("IsSelected");
        }

        private ICommandViewModel _EditCommand = null;
        public ICommandViewModel EditCommand
        {
            get
            {
                if (_EditCommand == null)
                {
                    _EditCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        "Edit",
                        "Edit this state change",
                        Edit,
                        null,
                        null);
                }
                return _EditCommand;
            }
        }

        public void Edit()
        {
            ViewModelFactory.ShowModel(DataObjectViewModel.Fetch(ViewModelFactory, DataContext, ViewModelFactory.GetWorkspace(DataContext), StateChange), true);
        }
    }
}
