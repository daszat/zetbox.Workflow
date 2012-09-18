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
    }
}
