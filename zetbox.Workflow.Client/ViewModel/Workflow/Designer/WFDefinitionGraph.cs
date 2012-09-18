using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace zetbox.Workflow.Client.ViewModel.Workflow.Designer
{
    [CLSCompliant(false)]
    public class WFDefinitionGraph
        : BidirectionalGraph<object, IEdge<object>>
    {
        public WFDefinitionGraph() { }

        public WFDefinitionGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public WFDefinitionGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}
