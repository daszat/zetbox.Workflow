﻿using System;
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
using zetbox.Workflow.Client.ViewModel.Workflow.Designer;

namespace zetbox.Workflow.Client.WPF.View.Workflow.Designer
{
    /// <summary>
    /// Interaction logic for WFDefinitionEditor.xaml
    /// </summary>
    [ViewDescriptor(Zetbox.App.GUI.Toolkit.WPF)]
    public partial class WFDefinitionEditor : UserControl, IHasViewModel<WFDefinitionDesigner>
    {
        public WFDefinitionEditor()
        {
            InitializeComponent();
        }

        public WFDefinitionDesigner ViewModel
        {
            get { return (WFDefinitionDesigner)DataContext; }
        }
    }
}
