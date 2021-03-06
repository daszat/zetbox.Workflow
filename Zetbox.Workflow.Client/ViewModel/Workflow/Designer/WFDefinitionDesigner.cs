﻿
namespace Zetbox.Workflow.Client.ViewModel.Workflow.Designer
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
            _hiddenAction = appCtx.Factory.CreateViewModel<HiddenGraphElementViewModel.Factory>().Invoke(dataCtx, this);
            _hiddenStateChange = appCtx.Factory.CreateViewModel<HiddenGraphElementViewModel.Factory>().Invoke(dataCtx, this);
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
        private Dictionary<wf.ParameterizedActionDefinition, ActionGraphViewModel> _actionViewModels = new Dictionary<wf.ParameterizedActionDefinition, ActionGraphViewModel>();
        private Dictionary<wf.StateChangeDefinition, StateChangeGraphViewModel> _stateChangeViewModels = new Dictionary<wf.StateChangeDefinition, StateChangeGraphViewModel>();
        private readonly HiddenGraphElementViewModel _hiddenAction;
        private readonly HiddenGraphElementViewModel _hiddenStateChange;

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

        private StateChangeGraphViewModel ToStateChangeViewModel(wf.StateChangeDefinition change)
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

        private ActionGraphViewModel ToActionViewModel(wf.ParameterizedActionDefinition action)
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
                vm.Action.PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "InvokeAction":
                            ResetStateDefinitionGraph();
                            break;
                    }
                };

            }
            return _actionViewModels[action];
        }
        #endregion

        #region Graphs
        #region DefinitionGraph
        private WFDefinitionGraph _definitionGraph;
        [CLSCompliant(false)]
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
        [CLSCompliant(false)]
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
            _StatedefinitionGraph = new StateDefinitionGraph(true);
            if (SelectedStateDefinition == null) return;

            // Actions ------------------------------
            var scheduledActions = SelectedStateDefinition.StateDefinition.Actions.OfType<wf.ScheduledActionDefinition>().ToList();
            foreach (var action in scheduledActions)
            {
                _StatedefinitionGraph.AddVertex(ToActionViewModel(action));
            }
            foreach (var action in SelectedStateDefinition.StateDefinition.Actions.Except(scheduledActions))
            {
                _StatedefinitionGraph.AddVertex(ToActionViewModel(action));
            }
            _StatedefinitionGraph.AddVertex(_hiddenAction);

            // State changes ------------------------------
            foreach (var change in SelectedStateDefinition.StateDefinition.StateChanges)
            {
                _StatedefinitionGraph.AddVertex(ToStateChangeViewModel(change));
            }
            _StatedefinitionGraph.AddVertex(_hiddenStateChange);

            // State definitions ------------------------------
            foreach (var state in WFDefinition.StateDefinitions)
            {
                _StatedefinitionGraph.AddVertex(ToStateDefinitionViewModel(state));
            }

            // edges ------------------------------
            foreach (var change in SelectedStateDefinition.StateDefinition.StateChanges)
            {
                foreach (var action in change.InvokedByActions)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(ToActionViewModel(action), ToStateChangeViewModel(change), ToStateChangeViewModel(change)));
                }

                foreach (wf.StateDefinition dest in change.NextStates)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, StateChangeGraphViewModel>(ToStateChangeViewModel(change), ToStateDefinitionViewModel(dest), ToStateChangeViewModel(change)));
                }
            }

            foreach (var vertex in _StatedefinitionGraph.Vertices)
            {
                if (_StatedefinitionGraph.IsInEdgesEmpty(vertex))
                {
                    if (vertex is StateChangeGraphViewModel)
                    {
                        _StatedefinitionGraph.AddEdge(new TaggedEdge<object, HiddenGraphElementViewModel>(_hiddenAction, vertex, _hiddenStateChange));
                    }
                    else if (vertex is StateDefinitionGraphViewModel)
                    {
                        _StatedefinitionGraph.AddEdge(new TaggedEdge<object, HiddenGraphElementViewModel>(_hiddenStateChange, vertex, _hiddenStateChange));
                    }
                }
            }

            foreach (var schedulerAction in SelectedStateDefinition.StateDefinition.Actions.OfType<wf.ScheduledActionDefinition>())
            {
                if (schedulerAction.InvokeAction != null)
                {
                    _StatedefinitionGraph.AddEdge(new TaggedEdge<object, ActionGraphViewModel>(ToActionViewModel(schedulerAction), ToActionViewModel(schedulerAction.InvokeAction), ToActionViewModel(schedulerAction)));
                }
            }

            _StatedefinitionGraph.AddEdge(new TaggedEdge<object, HiddenGraphElementViewModel>(_hiddenAction, _hiddenStateChange, _hiddenStateChange));
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
            stateDef.Module = WFDefinition.Module;
            SelectedStateDefinition = ToStateDefinitionViewModel(stateDef);
            ResetDefinitionGraph();
        }

        private ICommandViewModel _DeleteStateCommand = null;
        public ICommandViewModel DeleteStateCommand
        {
            get
            {
                if (_DeleteStateCommand == null)
                {
                    _DeleteStateCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, "Delete State", "Deletes the current state", 
                        DeleteState, 
                        () => SelectedStateDefinition != null, 
                        null);
                }
                return _DeleteStateCommand;
            }
        }

        public void DeleteState()
        {
            if (SelectedStateDefinition != null)
            {
                if (ViewModelFactory.GetDecisionFromUser("Delete this state?", "Are you sure?"))
                {
                    _stateViewModels.Remove(SelectedStateDefinition.StateDefinition);
                    DataContext.Delete(SelectedStateDefinition.StateDefinition);
                    SelectedStateDefinition = null;
                    ResetDefinitionGraph();
                    ResetStateDefinitionGraph();
                }
            }
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
                    if (string.IsNullOrEmpty(SelectedStateChange.StateChange.Name))
                    {
                        SelectedStateChange.StateChange.Name = SelectedAction.Action.Name;
                    }

                    foreach (var change in SelectedStateDefinition.StateDefinition.StateChanges.Except(new[] { SelectedStateChange.StateChange }))
                    {
                        if (change.InvokedByActions.Contains(SelectedAction.Action))
                            change.InvokedByActions.Remove(SelectedAction.Action);
                    }

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
                        wf.ParameterizedActionDefinition actionToSelect = null;
                        foreach (var action in lst.Select(i => i.Object).Cast<wf.Action>())
                        {
                            var paramAction = (wf.ParameterizedActionDefinition)DataContext.Create(DataContext.GetInterfaceType(action.ParameterType.GetDataType()));
                            paramAction.Action = action;
                            paramAction.Module = WFDefinition.Module;
                            paramAction.Name = action.Name;
                            paramAction.IsVisible = action.IsVisibleDefault;
                            paramAction.IsOnEnterAction = action.IsOnEnterActionDefault;
                            SelectedStateDefinition.StateDefinition.Actions.Add(paramAction);

                            if (actionToSelect == null) actionToSelect = paramAction;
                        }
                        if (actionToSelect != null)
                        {
                            SelectedAction = ToActionViewModel(actionToSelect);
                        }
                        ResetStateDefinitionGraph();
                    },
                    null);
                ViewModelFactory.ShowDialog(dlg);
            }
        }

        private ICommandViewModel _DeleteActionCommand = null;
        public ICommandViewModel DeleteActionCommand
        {
            get
            {
                if (_DeleteActionCommand == null)
                {
                    _DeleteActionCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, "Delete Action", "Deletes the current action", 
                        DeleteAction, 
                        () => SelectedAction != null, 
                        null);
                }
                return _DeleteActionCommand;
            }
        }

        public void DeleteAction()
        {
            if (SelectedAction != null)
            {
                if (ViewModelFactory.GetDecisionFromUser("Delete this action?", "Are you sure?"))
                {
                    _actionViewModels.Remove(SelectedAction.Action);
                    DataContext.Delete(SelectedAction.Action);
                    SelectedAction = null;
                    ResetStateDefinitionGraph();
                }
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
                var change = DataContext.Create<wf.StateChangeDefinition>();
                change.Module = WFDefinition.Module;
                SelectedStateDefinition.StateDefinition.StateChanges.Add(change);
                SelectedStateChange = ToStateChangeViewModel(change);
                ResetStateDefinitionGraph();
            }
        }

        private ICommandViewModel _DeleteStateChangeCommand = null;
        public ICommandViewModel DeleteStateChangeCommand
        {
            get
            {
                if (_DeleteStateChangeCommand == null)
                {
                    _DeleteStateChangeCommand = ViewModelFactory.CreateViewModel<SimpleCommandViewModel.Factory>().Invoke(DataContext, this, "Delete state change", "Deletes the current state change", 
                        DeleteStateChange, 
                        () => SelectedStateChange != null, 
                        null);
                }
                return _DeleteStateChangeCommand;
            }
        }

        public void DeleteStateChange()
        {
            if (SelectedStateChange != null)
            {
                if (ViewModelFactory.GetDecisionFromUser("Delete this state change?", "Are you sure?"))
                {
                    _stateChangeViewModels.Remove(SelectedStateChange.StateChange);
                    DataContext.Delete(SelectedStateChange.StateChange);
                    SelectedStateChange = null;
                    ResetStateDefinitionGraph();
                }
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
