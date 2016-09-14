using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LockeScreen.ValueConverters
{
    public class PanoramaVisibilityValueConverter:IValueConverter
    {
        
        public object Convert(object value, Type type,object param, CultureInfo culture)
        {
            if (value is bool)
            {
                bool myValue = (bool)value;

                if (!myValue)
                    return System.Windows.Visibility.Collapsed;
                else
                    return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type type, object param, CultureInfo culture)
        {
            if (value is bool)
            {
                bool myValue = (bool)value;

                if (myValue)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }

            return System.Windows.Visibility.Collapsed;
        } 
    }
}
