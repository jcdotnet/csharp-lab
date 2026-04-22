#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
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
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
#endregion

namespace MessengerServer
{    
    /// <summary>
    /// WPF MESSENGER SERVER
    /// </summary>
    public partial class MainWindow : Window
    {
        
        #region IP y PUERTO del servidor

        private const string SERVER_IP = "192.168.1.45";
        private const int MESSAGES_PORT = 4660; 
        private const int VOICE_PORT = 4661; // servidor de voz de WPF Messenger

        #endregion
        
        #region constantes comunicación TCP/IP

        private const byte NEW_CONNECTION = 0;
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
        //
        private const byte USER_ONLINE = 12;
        private const byte DISCONNECT_USER = 13;

        #endregion
        
        #region atributos / fields

        // certificado SSL: se ha creado en proyecto -> propiedades -> firma -> firmar certificado de prueba
        //private X509Certificate2 cert = new X509Certificate2("MessengerServer_TemporaryKey.pfx", "wpf");

        private TcpListener messages = new TcpListener(IPAddress.Parse(SERVER_IP), MESSAGES_PORT); 
        private TcpListener voice = new TcpListener(IPAddress.Parse(SERVER_IP), VOICE_PORT); // UdpListener para UDP
        private List<User> users = new List<User>();
        private System.ComponentModel.BackgroundWorker bgwMensajes = new System.ComponentModel.BackgroundWorker();
        private System.ComponentModel.BackgroundWorker bgwVoice = new System.ComponentModel.BackgroundWorker();
        //private BinaryReader br; // si no va volver a descomentar
        //private BinaryWriter bw; // si no va volver a descomentar

        #endregion

        #region constructor

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Mensajes();
                Voz();
            }
            catch (Exception exception) { MessageBox.Show(exception.Message); }
            
        }

        #endregion

        #region mensajes

        private void Mensajes()
        {            
            messages.Start();
            listBox1.Items.Add("Listening puerto " + MESSAGES_PORT);
            bgwMensajes.DoWork += new System.ComponentModel.DoWorkEventHandler(bgwMensajes_DoWork);
            bgwMensajes.WorkerSupportsCancellation = true;
            bgwMensajes.RunWorkerAsync();
        }
        
        private void bgwMensajes_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!bgwMensajes.CancellationPending) 
            {               
                TcpClient connection = messages.AcceptTcpClient();
                ManageConnection(connection);               
            }
            e.Cancel = true;
        }
        
        private void ManageConnection(TcpClient client)
        {                        
            try
            {
                bool firstConnection = true;
                foreach (User user in users)
                {
                    if (user.TcpClient == client) firstConnection = false;
                }
                if (firstConnection)
                {
                    NetworkStream stream = client.GetStream();
                    //SslStream ssl = new SslStream(stream, false);
                    //ssl.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);

                    BinaryReader br = new BinaryReader(stream /* ssl */, Encoding.UTF8);
                    BinaryWriter bw = new BinaryWriter(stream /* ssl */, Encoding.UTF8);

                    Thread thread = new Thread(new ParameterizedThreadStart(ListenClientMessages));
                    thread.Start(new object[]{client, br, bw});
                }
            }
            catch (Exception exception)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    listBox1.Items.Add("Excepción :( " + exception.Message);
                }));
            }
        }

        private void ListenClientMessages(object argument)
        {
            TcpClient client = (TcpClient)((object[])argument)[0];
            BinaryReader br = (BinaryReader)((object[])argument)[1];
            BinaryWriter bw = (BinaryWriter)((object[])argument)[2];
            try
            {
                byte paquete = br.ReadByte();
                while (client.Client.Connected)//(client.Connected)
                {
                    if (paquete == USERS_REQUEST)
                    {
                        // enviamos al cliente los usuarios conectados
                        bw.Write(users.Count);
                        foreach (User user in users)
                        {
                            bw.Write(user.UserName);
                        }
                        bw.Flush();
                    }

                    else if (paquete == USER_CONNECTED)
                    {
                        // introducimos en users
                        User newUser = new User(client);
                        newUser.UserName = br.ReadString();
                        // informamos
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            users.Add(newUser);
                            listBox3.Items.Add(newUser.UserName);
                            listBox1.Items.Add(newUser.UserName + " se acaba de conectar " + DateTime.Now);
                        }));

                        // enviamos
                        foreach (User user in users)
                        {
                            if (user.UserName != newUser.UserName)
                            {
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                userBW.Write(USER_CONNECTED);
                                userBW.Write(newUser.UserName);
                                userBW.Flush();
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    listBox2.Items.Add("Notificación a " + user.UserName + ": " + newUser.UserName + " se acaba de conectar");
                                }));
                            }
                        }
                    }
                    else if (paquete == USER_DISCONNECTED)
                    {                        
                        string userDisconnected = br.ReadString();
                       
                        User userToRemove = null;
                        foreach (User user in users)
                        {
                            if (user.UserName == userDisconnected)
                            {
                                userToRemove = user;
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    listBox3.Items.Remove(user.UserName);
                                    listBox1.Items.Add(user.UserName + " se acaba de desconectar " + DateTime.Now);
                                }));
                            }
                        }
                        
                        users.Remove(userToRemove); 
                                               
                        // enviamos notificación a demás usuarios
                        foreach (User user in users)
                        {
                            NetworkStream userStream = user.TcpClient.GetStream();
                            BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                            userBW.Write(USER_DISCONNECTED);
                            userBW.Write(userDisconnected);
                            userBW.Flush();
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                listBox2.Items.Add("Notificación a " + user.UserName + ": " + userDisconnected + " se acaba de desconectar ");
                            }));
                        }
                        // dejamos de escuchar al usuario desconectado
                        client.Client.Close();
                        break;
                    }
                    else if (paquete == MENSAJE_ENVIADO)
                    {
                        ArrayList toUsers = new ArrayList();
                        string fromUser = br.ReadString(); 
                        int toUserCount = br.ReadInt32(); 
                        for (int i = 0; i < toUserCount;i++ ) 
                        {
                            toUsers.Add(br.ReadString()); 
                        }
                        string msg = br.ReadString();

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            foreach (string toUser in toUsers)
                              listBox1.Items.Add(fromUser + " envía un mensaje a " + toUser + DateTime.Now);
                        }));

                        bool receptorOnline = false;
                        foreach (User user in users)
                        {
                            if (toUsers.Contains(user.UserName))
                            {
                                receptorOnline = true;
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                userBW.Write(MENSAJE_RECIBIDO);
                                ArrayList temp = new ArrayList();
                                userBW.Write(fromUser);
                                CopiarArrayList(toUsers, temp);
                                temp.Remove(user.UserName);                                
                                userBW.Write(toUserCount);
                                userBW.Write(fromUser);
                                foreach (string otherUser in temp)
                                    userBW.Write(otherUser);
                                userBW.Write(msg);
                                userBW.Flush();
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    listBox2.Items.Add(user.UserName + " recibe mensaje de " + fromUser + DateTime.Now);
                                }));
                            }
                        }
                        if (!receptorOnline)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                listBox2.Items.Add("Receptor offline: El mensaje no pudo ser enviado");
                            }));
                        }
                    }                    
                    else if (paquete == VOICE_REQUEST)
                    {
                        ArrayList toUsers = new ArrayList();
                        string fromUser = br.ReadString();
                        int localVoicePort = br.ReadInt32();
                        int toUserCount = br.ReadInt32();                        
                        for (int i = 0; i < toUserCount; i++)
                        {
                            toUsers.Add(br.ReadString());
                        }
                        string voiceRoom = br.ReadString();
                        int hablando = 1; // usuarios hablando, el usuario que solicita es uno de ellos 
                        int pendientes = 0; // calculamos usuarios que están pendientes de participar                        
                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {
                                user.Hablando = 2; // usuario que envió la solicitud es el único activo
                                user.VoicePort = localVoicePort;
                                user.VoiceRoom = voiceRoom;
                            }
                            if (toUsers.Contains(user.UserName))
                            {
                                user.Hablando = 1; // los demás están pendientes de participar
                                pendientes++;
                            }
                        }

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            foreach (string toUser in toUsers)
                                listBox1.Items.Add(fromUser + " realiza una llamada a " + toUser + DateTime.Now);
                        }));

                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {
                                bw.Write(VOICE_REQUEST);
                                bw.Write(fromUser);
                                bw.Write(toUserCount);
                                foreach (string toUser in toUsers)
                                    bw.Write(toUser);
                                bw.Write(voiceRoom);
                                bw.Write(hablando);
                                bw.Write(pendientes);
                                bw.Flush();
                            }
                            else if (toUsers.Contains(user.UserName))
                            {
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                userBW.Write(VOICE_REQUEST);
                                ArrayList temp = new ArrayList();
                                // ahora fromUser es un receptor de user.UserName
                                userBW.Write(fromUser); // es el receptor activo
                                CopiarArrayList(toUsers, temp);
                                temp.Remove(user.UserName);
                                userBW.Write(toUserCount);
                                userBW.Write(fromUser); // lo incluimos a la lista de receptores
                                foreach (string otherUser in temp)
                                    userBW.Write(otherUser); // incluimos a los demás
                                userBW.Write(voiceRoom);
                                userBW.Write(hablando);
                                userBW.Write(pendientes);
                                userBW.Flush();
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    listBox2.Items.Add(user.UserName + " recibe la llamada de " + fromUser + ": " + DateTime.Now);
                                }));
                            }
                        }
                    }

                    else if (paquete == VOICE_ACCEPT)
                    {
                        ArrayList toUsers = new ArrayList();
                        string fromUser = br.ReadString();
                        int toUserCount = br.ReadInt32();
                        for (int i = 0; i < toUserCount; i++)
                        {
                            toUsers.Add(br.ReadString());
                        }

                        string voiceRoom = br.ReadString();
                        int localPort = br.ReadInt32();

                        int hablando = 1; // usuarios hablando, el que acepta es uno de ellos
                        int pendientes = 0; // calculamos usuarios pendientes de participar

                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {
                                user.Hablando = 2; // lo incluimos como activo en la conversación
                                user.VoicePort = localPort;
                                user.VoiceRoom = voiceRoom;                               
                            }
                            if (toUsers.Contains(user.UserName) && user.Hablando==1) pendientes++;
                            if (toUsers.Contains(user.UserName) && user.Hablando==2) hablando++;
                        }
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            listBox1.Items.Add(fromUser + " acepta la llamada" + ": " + DateTime.Now);
                        }));
                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {
                                bw.Write(VOICE_ACCEPT);
                                bw.Write(fromUser);
                                bw.Write(toUserCount);
                                foreach (string otherUser in toUsers)
                                    bw.Write(otherUser);
                                bw.Write(hablando);
                                bw.Write(pendientes);
                                bw.Flush();
                            }
                            else if (toUsers.Contains(user.UserName))
                            {
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                userBW.Write(VOICE_ACCEPT);
                                ArrayList temp = new ArrayList();
                                userBW.Write(fromUser);
                                CopiarArrayList(toUsers, temp);
                                temp.Remove(user.UserName);
                                userBW.Write(toUserCount);
                                userBW.Write(fromUser);
                                foreach (string otherUser in temp)
                                    userBW.Write(otherUser);
                                userBW.Write(hablando);
                                userBW.Write(pendientes);
                                userBW.Flush();
                            }
                        }
                    }

                    else if (paquete == VOICE_REJECT)
                    {
                        ArrayList toUsers = new ArrayList();
                        string fromUser = br.ReadString();

                        int toUserCount = br.ReadInt32();
                        for (int i = 0; i < toUserCount; i++)
                        {
                            toUsers.Add(br.ReadString());
                        }
                        int hablando = 0;
                        int pendientes = 0;
                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser) user.Hablando = 0; 
                            if (toUsers.Contains(user.UserName) && user.Hablando==2) hablando++;
                            if (toUsers.Contains(user.UserName) && user.Hablando==1) pendientes++;
                        }
                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {
                                if (hablando == 1 && pendientes == 0)
                                    bw.Write(VOICE_END);
                                else
                                    bw.Write(VOICE_REJECT);
                                bw.Write(fromUser);
                                bw.Write(toUserCount);
                                foreach (string otherUser in toUsers)
                                    bw.Write(otherUser);
                                bw.Write(hablando);
                                bw.Write(pendientes);
                                bw.Flush();
                            }
                            if (toUsers.Contains(user.UserName))
                            {
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                if (hablando == 1 && pendientes == 0)
                                    userBW.Write(VOICE_END);
                                else
                                    userBW.Write(VOICE_REJECT);
                                ArrayList temp = new ArrayList();
                                userBW.Write(fromUser);
                                CopiarArrayList(toUsers, temp);
                                temp.Remove(user.UserName);
                                userBW.Write(toUserCount);
                                userBW.Write(fromUser);
                                foreach (string otherUser in temp)
                                    userBW.Write(otherUser);
                                userBW.Write(hablando);
                                userBW.Write(pendientes);
                                userBW.Flush();
                            }
                            if (hablando == 1 && pendientes == 0)
                                FinalizarLlamada();
                        }
                    }                  

                    else if (paquete == VOICE_END)
                    {
                        ArrayList toUsers = new ArrayList();
                        string fromUser = br.ReadString();
                        int toUserCount = br.ReadInt32();
                        for (int i = 0; i < toUserCount; i++)
                        {
                            toUsers.Add(br.ReadString());
                        }

                        int hablando = 0;
                        int pendientes = 0;
                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                                user.Hablando = 0;
                            if (toUsers.Contains(user.UserName) && user.Hablando==2) hablando++;
                            if (toUsers.Contains(user.UserName) && user.Hablando==1) pendientes++;
                        }
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            listBox1.Items.Add(fromUser + " abandona la llamada" + ": " + DateTime.Now);
                        }));

                        foreach (User user in users)
                        {
                            if (user.UserName == fromUser)
                            {                                
                                bw.Write(VOICE_END);
                                bw.Write(fromUser);
                                bw.Write(toUserCount);
                                foreach (string otherUser in toUsers)
                                    bw.Write(otherUser);
                                bw.Write(hablando);
                                bw.Write(pendientes);
                                bw.Flush();
                            }

                            if (toUsers.Contains(user.UserName))   
                            {
                                NetworkStream userStream = user.TcpClient.GetStream();
                                BinaryWriter userBW = new BinaryWriter(userStream /*ssl*/, Encoding.UTF8);
                                userBW.Write(VOICE_END);
                                ArrayList temp = new ArrayList();
                                userBW.Write(fromUser);
                                CopiarArrayList(toUsers, temp);
                                temp.Remove(user.UserName);
                                userBW.Write(toUserCount);
                                userBW.Write(fromUser);
                                foreach (string otherUser in temp)
                                    userBW.Write(otherUser);
                                userBW.Write(hablando);
                                userBW.Write(pendientes);
                                userBW.Flush();
                            }
                        }
                        if (hablando == 0 || (hablando == 1 && pendientes == 0))
                        {
                            FinalizarLlamada();                            
                        }
                    }                   
                    paquete = br.ReadByte();
                }
            }
            catch (Exception exception)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    listBox1.Items.Add("Excepción :( " + exception.Message);
                }));
            }
        }

        private void CopiarArrayList(ArrayList original, ArrayList copia)
        {
            foreach (string usuario in original)
            {
                copia.Add(usuario);
            }
        }

        private void FinalizarLlamada()
        {
            foreach (User user in users)
            {
                user.Hablando = 0;
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                listBox1.Items.Add("Llamada finalizada: " + DateTime.Now);
            }));
        }
        
        #endregion

        #region voz

        // el servidor se pone a la escucha de solicitudes de conexiones entrantes 
        private void Voz()
        {
            voice.Start();
            listBox1.Items.Add("Listening puerto " + VOICE_PORT);
            bgwVoice.DoWork += new System.ComponentModel.DoWorkEventHandler(bgwVoice_DoWork);
            bgwVoice.WorkerSupportsCancellation = true;                         
            bgwVoice.RunWorkerAsync();       
        }

        // acepta conexiones de clientes entrantes
        private void bgwVoice_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!bgwVoice.CancellationPending)
            {
                TcpClient client = voice.AcceptTcpClient();
                
                Thread.Sleep(300); // para que el usuario recoja su TcpClientVoice 
                try
                {
                    foreach (User user in users)
                    {
                        string userIP = ((IPEndPoint)user.TcpClient.Client.RemoteEndPoint).Address.ToString();
                        string clienteIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                        int userPort = user.VoicePort;
                        int clientePort = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
                        if (userIP == clienteIP && userPort == clientePort)
                        {
                            user.TcpClientVoice = client;
                        }
                    }
                    
                    Thread thread = new Thread(new ParameterizedThreadStart(ManageClient));
                    thread.Start(client);
                    
                }
                catch (Exception exception)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        listBox1.Items.Add("Excepción :( " + exception.Message);
                    }));
                }

            }
            e.Cancel = true;
        }

        // trata la conexión entrante
        private void ManageClient(object argument)
        {
            TcpClient client = (TcpClient)argument;
            NetworkStream stream = client.GetStream();
            //SslStream ssl = new SslStream(stream, false);
            //ssl.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);

            BinaryReader br = new BinaryReader(stream /* ssl */, Encoding.UTF8);
            BinaryWriter bw = new BinaryWriter(stream /* ssl */, Encoding.UTF8);

            try
            {
                while (client.Client.Connected)//(client.Connected)
                {
                    int read = 0, readSoFar = 0;
                    byte[] buffer = new byte[2205];

                    while (readSoFar < 2205)
                    {
                        read = br.Read(buffer, readSoFar, buffer.Length - readSoFar);
                        readSoFar += read;
                        if (read == 0)
                        {
                            break;
                        }
                    }
                    
                    if (read != 0)
                    {
                        User clientUser = null;
                        foreach (User user in users)
                        {
                            if (user.TcpClientVoice == client)
                                clientUser = user;
                        }

                        foreach (User user2 in users)
                        {
                            // para pruebas puedo quitar la segunda condición (para recibir tb por la ventana que se emite el sonido)
                            if (user2.TcpClientVoice != null && client!=user2.TcpClientVoice && clientUser.VoiceRoom == user2.VoiceRoom)
                            {
                                try
                                {
                                    user2.TcpClientVoice.Client.Send(buffer);
                                }
                                catch (SocketException) { /*user2.TcpClientVoice.Client.Close(); */}
                            }

                        }
                    }                   
                }
                client.Client.Shutdown(SocketShutdown.Receive);
                client.Close();
                client = null;
            }
            catch (IOException){ }
        }

        #endregion

    }
}
