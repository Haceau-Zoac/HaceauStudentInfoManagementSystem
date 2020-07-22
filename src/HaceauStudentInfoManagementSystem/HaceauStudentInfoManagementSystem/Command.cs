using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using SimpleExcel;

namespace Haceau.StudentInformationManagementSystem
{
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
                return File.ReadAllText("config.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("在读取文件的过程中发生了异常！请检查config.txt");
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
            // 添加数据
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

        /// <summary>
        /// 清空数据
        /// </summary>
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
                string sql = "TRUNCATE TABLE student";
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
        /// 导出数据
        /// </summary>
        public static void FileExit()
        {
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM student";
                // 读取数据
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                // 导出格式
                int file = 1;
                Console.WriteLine("导出文件格式为？");
                Console.WriteLine("1. txt文件");
                Console.WriteLine("2. excel文件");
                Console.Write("File > ");
                while (!int.TryParse(Console.ReadLine(), out file))
                {
                    Console.WriteLine("输入错误！");
                    Console.Write("File > ");
                }
                // 导出数据
                int mode = 0;
                // txt文件
                if (file == 1)
                {
                    if (File.Exists($@"{Environment.CurrentDirectory}\student.txt"))
                    {
                        Console.WriteLine("student.txt文件已存在！");
                        Console.WriteLine("1.覆盖文件");
                        Console.WriteLine("2.在末尾追加");
                        Console.WriteLine("3.取消操作");
                        Console.Write("Mode > ");
                        while ((!int.TryParse(Console.ReadLine(), out mode)) || (mode != 1 && mode != 2 && mode != 3))
                        {
                            Console.WriteLine("输入错误！");
                            Console.Write("Mode > ");
                        }
                    }
                    if (mode == 3)
                        return;
                    FileStream fs = new FileStream($@"student.txt", (mode == 0) ? FileMode.Create : (mode == 1) ? FileMode.Truncate : FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write("ID\t\t\t姓名\t\t\t年龄\n");
                    while (reader.Read())
                        sw.Write($"{reader.GetUInt32("id")}\t\t\t{reader.GetString("name")}\t\t\t{reader.GetUInt32("age")}\n");
                    sw.Close();
                    fs.Close();
                    Console.WriteLine("已导出到exe文件根目录下的student.txt！");
                }
                // excel文件
                else
                {
                    if (File.Exists($@"{Environment.CurrentDirectory}\student.xls"))
                    {
                        Console.WriteLine("student.xls文件已存在！");
                        Console.WriteLine("1.覆盖文件");
                        Console.WriteLine("2.取消操作");
                        Console.Write("Mode > ");
                        while ((!int.TryParse(Console.ReadLine(), out mode)) || (mode != 1 && mode != 2))
                        {
                            Console.WriteLine("输入错误！");
                            Console.Write("Mode > ");
                        }
                    }
                    if (mode == 1)
                        File.Delete("student.xls");
                    else if (mode == 2)
                        return;
                    WorkBook workBook = new WorkBook();
                    Sheet sheet = workBook.NewSheet("student");
                    sheet.Rows[0][0].Value = "ID";
                    sheet.Rows[0][1].Value = "姓名";
                    sheet.Rows[0][2].Value = "年龄";
                    for (int i = 1; reader.Read(); ++i)
                    {
                        sheet.Rows[i][0].Value = reader.GetUInt32("id").ToString();
                        sheet.Rows[i][1].Value = reader.GetString("name");
                        sheet.Rows[i][2].Value = reader.GetUInt32("age").ToString();
                    }

                    workBook.Save($@"{Environment.CurrentDirectory}\student.xls");
                    Console.WriteLine("已导出到exe文件根目录下的student.xls！");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("导出数据时发生了错误！");
                Console.WriteLine($"错误信息：{e.Message}");
            }
            finally
            {
                conn.Clone();
                Back();
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static void FileIn()
        {
            Console.WriteLine("导入方式？");
            Console.WriteLine("1.覆盖");
            Console.WriteLine("2.在结尾添加");
            int mode;
            Console.Write("Mode > ");
            while (!int.TryParse(Console.ReadLine(), out mode))
            {
                Console.WriteLine("输入错误！");
                Console.Write("Mode > ");
            }
            Console.WriteLine("文件路径？");
            Console.Write("File > ");
            string file = Console.ReadLine();
            if (!File.Exists(file))
            {
                Console.WriteLine("文件不存在！");
                Console.Write("File > ");
                file = Console.ReadLine();
            }
            FileStream fs = new FileStream($@"{file}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            // 读取数据
            MySqlConnection conn = CreateMySql(connect);
            try
            {
                conn.Open();
                string sql = "";
                MySqlCommand cmd;
                if (mode == 1)
                {
                    sql = "TRUNCATE TABLE student";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                string line;
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> strArr = new List<string>();
                    strArr = line.Split(new string[] { "\t\t\t" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    sql = $"INSERT INTO student(name, age) VALUES('{strArr[1]}', {strArr[2]})";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("数据读取完成！");

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
