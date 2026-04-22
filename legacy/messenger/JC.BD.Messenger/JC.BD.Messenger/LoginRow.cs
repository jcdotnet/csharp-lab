using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC.BD.Messenger
{
    public class LoginRow
    {
        private int id;
        private string usuario;
        private string clave;

        public int Id
        {
            get { return id; }
            set { id = value; }
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
    }
}
