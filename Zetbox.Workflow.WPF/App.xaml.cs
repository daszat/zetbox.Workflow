using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Zetbox.Workflow.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Zetbox.Client.WPF.App
    {
        protected override string GetConfigFileName()
        {
            return "Zetbox.Workflow.WPF.xml";
        }
    }
}
