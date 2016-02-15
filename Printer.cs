using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var type = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo[] fields = type.GetFields(flags);

            var res = string.Empty;
            foreach (var fieldInfo in fields)
            {
                if (ShouldSkip(fieldInfo))
                    continue;
                res += string.Format("{0}: {1} ", fieldInfo.Name, fieldInfo.GetValue(obj));
            }

            return res;
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
