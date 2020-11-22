using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MS.FileRoller{

    public class DateRollingFileStream:BaseRollingFileStream
    {

        public class Options{
            public int maxBackups = 3;
            public int maxFileSize = 1024 * 10;
            public bool keepExts = false;
        }

        private int _maxBackups;
        private int _maxFileSize;

        private bool _keepExts;

        private System.DateTime _writingDate;

        public DateRollingFileStream(string filePath,Options options):base(filePath){
            _maxBackups = Mathf.Max(1,options.maxBackups);
            _maxFileSize = Mathf.Max(1024 * 10,options.maxFileSize);
            _keepExts = options.keepExts;
        }


        protected override void OnFileStreamCreated()
        {
            base.OnFileStreamCreated();
            _writingDate = System.DateTime.Now.Date;
        }

        protected override bool ShouldRoll()
        {
             var date = System.DateTime.Now.Date;
             if(_writingDate != date){
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
            var date = System.DateTime.Now.Date;
            var dateStr = date.ToString("yyyyMMdd");
            var datedFiles = FileRollUtil.GetDatedBackupFiles(dir,fileName,_keepExts);
            while(datedFiles.Count >= _maxBackups){
                var oldest = datedFiles[datedFiles.Count - 1];
                datedFiles.RemoveAt(datedFiles.Count - 1);
                File.Delete(oldest.path);
            }
            var todayFileCount = CountOfFilesToday(datedFiles);

            var todayFileName = FileRollUtil.GetDatedFileName(fileName,date,0,_keepExts);
            FileRollUtil.RollFiles(dir,todayFileName,todayFileCount + 1,_keepExts);
            if(File.Exists(path)){
                File.Move(path,Path.Combine(dir,todayFileName));
            }
        }

        private int CountOfFilesToday(List<DatedFileInfo> fileInfos){
            var date = System.DateTime.Now.Date;
            int count = 0;
            foreach(var f in fileInfos){
                if(f.date == date){
                    count ++;
                }
            }
            return count;
        }


    }

}
