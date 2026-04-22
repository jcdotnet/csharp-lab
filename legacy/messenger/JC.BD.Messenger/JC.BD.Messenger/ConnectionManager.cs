using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JC.BD.Messenger
{
    internal class ConnectionManager
    {
        private MySqlConnection cn;
        
        //private const int MaxConexiones = 10; 
        //private static int numConexiones = 0;
        
        internal ConnectionManager()
        {
            //if (numConexiones < MaxConexiones)
            //{
                cn = new MySqlConnection();
                cn.ConnectionString = "server=localhost;database=messenger;uid=root;";
                cn.Open();
            //  numConexiones++;
            //}
            //else
            //    throw new Exception("Número máximo de conexiones alcanzadas, intente en otro momento");

        }

        internal MySqlConnection Get()
        {
            return cn;
        }
  
        internal void Cerrar()
        {
            //if (numConexiones > 0)
            //{
            //    numConexiones--;
                cn.Close();
                cn.Dispose();
                cn = null;
            //}
        }
    }
}
