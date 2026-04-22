using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JC.BD.Messenger
{
    public class Login
    {
        public bool Insert(LoginRow login)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "INSERT INTO login (usuario, clave) VALUES (@v1, @v2)";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@v1", MySqlDbType.VarChar, 10).Value = login.Usuario;
                cm.Parameters.Add("@v2", MySqlDbType.VarChar, 10).Value = login.Clave;
                 rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();
            return (rowAffected >= 1);
        }

        public bool Update(LoginRow login)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "UPDATE login SET usuario=@v1, clave=@v2 WHERE id=@vid";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@vid", MySqlDbType.Int32).Value = login.Id;
                cm.Parameters.Add("@v1", MySqlDbType.VarChar, 10).Value = login.Usuario;
                cm.Parameters.Add("@v2", MySqlDbType.VarChar, 10).Value = login.Clave;
                rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();

            return (rowAffected >= 1);
        }

        public bool UpdateClave(int id, string clave)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "UPDATE login SET clave=@clave WHERE id=@vid";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@vid", MySqlDbType.Int32).Value = id;
                cm.Parameters.Add("@clave", MySqlDbType.VarChar, 10).Value = clave;
                rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();

            return (rowAffected >= 1);
        }

        public bool Delete(int idLogin)
        {
            int rowAffected;
            ConnectionManager cnn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "DELETE FROM login WHERE id='" + idLogin + "'";
                cm.Connection = cnn.Get();
                rowAffected = cm.ExecuteNonQuery();
            }
            cnn.Cerrar();
            return (rowAffected >= 1);
        }
       
        public DataTable Get()
        {
            return Get(null, null);
        }

        public DataTable Get(string where)
        {
            return Get(where, null);
        }

        public DataTable Get(string where, string order)
        {
            DataTable dataTable = CreateDataTable();
            ConnectionManager cn = new ConnectionManager();
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                MySqlCommand cm = new MySqlCommand();
                cm.CommandText = CreateSQL(where, order, false);
                cm.Connection = cn.Get();
                da.SelectCommand = cm;
                da.Fill(dataTable);
            }
            cn.Cerrar();
            return dataTable;
        }

        #region private

        private string CreateSQL(string whereSql, string orderBySql, bool count)
        {
            string sql;
            if (count) sql = "SELECT COUNT(*) FROM login";
            else sql = " SELECT * FROM login";
            if (null != whereSql && 0 < whereSql.Length)
                sql += " WHERE " + whereSql;
            if (null != orderBySql && 0 < orderBySql.Length)
                sql += " ORDER BY " + orderBySql;
            return sql;
        }
       
        private DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "Login";
            DataColumn dataColumn;
            dataColumn = dataTable.Columns.Add("Id", typeof(int));
            dataColumn.AllowDBNull = false;
            dataColumn.ReadOnly = true;
            dataColumn.Unique = true;
            dataColumn.AutoIncrement = true;
            dataColumn = dataTable.Columns.Add("Usuario", typeof(string));
            dataColumn.AllowDBNull = false;
            dataColumn.MaxLength = 10;
            dataColumn = dataTable.Columns.Add("Clave", typeof(string));
            dataColumn.AllowDBNull = false;
            dataColumn.MaxLength = 10;

            return dataTable;
        }

        #endregion
    }
}
