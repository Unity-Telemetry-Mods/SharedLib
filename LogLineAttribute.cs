using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryLib
{
    internal class LogLineAttribute
    {
        public string Name { get; private set; }
        public int Line { get; private set; }
        public LogLineAttribute(int line, string name = null)
        {
            Name = name;
            Line = line;
        }
    }
}
