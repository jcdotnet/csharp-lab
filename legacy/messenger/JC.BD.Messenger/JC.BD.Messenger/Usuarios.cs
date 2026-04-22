using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JC.BD.Messenger
{
    public class Usuarios
    {
        public bool Insert(UsuariosRow usuario)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "INSERT INTO aa_coordenadas (idEmpresa, usuario, clave, estado, estadoConexion) VALUES (@v1, @v2, @v3, @v4, @v5)";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@v1", MySqlDbType.Int32).Value = usuario.IdEmpresa;
                cm.Parameters.Add("@v2", MySqlDbType.VarChar, 20).Value = usuario.Usuario;
                cm.Parameters.Add("@v3", MySqlDbType.VarChar, 50).Value = usuario.Clave;
                cm.Parameters.Add("@v4", MySqlDbType.Int32).Value = usuario.Estado;
                cm.Parameters.Add("@v5", MySqlDbType.Int16).Value = usuario.EstadoConexion;
                 rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();
            return (rowAffected >= 1);
        }

        public bool Update(UsuariosRow usuario)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "UPDATE aa_usuario SET idEmpresa=@v1, usuario=@v2, clave=@v3, estado=@v4, estadoConexion=@v5 WHERE idUsuario=@vid";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@vid", MySqlDbType.Int32).Value = usuario.IdUsuario;
                cm.Parameters.Add("@v1", MySqlDbType.Int32).Value = usuario.IdEmpresa;
                cm.Parameters.Add("@v2", MySqlDbType.VarChar, 20).Value = usuario.Usuario;
                cm.Parameters.Add("@v3", MySqlDbType.VarChar, 50).Value = usuario.Clave;
                cm.Parameters.Add("@v4", MySqlDbType.Int32).Value = usuario.Estado;
                cm.Parameters.Add("@v5", MySqlDbType.Int16).Value = usuario.EstadoConexion;
                rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();

            return (rowAffected >= 1);
        }

        public bool UpdateClave(int idUsuario, string clave)
        {
            int rowAffected;
            ConnectionManager cn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "UPDATE aa_usuario SET clave=@clave WHERE idUsuario=@vid";
                cm.Connection = cn.Get();
                cm.Parameters.Add("@vid", MySqlDbType.Int32).Value = idUsuario;
                cm.Parameters.Add("@clave", MySqlDbType.VarChar, 50).Value = clave;
                rowAffected = cm.ExecuteNonQuery();
            }
            cn.Cerrar();

            return (rowAffected >= 1);
        }

        public bool Delete(int idUsuario)
        {
            int rowAffected;
            ConnectionManager cnn = new ConnectionManager();
            using (MySqlCommand cm = new MySqlCommand())
            {
                cm.CommandText = "DELETE FROM aa_usuario WHERE id='" + idUsuario + "'";
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
