﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    internal class ValidationService
    {
        public static bool isCommand(string value)
        {
            return value.Length > 0 && value[0].CompareTo('/') == 0;
        }
    }
}