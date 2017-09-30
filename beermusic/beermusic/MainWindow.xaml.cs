using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System.IO;
using System.Diagnostics;

namespace beermusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleHTTPServer barServer;
        SpotifyLocalAPI spotifyController;
        StatusResponse spotifyStatus;
        public MainWindow()
        {
            InitializeComponent();

            //A string abaixo define a pasta que conterá os ".html" que serão servidos
            string httpServeAddress = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString() + @"\webresources";
            //Inicializa o server no seu IP na porta 8084
            barServer = new SimpleHTTPServer(httpServeAddress, 8084);
            labelStatus.Content = "Server is running on this port: " + barServer.Port.ToString();

            //Para acessar, entre em "localhost:8084" ou seuip:8084 na rede local.
            //Desative o firewall do windows! ACREDITE!

            spotifyController = new SpotifyLocalAPI();
            //Verifica se o spotify está aberto.
            if (!SpotifyLocalAPI.IsSpotifyRunning())
                return;
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                return;

            if (!spotifyController.Connect())
                return;

            spotifyStatus = spotifyController.GetStatus();

            musicName.Content = spotifyStatus.Track.TrackResource.Name;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Para o servidor HTTP quando o programa fechar.
            barServer.Stop();
        }

        private void pausePlay_Click(object sender, RoutedEventArgs e)
        {
            if (spotifyController != null)
            {
                spotifyStatus = spotifyController.GetStatus();
                if (spotifyStatus.Playing)
                {
                    spotifyController.Pause();
                } else
                {
                    spotifyController.Play();
                }
            }
        }
    }
}
