# 功能

1. Tab Right : 注释往右对齐

2. Tab Left  : 注释往左对齐

   当注释已经对齐, 则 Tab Right 会使所有注释往右一个 Tab 宽度, Tab Left 会使所有注释往左一个 Tab 宽度

2. Comment To C: 把选择的注释变成 C 语言的注释风格，比如:

   /*-----------------------------------------------------------------------------------------------------------*/

   /* 注释代码                                                                                                   */

   /*-----------------------------------------------------------------------------------------------------------*/


3. 安装完成后，请自定义配置快捷键

   工具 - 选项 - 环境 - 键盘，输入 FormatComment 来搜索，建议设置：

   Tools.FormatCommentCommentToC : Ctrl + M, Ctrl + Z

   Tools.FormatCommentTabRight   : Ctrl + M, Ctrl + Tab

   Tools.FormatCommentTabLeft    : Ctrl + M, Ctrl + Shift + Tab


# Visual Studio 扩展开发

  地址：[https://learn.microsoft.com/zh-cn/visualstudio/extensibility/vsix/get-started/get-tools?view=vs-2022]

  文档：![本地文档](doc/visualstudio-extensibility-vsix-vs-2022.pdf)


## 开发指南 ##

1. 打开 Visual Studio Installer, 安装 "Visual Studio 扩展开发"

   ![如图所示](doc/1.jpg)


2. 安装扩展 "Extensibility Essentials 2022"
   
   [扩展(X)]-[管理扩展]-[联机]-[搜索]


3. 确认项目中是否已安装 NuGet 程序包：

   Community.VisualStudio.Toolkit.17, Community.VisualStudio.VSCT, Microsoft.VSSDK.BuildTools