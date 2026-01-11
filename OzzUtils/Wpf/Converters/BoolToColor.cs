using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OzzUtils.Wpf.Converters
{
    /// <summary>
    /// Boolean value to SolidColorBrush converter
    /// </summary>
    public class BoolToColor : IValueConverter
    {
        public BoolToColor()
        {
            SetDefaultColors();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool && (bool)value) || (value is bool? && (((bool?)value) ?? false)))
            {
                return new SolidColorBrush(ColorForTrueValue);
            }
            return new SolidColorBrush(ColorForFalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Brush color for boolean true
        /// </summary>
        public Color ColorForTrueValue { get; set; }
        /// <summary>
        /// Brush color for boolean false
        /// </summary>
        public Color ColorForFalseValue { get; set; }

        private void SetDefaultColors()
        {
            ColorForTrueValue = Colors.White;
            ColorForFalseValue = Colors.LightPink;
        }
    }
}
