using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using Zetbox.Workflow.Client.ViewModel.Workflow.Designer;
using GraphSharp.Controls;

namespace Zetbox.Workflow.Client.WPF.View.Workflow.Designer
{
    public class WFDefinitionGraphLayout : GraphLayout<StateDefinitionGraphViewModel, IEdge<StateDefinitionGraphViewModel>, WFDefinitionGraph>
    {
        public WFDefinitionGraphLayout()
        {
            this.LayoutAlgorithmType = "Tree";
            this.OverlapRemovalAlgorithmType = "FSA";
            this.DestructionTransition = null;
            this.CreationTransition = null;

            var p = this.LayoutParameters as GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters;
            if (p != null)
            {
                p.Direction = GraphSharp.Algorithms.Layout.LayoutDirection.LeftToRight;
                p.VertexGap = 50;
                p.LayerGap = 50;
            }
        }
    }

    public class StateDefinitionGraphLayout : GraphLayout<object, IEdge<object>, StateDefinitionGraph>
    {
        public StateDefinitionGraphLayout()
        {
            this.LayoutAlgorithmType = "Tree";
            this.OverlapRemovalAlgorithmType = "FSA";
            this.DestructionTransition = null;
            this.CreationTransition = null;

            var p = this.LayoutParameters as GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters;
            if (p != null)
            {
                p.Direction = GraphSharp.Algorithms.Layout.LayoutDirection.LeftToRight;
                p.VertexGap = 50;
                p.LayerGap = 50;
            }
        }
    }
}
