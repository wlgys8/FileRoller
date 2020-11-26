using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MS.FileRoller{

    public class DateRollingFileStream:BaseRollingFileStream
    {

        public const int DEFAULT_MAX_BACKUPS = 3;
        public const int MIN_BACKUPS = 1;

        public const int DEFAULT_MAX_FILE_SIZE = 1024 * 1024;
        public const int MIN_FILE_SIZE = 10 * 1024;

        public const bool DEFAULT_KEEP_EXT = true;


        public class Options{
            public int maxBackups = DEFAULT_MAX_BACKUPS;
            public int maxFileSize = DEFAULT_MAX_FILE_SIZE;
            public bool keepExt = DEFAULT_KEEP_EXT;
        }

        private int _maxBackups;
        private int _maxFileSize;

        private bool _keepExt;

        private System.DateTime _creationTime;

        public DateRollingFileStream(string filePath,Options options):base(filePath){
            _maxBackups = Mathf.Max(MIN_BACKUPS,options.maxBackups);
            _maxFileSize = Mathf.Max(MIN_FILE_SIZE,options.maxFileSize);
            _keepExt = options.keepExt;
        }

        protected override void BeforeFileStreamCreate()
        {
            base.BeforeFileStreamCreate();
            var today = System.DateTime.Now.Date;
            if(File.Exists(path)){
                _creationTime = File.GetCreationTime(path);
                if(_creationTime.Date != today){
                    new DatedFileRoller(dir,fileName,_keepExt,_maxBackups).Roll();
                }
            }
        }

        protected override void OnFileStreamCreated()
        {
            base.OnFileStreamCreated();
            if(File.Exists(path)){
                _creationTime = File.GetCreationTime(path);
            }else{
                _creationTime = System.DateTime.Now;
            }
        }

        protected override bool ShouldRoll()
        {
             var date = System.DateTime.Now.Date;
             if(_creationTime.Date != date){
                 //date changed
                 return true;
             }
            if(this.Length >= _maxFileSize){
                //file size exceed
                return true;
            }
            return false;
        }

        protected override void StartRoll()
        {
            new DatedFileRoller(dir,fileName,_keepExt,_maxBackups).Roll();
        }
    }

}
