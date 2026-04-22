using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MessengerWPF
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {

        //private Mutex _tcMutex = null; // descomentar en versión de lanzamiento
        //private bool createdNew=true; // descomentar en versión de lanzamiento

        public App()
        {
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // obtenemos el idioma por defecto establecido en el sistema operativo
 
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InstalledUICulture;
            ResourceDictionary dict = new ResourceDictionary();
            switch (System.Globalization.CultureInfo.InstalledUICulture.Name)
            {
                case "es-ES": dict.Source = new Uri("/Localization/Messenger.es-ES.xaml", UriKind.Relative); break;
                case "en-GB": dict.Source = new Uri("/Localization/Messenger.en-GB.xaml", UriKind.Relative); break;
                default: // si el idioma del SO es otro se muestra la interfaz en inglés 
                    dict.Source = new Uri("/Localization/Messenger.en-GB.xaml", UriKind.Relative); break;
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);
            
            /* descomentar en versión de lanzamiento
            _tcMutex = new Mutex(true, "tcMutex", out createdNew);
            
            if (!createdNew)
            {
                // myApp is already running...
                MessageBox.Show("myApp is already running!", "Multiple Instances");
                Application.Current.Shutdown();
                return;
            }
            */
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            /* descomentar en versión de lanzamiento
            if (_tcMutex != null && createdNew)
                _tcMutex.ReleaseMutex();
             */
            base.OnExit(e);
        }
        
    }
}
    

