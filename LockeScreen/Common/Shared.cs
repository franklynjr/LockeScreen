using Coding4Fun.Toolkit.Controls;
using LockeScreen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockeScreen.Common
{
    public class Shared : ModelBase
    {
        public static Dictionary<string, object> ApplicationData = new Dictionary<string,object>();

        //private static SettingsModel _Settings;



        public static SettingsModel Settings
        {
            get;
            set;
        }

        
        public static void LoadSettings()
        {
            //Settings = new SettingsModel();
            //Thread SettingsAsync = new Thread(new ThreadStart(Settings.Load));
            //SettingsAsync.Start();
            //GetFacebookAlbumsAsync();   
            Settings = new SettingsModel();
            Settings.Load();
        }


        public static void ShowToast(string Message)
        {
            if (Shared.Settings.ToastEnabled)
            {
                ToastPrompt toast = new ToastPrompt();
                toast.Message = Message;
                toast.Show();
            }
        }


    }
}
