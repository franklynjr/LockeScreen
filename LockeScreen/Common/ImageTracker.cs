using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using LockeScreen.Models;

namespace Sample.Common
{
    public class ImageTracker:ModelBase
    {
        List<string> _Used;
        List<string> _Unused;
        List<string> _All;

        List<string> All
        {
            get { return _All; }
            set
            {
                if (_All != value)
                {
                    _All = _Unused = value;
                    if (_Used != null)
                    {
                        foreach (string s in _Used)
                        {
                            _Unused.Remove(s);
                        }
                    }
                    else
                    {
                        //Load
                        LoadUsed();
                        if (_Used != null)
                        {
                            foreach (string s in _Used)
                            {
                                _Unused.Remove(s);
                            }
                        }

                    }
                }


            }
        }

        public List<string> Used
        {
            get { return _Used; }
            set
            {
                if (_Used != value)
                {
                    _Used = value;
                    NotifyPropertyChanged("Used");
                }
            }
        }

        public List<string> Unused
        {
            get { return _Unused; }
            set
            {
                if (_Unused != value)
                {
                    _Unused = value;
                    NotifyPropertyChanged("Unused");
                }
            }
        }

        private void LoadUsed()
        {

            IsolatedStorageSettings.ApplicationSettings.TryGetValue("UsedImages", out _Used);
        }


        private void ResetUsed()
        {

            IsolatedStorageSettings.ApplicationSettings["UsedImages"] = null;
        }

        public bool TryUseIndex(int i, out string value)
        {
            value = null;
            
            List<string> images = _Unused;

            try
            {
                if (i < images.Count)
                {
                    value = images[i];

                    _Used.Add(value);
                    images.RemoveAt(i);

                    Unused = images;

                    if (images.Count == 0)
                        ResetUsed();
                }
            }
            catch { }

            return false;
        }
    }
}
