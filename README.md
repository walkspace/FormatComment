# 简介

Visual Studio 2022 的扩展，主要用于写代码时，对齐注释文字。

# 功能

1. Tab Right

   注释往右对齐，用于在代码行后面的注释文字

2. Tab Left

   注释往左对齐，用于在代码行后面的注释文字

   **当注释已经对齐, 则 Tab Right 会使所有注释往右一个 Tab 宽度, Tab Left 会使所有注释往左一个 Tab 宽度**

```C#
   int width = 100;        // 宽度
   int height = 100;       // 高度
```

3. Comment To C

   把选择的注释变成 C 语言的注释风格，用于在代码行上面的注释文字，比如:

```C#
   /*------------------------------------------------------------------------------------*/

   /* 注释代码                                                                            */

   /*------------------------------------------------------------------------------------*/
   public void Function() { }
```

4. 安装完成后，请自定义设置快捷键

   [工具] - [选项] - [环境] - [键盘]，输入 FormatComment 来搜索，建议设置：

   Tools.FormatCommentCommentToC : <kbd>Ctrl</kbd> + <kbd>M</kbd>, <kbd>Ctrl</kbd> + <kbd>Z</kbd>

   Tools.FormatCommentTabRight   : <kbd>Ctrl</kbd> + <kbd>M</kbd>, <kbd>Ctrl</kbd> + <kbd>Tab</kbd>

   Tools.FormatCommentTabLeft    : <kbd>Ctrl</kbd> + <kbd>M</kbd>, <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>Tab</kbd>

5. 配置

   [工具] - [选项] - [FormatComment]

```C#
   CommentText    : -      // 转成 C 语言注释风格的包围符号
   MaximumColumn  : 120    // 转成 C 语言注释风格的最大列数，实际代码超出，则以其为主
   TabSpace       : 4      // 单个 Tab 键转成的空格个数*
```

# 下载

地址：[https://github.com/walkspace/FormatComment/wiki]

# Visual Studio 扩展开发

  地址：[https://learn.microsoft.com/zh-cn/visualstudio/extensibility/vsix/get-started/get-tools?view=vs-2022]

  文档：[doc/visualstudio-extensibility-vsix-vs-2022.pdf]


## 扩展开发 ##

1. 打开 Visual Studio Installer, 安装 "Visual Studio 扩展开发"

   ![如图所示](doc/1.jpg)


2. 安装扩展 "Extensibility Essentials 2022"
   
   [扩展(X)]-[管理扩展]-[联机]-[搜索]


3. 确认项目中是否已安装 NuGet 程序包：

   Community.VisualStudio.Toolkit.17, Community.VisualStudio.VSCT, Microsoft.VSSDK.BuildTools
