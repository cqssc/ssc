using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DBHelp
{
    /// <summary>
    /// DataBase Class
    /// </summary>
    public abstract class DbHelperSQL
    {
        public DbHelperSQL()
        {
        }

        #region Procedure operation


        public static SqlConnection GetConnection(string strConn)
        {
            return new SqlConnection(strConn);
        }

        public static SqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            return new SqlDataAdapter(strSql, objConn as SqlConnection);
        }

        public static SqlCommandBuilder GetCommandBuilder(IDbDataAdapter objAdapter)
        {
            return new SqlCommandBuilder(objAdapter as SqlDataAdapter);
        }

        /// <summary>
        /// Excute Procedure
        /// </summary>
        /// <param name="storedProcName">name</param>
        /// <param name="parameters">parameter</param>
        /// <param name="tableName">table name for DataSet</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string connectionString, string storedProcName,
            IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                connection.Dispose();
                return dataSet;
            }
        }

        public static bool IsDBConnected(string connstr)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connstr);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Excute Reader
        /// </summary>
        /// <param name="sql">string to be excute</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader GetDataReader(string connstr, string sql)
        {
            SqlConnection connection = new SqlConnection(connstr);
            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandTimeout = 0;
            SqlDataReader sdr = cmd.ExecuteReader();

            return sdr;
        }

        public static DataSet GetDataSet(string connstr, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(sql, connection);
                da.Fill(ds);

                connection.Close();
                connection.Dispose();

                return ds;
            }
        }
        /// <summary>
        /// Excute Sql
        /// </summary>
        /// <param name="sql">Excute Sql</param>
        /// <returns>void</returns>
        public static void ExcuteSql(string connstr, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                connection.Open();
                SqlCommand cmd = BuildQueryCommand(connection, sql);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                connection.Close();
                connection.Dispose();
            }
        }

        public static int ExecuteScalar(string connstr, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                connection.Open();
                SqlCommand cmd = BuildQueryCommand(connection, sql);
                cmd.CommandTimeout = 0;
                int r = (int)cmd.ExecuteScalar();
                connection.Close();
                connection.Dispose();

                return r;
            }
        }
        /// <summary>
        /// Excute Procedure,return rows to affect
        /// </summary>
        /// <param name="storedProcName">name</param>
        /// <param name="parameters">parameter</param>
        /// <param name="rowsAffected">rows to affect</param>
        /// <returns></returns>
        public static int RunProcedure(string connectionString, string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                command.CommandTimeout = 0;
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                connection.Close();
                connection.Dispose();
                return result;
            }
        }
        /// <summary>
        /// Create SqlCommand (return a int)	
        /// </summary>
        /// <param name="storedProcName">name</param>
        /// <param name="parameters">parameter</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandTimeout = 0;
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        /// <summary>
        /// Create SqlCommand (return a DataSet)
        /// </summary>
        /// <param name="connection">Connection String</param>
        /// <param name="storedProcName">name</param>
        /// <param name="parameters">parameter</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandTimeout = 0;
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        private static SqlCommand BuildQueryCommand(SqlConnection connection, string sql)
        {
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;
            command.CommandType = CommandType.Text;

            return command;
        }

        #endregion
    }
}
