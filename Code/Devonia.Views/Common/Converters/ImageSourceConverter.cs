/// Written by: Yulia Danilova
/// Creation Date: 5th of November, 2019
/// Purpose: Converter for image sources from strings
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using System.Globalization;
using Avalonia.Media.Imaging;
using Avalonia.Data.Converters;
#endregion

namespace Devonia.Views.Common.Converters
{
    public class ImageSourceConverter : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && (targetType == typeof(IBitmap) || targetType == typeof(IImage)))
            {
                Uri uri = new Uri("avares://Devonia/Assets/" + path, UriKind.RelativeOrAbsolute);
                string scheme = uri.IsAbsoluteUri ? uri.Scheme : "file";
                switch (scheme)
                {
                    case "file":
                        return new Bitmap(path);
                    default:
                        IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                        return new Bitmap(assets.Open(uri));
                }
            }
            throw new NotSupportedException();
            //if (!string.IsNullOrEmpty(value as string))
            //    return new BitmapImage(new Uri(@"pack://application:,,,/Devonia;component/Resources/" + (value as string), UriKind.Absolute));
            //return DependencyProperty.UnsetValue;
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
