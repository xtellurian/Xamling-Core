using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using XamlingCore.Portable.Contract.Infrastructure.LocalStorage;
using XamlingCore.Portable.Util.Lock;
using AsyncLock = ModernHttpClient.AsyncLock;

namespace XamlingCore.iOS.Unified.Implementations
{
    public class LocalStorage : ILocalStorage
    {
        public async Task<bool> IsZero(string fileName)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);

                var r = File.Exists(path);


                if (!r)
                {
                    return false;
                }

                var b = await Load(fileName);

                return b.Length == 0;
            }
        }

        public async Task<bool> Copy(string source, string newName, bool replace = true)
        {
            var _lock = NamedLock.Get(newName);
            using (var releaser = await _lock.LockAsync())
            {

                var sFile = _getPath(source);
                var tFile = _getPath(newName);

                var r = File.Exists(sFile);

                if (!r)
                {
                    return false;
                }

                var dir = Path.GetDirectoryName(tFile);

                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(sFile, tFile, replace);

                return true;
            }
        }

        public async Task<bool> DeleteFile(string fileName)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);

                var r = File.Exists(path);

                if (r)
                {
                    File.Delete(path);
                }

                return true;
            }
        }

        public async Task<bool> FileExists(string fileName)
        {
            var p = _getPath(fileName);

            var r = File.Exists(p);

            return r;
        }

        public async Task<List<string>> GetAllFilesInFolder(string folderPath)
        {
            var p = _getPath(folderPath);
            if (!Directory.Exists(p))
            {
                return null;
            }

            var f = Directory.GetFiles(p, "*.*", SearchOption.AllDirectories);

            return f != null ? f.ToList() : null;
        }


        public async Task<byte[]> Load(string fileName)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);

                if (!File.Exists(path))
                {
                    return null;
                }

                var r = File.ReadAllBytes(path);

                return r;
            }
        }

        public async Task<System.IO.Stream> LoadStream(string fileName)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);
                if (!File.Exists(path))
                {
                    return null;
                }

                var r = File.Open(path, FileMode.Open);
                return r;
            }
        }



        public async Task<string> LoadString(string fileName)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);

                if (!File.Exists(path))
                {
                    return null;
                }

                var r = File.ReadAllText(path);

                return r;
            }
        }

        public async Task Save(string fileName, byte[] data)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);
                _createDirForFile(path);

                File.WriteAllBytes(path, data);
            }
        }

        public async Task SaveStream(string fileName, System.IO.Stream stream)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);

                _createDirForFile(path);

                using (var s = File.Create(path))
                {
                    await stream.CopyToAsync(s);
                }
            }
        }

        static AsyncLock wl = new AsyncLock();
        public async Task<bool> SaveString(string fileName, string data)
        {
            var _lock = NamedLock.Get(fileName);

            using (var releaser = await _lock.LockAsync())
            {
                var path = _getPath(fileName);
                _createDirForFile(path);

                File.WriteAllText(path, data);

                return true;
            }

        }

        void _createDirForFile(string fileName)
        {
            var dir = Path.GetDirectoryName(fileName);

            //Debug.WriteLine("Creating Directory: {0}", fileName);

            if (dir == null)
            {
                return;
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// iOS 8 Specific get path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>

        private static string path = null;

        private string _getPath(string fileName)
        {
            if (path == null)
            {
                var documents = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User)[0];
                path = documents.Path;
            }

            var result = Path.Combine(path, fileName);
            return result;
        }
    }
}