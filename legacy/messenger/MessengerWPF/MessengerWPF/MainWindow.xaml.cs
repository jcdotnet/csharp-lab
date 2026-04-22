using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JC.BD.Messenger;

namespace MessengerWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region comunicación protocolo TCP/IP

        private const byte USERS_REQUEST = 1;
        private const byte USER_CONNECTED = 2;
        private const byte USER_DISCONNECTED = 3;
        private const byte MENSAJE_ENVIADO = 4;
        private const byte MENSAJE_RECIBIDO = 5;
        private const byte SERVER_SHUTDOWN = 6;
        private const byte VOICE_REQUEST = 7;
        private const byte VOICE_ACCEPT = 8;
        private const byte VOICE_END = 9;
        private const byte VOICE_REJECT = 10;
        //private const byte USERS_AVAILABLE = 11;
        private const byte USER_ONLINE = 12;
        private const byte DISCONNECT_USER = 13; 

        #endregion

        #region declaración de eventos Messenger

        public delegate void ChatEventHandler(object sender, ChatEventArgs e); 
        public static event ChatEventHandler MensajeRecibido;
      
        public static event ChatEventHandler LlamadaRecibida;
        public static event ChatEventHandler LlamadaFinalizada;
        public static event ChatEventHandler PendientesChanged;
        public static event EventHandler UsuarioDesconectado;

        #endregion

        #region fields

        private User usuario; // usuario que ha iniciado sesión
        private BinaryReader br;
        private static BinaryWriter bw;
        private BackgroundWorker bg = new BackgroundWorker();
        ObservableCollection<User> contactList = new ObservableCollection<User>(); 
        private bool exitOnButton = false, isExiting = false;

        private static List<ChatAbierto> chatsAbiertos = new List<ChatAbierto>(); // ventanas de conversación 

        internal static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon(); 
        

        #endregion

        #region properties

        /// <summary>
        /// Lista de contactos del usuario que ha iniciado sesión
        /// </summary>
        public ObservableCollection<User> ContactList
        {
            get { return contactList; }
            set { contactList = value; }
        }

        #endregion

        #region constructor

        public MainWindow(User usuario)
        {
            InitializeComponent();
            this.DataContext = this;
            this.usuario = usuario;
            this.Title = usuario.UserName;
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);

            try
            {
                // stream para el envío y recepción al servidor
                NetworkStream stream = usuario.TcpClient.GetStream();

                // conexión segura
                //SslStream ssl = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateCert));
                //ssl.AuthenticateAsClient("MessengerServer");

                br = new BinaryReader(stream /*ssl*/, Encoding.UTF8);
                bw = new BinaryWriter(stream /*ssl*/, Encoding.UTF8);

                // Cargamos todos los usuarios, habría que hacer una tabla de contactos para cargar solo
                // sa los que tenga agregado. Por ejemplo create tabla conectados, agregados o lo que sea
                // columna id PK increment / columna usuario1 FK AccesoMaestro / usuario2 FK AccesoMaestro

                DataTable dtUsuarios = new Login().Get();
                foreach (DataRow drUsuario in dtUsuarios.Rows)
                {
                    if ((string)drUsuario["Usuario"]!=usuario.UserName)
                        contactList.Add(new User((string)drUsuario["Usuario"], null));
                }
                
                // cargamos usuarios conectados en el servidor
                bw.Write(USERS_REQUEST);
                bw.Flush();
                int users = br.ReadInt32();
                for (int i = 0; i < users; i++)
                {
                    string userOnline = br.ReadString();
                    foreach (User user in contactList)
                    {
                        if (user.UserName == userOnline)
                            user.Image = new BitmapImage(new Uri(@"Images/online.gif", UriKind.Relative));
                    } 
                }

                // Sesión única, ccuando usemos la tabla real ya sabemos si está online, al iniciar la otra sesión
                // se modificó la columna estado y haríamos los siguientes pasos. Ahora hay que hacerlos siempre. 
                // 1) comprobar si está online: 
                // bw.Write(USER_ONLINE);
                // bw.Write(usuario.UserName);
                // bw.Flush();
                // 2) servidor recoge paquete, si está online envía un paquete DISCONNECT_USER al cliente conectado
                // (userBW, no bw) y lo elimina de la lista users.
                // 3) cliente recibe DISCONNECT_USER
                // en ese caso se manda un mensaje que se ha iniciado sesión en otra ubicación, se puede leer en la 
                // tabla accesomaestro, y se cierra su MainWindow, en servershutdown tb debería mostrar msg y cerrar.                
                // TO DO 
                
                // cargamos usuario en el servidor 
                bw.Write(USER_CONNECTED);
                bw.Write(usuario.UserName);
                bw.Flush();

                // set columna "estado_conexion" a true, al valor numérico que corresponda a conectado o a lo que sea 
                // TO DO 

                // cliente escucha en segundo plano
                bg.WorkerSupportsCancellation = true;
                bg.DoWork += new DoWorkEventHandler(bg_DoWork);
                bg.RunWorkerAsync(new object[] { usuario.TcpClient, br }); // recibe paquetes de datos mientras el cliente esté conectado
                bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            }
            catch (Exception exception) { MessageBox.Show("CLIENTE: "+exception.Message); }
        }

        #endregion

        #region background worker

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpClient client = (TcpClient)((object[])e.Argument)[0];
            BinaryReader br = (BinaryReader)((object[])e.Argument)[1];
            
            try
            {
                byte paquete = br.ReadByte();

                while (client.Connected)
                {                
                    if (paquete == MENSAJE_RECIBIDO)
                    {
                        /* podemos hacer que si el programa no está en la barra de tareas cuando se reciba un mensaje
                         * cambie el icono y aparezca una notificación, pero eso solo si cuando el programa va al tray
                         * las ventanas también desaparecen de la barra de tareas, ¡¡¡eso es más útil en skype o en una 
                         * aplicación que todas las conversaciones estén dentro de la ventana principal que aquí!!!!*/

                        /* debemos cambiar el mensaje en la base de datos al estado recibido, en chat.xaml, cuando tenga
                         * focus, cambiamos a mensaje leido, el mensaje se creo en el server, al recibir el paquete 
                         * MENSAJE_ENVIADO, con el estado enviado, cuando lo envíe el servidor se cambia a pendiente, 
                         * se debe introducir una columna con la hora, cuando el usuario inicia sesión se debe mirar en 
                         * la BD todos los mensajes que tenga pendientes y cargarlos en el chat correspondiente*/

                        string from = br.ReadString();
                        int receptoresCount = br.ReadInt32(); 
                        ArrayList participantes = new ArrayList();
                        for (int i =0; i<receptoresCount; i++)
                                              
                           participantes.Add(br.ReadString());

                        string msg = br.ReadString();
                        ChatAbierto chatAbierto = new ChatAbierto(usuario, participantes);
                        chatAbierto.ReceptorActivo = from;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (!chatsAbiertos.Contains(chatAbierto))
                            {
                                ParameterizedThreadStart tarea = new ParameterizedThreadStart(NuevoChat);
                                Thread thread = new Thread(tarea); // crea el thread
                                thread.SetApartmentState(ApartmentState.STA); //
                                thread.IsBackground = true; //                              
                                thread.Start(chatAbierto); // ejecuta el thread
                                chatsAbiertos.Add(chatAbierto);
                                
                            }
                            Thread.Sleep(100); // damos tiempo al nuevo thread para controlar MensajeRecibido 
                            if (MensajeRecibido != null)
                                MensajeRecibido(this, new ChatEventArgs(chatAbierto, msg));
                        }));
                    }
                    else if (paquete == VOICE_REQUEST)
                    {
                        string from = br.ReadString();
                        int receptoresCount = br.ReadInt32();
                        ArrayList participantes = new ArrayList();
                        for (int i = 0; i < receptoresCount; i++)

                            participantes.Add(br.ReadString());

                        string voiceRoom = br.ReadString();

                        ChatAbierto chatAbierto = new ChatAbierto(usuario, participantes);
                        chatAbierto.ReceptorActivo = from;
                        chatAbierto.Hablando = br.ReadInt32();
                        chatAbierto.Pendientes = br.ReadInt32();
                        
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (!chatsAbiertos.Contains(chatAbierto))
                            {                                
                                ParameterizedThreadStart tarea = new ParameterizedThreadStart(NuevoChat);
                                Thread thread = new Thread(tarea); // crea el thread
                                thread.SetApartmentState(ApartmentState.STA); //
                                thread.IsBackground = true; //                              
                                thread.Start(chatAbierto); // ejecuta el thread
                                chatsAbiertos.Add(chatAbierto);                                                                
                            }
                            Thread.Sleep(200); // damos tiempo para que se creen las nuevas ventanas 
                            if (PendientesChanged!=null) 
                                PendientesChanged(this, new ChatEventArgs(chatAbierto, "request"));
                            /* si el chat no está abierto y es la primera vez que se llama el evento llamadarecibida  
                             * se dispara una vez, si es la segunda vez que se llama dos veces, así sucesivamente,
                             * mientras se resuelve uso una variable estática para controlar que solo se entre una vez */
                            Chat.entradasEvento = 0;
                            if (LlamadaRecibida != null && from!= usuario.UserName)
                                LlamadaRecibida(this, new ChatEventArgs(chatAbierto, voiceRoom));
                        }));
                    }                    
                    else if (paquete == VOICE_ACCEPT)
                    {
                        string from = br.ReadString();
                        int receptoresCount = br.ReadInt32();
                        ArrayList participantes = new ArrayList();
                        for (int i = 0; i < receptoresCount; i++)
                            participantes.Add(br.ReadString());

                        int hablando = br.ReadInt32();
                        int pendientes = br.ReadInt32();

                        ChatAbierto chatAbierto = new ChatAbierto(usuario, participantes);
                        chatAbierto.ReceptorActivo = from;
                        chatAbierto.Hablando = hablando;
                        chatAbierto.Pendientes = pendientes;
                        
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (PendientesChanged != null)
                                PendientesChanged(this, new ChatEventArgs(chatAbierto, null));
                        }));
                    }
                    else if (paquete == VOICE_REJECT)
                    {
                        string from = br.ReadString();
                        int receptoresCount = br.ReadInt32();
                        ArrayList participantes = new ArrayList();
                        for (int i = 0; i < receptoresCount; i++)

                            participantes.Add(br.ReadString());

                        int hablando = br.ReadInt32();
                        int pendientes = br.ReadInt32();

                        ChatAbierto chatAbierto = new ChatAbierto(usuario, participantes);
                        chatAbierto.ReceptorActivo = from;
                        chatAbierto.Hablando = hablando;
                        chatAbierto.Pendientes = pendientes;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (PendientesChanged != null)
                                PendientesChanged(this, new ChatEventArgs(chatAbierto, null));
                        }));
                    }
                    else if (paquete == VOICE_END)
                    {
                        string from = br.ReadString();
                        int receptoresCount = br.ReadInt32();
                        ArrayList participantes = new ArrayList();
                        for (int i = 0; i < receptoresCount; i++)

                            participantes.Add(br.ReadString());

                        int hablando = br.ReadInt32();
                        int pendientes = br.ReadInt32();                        

                        ChatAbierto chatAbierto = new ChatAbierto(usuario, participantes);
                        chatAbierto.ReceptorActivo = from;
                        chatAbierto.Hablando = hablando;
                        chatAbierto.Pendientes = pendientes;

                        if (hablando == 0 || hablando == 1 && pendientes == 0) // finalización de la llamada
                        {
                            // establecemos llamada=false en todos los chats, el evento solo cerrará las conexiones
                            foreach (ChatAbierto chat in chatsAbiertos)
                            {
                                if (chat.Equals(chatAbierto))
                                {
                                    chat.Emisor.Hablando = false;
                                    chat.Llamada = false;
                                }
                            }
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                Thread.Sleep(300);
                                if (LlamadaFinalizada != null && from != usuario.UserName) // éste ya ha ha cerrado su conexión
                                    LlamadaFinalizada(this, new ChatEventArgs(chatAbierto, null));
                            }));
                        }
                        else // se notifica a los demás 
                        {
                            if (PendientesChanged != null) 
                                PendientesChanged(this, new ChatEventArgs(chatAbierto, null));
                        }
                    }
                    else if (paquete == USER_CONNECTED)
                    {
                        string newUser = br.ReadString();

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            User userToUpdate = null;
                            foreach (User user in contactList)
                            {
                                if (user.UserName == newUser)
                                    userToUpdate = user;
                            }
                            contactList.Remove(userToUpdate);
                            userToUpdate.Image = new BitmapImage(new Uri(@"Images\online.gif", UriKind.Relative));
                            contactList.Add(userToUpdate);
                            notifyIcon.BalloonTipText = newUser + " acaba de iniciar sesión";
                            notifyIcon.ShowBalloonTip(1000);
                        }));

                    }
                    else if (paquete == USER_DISCONNECTED)
                    {
                        string newUser = br.ReadString();

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            User userToUpdate = null;
                            foreach (User user in contactList)
                            {
                                if (user.UserName == newUser)
                                    userToUpdate = user;
                            }
                            contactList.Remove(userToUpdate);
                            userToUpdate.Image = null;
                            contactList.Add(userToUpdate);
                        }));
                    }
                    else if (paquete == SERVER_SHUTDOWN)
                    {
                        return;
                    }
                    paquete = br.ReadByte();
                }
            }
            catch (IOException) { }
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        #endregion

        #region métodos privados

        private void NuevoChat(object param)
        {
            Chat nuevoChat = new Chat((ChatAbierto)param);
            nuevoChat.Show();
            System.Windows.Threading.Dispatcher.Run(); 
        }

        internal static void EnviarMensaje(ChatAbierto chat, string mensaje)
        {
            bw.Write(MENSAJE_ENVIADO);
            bw.Write(chat.Emisor.UserName);
            bw.Write(chat.Receptores.Count);
            foreach (string receptor in chat.Receptores)
            bw.Write(receptor);
            bw.Write(mensaje);        
            bw.Flush();           
        }

        internal static void SolicitarAudio(ChatAbierto chat, int localPort)
        {                      
            bw.Write(VOICE_REQUEST);
            bw.Write(chat.Emisor.UserName);
            bw.Write(localPort);
            bw.Write(chat.Receptores.Count);
            string voiceRoom = chat.Emisor.UserName;
            foreach (string receptor in chat.Receptores)
            {
                voiceRoom += receptor;
                bw.Write(receptor);
            }
            bw.Write(voiceRoom);           
            bw.Flush();              
        }

        internal static void AceptarLLamada(ChatAbierto chat, string voiceRoom, int localPort)
        {
            bw.Write(VOICE_ACCEPT);
            bw.Write(chat.Emisor.UserName);
            bw.Write(chat.Receptores.Count);
            foreach (string receptor in chat.Receptores)
                bw.Write(receptor);
            bw.Write(voiceRoom);
            bw.Write(localPort);
            bw.Flush();
        }

        internal static void RechazarLlamada(ChatAbierto chat)
        {
            bw.Write(VOICE_REJECT);
            bw.Write(chat.Emisor.UserName);
            bw.Write(chat.Receptores.Count);
            foreach (string receptor in chat.Receptores)
            {
                bw.Write(receptor);
            }
            bw.Flush();
        }

        internal static void ColgarUsuario(ChatAbierto chat)
        {
            bw.Write(VOICE_END);
            bw.Write(chat.Emisor.UserName);
            bw.Write(chat.Receptores.Count);
            foreach (string receptor in chat.Receptores)
                bw.Write(receptor);
            bw.Flush();
        }

        internal static void CerrarChat(ChatAbierto chat)
        {
            chatsAbiertos.Remove(chat);
            chat = null; // elimina chat, libera memoria y se soluciona la múltiple entrada a eventos en chat.xaml.cs
        }
        #endregion

        #region controladores eventos

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;
            User item = ((User)listView.ItemContainerGenerator.ItemFromContainer(dep));

            ArrayList receptores = new ArrayList();
            receptores.Add(item.UserName);
            ChatAbierto chatAbierto = new ChatAbierto(usuario, receptores);
            if (!chatsAbiertos.Contains(chatAbierto))
            {
                ParameterizedThreadStart tarea = new ParameterizedThreadStart(NuevoChat);
                Thread thread = new Thread(tarea); // crea el thread
                thread.SetApartmentState(ApartmentState.STA); //
                thread.IsBackground = true; //            
                thread.Start(chatAbierto); // ejecuta el thread
                chatsAbiertos.Add(chatAbierto);
            }          
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon.BalloonTipText = "Messenger sigue ejecutándose";
                notifyIcon.ShowBalloonTip(666);
                /* si las ventanas de conversación también deben desaparecer de la barra de tareas:
                 * Evento MainWindow.ChatHidden (ChateventHandler)
                 * Evento MainWindow.ChatVisible (ChateventHandler)
                 * handler ChatHiden en chat.xaml.cs -> showintaskbar = false; visibility = false
                 * ídem ChatVisible
                 */

                /* también, en el caso de que las ventanas de conversación desaparezcan, se puede mostrar una lista 
                 * con todas las conversaciones abiertas, y al pinchar en una el handler de chatVisible solo muestra
                 * esa, dependiendo de la complejidad podemos hacerlo o seguir con otras cosas y eso más adelante */
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
            //this.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isExiting)
            {
                e.Cancel = false;
            }
            if (exitOnButton)
            {
                try
                {
                    foreach (ChatAbierto chatAbierto in chatsAbiertos)
                    {
                        if (chatAbierto.Emisor.Hablando) //(chatAbierto.Llamada)
                        {
                            MessageBox.Show("Cierre primero todas las llamadas en curso");
                            e.Cancel = true;
                            return;
                        }
                    }
                    this.Title += " desconectando..";
                    bw.Write(USER_DISCONNECTED);
                    bw.Write(usuario.UserName);
                    bw.Flush();
                    bg.CancelAsync();
                    usuario.TcpClient.Close();

                    // set columna "estado_conexion" a false o al valor numérico que corresponda a desconectado
                    // TO DO 

                    notifyIcon.Text = "Messenger (sesión no iniciada)";

                    if (UsuarioDesconectado != null)
                        UsuarioDesconectado(this, EventArgs.Empty);
                    Thread.Sleep(1000);
                    isExiting = true;
                    this.Close();

                }
                catch { }
            }
            else
            {
                e.Cancel = true;
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void menuSalir_Click(object sender, RoutedEventArgs e)
        {
            this.exitOnButton = true;
            this.Close();
        }

        #endregion            
    }

    #region clase ChatEventArgs

    public class ChatEventArgs : EventArgs
    {
        private ChatAbierto _chatReceptor;
        private string _mensaje;
        
        public ChatAbierto ChatReceptor
        {
            get { return _chatReceptor; }
            //set { _usuario = value; }
        }

        public string Mensaje
        {
            get { return _mensaje; }
            //set { _mensaje = value; }
        }

        public ChatEventArgs(ChatAbierto chat, string mensaje)
        {
            _chatReceptor = chat;
            _mensaje = mensaje;
        }
    }

    #endregion

}
