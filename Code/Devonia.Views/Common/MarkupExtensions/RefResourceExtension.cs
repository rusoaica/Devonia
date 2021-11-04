using Avalonia;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devonia.Views
{
    public class RefResourceExtension : MarkupExtension
    {
        public object Source { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public Type TypeConverterType { get; set; }

        public RefResourceExtension(object source)
        {
            Source = source;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget provideValueTarget = serviceProvider as IProvideValueTarget;

            Type sourceType = Source.GetType();

            Type targetType = (provideValueTarget.TargetProperty as AvaloniaProperty).PropertyType;

            TypeConverter converter = null;

            if (TypeConverterType != null)
            {
                converter = (TypeConverter)Activator.CreateInstance(TypeConverterType);

            }
            else
            {
                TypeConverterAttribute attr = sourceType.GetCustomAttribute<TypeConverterAttribute>();

                if (attr != null)
                {
                    converter = (TypeConverter)Activator.CreateInstance(Type.GetType(attr.ConverterTypeName));
                }
                else
                {
                    throw new Exception("Source property type should has TypeConverter attribute or set TypeConverterType property.");
                }
            }

            return converter.ConvertTo(Source, targetType);


        }


    }
}
