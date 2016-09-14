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
using LockeScreen.Common;

namespace LockeScreen.Views
{
    public partial class quickset : PhoneApplicationPage
    {

        QuickSetViewModel quickSetViewModel;

        public quickset()
        {
            InitializeComponent();
            quickSetViewModel = new QuickSetViewModel();
            this.DataContext = quickSetViewModel;
            this.Loaded += quickset_Loaded;
        }

        void quickset_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            Shared.ShowToast("Tap any image to set as lockscreen background");
                       
        }

        private void lbxQuickSet_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var image = e.AddedItems[0];
                    LockScreenHelper.SetFileAsLockscreenImage(((ImageViewModel)image).Uri);
                    Shared.ApplicationData[KEYS.BACKGROUND_CHANGED] = true;
                    Shared.ShowToast("Lockscreen image upadated.");
                }
            }
            catch { }
        }

        private void lbxQuickSet_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            cm.Items.Add("delete");
            
        }






    }
}