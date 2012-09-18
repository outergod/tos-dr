using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TOSCheckerTrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var icon = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/tos-dr.ico"));
            TOSTrayIcon.createTrayIcon(this, icon.Stream);
        }
    }
}
