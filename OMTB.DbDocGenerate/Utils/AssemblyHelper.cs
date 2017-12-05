using System;
using System.IO;
using System.Reflection;

namespace DbDocGenerate.Utils
{
    public class AssemblyHelper
    {
        public void GetAssemblyInfo()
        {
            var currentDirectory = Directory.GetParent(Environment.CurrentDirectory);
            if (currentDirectory.Parent != null)
            {
                var path = currentDirectory.Parent.FullName + "\\Study.Framework.dll";
                Assembly assembly = Assembly.LoadFile(path);
                var assemblyFullName = assembly.GetName();
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var x = type;
                }
            }
        }
        
        
    }
}