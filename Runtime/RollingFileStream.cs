using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace MS.FileRoller{



    public class RollingFileStream:BaseRollingFileStream
    {

        public class Options{
            public int numToKeep;
            public int maxFileSize;

            public bool keepExts;
        }

        private int _maxFileSize;

        private int _maxBackups;

        private bool _keepExts;


        public RollingFileStream(string filePath,Options options):base(filePath){
            _maxBackups = Mathf.Max(1,options.numToKeep);
            _maxFileSize = Mathf.Max(1024 * 10,options.maxFileSize);
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
             FileRollUtil.RollFiles(dir,fileName,_maxBackups,_keepExts);
        }

    }
}
