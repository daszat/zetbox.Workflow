
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
    using Zetbox;
    using Zetbox.App.Base;

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
                    if (_selectedStateDefinition != null)
                    {
                        _selectedStateDefinition.IsSelected = true;
                    }
                    OnPropertyChanged("SelectedStateDefinition");
                    ResetStateDefinitionGraph();
                }
            }
        }

        private StateDefinitionGraphViewModel _selectedDestinationStateDefinition;
        public StateDefinitionGraphViewModel SelectedDestinationStateDefinition
        {
            get
            {
                return _selectedDestinationStateDefinition;
            }
            set
            {
                if (_selectedDestinationStateDefinition != value)
                {
                    _selectedDestinationStateDefinition = value;
                    if (_selectedDestinationStateDefinition != null)
                    {
                        _selectedDestinationStateDefinition.IsSelectedDestination = true;
                    }
                    OnPropertyChanged("SelectedDestinationStateDefinition");
                }
            }
        }

        private StateChangeGraphViewModel _selectedStateChange;
        public StateChangeGraphViewModel SelectedStateChange
        {
            get
            {
                return _selectedStateChange;
            }
            set
            {
                if (_selectedStateChange != value)
                {
                    _selectedStateChange = value;
                    if (_selectedStateChange != null)
                    {
                        _selectedStateChange.IsSelected = true;
                    }
                    OnPropertyChanged("SelectedStateChange");
                }
            }
        }

        private ActionGraphViewModel _selectedAction;
        public ActionGraphViewModel SelectedAction
        {
            get
            {
                return _selectedAction;
            }
            set
            {
                if (_selectedAction != value)
                {
                    _selectedAction = value;
                    if (_selectedAction != null)
                    {
                        _selectedAction.IsSelected = true;
                    }
                    OnPropertyChanged("SelectedAction");
                }
            }
        }
        #endregion

        #region ViewModelsCache
        private Dictionary<wf.StateDefinition, StateDefinitionGraphViewModel> _stateViewModels = new Dictionary<wf.StateDefinition, StateDefinitionGraphViewModel>();
        private Dictionary<wf.Action, ActionGraphViewModel> _actionViewModels = new Dictionary<wf.Action, ActionGraphViewModel>();
        private Dictionary<wf.StateChange, StateChangeGraphViewModel> _stateChangeViewModels = new Dictionary<wf.StateChange, StateChangeGraphViewModel>();

        private StateDefinitionGraphViewModel ToStateDefinitionViewModel(wf.StateDefinition state)
        {
            if (!_stateViewModels.ContainsKey(state))
            {
                var vm = _stateViewModels[state] = ViewModelFactory.CreateViewModel<StateDefinitionGraphViewModel.Factory>().Invoke(DataContext, this, state);
                vm.IsSelectedChanged += (s, e) =>
                {
                    var sender = (StateDefinitionGraphViewModel)s;
                    if (sender.IsSelected)
                    {
                        _stateViewModels.Values.Except(new[] { sender }).ForEach(i => i.IsSelected = false);
                        SelectedStateDefinition = sender;
                    }
                    else if (SelectedStateDefinition == sender)
                    {
                        SelectedStateDefinition = null;
                    }
                };
                vm.IsSelectedDestinationChanged += (s, e) =>
                {
                    var sender = (StateDefinitionGraphViewModel)s;
                    if (sender.IsSelectedDestination)
                    {
                        _stateViewModels.Values.Except(new[] { sender }).ForEach(i => i.IsSelectedDestination = false);
                        SelectedDestinationStateDefinition = sender;
                    }
                    else if (SelectedDestinationStateDefinition == sender)
                    {
                        SelectedDestinationStateDefinition = null;
                    }
                };
            }
            return _stateViewModels[state];
        }

        private StateChangeGraphViewModel ToStateChangeViewModel(wf.StateChange change)
        {
            if (!_stateChangeViewModels.ContainsKey(change))
            {
                var vm = _stateChangeViewModels[change] = ViewModelFactory.CreateViewModel<StateChangeGraphViewModel.Factory>().Invoke(DataContext, this, change);
                vm.IsSelectedChanged += (s, e) =>
                {
                    var sender = (StateChangeGraphViewModel)s;
                    if (sender.IsSelected)
                    {
                        _stateChangeViewModels.Values.Except(new[] { sender }).ForEach(i => i.IsSelected = false);
                        SelectedStateChange = sender;
                    }
                    else if (SelectedStateChange == sender)
                    {
                        SelectedStateChange = null;
                    }
                };
            }
            return _stateChangeViewModels[change];
        }

        private ActionGraphViewModel ToActionViewModel(wf.Action action)
        {
            if (!_actionViewModels.ContainsKey(action))
            {
                var vm = _actionViewModels[action] = ViewModelFactory.CreateViewModel<ActionGraphViewModel.Factory>().Invoke(DataContext, this, action);
                vm.IsSelectedChanged += (s, e) =>
                {
                    var sender = (ActionGraphViewModel)s;
                    if (sender.IsSelected)
                    {
                        _actionViewModels.Values.Except(new[] { sender }).ForEach(i => i.IsSelected = false);
                        SelectedAction = sender;
                    }
                    else if (SelectedAction == sender)
                    {
                        SelectedAction = null;
                    }
                };
            }
            return _actionViewModels[action];
        }
        #endregion

        #region Graphs
        #region DefinitionGraph
        private WFDefinitionGraph _definitionGraph;
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

            foreach (var state in WFDefinition.StateDefinitions)
            {
                _definitionGraph.AddVertex(ToStateDefinitionViewModel(state));
            }

            foreach (var change in WFDefinition.StateDefinitions.SelectMany(sd => sd.StateChanges))
            {
                foreach (wf.StateDefinition dest in change.NextStates)
                {
                    _definitionGraph.AddEdge(new TaggedEdge<StateDefinitionGraphViewModel, StateChangeGraphViewModel>(ToStateDefinitionViewModel(change.StateDefinition), ToStateDefinitionViewModel(dest), ToStateChangeViewModel(change)));
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

            foreach (var action in SelectedStateDefinition.StateDefinition.Actions)
            {
                _StatedefinitionGraph.AddVertex(ToActionViewModel(action));
            }

            foreach (var state in WFDefinition.StateDefinitions)
            {
                _StatedefinitionGraph.AddVertex(ToStateDefinitionViewModel(state));
            }

            foreach (var change in SelectedStateDefinition.StateDefinition.StateChanges)
            {
                _StatedefinitionGraph.AddVertex(ToStateChangeViewModel(change));
                foreach (var action in change.InvokedByActions)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(ToActionViewModel(action), ToStateChangeViewModel(change), ToStateChangeViewModel(change)));
                }

                foreach (wf.StateDefinition dest in change.NextStates)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(ToStateChangeViewModel(change), ToStateDefinitionViewModel(dest), ToStateChangeViewModel(change)));
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
            SelectedStateDefinition = ToStateDefinitionViewModel(stateDef);
            ResetDefinitionGraph();
        }

        private ICommandViewModel _LinkActionChangeCommand = null;
        public ICommandViewModel LinkActionChangeCommand
        {
            get
            {
                if (_LinkActionChangeCommand == null)
                {
                    _LinkActionChangeCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        "Link action & change",
                        "Links an Action and a State Change Logic",
                        LinkActionChange,
                        () => SelectedAction != null && SelectedStateChange != null && !SelectedStateChange.StateChange.InvokedByActions.Contains(SelectedAction.Action),
                        null);
                }
                return _LinkActionChangeCommand;
            }
        }

        public void LinkActionChange()
        {
            if (SelectedAction != null && SelectedStateChange != null)
            {
                if (!SelectedStateChange.StateChange.InvokedByActions.Contains(SelectedAction.Action))
                {
                    SelectedStateChange.StateChange.InvokedByActions.Add(SelectedAction.Action);
                    ResetStateDefinitionGraph();
                }
            }
        }

        private ICommandViewModel _LinkChangeStateCommand = null;
        public ICommandViewModel LinkChangeStateCommand
        {
            get
            {
                if (_LinkChangeStateCommand == null)
                {
                    _LinkChangeStateCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, 
                        "Link change & state",
                        "Links a state change logic and a destination state", 
                        LinkStateChange,
                        () => SelectedStateChange != null && SelectedDestinationStateDefinition != null && !SelectedStateChange.StateChange.NextStates.Contains(SelectedDestinationStateDefinition.StateDefinition), 
                        null);
                }
                return _LinkChangeStateCommand;
            }
        }

        public void LinkStateChange()
        {
            if (SelectedStateChange != null && SelectedDestinationStateDefinition != null)
            {
                if (!SelectedStateChange.StateChange.NextStates.Contains(SelectedDestinationStateDefinition.StateDefinition))
                {
                    SelectedStateChange.StateChange.NextStates.Add(SelectedDestinationStateDefinition.StateDefinition);
                    ResetStateDefinitionGraph();
                    ResetDefinitionGraph();
                }
            }
        }

        private ICommandViewModel _AddActionCommand = null;
        public ICommandViewModel AddActionCommand
        {
            get
            {
                if (_AddActionCommand == null)
                {
                    _AddActionCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, 
                        "Add an Action",
                        "Adds a existing Action to this state", 
                        AddAction, 
                        () => SelectedStateDefinition != null, 
                        null);
                }
                return _AddActionCommand;
            }
        }

        public void AddAction()
        {
            if (SelectedStateDefinition != null)
            {
                var dlg = ViewModelFactory.CreateViewModel<DataObjectSelectionTaskViewModel.Factory>().Invoke(DataContext, this,
                    (ObjectClass)NamedObjects.Base.Classes.Zetbox.Basic.Workflow.Action.Find(FrozenContext),
                    null,
                    (lst) =>
                    {
                        foreach (var action in lst.Select(i => i.Object).Cast<wf.Action>())
                        {
                            SelectedStateDefinition.StateDefinition.Actions.Add(action);
                        }
                        if(lst.Count() > 0)
                        {
                            SelectedAction = ToActionViewModel((wf.Action)lst.First().Object);
                        }
                        ResetStateDefinitionGraph();
                    },
                    null);
                ViewModelFactory.ShowDialog(dlg);
            }
        }

        private ICommandViewModel _NewStateChangeCommand = null;
        public ICommandViewModel NewStateChangeCommand
        {
            get
            {
                if (_NewStateChangeCommand == null)
                {
                    _NewStateChangeCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, "New state change", "Creates a new state change logic", 
                        NewStateChange, 
                        () => SelectedStateDefinition != null, 
                        null);
                }
                return _NewStateChangeCommand;
            }
        }

        public void NewStateChange()
        {
            if (SelectedStateDefinition != null)
            {
                var change = DataContext.Create<wf.StateChange>();
                SelectedStateDefinition.StateDefinition.StateChanges.Add(change);
                SelectedStateChange = ToStateChangeViewModel(change);
                ResetStateDefinitionGraph();
            }
        }

        private ICommandViewModel _UnlinkActionChangeCommand = null;
        public ICommandViewModel UnlinkActionChangeCommand
        {
            get
            {
                if (_UnlinkActionChangeCommand == null)
                {
                    _UnlinkActionChangeCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        "Unlink action & change",
                        "Unlinks an Action and a State Change Logic",
                        UnlinkActionChange,
                        () => SelectedAction != null && SelectedStateChange != null && SelectedStateChange.StateChange.InvokedByActions.Contains(SelectedAction.Action),
                        null);
                }
                return _UnlinkActionChangeCommand;
            }
        }

        public void UnlinkActionChange()
        {
            if (SelectedAction != null && SelectedStateChange != null)
            {
                if (SelectedStateChange.StateChange.InvokedByActions.Contains(SelectedAction.Action))
                {
                    SelectedStateChange.StateChange.InvokedByActions.Remove(SelectedAction.Action);
                    ResetStateDefinitionGraph();
                }
            }
        }

        private ICommandViewModel _UnlinkChangeStateCommand = null;
        public ICommandViewModel UnlinkChangeStateCommand
        {
            get
            {
                if (_UnlinkChangeStateCommand == null)
                {
                    _UnlinkChangeStateCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this,
                        "Unlink change & state",
                        "Unlinks a state change logic and a destination state", 
                        UnlinkChangeState,
                        () => SelectedStateChange != null && SelectedDestinationStateDefinition != null && SelectedStateChange.StateChange.NextStates.Contains(SelectedDestinationStateDefinition.StateDefinition),
                        null);
                }
                return _UnlinkChangeStateCommand;
            }
        }

        public void UnlinkChangeState()
        {
            if (SelectedStateChange != null && SelectedDestinationStateDefinition != null)
            {
                if (SelectedStateChange.StateChange.NextStates.Contains(SelectedDestinationStateDefinition.StateDefinition))
                {
                    SelectedStateChange.StateChange.NextStates.Remove(SelectedDestinationStateDefinition.StateDefinition);
                    ResetStateDefinitionGraph();
                    ResetDefinitionGraph();
                }
            }
        }
        #endregion
    }
}
