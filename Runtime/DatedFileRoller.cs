using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;

namespace MS.FileRoller{
    public class DatedFileRoller : BaseFileRoller<DatedFileRoller.DatedFileInfo>
    {

        public readonly string fileName;
        public readonly bool keepExt;

        public DatedFileRoller(string dir,string fileName,bool keepExt,int maxNumOfBackups)
        :base(dir,CreateBackupFileNamePattern(fileName,keepExt),maxNumOfBackups){
            this.fileName = fileName;
            this.keepExt = keepExt;
        }

        protected override int CompareFile(DatedFileInfo f1, DatedFileInfo f2)
        {
            if(!f1.hasDate && f2.hasDate){
                return -1;
            }else if(f1.hasDate && !f2.hasDate){
                return 1;
            }
            var res = f1.date.CompareTo(f2.date);
            if(res != 0){
                //新的文件排前面
                return - res;
            }
            //小的序号排前面
            return f1.index.CompareTo(f2.index);
        }

        protected override DatedFileInfo ExtractFileInfo(Match match)
        {
            var dateGroup = match.Groups[2];
            var hasDate = dateGroup.Success;
            System.DateTime date = default(System.DateTime);
            if(hasDate){
                if(System.DateTime.TryParseExact(dateGroup.Value,"yyyyMMdd",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None, out date)){
                }
            }
            int index = 0;
            var indexMatch = match.Groups[4];
            if(indexMatch.Success){
                var indexStr = indexMatch.Value;
                if(!int.TryParse(indexStr,out index)){
                    index = 0;
                }
            }
            return new DatedFileInfo(){
                hasDate = hasDate,
                date = date,
                index = index,
                fileName = match.Value,
            };
        }

        protected override string ResolveRollName(DatedFileInfo m)
        {

            var today = System.DateTime.Now.Date;
            if(!m.hasDate){
                var file = Path.Combine(dir,m.fileName);
                var fileCreateTime = File.GetCreationTime(file);
                return Path.Combine(dir,GetDatedFileName(fileName,fileCreateTime.Date,0,keepExt));
            }else{
                if(m.date != today){
                    return null;
                }
                return Path.Combine(dir,GetDatedFileName(fileName,m.date,m.index + 1,keepExt));
            }
        }

        public static string CreateBackupFileNamePattern(string fileName,bool keepExt){
            if(keepExt){
                string fileNameWithoutExt;
                string ext;
                FileRollUtil.GetFileNameAndExt(fileName,out fileNameWithoutExt,out ext);

                var escapedFileName = fileNameWithoutExt.Replace(".","\\.");
                if(ext == null){
                    //{fileName}.{yyyyMMdd}.{index}
                    return $"{escapedFileName}(\\.([0-9]*))?(\\.([0-9]*))?";
                }else{
                    //{fileName}.{yyyyMMdd}.{index}.{ext}
                    return $"{escapedFileName}(\\.([0-9]*))?(\\.([0-9]*))?\\.{ext}";
                }
            }else{
                var escapedFileName = fileName.Replace(".","\\.");
                return $"{escapedFileName}(\\.([0-9]*))?(\\.([0-9]*))?";
            }
        }     

        public class DatedFileInfo{
            public System.DateTime date;
            public int index;

            public bool hasDate;
            public string fileName;
        }


        public static string GetDatedFileName(string baseFileName,System.DateTime date,int index = 0,bool keepExts = false){
            var dateStr = date.ToString("yyyyMMdd");
            if(!keepExts){
                if(index == 0){
                    return $"{baseFileName}.{dateStr}";
                }else{
                    return $"{baseFileName}.{dateStr}.{index}";
                }
            }else{
                string fileNameWithoutExt,ext;
                if(FileRollUtil.GetFileNameAndExt(baseFileName,out fileNameWithoutExt,out ext)){
                    if(index == 0){
                        return $"{fileNameWithoutExt}.{dateStr}.{ext}";
                    }else{
                        return $"{fileNameWithoutExt}.{dateStr}.{index}.{ext}";
                    }
                }else{
                    if(index == 0){
                        return $"{baseFileName}.{dateStr}";
                    }else{
                        return $"{baseFileName}.{dateStr}.{index}";
                    }
                }
            }
        }
    }
}
