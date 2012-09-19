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
    public class ActionGraphViewModel : ViewModel
    {
        public new delegate ActionGraphViewModel Factory(IZetboxContext dataCtx, ViewModel parent, wf.Action action);

        public ActionGraphViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.Action action)
            : base(appCtx, dataCtx, parent)
        {
            Action = action;
        }

        public wf.Action Action { get; private set; }

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
    }
}
