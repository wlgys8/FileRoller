# FileRoller

简单的文件滚动备份实现.

# Install

```json
"com.ms.fileroller":"https://github.com/wlgys8/FileRoller.git"
```

# Usage


## 1. IndexedFileRoller

```csharp

var roller = new IndexedFileRoller("./","app.log",true,3);
roller.Roll();

```

以上代码，会将对目录`./`下的文件进行如下操作:

```
delete app3.log
app.2.log -> app.3.log
app.1.log -> app.2.log
app.log -> app.1.log

```

### 构造参数

- dir 指定目录
- fileName 原文件名
- keepExt 进行备份的时候，是否保持后缀

        如果不保持后缀,那么app.log会被备份为app.log.1，否则就是app.1.log
- maxNumOfBackups 最多备份文件数量


## 2. DatedFileRoller

备份文件的时候，会在文件名上添加日期.

```csharp
var roller = new DatedFileRoller("./","app.log",true,3);
roller.Roll();

```
以上代码，会将对目录`./`下的文件进行如下操作:

```
app.log -> app.{yyyyMMdd}.log
```

如果 `app.{yyyyMMdd}.log`已经存在，那么会按照IndexedFileRoller的规则，以此往后备份，即

```
app.{yyyyMMdd}.log -> app.{yyyyMMdd}.1.log
```

对于超出`maxNumOfBackups`数量的备份文件，都会予以删除。

## 3. IndexedRollingFileStream



## 4. DatedRollingFileStream


