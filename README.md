# Visual Studio 扩展开发

技术文档：https://learn.microsoft.com/zh-cn/visualstudio/extensibility/?view=vs-2022


## 注意事项 ##

1. 使用 VS 社区工具包：Visual Studio Community Toolkit

2. 清理实验环境：
   执行命令：<VSSDK installation>\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe /Reset /VSInstance=<version> /RootSuffix=Exp && PAUSE

#  说明
   1. 参数：<VSSDK installation>
      本机为：C:\Program Files\Microsoft Visual Studio\2022\Professional\VSSDK

   2. 参数：<version>
      填写 Visual Studio 的版本号，具体取决于您计算机上安装的 Visual Studio 版本，本机为：17.0_1b76d7d4
      可在目录 "C:\Users\xzhang\AppData\Local\Microsoft\VisualStudio" 中查看

   3. 参数：RootSuffix
      /RootSuffix=Exp         ：用于创建或访问实验性实例。
      /RootSuffix=Roslyn      ：用于创建或访问使用 Roslyn 编译器的实例。
      /RootSuffix=Preview     ：用于创建或访问预览版本实例。
      /RootSuffix=CustomSuffix：您可以使用自定义的后缀名称来创建具有唯一标识符的实例。

#  操作
   > cd "C:\Program Files\Microsoft Visual Studio\2022\Professional\VSSDK\VisualStudioIntegration\Tools\Bin"
   > CreateExpInstance.exe /Reset /VSInstance=17.0_1b76d7d4 /RootSuffix=Exp && PAUSE
