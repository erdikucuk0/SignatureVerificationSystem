using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SignatureVerificationSystem.Models.Authorization
{
    class ButtonEnableControlMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool auth = false;

                foreach (var value in values)
                {
                    auth = auth || (bool)value;
                }
                return auth;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
