using Saboteur.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Saboteur
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.mainWindow = new MainWindow();
            this.mainWindow.Show();
        }

    }
}
