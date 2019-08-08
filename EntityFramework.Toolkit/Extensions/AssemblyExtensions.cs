using System;
using System.Linq;
using System.Reflection;

namespace EntityFramework.Toolkit.Extensions
{
    internal static class AssemblyExtensions
    {
        internal static Type[] TryGetTypes(this Assembly assembly)
        {
            Type[] types = null;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            return types;
        }
    }
}