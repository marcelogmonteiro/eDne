using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WebAPIWithSwagger
{
    public class SQLHelper
    {
        private static string _connectionString;

        public SQLHelper(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public object ExecuteScalar(CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);

                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
        }

        public DataTable Fill(CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);

                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dad.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
        }

        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);

                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dad.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        public IDataReader ExecuteReader(CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            IDbConnection conn = new SqlConnection(_connectionString);

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);

            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        private void PrepareCommand(SqlCommand cmd, IDbConnection conn, IDbTransaction trans, CommandType cmdType, string cmdText, List<IDataParameter> cmdParms)
        {
            if (!(conn.State == ConnectionState.Open))
                conn.Open();

            cmd.Connection = (SqlConnection)conn;
            cmd.CommandText = cmdText;
            //cmd.CommandTimeout = HelperFactory.CommandTimeout;

            if ((trans != null))
            {
                cmd.Transaction = (SqlTransaction)trans;
            }

            cmd.CommandType = cmdType;

            if ((cmdParms != null))
            {
                foreach (IDataParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}
