using System;
using MySql.Data.MySqlClient;

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
        }
    }

    public static class Command
    {
        static string connect = ReadConfig();

        /// <summary>
        /// 读取config.txt文件
        /// </summary>
        private static string ReadConfig()
        {
            try
            {
                return System.IO.File.ReadAllText("config.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("在读取文件的过程中发生了异常！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            Back();
            return "";
        }
        /// <summary>
        /// 退出控制台程序
        /// </summary>
        public static void Exit()
        {
            Console.WriteLine("再见~");
            Console.Write("按任意键退出...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public static void Read()
        {
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM student";
                // 读取数据
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                // 打印数据
                Console.WriteLine("ID\t\t姓名\t\t年龄");
                while (reader.Read())
                    Console.WriteLine($"{reader.GetUInt32("id")}\t\t{reader.GetString("name")}\t\t{reader.GetUInt32("age")}");
            }
            catch (Exception e)
            {
                Console.WriteLine("读取数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public static void Write()
        {
            // 输入数据
            Student student = new Student();
            Console.WriteLine("请输入学生姓名");
            Console.Write("name > ");
            student.name = Console.ReadLine();
            Console.WriteLine("请输入学生年龄");
            Console.Write("age > ");
            while (!int.TryParse(Console.ReadLine(), out student.age))
            {
                Console.WriteLine("输入错误！请重新输入");
                Console.Write("age > ");
            }
            // 存储数据
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = $"INSERT INTO student(name, age) VALUES('{student.name}', {student.age})";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("数据添加完成！");

            }
            catch (Exception e)
            {
                Console.WriteLine("添加数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }

        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static void Change()
        {
            // 输入数据
            Console.WriteLine("请输入学生ID");
            int id;
            Console.Write("ID > ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("输入错误！");
                Console.Write("ID > ");
            }
            Student student = new Student();
            Console.WriteLine("请输入学生姓名");
            Console.Write("name > ");
            student.name = Console.ReadLine();
            Console.WriteLine("请输入学生年龄");
            Console.Write("age > ");
            while (!int.TryParse(Console.ReadLine(), out student.age))
            {
                Console.WriteLine("输入错误！请重新输入");
                Console.Write("age > ");
            }

            // 存储数据
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = $"UPDATE student SET name='{student.name}',age='{student.age}' WHERE id = {id}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("数据更改完成！");
            }
            catch (Exception e)
            {
                Console.WriteLine("更改数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public static void Remove()
        {
            // 输入数据
            Console.WriteLine("请输入学生ID");
            int id;
            Console.Write("ID > ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("输入错误！");
                Console.Write("ID > ");
            }

            // 删除数据
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = $"DELETE FROM student WHERE id={id}";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                sql = "ALTER TABLE student DROP id;";
                MySqlCommand cmd1 = new MySqlCommand(sql, conn);
                cmd1.ExecuteNonQuery();
                sql = "ALTER TABLE student ADD id int NOT NULL FIRST;";
                MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                cmd2.ExecuteNonQuery();
                sql = "ALTER TABLE student MODIFY COLUMN id int NOT NULL AUTO_INCREMENT,ADD PRIMARY KEY(id);";
                MySqlCommand cmd3 = new MySqlCommand(sql, conn);
                cmd3.ExecuteNonQuery();
                Console.WriteLine("数据删除完成！");

            }
            catch (Exception e)
            {
                Console.WriteLine("删除数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }
        }

        public static void Clean()
        {
            Console.WriteLine("此操作不可逆。确定要清除吗？");
            char ch;
            Console.Write("y/n > ");
            while (!char.TryParse(Console.ReadLine(), out ch))
            {
                Console.WriteLine("输入错误");
                Console.Write("y/n > ");
            }
            if (ch != 'y')
            {
                Console.WriteLine("已取消清除操作。");
                Back();
                return;
            }

            // 清空数据
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = $"TRUNCATE TABLE student";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("数据清空完成！");
            }
            catch (Exception e)
            {
                Console.WriteLine("清空数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }
        }

        /// <summary>
        /// 与数据库建立连接
        /// </summary>
        /// <param name="connect"></param>
        /// <returns></returns>
        public static MySqlConnection CreateMySql(string connect)
        {
            MySqlConnection conn = new MySqlConnection(connect);
            return conn;
        }

        /// <summary>
        /// 学生结构体
        /// </summary>
        private struct Student
        {
            public string name;
            public int age;
        }

        /// <summary>
        /// 返回
        /// </summary>
        public static void Back()
        {
            Console.Write("请按任意键返回...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
