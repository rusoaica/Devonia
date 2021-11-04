/// Written by: Yulia Danilova
/// Creation Date: 27th of September, 2021
/// Purpose: Code behind for the MainWindowV view

#region ========================================================================= USING =====================================================================================

using Avalonia.Media;
using System.ComponentModel;
#endregion

namespace Devonia.Views.Main
{
    [TypeConverter(typeof(YTypeFaceConverter))]
    public class YTypeFace
    {
        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }

        public static implicit operator Typeface(YTypeFace myTypeFace)
        {
            return new Typeface(myTypeFace.FontFamily, myTypeFace.FontStyle, myTypeFace.FontWeight);
        }

        public static implicit operator YTypeFace(Typeface typeFace)
        {
            return new YTypeFace { FontFamily = typeFace.FontFamily, FontWeight = typeFace.Weight, FontStyle = typeFace.Style };
        }


    }
}