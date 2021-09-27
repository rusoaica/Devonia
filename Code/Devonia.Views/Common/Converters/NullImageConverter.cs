/// Written by: Yulia Danilova
/// Creation Date: 26th of August, 2021
/// Purpose: Converter for image sources that might be null
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using System.IO;
using Avalonia.Media;
using Avalonia.Platform;
using System.Globalization;
using Avalonia.Media.Imaging;
using Avalonia.Data.Converters;
#endregion

namespace Devonia.Views.Common.Converters
{
    public class NullImageConverter : IValueConverter
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
            IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            if (value is string path && (targetType == typeof(IBitmap) || targetType == typeof(IImage)))
            {
                if (File.Exists(path))
                    return new Bitmap(path);
                else if (!string.IsNullOrEmpty(path))
                {
                    Uri uri = new Uri("avares://Devonia/Assets/" + path, UriKind.RelativeOrAbsolute);
                    string scheme = uri.IsAbsoluteUri ? uri.Scheme : "file";
                    switch (scheme)
                    {
                        case "file":
                            return new Bitmap(path);
                        default:
                            return new Bitmap(assets.Open(uri));
                    }
                }
            }
            return new Bitmap(assets.Open(new Uri("avares://Devonia/Assets/tp.png", UriKind.RelativeOrAbsolute)));
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="_value">The value that is produced by the binding target.</param>
        /// <param name="_targetType">The type to convert to.</param>
        /// <param name="_parameter">The converter parameter to use.</param>
        /// <param name="_culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
