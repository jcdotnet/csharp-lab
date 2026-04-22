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
using JC.BD.Messenger;

namespace MessengerWPF
{
    /// <summary>
    /// Lógica de interacción para Registro.xaml
    /// </summary>
    public partial class Registro : Window
    {
        public Registro()
        {
            InitializeComponent();
        }

        private void btRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsuario.Text.Length == 0 || tbUsuario.Text.Length > 10)
                MessageBox.Show("El nombre de usuario debe contener entre 1 y 10 caracteres");
            else if (tbPassword.Password.Length == 0 || tbPassword.Password.Length > 10)
                MessageBox.Show("La contraseña debe contener entre 1 y 10 caracteres");
            else if (tbPassword.Password.Length == 0 || tbPassword.Password.Length > 10)
                MessageBox.Show("La contraseña debe contener entre 1 y 10 caracteres");
            else if (tbPassword.Password != tbPassword2.Password)
                MessageBox.Show("Las contraseñas no coinciden");
            else
            {
                try
                {
                    LoginRow registro = new LoginRow();
                    registro.Usuario = tbUsuario.Text;
                    registro.Clave = tbPassword.Password;
                    bool insert = new Login().Insert(registro);
                    this.Close();
                }
                catch (Exception exception) { MessageBox.Show(exception.Message); }

            }
        }
    }
}
