using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;

namespace MessengerServer
{
    public class User
    {

        #region fields

        private string _userName;
        private TcpClient _tcpClient;

        private int _hablando;
        private TcpClient _tcpClientVoice;
        private int _voicePort;
        private string _voiceRoom;

        #endregion

        #region properties

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
  

        public TcpClient TcpClientVoice // nuevo
        {
            get { return _tcpClientVoice; }
            set { _tcpClientVoice = value; }
        }

        /// <summary>
        /// VOZ:
        /// 0: el usuario ha finalizado la llamada  
        /// 1: el usuario está pendiente de contestar 
        /// 2: el usuario participa en la llamada 
        /// </summary>
        public int Hablando
        {
            get { return _hablando; }
            set { _hablando = value; }
        }

        public int VoicePort
        {
            get { return _voicePort; }
            set { _voicePort = value; }
        }

        public string VoiceRoom
        {
            get { return _voiceRoom; }
            set { _voiceRoom = value; }
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

        #region overrides

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(User)) return false;
            if (ReferenceEquals(null, obj)) return false;

            return (this._userName.Equals(((User)obj).UserName));          
        }

        public override int GetHashCode()
        {
            return string.Format("{0}", this._userName).GetHashCode();
        }

        #endregion
    }
}
