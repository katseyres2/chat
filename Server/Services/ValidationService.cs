using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class ValidationService
    {
        /// <summary>
        /// Check if this value can be a command.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCommand(string value)
        {
            return value.Length > 0 && value[0].CompareTo('/') == 0;
        }
    }
}
