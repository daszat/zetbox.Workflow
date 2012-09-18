
namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;
    using QuickGraph;

    [ViewModelDescriptor]
    public class WFDefinitionDesigner : ViewModel
    {
        public new delegate WFDefinitionDesigner Factory(IZetboxContext dataCtx, ViewModel parent, IDataObject obj);

        public WFDefinitionDesigner(IViewModelDependencies appCtx, IZetboxContext dataCtx, ViewModel parent, wf.WFDefinition obj)
            : base(appCtx, dataCtx, parent)
        {
            WFDefinition = obj;
        }

        public wf.WFDefinition WFDefinition { get; private set; }

        public override string Name
        {
            get { return WFDefinition.ToString(); }
        }

        #region SelectedItems
        private StateDefinitionGraphViewModel _selectedStateDefinition;
        public StateDefinitionGraphViewModel SelectedStateDefinition
        {
            get
            {
                return _selectedStateDefinition;
            }
            set
            {
                if (_selectedStateDefinition != value)
                {
                    _selectedStateDefinition = value;
                    OnPropertyChanged("SelectedStateDefinition");
                    ResetStateDefinitionGraph();
                }
            }
        }
        #endregion

        #region Graphs
        #region DefinitionGraph
        private WFDefinitionGraph _definitionGraph;
        private Dictionary<wf.StateDefinition, StateDefinitionGraphViewModel> _stateViewModels = new Dictionary<wf.StateDefinition, StateDefinitionGraphViewModel>();
        public WFDefinitionGraph DefinitionGraph
        {
            get
            {
                if (_definitionGraph == null)
                {
                    CreateDefinitionGraph();
                }
                return _definitionGraph;
            }
        }
        
        private void CreateDefinitionGraph()
        {
            _definitionGraph = new WFDefinitionGraph(true);
            _stateViewModels = new Dictionary<wf.StateDefinition, StateDefinitionGraphViewModel>();

            foreach (var state in WFDefinition.StateDefinitions)
            {
                var vm = ViewModelFactory.CreateViewModel<StateDefinitionGraphViewModel.Factory>().Invoke(DataContext, this, state);
                vm.IsSelectedChanged += (s, e) =>
                {
                    var sender = (StateDefinitionGraphViewModel)s;
                    if (sender.IsSelected)
                    {
                        _stateViewModels.Values.Except(new[] { sender }).ForEach(i => i.IsSelected = false);
                        SelectedStateDefinition = sender;
                    }
                };
                _stateViewModels[state] = vm;
                _definitionGraph.AddVertex(vm);
            }

            foreach (var change in WFDefinition.StateDefinitions.SelectMany(sd => sd.StateChanges))
            {
                foreach (wf.StateDefinition dest in change.NextStates)
                {
                    _definitionGraph.AddEdge(new TaggedEdge<StateDefinitionGraphViewModel, wf.StateChange>(_stateViewModels[change.StateDefinition], _stateViewModels[dest], change));
                }
            }
        }

        private void ResetDefinitionGraph()
        {
            _definitionGraph = null;
            OnPropertyChanged("DefinitionGraph");
        }
        #endregion


        #region StateDefinitionGraph
        private StateDefinitionGraph _StatedefinitionGraph;
        private Dictionary<wf.Action, ActionGraphViewModel> _actionViewModels = new Dictionary<wf.Action, ActionGraphViewModel>();
        private Dictionary<wf.StateChange, StateChangeGraphViewModel> _stateChangeViewModels = new Dictionary<wf.StateChange, StateChangeGraphViewModel>();
        public StateDefinitionGraph StateDefinitionGraph
        {
            get
            {
                if (_StatedefinitionGraph == null)
                {
                    CreateStateDefinitionGraph();
                }
                return _StatedefinitionGraph;
            }
        }

        private void CreateStateDefinitionGraph()
        {
            if (SelectedStateDefinition == null) return;
            _StatedefinitionGraph = new StateDefinitionGraph(true);
            _actionViewModels = new Dictionary<wf.Action, ActionGraphViewModel>();
            _stateChangeViewModels = new Dictionary<wf.StateChange, StateChangeGraphViewModel>();

            foreach (var action in SelectedStateDefinition.StateDefinition.Actions)
            {
                _StatedefinitionGraph.AddVertex(_actionViewModels[action] = ViewModelFactory.CreateViewModel<ActionGraphViewModel.Factory>().Invoke(DataContext, this, action));
            }

            foreach (var state in WFDefinition.StateDefinitions)
            {
                _StatedefinitionGraph.AddVertex(_stateViewModels[state]);
            }

            foreach (var change in SelectedStateDefinition.StateDefinition.StateChanges)
            {
                var changeVM = _stateChangeViewModels[change] = ViewModelFactory.CreateViewModel<StateChangeGraphViewModel.Factory>().Invoke(DataContext, this, change);
                _StatedefinitionGraph.AddVertex(changeVM);
                foreach (var action in change.InvokedByActions)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(_actionViewModels[action], changeVM, changeVM));
                }

                foreach (wf.StateDefinition dest in change.NextStates)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(changeVM, _stateViewModels[dest], changeVM));
                }
            }
        }
        
        private void ResetStateDefinitionGraph()
        {
            _StatedefinitionGraph = null;
            OnPropertyChanged("StateDefinitionGraph");
        }
        #endregion
        #endregion

        #region Commands
        private ICommandViewModel _NewStateCommand = null;
        public ICommandViewModel NewStateCommand
        {
            get
            {
                if (_NewStateCommand == null)
                {
                    _NewStateCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, "New State", "Creates a new workflow state", NewState, null, null);
                }
                return _NewStateCommand;
            }
        }

        public void NewState()
        {
            var stateDef = DataContext.Create<wf.StateDefinition>();
            WFDefinition.StateDefinitions.Add(stateDef);
            ResetDefinitionGraph();
        }
        #endregion
    }
}
