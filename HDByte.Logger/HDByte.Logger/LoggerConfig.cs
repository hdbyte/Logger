using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger
{
    public static class LoggerConfig
    {
        public static DateTime LaunchDateTime = DateTime.Now;

        private static Dictionary<string, string> CustomVariables = new Dictionary<string, string>();

        public static string GetCustomVariable(string name)
        {
            if (!CustomVariables.ContainsKey(name))
                throw new Exception($"No Custom Variable Name of '{name} exists in LoggerConfig");

            return CustomVariables[name];
        }

        public static void SetCustomVariable(string name, string value)
        {
            CustomVariables[name] = value;
        }
    }
}
