namespace Zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;

    // [ViewModelDescriptor] Used internal
    public class ActionGraphViewModel : ViewModel
    {
        public new delegate ActionGraphViewModel Factory(IZetboxContext dataCtx, ViewModel parent, wf.ParameterizedActionDefinition action);

        public ActionGraphViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.ParameterizedActionDefinition action)
            : base(appCtx, dataCtx, parent)
        {
            Action = action;
        }

        public wf.ParameterizedActionDefinition Action { get; private set; }

        public override string Name
        {
            get { return Action.ToString(); }
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
                        "Edit this action",
                        Edit,
                        null,
                        null);
                }
                return _EditCommand;
            }
        }

        public void Edit()
        {
            ViewModelFactory.ShowModel(DataObjectViewModel.Fetch(ViewModelFactory, DataContext, ViewModelFactory.GetWorkspace(DataContext), Action), true);
        }
    }
}
