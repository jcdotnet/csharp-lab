#region usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JC.BD.Messenger;

#endregion

namespace MessengerWPF
{
    /// <summary>
    /// Lógica de interacción para Messenger.xaml
    /// </summary>
    public partial class Messenger : Window
    {
        public const string SERVER_IP = "192.168.1.100"; // "85.137.124.174"; // "192.168.1.57"; // 
        public const int SERVER_PORT = 4660;
        public const int SERVER_AUDIO = 4661;         

        public Messenger()
        {
            InitializeComponent();
            InitializeLanguageMenu();
            
            MainWindow.UsuarioDesconectado += new EventHandler(MainWindow_UsuarioDesconectado);

            MainWindow.notifyIcon.Visible = true;
            MainWindow.notifyIcon.Text = MessengerWPF.Properties.Resources.SesionNoIniciada; 
            // icono: props del archivo -> acción de compilación: recurso incrustado, copiar en dir si es posterior
            MainWindow.notifyIcon.Icon = new System.Drawing.Icon("Images/logo16.png");        
        }        

        private void linkPassWord_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            MessageBox.Show("TODO: Redireccionar a página Web");
        }

        private void linkRegistro_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            MessageBox.Show("TODO: Redireccionar a página Web\nMientras tanto podemos insertar usuarios en la tabla de pruebas");
            Registro registro = new Registro();
            registro.ShowDialog();
        }
        
        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsuario.Text.Length == 0 || tbUsuario.Text.Length > 10)
                MessageBox.Show("El nombre de usuario debe contener entre 1 y 10 caracteres");
            else if (tbPassword.Password.Length == 0 || tbPassword.Password.Length > 10)
                MessageBox.Show("La contraseña debe contener entre 1 y 10 caracteres");
            else
            {
                DataTable usuarios = new Login().Get("usuario='" + tbUsuario.Text + "' AND clave ='" + tbPassword.Password + "'");
                if (usuarios.Rows.Count == 0)
                    MessageBox.Show("Usuario y/o contraseña incorrectos");
                else
                {
                    try
                    {                     
                        User usuario = new User((string)usuarios.Rows[0]["Usuario"]);
                        usuario.TcpClient = new TcpClient(SERVER_IP, SERVER_PORT);
                        MainWindow main = new MainWindow(usuario);
                        main.Show();
                        tbPassword.Password = "";
                        this.Visibility = Visibility.Hidden;
                        // cambiamos texto icono, también se puede cambiar el icono
                        MainWindow.notifyIcon.Text = "Messenger (online)"; 
                    }
                    catch (Exception exception) { MessageBox.Show(exception.Message); }
                }
            }
        }

        private void MainWindow_UsuarioDesconectado(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void menuSalir_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.notifyIcon.Visible = false;
            MainWindow.notifyIcon.Dispose();
            MainWindow.notifyIcon = null;
        }

        private void menuEnglish_Click(object sender, RoutedEventArgs e)
        {
            if (!menuEnglish.IsChecked)
            {
                ChangeCulture(new System.Globalization.CultureInfo("en-GB"));                
                menuEnglish.IsChecked = true;
                menuEspañol.IsChecked = false;
            }
        }

        private void menuEspañol_Click(object sender, RoutedEventArgs e)
        {
            if (!menuEspañol.IsChecked)
            {
                ChangeCulture(new System.Globalization.CultureInfo("es-ES"));  
                menuEnglish.IsChecked = false;
                menuEspañol.IsChecked = true;
            }
        }

        #region private 

        private void InitializeLanguageMenu()
        {
            switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name)
            {
                case "es-ES": menuEspañol.IsChecked = true; break;
                case "en-GB": menuEnglish.IsChecked = true; break;
            }
        }

        private void ChangeCulture(System.Globalization.CultureInfo culture)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            ResourceDictionary dict = new ResourceDictionary();
            switch (culture.Name)
            {
                case "es-ES": dict.Source = new Uri("/Localization/Messenger.es-ES.xaml", UriKind.Relative); break;
                case "en-GB": dict.Source = new Uri("/Localization/Messenger.en-GB.xaml", UriKind.Relative); break;
            }            
            this.Resources.MergedDictionaries.Add(dict);
        }

        #endregion
    }
}