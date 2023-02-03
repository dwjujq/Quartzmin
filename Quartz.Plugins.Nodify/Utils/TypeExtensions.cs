using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Plugins.Nodify.Utils
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static void SetPropValue(this object obj,string propName,object value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var type = obj.GetType();

            if (string.IsNullOrWhiteSpace(propName))
            {
                throw new ArgumentException($"参数{propName}有误");
            }
            var dotIndex = propName.IndexOf(".");
            if (dotIndex == -1)
            {
                var prop = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                if (prop == null)
                {
                    throw new ArgumentOutOfRangeException(propName);
                }

                object objVal;
                if (value.TryChangeType(prop.PropertyType, out objVal))
                {
                    prop.SetValue(obj, objVal);
                    return;
                }
                throw new ArgumentException("value数据类型有误");
            }

            var tempPropName = propName.Substring(0, dotIndex);
            var tempProp= type.GetProperty(tempPropName, BindingFlags.Instance | BindingFlags.Public);
            if (tempProp == null)
            {
                throw new ArgumentOutOfRangeException(tempPropName);
            }

            var tempPropObj = tempProp.GetValue(obj);
            if (tempPropObj == null)
            {
                tempPropObj = Activator.CreateInstance(tempProp.PropertyType);
                tempProp.SetValue(obj, tempPropObj);
            }

            tempPropObj.SetPropValue(propName.Substring(dotIndex+1),value);
        }

        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        public static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
