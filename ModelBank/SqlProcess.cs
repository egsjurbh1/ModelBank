using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ModelBank
{
    class SqlProcess
    {
        /// <summary>
        /// 连接SQL服务器测试
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <returns></returns>
        public bool ConnectSQL(string connectionstring)
        {
            SqlConnection conn = null;  //  数据库连接对象。实现和数据库的连
            // 打开数据库连接
            conn = new SqlConnection(connectionstring);
            try
            {
                conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 执行sql语句，无返回数据，适用update、delete
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <param name="sql"></param>
        public bool ExecSingleSQL(string connectionstring, string sql)
        {
            SqlConnection conn = null;  //  数据库连接对象。实现和数据库的连接
            SqlCommand cmd = null;      //  数据库连接命令对象。指定执行的SQL语句
            // 打开数据库连接
            conn = new SqlConnection(connectionstring);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            // 执行sql，无返回
            cmd = new SqlCommand(sql, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ae)
            {
                MessageBox.Show(ae.Message.ToString());
            }
            conn.Close();
            return true;
        }

        /// <summary>
        /// 执行sql语句，返回dt，适用select
        /// </summary>
        /// <param name="connectionstring"></param>
        /// <param name="sql"></param>
        /// <param name="dt"></param>
        public bool ExecSingleSQL(string connectionstring, string sql, DataTable dt)
        {
            SqlConnection conn = null;  //  数据库连接对象。实现和数据库的连接
            // 打开数据库连接
            conn = new SqlConnection(connectionstring);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            // 执行sql，返回dt
            SqlDataAdapter adpt = new SqlDataAdapter(sql, conn);

            adpt.Fill(dt);
            conn.Close();
            return true;
        }
    }
}
