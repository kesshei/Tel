using System;

namespace Tel.Core.Utilitys
{
    public static class AssemblyUtility
    {
        public static Version GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
