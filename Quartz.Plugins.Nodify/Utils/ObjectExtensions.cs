using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Plugins.Nodify.Utils
{
    public static class ObjectExtensions
    {
        public static T To<T>(this object obj)
        {
            var targetType=typeof(T);

            if(obj==null)
            {
                if(!targetType.IsValueType)
                {
                    return default;
                }
                throw new ArgumentNullException(nameof(obj));
            }

            if(obj.GetType() == typeof(T))
            {
                return (T)obj;
            }

            if (targetType.IsNullableType())
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            return (T)Convert.ChangeType(obj, targetType);
        }

        public static bool TryChangeType(this object value, Type targetType, out object newValue)
        {
            if (targetType.IsNullableType())
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            newValue = null;
            try
            {
                newValue = Convert.ChangeType(value, targetType);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
