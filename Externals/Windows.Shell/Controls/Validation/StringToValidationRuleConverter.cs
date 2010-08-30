#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

#endregion

namespace Microsoft.Windows.Controls
{
    public class StringToValidationRuleConverter : TypeConverter
    {
        private static readonly Dictionary<string, Type> Rules;

        static StringToValidationRuleConverter()
        {
            Rules = new Dictionary<string, Type>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes().Where(type => type.IsSubclassOf(typeof (ValidationRule))))
                Rules.Add(type.Name, type);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;

            if (text != null)
            {
                try
                {
                    if (Rules.Any(type => type.Key == text))
                    {
                        var instance = Activator.CreateInstance(Rules[text]);
                        return instance;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Cannot convert '{0}' ({1}) because {2}", value, value.GetType(), e.Message), e);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            var rule = value as ValidationRule;

            if (rule != null && CanConvertTo(context, destinationType))
                return rule.GetType().Name;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}