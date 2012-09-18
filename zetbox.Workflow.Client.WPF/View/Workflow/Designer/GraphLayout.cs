using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using zetbox.Workflow.Client.ViewModel.Workflow.Designer;
using GraphSharp.Controls;

namespace zetbox.Workflow.Client.WPF.View.Workflow.Designer
{
    [CLSCompliant(false)]
    public class WFDefinitionGraphLayout : GraphLayout<StateDefinitionGraphViewModel, IEdge<StateDefinitionGraphViewModel>, WFDefinitionGraph>
    {
        public WFDefinitionGraphLayout()
        {
            this.LayoutAlgorithmType = "Tree";
            var p = this.LayoutParameters as GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters;
            if (p != null)
            {
                p.Direction = GraphSharp.Algorithms.Layout.LayoutDirection.LeftToRight;
            }
            this.OverlapRemovalAlgorithmType = "FSA";
            ((GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)this.OverlapRemovalParameters).HorizontalGap = 40f;
            ((GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)this.OverlapRemovalParameters).VerticalGap = 20f;
        }
    }

    [CLSCompliant(false)]
    public class StateDefinitionGraphLayout : GraphLayout<object, IEdge<object>, StateDefinitionGraph>
    {
        public StateDefinitionGraphLayout()
        {
            this.LayoutAlgorithmType = "Tree";
            var p = this.LayoutParameters as GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters;
            if (p != null)
            {
                p.Direction = GraphSharp.Algorithms.Layout.LayoutDirection.LeftToRight;
            }

            this.OverlapRemovalAlgorithmType = "FSA";
            ((GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)this.OverlapRemovalParameters).HorizontalGap = 40f;
            ((GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)this.OverlapRemovalParameters).VerticalGap = 20f;
        }
    }
}
