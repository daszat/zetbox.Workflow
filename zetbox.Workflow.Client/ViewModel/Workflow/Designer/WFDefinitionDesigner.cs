
namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Zetbox.Client.Presentables;
    using Zetbox.API;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.Basic.Workflow;
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
                _definitionGraph.AddVertex(state);
            }

            foreach (var change in WFDefinition.StateDefinitions.SelectMany(sd => sd.StateChanges))
            {
                foreach (var dest in change.NextStates)
                {
                    _definitionGraph.AddEdge(new TaggedEdge<object, StateChange>(change.StateDefinition, dest, change));
                }
            }
        }
    }
}
