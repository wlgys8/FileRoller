using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace MS.FileRoller{
    public static class FileRollUtil
    {

        /// <summary>
        /// 
        /// filename.log -> filename.log.1 <br/>
        /// filename.log.1 -> filename.log.2 <br/>
        /// filename.log.{maxNumOfBackups} will be overrite written.
        /// 
        /// </summary>
        public static void RollFiles(string dir,string fileName,int maxNumOfBackups,bool keepExts = false){
            for(var i = maxNumOfBackups - 1; i >=0 ; i --){
                string srcPath = null;
                if(i == 0){
                    srcPath = Path.Combine(dir,fileName);
                }else{
                    srcPath =   Path.Combine(dir,AppendIndexToFileName(fileName,i,keepExts));
                }
                var dstPath = Path.Combine(dir,AppendIndexToFileName(fileName,i+1,keepExts));
                if(File.Exists(dstPath)){
                    File.Delete(dstPath);
                }
                if(File.Exists(srcPath)){
                    File.Move(srcPath,dstPath);
                }
            }
        }

        private static string AppendIndexToFileName(string baseFileName,int index,bool keepExts){
            if(!keepExts){
                return $"{baseFileName}.{index}";
            }else{
                var dotIndex = baseFileName.LastIndexOf('.');
                if(dotIndex < 0){
                    return $"{baseFileName}.{index}";
                }else{
                    var ext = baseFileName.Substring(dotIndex + 1);
                    var fileNameWithoutExt = baseFileName.Substring(0,baseFileName.Length - dotIndex);
                    return $"{fileNameWithoutExt}.{index}.{ext}";
                }
            }
        }

        private static bool GetFileNameAndExt(string baseFileName,out string fileNameWithoutExt,out string ext){
            var dotIndex = baseFileName.LastIndexOf('.');
            if(dotIndex < 0){
                fileNameWithoutExt = baseFileName;
                ext = null;
                return false;
            }else{
                ext = baseFileName.Substring(dotIndex + 1);
                fileNameWithoutExt = baseFileName.Substring(0,baseFileName.Length - dotIndex);
                return true;
            }
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
                if(GetFileNameAndExt(baseFileName,out fileNameWithoutExt,out ext)){
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

        public static List<DatedFileInfo> GetDatedBackupFiles(string dir,string baseFileName,bool keepExts = false){
            Regex regex = null;
            if(keepExts){
                var dotIndex = baseFileName.LastIndexOf('.');
                if(dotIndex < 0){
                    regex = new Regex($"{baseFileName}.([0-9]*)(\\.([0-9]*))?");
                }else{
                    var ext = baseFileName.Substring(dotIndex + 1);
                    var fileNameWithoutExt = baseFileName.Substring(0,baseFileName.Length - dotIndex);
                    regex = new Regex($"{fileNameWithoutExt}.([0-9]*)(\\.([0-9]*))?\\.{ext}");
                }
            }else{
                regex = new Regex($"{baseFileName}.([0-9]*)(\\.([0-9]*))?");
            }
            var files = Directory.GetFiles(dir);
            var list = new List<DatedFileInfo>();
            foreach(var path in files){
                string fileName = Path.GetFileName(path);
                var match = regex.Match(fileName);
                if(!match.Success){
                    continue;
                }
                string dateString = match.Groups[1].Value;
                System.DateTime date;
                if(System.DateTime.TryParseExact(dateString,"yyyyMMdd",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None, out date)){
                    int index = 0;
                    var indexMatch = match.Groups[3];
                    if(indexMatch.Success){
                        var indexStr = indexMatch.Value;
                        if(!int.TryParse(indexStr,out index)){
                            index = 0;
                        }
                    }
                    var fileInfo = new DatedFileInfo(){path = path,date = date,index = index};
                    list.Add(fileInfo);
                }
            }
            list.Sort(DatedFileInfoSort);
            return list;
        }

        private static int DatedFileInfoSort(DatedFileInfo f1,DatedFileInfo f2){
            var res = f1.date.CompareTo(f2.date);
            if(res != 0){
                //新的文件排前面
                return - res;
            }
            //小的序号排前面
            return f1.index.CompareTo(f2.index);
        }
    }

    public struct DatedFileInfo{
        public string path;
        public System.DateTime date;
        public int index;
    }
}
