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
    public class StateDefinitionGraphViewModel : ViewModel
    {
        public new delegate StateDefinitionGraphViewModel Factory(IZetboxContext dataCtx, ViewModel parent, wf.StateDefinition stateDef);

        public StateDefinitionGraphViewModel(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.StateDefinition stateDef)
            : base(appCtx, dataCtx, parent)
        {
            StateDefinition = stateDef;
        }

        public wf.StateDefinition StateDefinition { get; private set; }

        public override string Name
        {
            get { return StateDefinition.ToString(); }
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
