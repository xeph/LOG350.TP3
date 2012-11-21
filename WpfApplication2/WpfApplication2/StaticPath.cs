using System;
using System.ComponentModel;
using System.Reflection;

namespace WpfApplication2
{
    public static class StaticPath()
    {
        public enum Path
        {
            [Description(@"C:\Users\Don\Documents\GitHub\LOG350.TP3\WpfApplication2\WpfApplication2\ToDoAny.sqlite")]DB
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = 
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
