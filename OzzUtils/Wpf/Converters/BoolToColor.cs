using System;
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
        /// Converts a boolean value to a SolidColorBrush based on the ColorForTrueValue and ColorForFalseValue properties.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">An optional parameter for the conversion.</param>
        /// <param name="culture">The culture information for the conversion.</param>
        /// <returns>A SolidColorBrush representing the boolean value.</returns>
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
