using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace MS.FileRoller{
    public abstract class BaseRollingFileStream
    {

        private string _dir;
        private string _fileName;    
        private string _filePath;
        private FileStream _fileStream;

        public BaseRollingFileStream(string filePath){
            _filePath = filePath;
            _dir = Path.GetDirectoryName(filePath);
            _fileName = Path.GetFileName(filePath);
        }

        public string path{
            get{
                return _filePath;
            }
        }

        public string dir{
            get{
                return _dir;
            }
        }

        public string fileName{
            get{
                return _fileName;
            }
        }

        private FileStream EnsureFileStream(){
            if(_fileStream == null){
                if(!Directory.Exists(_dir)){
                    Directory.CreateDirectory(_dir);
                }
                BeforeFileStreamCreate();
                _fileStream = new FileStream(this._filePath,FileMode.Append);
                OnFileStreamCreated();
            }
            return _fileStream;
        }

        public void Close(){
            if(_fileStream != null){
                _fileStream.Close();
                _fileStream = null;
                OnFileStreamClosed();
            }
        }

        public long Length{
            get{
                return _fileStream == null ? 0 : _fileStream.Length;
            }
        }


        public async Task WriteAsync(byte[] buffer,int offset,int count){
            var fs = EnsureFileStream();
            await fs.WriteAsync(buffer,offset,count);
            await RollAsyncIfNeed();
        }

        public Task FlushAsync(){
            if(_fileStream != null){
                return _fileStream.FlushAsync();
            }
            return Task.CompletedTask;
        }

        public void Roll(){
            Close();
            StartRoll();            
        }
        

        private async Task RollAsyncIfNeed(){
            if(!ShouldRoll()){
                return;
            }
            await FlushAsync();
            Close();
            StartRoll();
        }


        protected abstract bool ShouldRoll();

        protected abstract void StartRoll();

        protected virtual void BeforeFileStreamCreate(){}

        protected virtual void OnFileStreamCreated(){}
        
        protected virtual void OnFileStreamClosed(){}
        

    }
}
