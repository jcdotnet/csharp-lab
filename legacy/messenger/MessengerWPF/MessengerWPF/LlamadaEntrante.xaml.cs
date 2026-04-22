using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Lógica de interacción para LlamadaEntrante.xaml
    /// </summary>
    public partial class LlamadaEntrante : Window
    {
        // poner background o bordes al color de Messenger en vez de blanco
        public event EventHandler RespuestaUsuario;

        private string _usuario;
        private string _sala;
        private bool _exitOnButton=false;

        public LlamadaEntrante(string usuario, string sala)
        {
            InitializeComponent();
            Chat.ChatCerrado+=new EventHandler(Chat_ChatCerrado);
            _usuario=usuario;
            _sala = sala;
            this.lbUsuario.Content = "Llamada de " + usuario;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // RING RING
        }

        private void btAceptar_Click(object sender, RoutedEventArgs e)
        {
            RespuestaUsuario(new object[]{true, _sala}, EventArgs.Empty);
            _exitOnButton = true;
            this.Close();
        }

        private void btRechazar_Click(object sender, RoutedEventArgs e)
        {
            RespuestaUsuario(new object[] { false, _sala }, EventArgs.Empty);
            _exitOnButton = true;
            this.Close();
        }

        private void Chat_ChatCerrado(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string usuario = (string)sender;
                if (usuario==_usuario)
                    this.Close();
            }));
        }      

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_exitOnButton)
            {
                e.Cancel = false;
            }
            else //if timeOut // si implementamos tiempo máximo de espera
            {               
                // e.Cancel=true
                // lanzamos evento que se controla en chat.xaml.cs y lo que hace es llamar a MainWindow.RechazarLlamada
                // establecemos timeOut= false para que no entre en e.Cancel=true y haga el close
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() != typeof(Button)) // quizás no haga falta
                this.DragMove();
        }       

    }
}
