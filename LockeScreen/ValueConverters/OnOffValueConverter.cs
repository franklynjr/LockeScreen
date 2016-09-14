using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LockeScreen.ValueConverters
{
    public class OnOffValueConverter:IValueConverter
    {
        
        public object Convert(object value, Type type,object param, CultureInfo culture)
        {
            if (value is bool)
            {
                bool myValue = (bool)value;

                if (myValue)
                    return "on";
                else
                    return "off";
            } 
            return "Error";
        }

        public object ConvertBack(object value, Type type, object param, CultureInfo culture)
        {
            if (value is bool)
            {
                bool myValue = (bool)value;

                if (myValue)
                    return "";
                else
                    return "";
            }

            return "";
        } 
    }
}
