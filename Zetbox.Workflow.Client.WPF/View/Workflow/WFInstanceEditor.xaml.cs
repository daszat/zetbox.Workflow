
namespace Zetbox.Workflow.Client.WPF.View.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Zetbox.Client.GUI;
    using Zetbox.Workflow.Client.ViewModel.Workflow;
    
    /// <summary>
    /// Interaction logic for StateEditor.xaml
    /// </summary>
    [ViewDescriptor(Zetbox.App.GUI.Toolkit.WPF)]
    public partial class WFInstanceEditor : UserControl, IHasViewModel<WFInstanceViewModel>
    {
        public WFInstanceEditor()
        {
            InitializeComponent();
        }

        public WFInstanceViewModel ViewModel
        {
            get { return (WFInstanceViewModel)DataContext; }
        }
    }
}
