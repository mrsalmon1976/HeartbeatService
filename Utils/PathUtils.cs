using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Utils
{
    public class PathUtils
    {
        public static string AppFolder()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
