using System;

namespace Haceau.StudentInformationManagementSystem
{
    class Run
    {
        /// <summary>
        /// 主函数
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Haceau-学生信息管理系统1.0.0");

            // 获取输入
            int command;
            while (true)
            {
                Prompt();
                Console.Write(">>> ");
                while (!int.TryParse(Console.ReadLine(), out command))
                {
                    Console.WriteLine("输入错误！");
                    Console.Write(">>> ");
                }

                // 判断输入
                switch (command)
                {
                    case 0:
                        Command.Exit();
                        break;
                    case 1:
                        Command.Read();
                        break;
                    case 2:
                        Command.Write();
                        break;
                    case 3:
                        Command.Change();
                        break;
                    case 4:
                        Command.Remove();
                        break;
                    case 5:
                        Command.Clean();
                        break;
                    case 6:
                        Command.FileExit();
                        break;
                    default:
                        Console.WriteLine("输入错误！");
                        Command.Back();
                        break;
                }
            }
        }

        /// <summary>
        /// 菜单
        /// </summary>
        public static void Prompt()
        {
            Console.WriteLine("0.退出");
            Console.WriteLine("1.查看");
            Console.WriteLine("2.添加");
            Console.WriteLine("3.修改");
            Console.WriteLine("4.删除");
            Console.WriteLine("5.清空");
            Console.WriteLine("6.导出");
        }
    }
}
