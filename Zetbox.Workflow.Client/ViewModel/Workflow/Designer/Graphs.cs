using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace Zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    [CLSCompliant(false)]
    public class WFDefinitionGraph
        : BidirectionalGraph<StateDefinitionGraphViewModel, IEdge<StateDefinitionGraphViewModel>>
    {
        public WFDefinitionGraph() { }

        public WFDefinitionGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public WFDefinitionGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }

    [CLSCompliant(false)]
    public class StateDefinitionGraph
        : BidirectionalGraph<object, IEdge<object>>
    {
        public StateDefinitionGraph() { }

        public StateDefinitionGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public StateDefinitionGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}
