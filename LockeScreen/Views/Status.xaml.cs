using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LockeScreen.ViewModels;

namespace LockeScreen.Views
{
    public partial class Status : PhoneApplicationPage
    {
        StorageInfoViewModel storage;

        public Status()
        {
            InitializeComponent();
            storage = new StorageInfoViewModel();
            this.Loaded += Status_Loaded;
        }

        void Status_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = storage;
            
            //throw new NotImplementedException();
        }
    }
}