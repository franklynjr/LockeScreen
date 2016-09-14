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
using LockeScreen.Common;

namespace LockeScreen.Models
{
    public class StorageInfo:ModelBase
    {
        string _ImageCount;
        string _SourceName;

        public string ImageCount
        {
            get { return _ImageCount; }
            set
            {
                if (_ImageCount != value)
                {
                    _ImageCount = value;
                    this.NotifyPropertyChanged("ImageCount");
                }
            }
            
        }


        public string AllImageCount
        {
            get { return _ImageCount; }
            set
            {
                if (_ImageCount != value)
                {
                    _ImageCount = value;
                    this.NotifyPropertyChanged("AllImageCount");
                }
            }

        }

        public string SourceName
        {
            get { return _SourceName; }
            set
            {
                if (_SourceName != value)
                {
                    _SourceName = value;

                    ImageCount = FileOperation.SavedImagesCount(_SourceName).ToString();
                    this.NotifyPropertyChanged("SourceName");
                }
            }
        }



    }
}
