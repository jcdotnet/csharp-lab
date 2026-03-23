using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MessengerWPF
{
    /// <summary>
    /// Clase que representa el usuario que entra en la aplicación
    /// </summary>
    public class User
    {
        private int _identificador;
        private string _userName;
        private BitmapImage _image;
        private bool _hablando;

        #region properties

        private TcpClient _tcpClient;
        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        public int Identificador
        {
            get { return _identificador; }
            set { _identificador = value; }
        } 

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public BitmapImage Image { get; set; }

        /// <summary>
        /// Indica si el usuario está hablando (voz) en alguna ventana de conversación 
        /// </summary>
        public bool Hablando
        {
            get { return _hablando; }
            set { _hablando = value; }
        }

        #endregion

        public User() { }
        
        public User(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public User(string userName)
        {
            _userName = userName;
        }

        public User(string userName, BitmapImage status)
        {
            _userName = userName;
            _image = status;
        }

        #region overrides

        public override string ToString()
        {
            return _userName;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(User)) return false;
            if (ReferenceEquals(null, obj)) return false;

            return (this._userName.Equals(( (User)obj).UserName));
        }

        public override int GetHashCode()
        {
            return string.Format("{0}", this._userName).GetHashCode();
        }

        #endregion

    }
}
