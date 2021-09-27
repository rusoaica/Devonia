/// Written by: Yulia Danilova
/// Creation Date: 26th of August, 2021
/// Purpose: Converter for timespan string formatted values
#region ========================================================================= USING =====================================================================================
using System;
using System.Globalization;
using Avalonia.Data.Converters;
#endregion

namespace Devonia.Views.Common.Converters
{
    public class TimeSpanFormatConverter : IValueConverter
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
                return "00:00:00";
            else if (TimeSpan.TryParse(value.ToString(), out TimeSpan span))
                return (span.Days > 0 ? span.Days.ToString("00") + ":" : "") + span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
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
