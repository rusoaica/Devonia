/// Written by: Yulia Danilova
/// Creation Date: 27th of September, 2021
/// Purpose: Code behind for the MainWindowV view

#region ========================================================================= USING =====================================================================================

using System;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Devonia.Views.Main
{
    public class YTypeFaceConverter : AvaloniaPropertyTypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Typeface);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            YTypeFace yTypeface = (Typeface)value;
            return yTypeface;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Typeface);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Typeface typeface = (YTypeFace)value;
            return typeface;
        }
    }
}