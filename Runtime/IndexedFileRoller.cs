using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

namespace MS.FileRoller{
    
    internal class IndexedFileRoller:BaseFileRoller<IndexedFileRoller.IndexedFileInfo>
    {

        public class IndexedFileInfo{
            public int index;
        }

        private bool _keepExt;
        private string _dir;
        private string _fileName;

        public IndexedFileRoller(string dir,string fileName,bool keepExt,int maxNumOfBackups)
        :base(dir,CreateBackupFileNamePattern(fileName,keepExt),maxNumOfBackups){
            _keepExt = keepExt;
            _dir = dir;
            _fileName = fileName;
        }

        public static string CreateBackupFileNamePattern(string fileName,bool keepExt){
            string baseFileName;
            string ext;
            if(keepExt){
                FileRollUtil.GetFileNameAndExt(fileName,out baseFileName,out ext);
            }else{
                baseFileName = fileName;
                ext = null;
            }
            baseFileName = baseFileName.Replace(".","\\.");
            string pattern = $"({baseFileName})(\\.([0-9]*))?";
            if(ext != null){
                pattern += $"\\.({ext})";
            }
            return pattern;
        }

        public static readonly System.Func<Match,IndexedFileInfo> fileInfoExtractor = (match)=>{
            return new IndexedFileInfo(){
                index = IndexedFileRoller.GetIndex(match)
            };
        };

        private static int GetIndex(Match match){
            var indexGroup = match.Groups[3];
            var index = 0;
            if(indexGroup.Success){
                index = int.Parse(indexGroup.Value);
            }
            return index;
        }

        protected override int CompareFile(IndexedFileInfo m1, IndexedFileInfo m2)
        {
            return m1.index - m2.index;
        }

        protected override string ResolveRollName(IndexedFileInfo m)
        {
            if(!_keepExt){
                return Path.Combine(_dir,$"{_fileName}.{m.index + 1}");
            }else{
                string baseFileNameWithoutExt,ext;
                FileRollUtil.GetFileNameAndExt(_fileName,out baseFileNameWithoutExt,out ext);
                var path = Path.Combine(_dir,$"{baseFileNameWithoutExt}.{m.index + 1}");
                if(ext != null){
                    path += $".{ext}";
                }
                return path;
            }
        }

        protected override IndexedFileInfo ExtractFileInfo(Match match)
        {
            var index = GetIndex(match);
            return new IndexedFileInfo(){
                index = index
            };
        }
    }
}
