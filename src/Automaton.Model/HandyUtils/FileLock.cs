using System;
using Alphaleonis.Win32.Filesystem;

namespace Automaton.Model.HandyUtils
{
    public class FileLock : IDisposable
    {
        public bool IsLocked { get; set; }

        private System.IO.FileStream _lock;

        public FileLock(string path)
        {
            if (File.Exists(path))
            {
                _lock = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                IsLocked = true;
            }
        }

        public void Dispose()
        {
            if (_lock != null)
            {
                _lock.Dispose();
            }
        }
    }
}
