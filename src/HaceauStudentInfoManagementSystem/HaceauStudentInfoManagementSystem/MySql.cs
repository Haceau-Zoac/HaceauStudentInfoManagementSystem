using System;
using MySql.Data.MySqlClient;

namespace Haceau.StudentInformationManagementSystem
{
    public static class MySql
    {
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
        /// 创建读取数据库的MySqlDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static MySqlDataReader ReadData(string sql, MySqlConnection conn) =>
            new MySqlCommand(sql, conn).ExecuteReader();

        /// <summary>
        /// 执行修改数据的sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="conn"></param>
        public static void WriteData(string sql, MySqlConnection conn) =>
            new MySqlCommand(sql, conn).ExecuteNonQuery();
    }
}
