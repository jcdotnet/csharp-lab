using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessengerWPF
{
    /// <summary>
    /// Clase que representa una ventana de conversación
    /// </summary>
    public class ChatAbierto
    {
        private User _emisor;
        private ArrayList _receptores;
        private string _receptorActivo;
        private bool _llamada;
        private int _participantesLlamada;  
        private int _pendientes; 

        public User Emisor { get { return _emisor; } set { _emisor = value; } }

        public ArrayList Receptores {
            get { return _receptores; } 
            set { _receptores = value; } 
        }

        /// <summary>
        /// En una conversación con varios participantes (un emisor y varios receptores), el receptor activo es
        /// el usuario que está hablando con el emisor 
        /// </summary>
        public string ReceptorActivo
        {
            get { return _receptorActivo; }
            set { _receptorActivo = value; }
        }

        /// <summary>
        /// Indica si se está realizando una llamada en la ventana de onversación
        /// </summary>
        public bool Llamada
        {
            get { return _llamada; }
            set { _llamada = value; }
        }

        /// <summary>
        /// Usuarios que están hablando en una conversación de voz
        /// </summary>
        public int Hablando
        {
            get { return _participantesLlamada; }
            set { _participantesLlamada = value; }
        }

        /// <summary>
        /// Usuarios que están pendientes de responder a una llamada de voz. También puede indicar los usuarios 
        /// que se encuentran online en una ventana de conversación.
        /// </summary>
        public int Pendientes
        {
            get { return _pendientes; }
            set { _pendientes = value; }
        }

        public ChatAbierto(User emisor, ArrayList receptores)
        {
            _emisor = emisor;
            _receptores = receptores; 
        }
        
        public override bool Equals(object obj)
        {         
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ChatAbierto)) return false;

            ArrayList lista1 = this._receptores;
            ArrayList lista2 =((ChatAbierto)obj).Receptores; 
            if(lista1.Count != lista2.Count)
               return false;
            lista1.Sort();
            lista2.Sort();

            for(int i=0;i<lista1.Count;i++)
            {
               if(!lista1[i].Equals(lista2[i])) 
                   return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}", this._receptores).GetHashCode();
        }
    }
}
