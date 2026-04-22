using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JC.BD.Messenger
{
    public class Messenger
    {
        /// <summary>
        /// consulta DML a CUALQUIER tabla de la base de datos
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public DataTable Get(string select)
        {
            DataTable dataTable = new DataTable();
            ConnectionManager cn = new ConnectionManager();
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                MySqlCommand cm = new MySqlCommand();
                cm.CommandText = select;
                cm.Connection = cn.Get();
                da.SelectCommand = cm;
                da.Fill(dataTable);
            }
            cn.Cerrar();
            return dataTable;
        }
    }
}
