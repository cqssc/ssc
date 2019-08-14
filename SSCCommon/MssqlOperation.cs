﻿//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    e73be456-8c5c-4549-9a23-a9d7b14fea60
//        CLR Version:              4.0.30319.42000
//        Name:                     MssqlOperation
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Common
//        File Name:                MssqlOperation
//
//        Created by Charley at 2019/8/8 12:17:38
//        http://www.netinfo.com 
//
//======================================================================

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace ShiShiCai.Common
{
    /// <summary>
    /// Sql Server数据库的通用操作
    /// 这里的方法都是静态的，可以通过类名直接引用
    /// </summary>
    public class MssqlOperation
    {
        /// <summary>
        /// 创建一个DBConnection对象
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <returns></returns>
        public static SqlConnection GetConnection(string strConn)
        {
            return new SqlConnection(strConn);
        }
        /// <summary>
        /// 创建一个数据库适配器
        /// </summary>
        /// <param name="objConn">DBConnection</param>
        /// <param name="strSql">查询语句</param>
        /// <returns></returns>
        public static SqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            return new SqlDataAdapter(strSql, objConn as SqlConnection);
        }
        /// <summary>
        /// 创建一个数据库适配器
        /// </summary>
        /// <param name="objCmd"></param>
        /// <returns></returns>
        public static SqlDataAdapter GetDataAdapter(IDbCommand objCmd)
        {
            return new SqlDataAdapter(objCmd as SqlCommand);
        }
        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="objConn"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static SqlCommand GetCommand(IDbConnection objConn, string strSql)
        {
            return new SqlCommand(strSql, objConn as SqlConnection);
        }
        /// <summary>
        /// 创建Command
        /// </summary>
        /// <returns></returns>
        public static SqlCommand GetCommand()
        {
            return new SqlCommand();
        }
        /// <summary>
        /// 创建一个CommandBuilder
        /// </summary>
        /// <param name="objAdapter">DataAdapter</param>
        /// <returns></returns>
        public static SqlCommandBuilder GetCommandBuilder(IDbDataAdapter objAdapter)
        {
            return new SqlCommandBuilder(objAdapter as SqlDataAdapter);
        }
        /// <summary>
        /// 测试数据库是否连接成功
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <returns></returns>
        public static OperationReturn TestDBConnection(string strConn)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            try
            {
                sqlConnection.Open();
                optReturn.Message = sqlConnection.Database;
                optReturn.Data = sqlConnection.ConnectionTimeout;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">查询语句</param>
        /// <returns></returns>
        public static OperationReturn GetDataSet(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(strSql, sqlConnection);
            DataSet objDataSet = new DataSet();
            try
            {
                sqlAdapter.Fill(objDataSet);
                optReturn.Data = objDataSet;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MSSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }

        /// <summary>
        /// 执行存储过程，获取数据集
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static OperationReturn GetDataSetFromStoredProcedure(string strConn, string procedureName,
          DbParameter[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = procedureName;
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            DataSet objDataSet = new DataSet();
            for (int i = 0; i < parameters.Length; i++)
            {
                sqlCmd.Parameters.Add(parameters[i]);
            }
            try
            {
                sqlConnection.Open();
                sqlAdapter.Fill(objDataSet);
                optReturn.Data = objDataSet;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MSSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }

        /// <summary>
        /// 执行存储过程，获取数据集
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static OperationReturn GetDataSetFromStoredProcedure(string strConn, string procedureName,
            DBCmdParameter[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = procedureName;
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            DataSet objDataSet = new DataSet();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                string paramName = parameter.Name;
                int dataType = parameter.DataType;
                int length = parameter.Length;
                int direction = parameter.Direction;
                string paramValue = parameter.ParamValue;
                DbParameter dbParam;
                if (length > 0)
                {
                    dbParam = GetDbParameter(paramName, (MssqlDataType)dataType, length);
                }
                else
                {
                    dbParam = GetDbParameter(paramName, (MssqlDataType)dataType);
                }
                dbParam.Direction = (ParameterDirection)direction;
                dbParam.Value = paramValue;
                sqlCmd.Parameters.Add(dbParam);
            }
            try
            {
                sqlConnection.Open();
                sqlAdapter.Fill(objDataSet);
                optReturn.Data = objDataSet;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MYSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static OperationReturn ExecuteStoredProcedure(string strConn, string procedureName, DbParameter[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = procedureName;
            for (int i = 0; i < parameters.Length; i++)
            {
                sqlCmd.Parameters.Add(parameters[i]);
            }
            try
            {
                sqlConnection.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MSSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
        /// <summary>
        /// 根据指定的数据类型创建DBCommand的参数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static DbParameter GetDbParameter(string name, MssqlDataType dataType, int length)
        {
            switch (dataType)
            {
                case MssqlDataType.Varchar:
                    return new SqlParameter(name, SqlDbType.VarChar, length);
                case MssqlDataType.NVarchar:
                    return new SqlParameter(name, SqlDbType.NVarChar, length);
                case MssqlDataType.Char:
                    return new SqlParameter(name, SqlDbType.Char, length);
                case MssqlDataType.Int:
                    return new SqlParameter(name, SqlDbType.Int, length);
                case MssqlDataType.Bigint:
                    return new SqlParameter(name, SqlDbType.BigInt, length);
            }
            return null;
        }
        /// <summary>
        /// 根据指定的数据类型创建DBCommand的参数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public static DbParameter GetDbParameter(string name, MssqlDataType dataType)
        {
            switch (dataType)
            {
                case MssqlDataType.Varchar:
                    return new SqlParameter(name, SqlDbType.VarChar, 1024);
                case MssqlDataType.NVarchar:
                    return new SqlParameter(name, SqlDbType.NVarChar, 1024);
                case MssqlDataType.Char:
                    return new SqlParameter(name, SqlDbType.Char);
                case MssqlDataType.Int:
                    return new SqlParameter(name, SqlDbType.Int);
                case MssqlDataType.Bigint:
                    return new SqlParameter(name, SqlDbType.BigInt);
            }
            return null;
        }
        /// <summary>
        /// 执行指定的Sql语句
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">Sql语句</param>
        /// <returns></returns>
        public static OperationReturn ExecuteSql(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = sqlCmd.ExecuteNonQuery();
                optReturn.Data = count;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MSSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
        /// <summary>
        /// 获取记录条数
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">查询语句</param>
        /// <returns>OperationReturn的IntValue为记录条数</returns>
        public static OperationReturn GetRecordCount(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                optReturn.IntValue = count;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                var err = ex as SqlException;
                if (err != null)
                {
                    if (err.Number == DBAccessDefine.MSSQL_ERR_OBJECT_NOT_EXIST)
                    {
                        optReturn.Code = Defines.RET_DBACCESS_TABLE_NOT_EXIST;
                    }
                }
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
    }
}
