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

using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Cache;

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


            //Inicializa os demais componentes e recebe informações iniciais do spotify
            spotifyStatus = spotifyController.GetStatus();

            songProgress.Maximum = spotifyStatus.Track.Length;
            musicName.Content = spotifyStatus.Track.TrackResource.Name;
            artistNameLabel.Content = spotifyStatus.Track.ArtistResource.Name;
            //albumArt.Source = ByteImageConverter.ByteToImage(spotifyStatus.Track.GetAlbumArtAsByteArray(AlbumArtSize.Size640));
            updateCover(spotifyStatus.Track.GetAlbumArtUrl(AlbumArtSize.Size640));
            spotifyController.ListenForEvents = true;
            spotifyController.OnTrackTimeChange += SpotifyController_OnTrackTimeChange;
            spotifyController.OnTrackChange += SpotifyController_OnTrackChange;
        }

        private void SpotifyController_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                
                musicName.Content = e.NewTrack.TrackResource.Name;
                artistNameLabel.Content = e.NewTrack.ArtistResource.Name;
                songProgress.Maximum = e.NewTrack.Length;
                //albumArt.Source = ByteImageConverter.ByteToImage(spotifyStatus.Track.GetAlbumArtAsByteArray(AlbumArtSize.Size640));
                updateCover(e.NewTrack.GetAlbumArtUrl(AlbumArtSize.Size320));
                
            }));
        }

        private void SpotifyController_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                songProgress.Value = e.TrackTime;
            }));
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

        private void updateCover(string information)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(information);
                bitmapImage.EndInit();

                albumArt.Source = bitmapImage;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
