using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


namespace PrintObj
{
    public class Printer : IPrinter
    {
        public static IPrinter Print()
        {
            return new Printer();
        }

        public string PrintObj(object obj)
        {
            Dictionary<FieldInfo, PropertyInfo> backingFieldsToPropertyInfo = GetBackingFields(obj);
            FieldInfo[] fields = GeFields(obj);

            var res = string.Empty;
            foreach (var fieldInfo in fields)
            {
                if (ShouldSkip(fieldInfo))
                    continue;

                PropertyInfo pi;
                backingFieldsToPropertyInfo.TryGetValue(fieldInfo, out pi);

                if (pi != null)
                {
                    if (ShouldSkip(pi))
                        continue;
                    res += string.Format("{0}: {1} ", pi.Name, fieldInfo.GetValue(obj));
                }
                else
                {
                    res += string.Format("{0}: {1} ", fieldInfo.Name, fieldInfo.GetValue(obj));
                }
            }

            return res;
        }

        private bool ShouldSkip(PropertyInfo pi)
        {
            return pi.GetCustomAttributes(typeof (PrintObjIgnoreAttribute), false).Any();
        }

        private static Dictionary<FieldInfo, PropertyInfo>  GetBackingFields(object obj)
        {
            var type = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            Dictionary<FieldInfo, PropertyInfo> res = new Dictionary<FieldInfo, PropertyInfo>();

            PropertyInfo[] propertyInfos = type.GetProperties(flags);
            foreach (var propertyInfo in propertyInfos)
            {
                var backingField = GetBackingField(propertyInfo);
                if (backingField != null)
                {
                    res[backingField] = propertyInfo;
                }
            }

            return res;
        }

        private static FieldInfo GetBackingField(PropertyInfo pi) 
        {
            if (!pi.CanRead || !pi.GetGetMethod(nonPublic:true).IsDefined(typeof(CompilerGeneratedAttribute), inherit:true))
                return null;
            
            var backingField = pi.DeclaringType.GetField(string.Format("<{0}>k__BackingField", pi.Name), BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit:true))
                return null;
            return backingField;
        }

        private static FieldInfo[] GeFields(object obj)
        {
            var type = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo[] fields = type.GetFields(flags);
            return fields;
        }


        private bool ShouldSkip(FieldInfo fieldInfo)
        {
            var customAttributes = fieldInfo.GetCustomAttributes(false);
            if (HasIgnoreAttribute(customAttributes))
                return true;

            return false;
        } 

        private bool HasIgnoreAttribute(object[] customAttributes)
        {
            foreach (var customAttribute in customAttributes)
            {
                if (customAttribute.GetType() == typeof (PrintObjIgnoreAttribute))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public interface IPrinter
    {
        string PrintObj(object obj);
    }
}
