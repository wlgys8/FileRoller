using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace MS.FileRoller{



    public class IndexedRollingFileStream:BaseRollingFileStream
    {
        public const int DEFAULT_MAX_BACKUPS = 3;
        public const int MIN_BACKUPS = 1;

        public const int DEFAULT_MAX_FILE_SIZE = 1024 * 1024;
        public const int MIN_FILE_SIZE = 10 * 1024;

        public const bool DEFAULT_KEEP_EXT = true;

        public class Options{
            public int maxBackups = DEFAULT_MAX_BACKUPS;
            public int maxFileSize = DEFAULT_MAX_FILE_SIZE;
            public bool keepExts = DEFAULT_KEEP_EXT;
        }

        private int _maxFileSize;

        private int _maxBackups;

        private bool _keepExts;

        public IndexedRollingFileStream(string filePath,Options options):base(filePath){
            _maxBackups = Mathf.Max(MIN_BACKUPS,options.maxBackups);
            _maxFileSize = Mathf.Max(MIN_FILE_SIZE,options.maxFileSize);
            _keepExts = options.keepExts;
        }

        protected override bool ShouldRoll()
        {
            if(this.Length == 0){
                return false;
            }
            return Length >= _maxFileSize;
        }

        protected override void StartRoll()
        {
            if(!isFileExists){
                //if there's no file exists, no need to roll
                return;
            }
            new IndexedFileRoller(dir,fileName,_keepExts,_maxBackups).Roll();
        }

    }
}
