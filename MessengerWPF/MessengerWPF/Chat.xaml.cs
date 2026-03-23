using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel; //
using System.IO; // 
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MessengerWPF
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Chat : Window
    {

        #region fields

        private ChatAbierto chat;
        private TcpClient voiceClient;
        private DirectSoundHelper sound;
        private Thread thread = null;
        private BinaryWriter bw;
        private byte[] buffer = new byte[2205];
        private System.ComponentModel.BackgroundWorker bgwVoice = new System.ComponentModel.BackgroundWorker();
        private LlamadaEntrante llamadaWindow=null;
        internal static int entradasEvento = 0; // controla que no se dispare más de una vez LlamadaRecibida

        #endregion

        public static event EventHandler ChatCerrado;

        public Chat(object fromMain)
        {
            InitializeComponent();
            
            MainWindow.MensajeRecibido += new MainWindow.ChatEventHandler(MainWindow_MensajeRecibido);
            MainWindow.LlamadaRecibida += new MainWindow.ChatEventHandler(MainWindow_LlamadaRecibida);
            MainWindow.PendientesChanged += new MainWindow.ChatEventHandler(MainWindow_PendientesChanged);            
            MainWindow.LlamadaFinalizada += new MainWindow.ChatEventHandler(MainWindow_LlamadaFinalizada);
            MainWindow.UsuarioDesconectado += new EventHandler(MainWindow_UsuarioDesconectado);

            sound = new DirectSoundHelper();
            sound.OnBufferFulfill += new EventHandler(SendVoiceBuffer);  

            this.chat = (ChatAbierto)fromMain;
            this.Title = GetTitle();
            tbEntrada.Focus();           

            thread = new Thread(new ThreadStart(sound.StartCapturing));
            thread.IsBackground = true;
            thread.Start();
        }       

        #region controladores eventos de los controles

        private void btAgregar_Click(object sender, RoutedEventArgs e)
        {
            NuevoParticipante nuevoDialog = new NuevoParticipante(this.chat.Emisor.UserName, this.chat.Receptores);
            if (nuevoDialog.ShowDialog().Value)
            {
                this.Title = GetTitle();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //tbMensajes.Height = this.Height - 150;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (llamadaWindow != null)
            {
                if (ChatCerrado != null)                                // este evento static (+mem) para cerrar llamadaWindow se debe evitar
                    ChatCerrado(chat.ReceptorActivo, EventArgs.Empty); // con llamadaWindow.Close() o no poniendo nada debería ser suficiente
                //llamadaWindow.Close();                               
                llamadaWindow = null;
                MainWindow.RechazarLlamada(chat); // no hay socket abierto  
                Thread.Sleep(300);
            }

            else if (chat.Emisor.Hablando)
            {
                chat.Emisor.Hablando = false;
                MainWindow.ColgarUsuario(chat);

                Thread.Sleep(100);
                if (bgwVoice.IsBusy) bgwVoice.CancelAsync();
                if (voiceClient != null)
                {
                    voiceClient.Client.Shutdown(SocketShutdown.Send);
                    voiceClient.Close();
                    voiceClient = null;
                }
            }

            MainWindow.CerrarChat(this.chat);
        }

        #endregion

        #region controladores eventos de los controles (MENSAJES)

        private void btEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbEntrada.Text.Length > 0)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.EnviarMensaje(chat, tbEntrada.Text);
                    }));
                }
                if (tbEntrada.Text.Length > 0)
                {
                    tbMensajes.Text += String.Format("{0}\r\n", tbEntrada.Text);
                    tbEntrada.Text = "";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        #endregion

        #region controladores eventos de los controles (VOZ)

        private void btStartAudio_Click(object sender, RoutedEventArgs e)
        {
            if (chat.Emisor.Hablando || chat.Llamada)
            {
                MessageBox.Show("Espere que termine la llamda en curso");
                return;
            }
            sound.StopLoop = false; // ¿iría esto después de solicitar audio? así envía buffer antes de crear el socket. 
            ConectarVoz();
            Thread.Sleep(100); // damos tiempo a que se cree voiceClient que se ejecuta en paralelo 

            MainWindow.SolicitarAudio(chat, ((IPEndPoint)voiceClient.Client.LocalEndPoint).Port);

            chat.Emisor.Hablando = true;
            btStartAudio.IsEnabled = false;
            btStopAudio.IsEnabled = true;
            btAgregar.IsEnabled = false;
        }

        private void btStopAudio_Click(object sender, RoutedEventArgs e)
        {

            sound.StopLoop = true;

            chat.Emisor.Hablando = false;
            btStartAudio.IsEnabled = true;
            btStopAudio.IsEnabled = false;
            btAgregar.IsEnabled = true;

            MainWindow.ColgarUsuario(chat);

            Thread.Sleep(100);
            if (bgwVoice.IsBusy)
            {
                bgwVoice.CancelAsync();
            }
            if (voiceClient != null)
            {
                voiceClient.Client.Shutdown(SocketShutdown.Send); // ¿ambos?
                voiceClient.Close();
                voiceClient = null;
            }
        }

        private void bgwVoice_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (voiceClient != null)
            {
                voiceClient.Close();
                voiceClient = null;
            }
            Thread.Sleep(10); // ¿??
            // creamos socket TCP
            voiceClient = new TcpClient(Messenger.SERVER_IP, Messenger.SERVER_AUDIO);
            // leemos y enviamos byte[] 
            NetworkStream stream = voiceClient.GetStream();
            //SslStream ssl = new SslStream(stream, false);
            //ssl.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);

            BinaryReader br = new BinaryReader(stream /* ssl */, Encoding.UTF8);
            bw = new BinaryWriter(stream /* ssl */, Encoding.UTF8);
            while (!bgwVoice.CancellationPending)
            {
                try
                {
                    while (voiceClient.Client.Connected)//(client.Connected)
                    {
                        int read = 0, readSoFar = 0;
                        byte[] buffer = new byte[2205];

                        while (readSoFar < 2205)
                        {
                            read = br.Read(buffer, readSoFar, buffer.Length - readSoFar);
                            readSoFar += read;
                            if (read == 0)
                            {
                                return;  // break;   // se ha perdido la conexión
                            }
                        }
                        sound.PlayReceivedVoice(buffer);
                    }
                }
                catch (IOException) { }
            }
            e.Cancel = true;
        }

        #endregion

        #region controladores eventos personalizados

        private void MainWindow_UsuarioDesconectado(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Close();
            }));
        }
        
        #endregion
        
        #region controladores eventos personalizados (MENSAJES)

        private void MainWindow_MensajeRecibido(object sender, ChatEventArgs e)
        {
            // si la ventana no tiene el focus (o está minimizada) tiene que parpadear   
            // además debe haber un campo en la base de datos que indique si se notifica por sonido, tiene
            // que almacenarse en la BD para no tener q configurarlo cada vez que se ejecute TC
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (e.ChatReceptor.Equals(this.chat))
                    {
                        tbMensajes.AppendText(String.Format("{0} dice: {1}\r\n", e.ChatReceptor.ReceptorActivo, e.Mensaje));
                        tbMensajes.ScrollToEnd();
                    }
                }));
            }
            catch (Exception exception) { MessageBox.Show(exception.Message); }
            
        }

        
        #endregion

        #region controladores eventos personalizados (VOZ)

        private void llamada_RespuestaUsuario(object sender, EventArgs e)
        {
            bool respuesta = (bool)((object[])sender)[0];
            string sala = (string)((object[])sender)[1];
            if (respuesta)
            {
                sound.StopLoop = false;

                ConectarVoz();
                Thread.Sleep(100); // damos tiempo a que se cree voiceClient que se ejecuta en paralelo 

                MainWindow.AceptarLLamada(chat, sala, ((IPEndPoint)voiceClient.Client.LocalEndPoint).Port);

                chat.Emisor.Hablando = true;

                this.Dispatcher.Invoke((Action)(() =>
                {
                    btStartAudio.IsEnabled = false;
                    btStopAudio.IsEnabled = true;
                    btAgregar.IsEnabled = false;
                }));
            }
            else
            {
                MainWindow.RechazarLlamada(chat);
            }
            llamadaWindow = null;
        }

        private void MainWindow_LlamadaRecibida(object sender, ChatEventArgs e)
        {
            if (entradasEvento == 0 && e.ChatReceptor != null) 
            {
                // entradasevento: el método CerrarChat debe mantener solo las ventanas de conversación 
                // actuales, mirar si hay algún caso donde se cierre una ventana de conversación y no se le llame
                // si entra varias veces es porque hay varios chats asociados a la misma ventana de conversación
                entradasEvento++; 
                //  try
                // {                  
                //
                if (e.ChatReceptor.Equals(this.chat))
                {
                    llamadaWindow = new LlamadaEntrante(e.ChatReceptor.ReceptorActivo, e.Mensaje);
                    llamadaWindow.RespuestaUsuario += new EventHandler(llamada_RespuestaUsuario);
                    llamadaWindow.Show();
                    //         if (MessageBox.Show("LLamada de " + e.ChatReceptor.ReceptorActivo + ", ¿responder", "Llamada entrante",
                    //             MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    //         {

                    //         }
                    //         else
                    //          {
                    //              MainWindow.RechazarLlamada(chat);
                    //        }
                }

                //
                //catch (Exception exception) { MessageBox.Show("Excepción en Llamada Recibida: "+ exception.Message); }
            }
        }

        private void MainWindow_PendientesChanged(object sender, ChatEventArgs e)
        {
            if (e.ChatReceptor.Equals(chat) && e.Mensaje != null && e.Mensaje == "request")
                chat.Llamada = true;

            if (e.ChatReceptor.Equals(chat))
            {
                chat.Pendientes = e.ChatReceptor.Pendientes;
                chat.Hablando = e.ChatReceptor.Hablando;
            }

            this.Dispatcher.Invoke((Action)(() =>
            {
                if (e.ChatReceptor.Equals(chat) && chat.Emisor.Hablando) 
                {
                    tbMensajes.AppendText(String.Format("Chat de voz: Hablando {0}, pendientes: {1}\r\n", chat.Hablando, chat.Pendientes));
                    tbMensajes.ScrollToEnd();
                }
            }));
        }

        private void MainWindow_LlamadaFinalizada(object sender, ChatEventArgs e)
        {
            chat.Hablando = e.ChatReceptor.Hablando;
            chat.Pendientes = e.ChatReceptor.Pendientes;

            if (e.ChatReceptor.Equals(chat))
            {
                if (llamadaWindow != null)
                {
                    llamadaWindow.Visibility = Visibility.Hidden;
                    llamadaWindow.Close();
                    llamadaWindow = null;
                }
                else
                {
                    try
                    {
                        sound.StopLoop = true;

                        if (bgwVoice.IsBusy) bgwVoice.CancelAsync();
                        if (voiceClient != null) voiceClient.Close();

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            btStartAudio.IsEnabled = true;
                            btStopAudio.IsEnabled = false;
                            btAgregar.IsEnabled = true;
                        }));
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }

            this.Dispatcher.Invoke((Action)(() =>
            {
                if (e.ChatReceptor.Equals(this.chat))
                {
                    tbMensajes.AppendText(String.Format("Chat de voz finalizado: Hablando {0}, pendientes: {1}\r\n", chat.Hablando, chat.Pendientes));
                    tbMensajes.ScrollToEnd();
                }
            }));
        }

        #endregion

        #region métodos privados

        private string GetTitle()
        {
            string title = "Para ";
            foreach (string receptor in this.chat.Receptores)
            {
                title += receptor + " ";
            }
            return title;
        }

        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void ConectarVoz()
        {
            bgwVoice.DoWork += new System.ComponentModel.DoWorkEventHandler(bgwVoice_DoWork);
            bgwVoice.WorkerSupportsCancellation = true;
            bgwVoice.RunWorkerAsync();
        }

        private void SendVoiceBuffer(object VoiceBuffer, EventArgs e)
        {
            byte[] Buffer = (byte[])VoiceBuffer;
            SendBuffer(Buffer);
        }

        private void SendBuffer(byte[] buffer)
        {
            voiceClient.Client.Send(buffer);
            //bw.Write(buffer, 0, buffer.Length);
            //bw.Flush();
        }

        #endregion

    }
}
