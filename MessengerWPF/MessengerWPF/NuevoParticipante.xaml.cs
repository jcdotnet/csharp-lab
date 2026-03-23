using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Lógica de interacción para NuevoParticipante.xaml
    /// </summary>
    public partial class NuevoParticipante : Window
    {
        ArrayList _participantes;
        ObservableCollection<string> _disponibles = new ObservableCollection<string>();

        public ObservableCollection<string> Disponibles
        {
            get { return _disponibles; }
            set { _disponibles = value; }
        }

        public NuevoParticipante(string emisor, ArrayList participantes)
        {
            InitializeComponent();
            _participantes = participantes;
            DataTable dtUsuarios = new Login().Get(); // cuando esté la columna online -> WHERE online
            foreach (DataRow dr in dtUsuarios.Rows)
            {
                if (emisor !=(string)dr["Usuario"] && !participantes.Contains((string)dr["Usuario"]))
                    _disponibles.Add((string)dr["Usuario"]);
            }
            this.DataContext = this;

        }

        private void btAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (lbParticipantes.SelectedItem!=null)_participantes.Add(lbParticipantes.SelectedItem);   
            this.DialogResult = true;
            this.Close();
        }

        private void cbUsuario_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cbUsuario = (CheckBox)sender;
            _participantes.Add(cbUsuario.Content);
        }

      

        
    }
}
