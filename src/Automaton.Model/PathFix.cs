using Autofac;
using Automaton.Model.Interfaces;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Automaton.Model
{
    public class PathFix : IPathFix
    {
        private const string _apiKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem";
        private const string _keyName = "LongPathsEnabled";

        private readonly ILogger _logger;

        public PathFix(IComponentContext components)
        {
            _logger = components.Resolve<ILogger>();
        }

        public bool IsLongPathsEnabled()
        {
            return (int)Registry.GetValue(_apiKey, _keyName, -1) == 1;
        }

        public bool EnableLongPaths()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                FileName = "cmd.exe",
                Verb = "runas",
                Arguments = @"/C REG ADD HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem /v LongPathsEnabled /t REG_DWORD /d 1 /f"
            };

            process.StartInfo = startInfo;

            try
            {
                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }
    }
}
