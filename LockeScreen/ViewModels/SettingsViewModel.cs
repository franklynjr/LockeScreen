using LockeScreen.Models;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;

namespace LockeScreen.ViewModels
{
    public class SettingsViewModel : ModelBase
    {

        private SettingsModel _Settings;
        //private int _DynamicBackground;
        // Constructor
        public SettingsViewModel()
        {
            _Settings = new SettingsModel();
        }


        public SettingsModel Settings
        {
            get { return _Settings; }
            set
            {
                if (_Settings != value)
                {
                    _Settings = value;
                    NotifyPropertyChanged("Settings");
                }
            }
        }

        //public int DynamicBackground
        //{
        //    get { return _DynamicBackground; }
        //    set
        //    {
        //        if (_DynamicBackground != value)
        //        {
        //            _DynamicBackground = value;
        //            NotifyPropertyChanged("DynamicBackground");
        //        }
        //    }
        //}

    }
}
