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
    }

    [CLSCompliant(false)]
    public class StateDefinitionGraphLayout : GraphLayout<object, IEdge<object>, StateDefinitionGraph>
    {
    }
}
