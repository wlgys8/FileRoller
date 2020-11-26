using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MS.FileRoller{


    public static class FileRollUtil
    {

        public static bool GetFileNameAndExt(string baseFileName,out string fileNameWithoutExt,out string ext){
            var dotIndex = baseFileName.LastIndexOf('.');
            if(dotIndex < 0){
                fileNameWithoutExt = baseFileName;
                ext = null;
                return false;
            }else{
                ext = baseFileName.Substring(dotIndex + 1);
                fileNameWithoutExt = baseFileName.Substring(0, dotIndex);
                return true;
            }
        }

    }

}
