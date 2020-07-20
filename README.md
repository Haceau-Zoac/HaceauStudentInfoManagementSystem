Haceau-学生信息管理系统
===
![Language](https://img.shields.io/badge/Language-C%23-blue.svg?style=flat-square) ![.Net Framework](https://img.shields.io/badge/.Net&nbsp;Framework-4.7.2-blue.svg?style=flat-square) ![MySQL](https://img.shields.io/badge/MySQL-5.7-skyblue.svg?style=flat-square)

简介
---
该项目为个人练习制作的学生信息管理系统，使用\.Net Framework4.7.2平台上的C#语言编写，数据存储于MySQL。当前版本为1.0.0。使用了MySql.Data及其依赖的NuGet包。开源协议为MIT。保持更新中。

食用方法
-------
`前提`
* 安装MySQL
___

`clone项目到本地`

在控制台输入：

		git clone https://github.com/Haceau-Zoac/HaceauStudentInfoManagementSystem
___

`编辑config.txt`

编辑文件内容为：

		server=<IP>;port=<端口>;database=<数据库名>;user=<用户名>;password=<密码>;SslMode=none;
例：

		server=localhost;port=3306;database=test;user=root;password=pwd;SslMode=none;
* 注：数据库中须有名为“student”的数据表。

待办清单
-------
|版本|内容|
|---|---|
|1.1.0|导出数据|
|1.2.0|导入数据|
|???|???|