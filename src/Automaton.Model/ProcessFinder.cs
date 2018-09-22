using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class ProcessFinder
    {
        public static bool IsProcessAlreadyRunning()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            var fileInfo = new FileInfo(exePath);
            var exeName = fileInfo.Name;
            var bCreatedNew = false;

            var mutex = new Mutex(true, "Global\\" + exeName, out bCreatedNew);
            if (bCreatedNew)
                mutex.ReleaseMutex();

            return !bCreatedNew;
        }
    }
}
