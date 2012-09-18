﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;

using TOSCheckerTrayApp.ViewModel;
using Microsoft.FSharp.Core;

namespace TOSCheckerTrayApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceHost server;

        public MainWindow()
        {
            InitializeComponent();

            var receiver = new Converter<string, Unit>(delegate(string data) { ((MainWindowViewModel)this.DataContext).Domain = data; return null; });
            server = TOSTrayIcon.startServer(Application.Current, receiver);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.DataContext = new MainWindowViewModel();
        }

        private void HideWindow(object sender, EventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).ClosedTimestamp = DateTime.Now.Ticks;
            this.Hide();
        }
    }
}