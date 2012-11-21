using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace WpfApplication2
{
    public static class StaticPath
    {
        public static string DBPath
        {
            get
            {
                return Environment.CurrentDirectory.ToString() + "\\ToDoAny.sqlite";
            }
        }
    }
}
