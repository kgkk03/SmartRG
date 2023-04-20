using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace smartrg.Helpers
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //throw new NotImplementedException();
        }
    }
    public class InverseCheckImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) { return "ic_uncheck"; }
            else return "ic_check";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) { return "ic_uncheck"; }
            else return "ic_check";
            //throw new NotImplementedException();
        }
    }
    public class ConvertCheckImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) { return "ic_check"; }
            else return "ic_point";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(bool)value) { return "ic_check"; }
            else return "ic_point";
            //throw new NotImplementedException();
        }
    }

    public class Image2Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "ic_uncheck";
            if (values == null || !targetType.IsAssignableFrom(typeof(bool)))
            {
                return result;
                //return false;
                // Alternatively, return BindableProperty.UnsetValue to use the binding FallbackValue
            }
            bool stock = false;
            bool sale = false;
            if (values.Count() == 2)
            {
                sale = (bool)values[0];
                stock = (bool)values[1];
            }
            if (sale & stock) { result = "ic_check"; }

            return result;




            //foreach (var value in values)
            //{
            //    if (!(value is bool b))
            //    {
            //        return false;
            //        // Alternatively, return BindableProperty.UnsetValue to use the binding FallbackValue
            //    }
            //    else if (!b)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is bool b) || targetTypes.Any(t => !t.IsAssignableFrom(typeof(bool))))
            {
                // Return null to indicate conversion back is not possible
                return null;
            }

            if (b)
            {
                return targetTypes.Select(t => (object)true).ToArray();
            }
            else
            {
                // Can't convert back from false because of ambiguity
                return null;
            }
        }
    }

}
