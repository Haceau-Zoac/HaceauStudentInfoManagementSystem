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
                ErrorMessage(e, "请检查config.txt");
            }
            Back();
            return "";
        }

        /// <summary>
        /// 进行错误捕捉
        /// </summary>
        /// <param name="func">进行捕捉的函数</param>
        public static void TryCatch(Action<MySqlConnection> func)
        {
            MySqlConnection conn = MySql.CreateMySql(connect);
            try
            {
                conn.Open();
                func(conn);
            }
            catch (Exception e)
            {
                ErrorMessage(e);
            }
            finally
            {
                Back();
            }
        }

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="e"></param>
        /// <param name="other"></param>
        public static void ErrorMessage(Exception e, string other = "")
        {
            Console.WriteLine($"发生了异常！{other}");
            Console.WriteLine($"错误信息：{e.Message}");
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
        public static void Read(MySqlConnection conn)
        {
            string sql = "SELECT * FROM student";
            PrintData(MySql.ReadData(sql, conn));
        }

        /// <summary>
        /// 打印数据
        /// </summary>
        /// <param name="reader"></param>
        private static void PrintData(MySqlDataReader reader)
        {
            Console.WriteLine("ID\t\t姓名\t\t年龄");
            PlayReader(reader, Console.Write);
        }

        /// <summary>
        /// 使用reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="func">要使用的函数</param>
        private static void PlayReader(MySqlDataReader reader, Action<string> func)
        {
            while (reader.Read())
                func($"{reader.GetUInt32("id")}\t\t{reader.GetString("name")}\t\t{reader.GetUInt32("age")}\n");
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public static void Write(MySqlConnection conn)
        {
            Student student = InputStudentData();
            string sql = $"INSERT INTO student(name, age) VALUES('{student.name}', {student.age})";
            MySql.WriteData(sql, conn);
            Console.WriteLine("数据添加完成！");
        }

        /// <summary>
        /// 输入整数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="message"></param>
        private static void Input(out int input, string message)
        {
            PromptInput(message);
                while (!int.TryParse(Console.ReadLine(), out input))
                {
                    Console.WriteLine("输入错误！请重新输入");
                    PromptInput(message);
                }
        }

        /// <summary>
        /// 输入字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="message"></param>
        private static void Input(out char input, string message)
        {
            PromptInput(message);
            while (!char.TryParse(Console.ReadLine().ToLower(), out input))
            {
                Console.WriteLine("输入错误");
                PromptInput("Y/N");
            }
        }

        /// <summary>
        /// 提示输入
        /// </summary>
        /// <param name="message"></param>
        private static void PromptInput(string message) =>
            Console.Write($"{message} > ");

        /// <summary>
        /// 输入学生信息
        /// </summary>
        private static Student InputStudentData()
        {
            Student student = new Student();
            Console.WriteLine("请输入学生姓名");
            PromptInput("name");
            student.name = Console.ReadLine();
            Console.WriteLine("请输入学生年龄");
            Input(out student.age, "age");
            return student;
        }

        /// <summary>
        /// 输入学生ID
        /// </summary>
        /// <returns></returns>
        private static int InputStudentId()
        {
            Console.WriteLine("请输入学生ID");
            Input(out int id, "ID");
            return id;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static void Change(MySqlConnection conn)
        {
            int id = InputStudentId();
            Student student = InputStudentData();
            string sql = $"UPDATE student SET name='{student.name}',age='{student.age}' WHERE id = {id}";
            MySql.WriteData(sql, conn);
            Console.WriteLine("数据更改完成！");
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public static void Remove(MySqlConnection conn)
        {
            int id = InputStudentId();
            string sql = $"DELETE FROM student WHERE id={id}";
            MySql.WriteData(sql, conn);
            sql = "ALTER TABLE student DROP id;";
            MySql.WriteData(sql, conn);
            sql = "ALTER TABLE student ADD id int NOT NULL FIRST;";
            MySql.WriteData(sql, conn);
            sql = "ALTER TABLE student MODIFY COLUMN id int NOT NULL AUTO_INCREMENT,ADD PRIMARY KEY(id);";
            MySql.WriteData(sql, conn);
            Console.WriteLine("数据删除完成！");
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public static void Clean(MySqlConnection conn)
        {
            if (!Determine())
            {
                Cancel("已取消清除操作。");
                return;
            }

            string sql = "TRUNCATE TABLE student";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("数据清空完成！");
        }

        /// <summary>
        /// 确认执行
        /// </summary>
        /// <returns></returns>
        private static bool Determine()
        {
            Console.WriteLine("此操作不可逆。确定要清除吗？");
            Input(out char ch, "Y/N");
            return ch == 'y';
        }

        /// <summary>
        /// 给予提示并返回
        /// </summary>
        /// <param name="message"></param>
        private static void Cancel(string message)
        {
            Console.WriteLine(message);
            Back();
        }

        /// <summary>
        /// 选择文件扩展名
        /// </summary>
        /// <param name="isOut"></param>
        /// <returns></returns>
        private static bool FileSuffix(bool isOut)
        {
            Console.WriteLine($"导{(isOut ? '出' : '入')}格式为？");
            Console.WriteLine("1. txt文件");
            Console.WriteLine("2. excel文件");
            int file = 0;
            while (file != 1 && file != 2)
                Input(out file, "File");
            return file == 1;
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public static void FileExit(MySqlConnection conn)
        {
            string sql = "SELECT * FROM student";
            // txt文件
            if (FileSuffix(true))
                FileExitTxt(MySql.ReadData(sql, conn));
            // excel文件
            else
                FileExitExcel(MySql.ReadData(sql, conn));
        }

        /// <summary>
        /// 导出Txt文件（提示+导出）
        /// </summary>
        /// <param name="reader"></param>
        private static void FileExitTxt(MySqlDataReader reader)
        {
            int mode = PromptFileExit("txt", "覆盖文件", "在结尾添加", "取消操作");
            if (mode == 3)
            {
                Cancel("已取消导出操作。");
                return;
            }
            DoFileExitTxt(reader, mode);
        }

        /// <summary>
        /// 导出提示
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        private static int PromptFileExit(string suffix, string one, string two, string three = null)
        {
            int mode = 1;
            if (File.Exists($@"{Environment.CurrentDirectory}\student.{suffix}"))
            {
                Console.WriteLine($"student.{suffix}文件已存在！");
                mode = PromptFileInOut(one, two, three);
            }
            return mode;
        }

        /// <summary>
        /// 文件出入提示
        /// </summary>
        private static int PromptFileInOut(string one, string two, string three = null)
        {
            int mode = 1;
            Console.WriteLine($"1.{one}");
            Console.WriteLine($"2.{two}");
            Console.WriteLine($"{(three == null ? "" : $"3.{three}")}");
            do
                Input(out mode, "Mode");
            while (mode != 1 && mode != 2 && mode != 3);
            return mode;
        }

        /// <summary>
        /// 导出Txt文件
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="mode"></param>
        private static void DoFileExitTxt(MySqlDataReader reader, int mode)
        {
            FileStream fs = new FileStream($@"student.txt", (mode == 1) ? FileMode.Create : (mode == 2) ? FileMode.Truncate : FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write("ID\t\t\t姓名\t\t\t年龄\n");
            PlayReader(reader, sw.Write);
            sw.Close();
            fs.Close();
            Console.WriteLine("已导出到exe文件根目录下的student.txt！");
        }

        /// <summary>
        /// 导出Excel文件（提示+导出）
        /// </summary>
        /// <param name="reader"></param>
        private static void FileExitExcel(MySqlDataReader reader)
        {
            int mode = PromptFileExit("txt", "覆盖文件", "取消操作");
            if (mode == 2 || mode == 3)
                return;
            DoFileExitExcel(reader);
        }

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="reader"></param>
        private static void DoFileExitExcel(MySqlDataReader reader)
        {
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

        /// <summary>
        /// 导入数据
        /// </summary>
        public static void FileIn(MySqlConnection conn)
        {
            PromptFileIn(out int mode, out string file);
            if (mode == 3 || file == "exit")
                return;
            string sql;
            MySqlCommand cmd;
            if (mode == 1)
            {
                sql = "TRUNCATE TABLE student";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            // 导入txt
            if (FileSuffix(false))
                FileInTxt(conn, file);
            // 导入excel
            else
                FileInExcel(conn, file);
        }

        /// <summary>
        /// 输入文件提示
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="file"></param>
        private static void PromptFileIn(out int mode, out string file)
        {
            Console.WriteLine("导入方式？");
            mode = PromptFileInOut("覆盖", "在结尾添加", "取消");
            if (mode == 3)
            {
                file = null;
                return;
            }
            Console.WriteLine("文件路径？");
            Console.WriteLine("* 输入exit取消");
            PromptInput("File");
            file = Console.ReadLine();
            if (!File.Exists(file))
            {
                Console.WriteLine("文件不存在！");
                PromptInput("File");
                file = Console.ReadLine();
            }
        }

        /// <summary>
        /// 导入Txt文件
        /// </summary>
        private static void FileInTxt(MySqlConnection conn, string file)
        {
            FileStream fs = new FileStream($@"{file}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            string line;
            sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                List<string> strArr = new List<string>();
                strArr = line.Split(new string[] { "\t\t\t" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                string sql = $"INSERT INTO student(name, age) VALUES('{strArr[1]}', {strArr[2]})";
                MySql.WriteData(sql, conn);
            }
            Console.WriteLine("导入完成！");
        }

        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="file"></param>
        private static void FileInExcel(MySqlConnection conn, string file)
        {
            WorkBook workBook = new WorkBook(file);
            Sheet sheet = workBook.GetSheet(0);
            for (int i = 1; i < sheet.Rows.LastRowNum + 1; ++i)
            {
                string sql = $"INSERT INTO student(name, age) VALUES('{sheet.Rows[i][1].Value}', '{sheet.Rows[i][2].Value}')";
                MySql.WriteData(sql, conn);
            }
            Console.WriteLine("数据读取完成！");
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
        public static void Back(MySqlConnection conn = null)
        {
            if (conn != null) conn.Clone();
            Console.Write("请按任意键返回...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
