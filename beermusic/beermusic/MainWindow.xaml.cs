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

        List<Music> currentlyVotingSongs = new List<Music>();
        static Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            //Contador de tempo para iniciar o spotify
            Stopwatch sw = new Stopwatch();
            bool iniciou = false;

            //Pega o caminho da pasta roaming do usuário
            string pasta = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            spotifyController = new SpotifyLocalAPI();
            //Verifica se o spotify está aberto.
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                //se não estiver aberto, inicia o contador de tempo e tenta iniciar o sptf
                sw.Start();               
                System.Diagnostics.Process.Start(pasta + @"\Spotify\Spotify.exe");
                //Espera 15 segundos até o pc conseguir abrir o programa
                while(sw.Elapsed.Seconds <= 15){
                    if (SpotifyLocalAPI.IsSpotifyRunning())
                    {
                        iniciou = true;
                        //Após abrir, espera 10 segundos para a inicialização (gambiarras ftw)
                        System.Threading.Thread.Sleep(10000);
                        sw.Stop();
                        break;
                    }                        
                }
                if (!iniciou)
                {
                    MessageBox.Show(@"Não foi possível iniciar o programa, tente novamente");
                    return;
                }
            }

            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
                return;
            //if (!spotifyController.Connect())
            //    return;

            //A string abaixo define a pasta que conterá os ".html" que serão servidos
            string httpServeAddress = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString() + @"\webresources";

            //Inicializa o server no seu IP na porta 8084
            barServer = new SimpleHTTPServer(httpServeAddress, 8084);
            labelStatus.Content = "Server is running on this port: " + barServer.Port.ToString();

            //Para acessar, entre em "localhost:8084" ou seuip:8084 na rede local.
            //Desative o firewall do windows! ACREDITE!

            bool successful = spotifyController.Connect();
            if (successful)
            {
                //************************    Antes estava fora da condição "if"    **************************************************
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
            else
            {
                MessageBox.Show(@"Não foi possível conectar ao Spotify");
                return;
            }
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
            chooseSongsForVoting();
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

        private void chooseSongsForVoting()
        {
            //Verifica se já possui a lista de músicas prontas
            if (!Music.hasDBPopulated)
            {
                //Inicializa o "banco de dados" de músicas
                Music.parseMusicList();
            }

            for (int i = 0; i < 4; i++)
            {
                int r = rnd.Next(Music.musicDB.Count);
                if (i != 0)
                {
                    for (int j = 0; j < currentlyVotingSongs.Count; j++)
                    {
                        //Verifica se não há músicas repetidas
                        while(String.Compare(currentlyVotingSongs[j].name, Music.musicDB[r].name) == 0)
                        {
                            r = rnd.Next(Music.musicDB.Count);
                        }
                        //Adiciona a música na lista de músicas a serem votadas
                        if (currentlyVotingSongs.Count < 4)
                        {
                            currentlyVotingSongs.Add(Music.musicDB.ElementAt(r));
                        }
                    }
                }else
                {
                    //A primeira música não precisa ser verificada!
                    currentlyVotingSongs.Add(Music.musicDB.ElementAt(r));
                }
            }
            Debug.WriteLine("Finished selecting songs for voting!");
        }

    }
}
