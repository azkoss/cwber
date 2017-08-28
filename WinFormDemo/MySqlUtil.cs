using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace WinFormDemo
{
    class MySqlUtil
    {
        protected MySqlConnection Connection;
        protected string connectionstring;
        public MySqlUtil()
        {
            //connectionstring = System.Configuration.ConfigurationManager.AppSettings["connString"];
            //connectionstring = "Server=192.168.60.63;User Id=trans;Password=trans;Persist Security Info=True;Database=hospital";
            this.connectionstring = Properties.Settings.Default.MySqlConnectStr;
            this.Connection = new MySqlConnection(this.connectionstring);
        }
        public MySqlUtil(string connstring)
        {
            this.connectionstring = connstring;
            this.Connection = new MySqlConnection(this.connectionstring);
        }
        private MySqlCommand BuilderQueryCommand(string storedProcName, MySqlParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = this.Connection;
            command.CommandText = storedProcName.Trim();
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (MySqlParameter p in parameters)
                {
                    command.Parameters.Add(p);
                }
            }
            return command;
        }
        public MySqlDataReader GetDataReader(string storedProcName, MySqlParameter[] parameters)
        {
            MySqlDataReader reader;
            MySqlCommand cmd = this.BuilderQueryCommand(storedProcName, parameters);
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }
        public int GetEffect(string storedProcName, MySqlParameter[] parameters)
        {
            int result = 0;
            try
            {
                if (this.Connection.State == ConnectionState.Closed)
                {
                    this.Connection.Open();
                }
                MySqlTransaction trans = this.Connection.BeginTransaction();
                try
                {
                    MySqlCommand cmd = this.BuilderQueryCommand(storedProcName, parameters);
                    cmd.Transaction = trans;
                    result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    this.Connection.Close();
                    return result;
                }
                catch
                {
                    if (trans != null)
                    {
                        if (trans != null)
                        {
                            trans.Rollback();
                        }
                    }
                    return result;
                }
                finally
                {
                    if (trans != null)
                    {
                        trans.Dispose();
                    }
                    this.Connection.Close();
                }
            }
            catch (Exception ex1)
            {
                return 0;
            }
        }
        public DataSet GetDataSet(string ProcName, MySqlParameter[] parameters, string tableName)
        {
            try
            {
                DataSet ds = new DataSet();
                if (this.Connection.State == ConnectionState.Closed)
                {
                    this.Connection.Open();
                }
                MySqlDataAdapter myDa = new MySqlDataAdapter();
                myDa.SelectCommand = this.BuilderQueryCommand(ProcName, parameters);
                myDa.Fill(ds, tableName);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public DataSet GetDataSet(string ProcName, MySqlParameter[] parameters, int start, int maxRecord, string tableName)
        {
            try
            {
                DataSet ds = new DataSet();
                if (this.Connection.State == ConnectionState.Closed)
                {
                    this.Connection.Open();
                }
                MySqlDataAdapter myDa = new MySqlDataAdapter();
                myDa.SelectCommand = this.BuilderQueryCommand(ProcName, parameters);
                myDa.Fill(ds, start, maxRecord, tableName);
                return ds;
            }
            catch
            {
                this.Connection.Close();
                return null;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public object GetObject(string storedProcName, MySqlParameter[] parameters)
        {
            object result = null;
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            try
            {
                MySqlCommand cmd = this.BuilderQueryCommand(storedProcName, parameters);
                result = cmd.ExecuteScalar();
                this.Connection.Close();
                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public int RunProcByID(string ProcName, int id, string paraname)
        {
            try
            {
                MySqlParameter[] p = { new MySqlParameter(paraname, SqlDbType.Int) };
                p[0].Value = id;
                return this.GetEffect(ProcName, p);
            }
            catch
            {
                this.Connection.Close();
                return 0;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public int UpdateByID(string ProcName, int keyValue, string paraKeyName, string paraFieldName, string FieldValue, int length)
        {
            try
            {
                MySqlParameter[] p = { new MySqlParameter(paraFieldName, MySqlDbType.VarChar, length), new MySqlParameter(paraFieldName, SqlDbType.Int) };
                p[0].Value = FieldValue;
                p[1].Value = keyValue;
                return this.GetEffect(ProcName, p);
            }
            catch
            {
                this.Connection.Close();
                return 0;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public DataSet RunSql(string strSql, string tableName)
        {
            try
            {
                DataSet ds = new DataSet();
                if (this.Connection.State == ConnectionState.Closed)
                {
                    this.Connection.Open();
                }
                MySqlDataAdapter myDa = new MySqlDataAdapter();
                myDa.SelectCommand = new MySqlCommand(strSql, this.Connection);
                myDa.Fill(ds, tableName);
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public int RunSql(string strSql, out int effect)
        {
            effect = 0;
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            MySqlTransaction trans = this.Connection.BeginTransaction();
            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, this.Connection);
                cmd.Transaction = trans;
                effect = cmd.ExecuteNonQuery();
                trans.Commit();
                return effect;
            }
            catch
            {
                trans.Rollback();
                this.Connection.Close();
                return effect;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public MySqlDataReader RunSql(string strSql)
        {
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, this.Connection);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                this.Connection.Close();
                return null;
            }
        }
        public object GetObjectByRunSQL(string strsql)
        {
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
            object obj = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(strsql, this.Connection);
                obj = cmd.ExecuteScalar();
                this.Connection.Close();
                return obj;
            }
            catch
            {
                this.Connection.Close();
                return null;
            }
            finally
            {
                this.Connection.Close();
            }
        }
        public void Close()
        {
            if (this.Connection.State != ConnectionState.Closed)
            {
                this.Connection.Close();
            }
        }
    }
}
