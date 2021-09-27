/// Written by: Yulia Danilova
/// Creation Date: 25th of August, 2021
/// Purpose: Converter for text color (green to red) based on a numeric value
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia.Media;
using System.Globalization;
using Avalonia.Data.Converters;
#endregion

namespace Devonia.Views.Common.Converters
{
    public class NumberToColorConverter : IValueConverter
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null; 
            else if (decimal.TryParse(value.ToString(), out decimal rating))
            {
                if (rating >= 0.00M && rating < 2.50M)
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else if (rating >= 2.50M && rating < 5.00M)
                    return new SolidColorBrush(Color.FromRgb(225, 165, 255));
                else if (rating >= 5.00M && rating < 7.50M)
                    return new SolidColorBrush(Color.FromRgb(225, 225, 0));
                else if (rating >= 7.50M && rating <= 10.00M)
                    return new SolidColorBrush(Color.FromRgb(0, 255, 0));
                else
                    return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
                throw new NotSupportedException();
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
