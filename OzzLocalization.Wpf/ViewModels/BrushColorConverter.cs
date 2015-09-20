using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace OzzLocalization.Wpf.ViewModels
{
    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return new SolidColorBrush(ColorForTrueValue);
                }
            }
            return new SolidColorBrush(ColorForFalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Color ColorForTrueValue
        {
            get
            {
                if (_colorForTrueValue == null)
                {
                    _colorForTrueValue = Colors.White;
                }
                return _colorForTrueValue;
            }
            set { _colorForTrueValue = value; }
        }
        Color _colorForTrueValue;

        public Color ColorForFalseValue
        {
            get
            {
                if (_colorForFalseValue == null)
                {
                    _colorForFalseValue = Colors.LightPink;
                }
                return _colorForFalseValue;
            }
            set { _colorForFalseValue = value; }
        }
        Color _colorForFalseValue;
    }
}
