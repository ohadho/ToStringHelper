using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Server;

namespace PrintObj
{
    public class Printer : IPrinter
    {
        public static IPrinter Print()
        {
            Printer ret = new Printer();

            return ret;
        }

        public string PrintObj(object obj)
        {
            var type = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo[] fields = type.GetFields(flags);

            var res = string.Empty;
            foreach (var fieldInfo in fields)
            {
                res += fieldInfo.Name + ": " + fieldInfo.GetValue(obj).ToString() + " ";
            }

            return res;
        }
    }

    public interface IPrinter
    {
        string PrintObj(object obj);
    }
}
