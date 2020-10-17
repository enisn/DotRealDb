using System;

namespace DotRealDb.Client.Extensions
{
    public static class ObjectConversionExtentions
    {
        public static T CopyPropertiesFrom<T>(this T to, object from)
        {
            foreach (var property in typeof(T).GetProperties())
                try
                {
                    property.SetValue(to, from.GetType().GetProperty(property.Name)?.GetValue(from));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }
            return to;
        }
        public static T CopyPropertiesFrom<T>(this T to, object from, bool ignoreNulls, bool ignoreExceptions = true)
        {
            if (!ignoreNulls)
                return to.CopyPropertiesFrom(from);

            var toType = to.GetType();

            foreach (var property in typeof(T).GetProperties())
                try
                {
                    var value = from.GetType().GetProperty(property.Name)?.GetValue(from);
                    if (value == null) continue;

                    var toPropertyType = toType.GetProperty(property.Name).PropertyType;

                    property.SetValue(to, value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    if (!ignoreExceptions)
                        throw;
                }
            return to;
        }
    }
}
