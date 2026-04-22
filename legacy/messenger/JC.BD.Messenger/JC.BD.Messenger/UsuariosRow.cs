using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC.BD.Messenger
{
    public class UsuariosRow
    {
        private int idUsuario;
        private int idEmpresa;
        private string usuario;
        private string clave;
        private int estado;
        private short estadoConexion;

        public int IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }

        public int IdEmpresa
        {
            get { return idEmpresa; }
            set { idEmpresa = value; }
        }

        public string Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        public string Clave
        {
            get { return clave; }
            set { clave= value; }
        }

        public int Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        /// <summary>
        /// 0: no conectado
        /// 1: conectado
        /// </summary>
        public short EstadoConexion
        {
            get { return estadoConexion; }
            set { estadoConexion = value; }
        }
    }
}
