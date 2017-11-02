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
        int passes = 0; //Gambiarra

        public static MainWindow client;

        static List<Music> currentlyVotingSongs = new List<Music>();

        public MainWindow()
        {
            InitializeComponent();
            client = this;
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

            //Para acessar, entre em "localhost:8084" ou seuip:8084 na rede local.
            //Desative o firewall do windows! ACREDITE!

            bool successful;
            try
            {
                successful = spotifyController.Connect();
            }
            catch (Exception)
            {
                MessageBox.Show(@"Não foi possível abrir o Spotify! Verifique se o mesmo está instalado, e tente novamente.");
                throw;
            }

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
                currentlyVotingSongs = Music.chooseSongsForVoting();
                resetVotingLabels();

                if (!spotifyController.GetStatus().Playing)
                {
                    //Sempre toca The Final Countdown primeiro se spotify não estiver tocando nada
                    spotifyController.PlayURL(Music.musicDB[59].url);
                }

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
                resetVotingLabels();
                
            }));
        }

        private void SpotifyController_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                songProgress.Value = e.TrackTime;
                passes += 1;

                if ((e.TrackTime > songProgress.Maximum - 2) && (passes > 15))
                {
                    int maxId = 0;
                    int maxVot = 0;
                    //Pega a música mais votada
                    for (int i = 0; i < 4; i++)
                    {
                        if (currentlyVotingSongs[i].votos > maxVot)
                        {
                            maxId = i;
                            maxVot = currentlyVotingSongs[i].votos;
                        }
                        currentlyVotingSongs[i].votos = 0;
                    }
                    spotifyController.Pause();
                    Thread.Sleep(16);
                    Debug.WriteLine("");
                    Debug.WriteLine("Winning song: " + currentlyVotingSongs[maxId].name);
                    Debug.WriteLine("");
                    spotifyController.PlayURL(currentlyVotingSongs[maxId].url);
                    Thread.Sleep(64);
                    Music.chooseSongsForVoting();
                    passes = 0;
                }
            }));

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Para o servidor HTTP quando o programa fechar.
            try
            {
                barServer.Stop();
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao fechar servidor web!");
                throw;
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

        public void resetVotingLabels()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //São as piores linhas de código que ja vi.
                music1.Content = currentlyVotingSongs[0].name;
                music2.Content = currentlyVotingSongs[1].name;
                music3.Content = currentlyVotingSongs[2].name;
                music4.Content = currentlyVotingSongs[3].name;

                artist1.Content = currentlyVotingSongs[0].artist;
                artist2.Content = currentlyVotingSongs[1].artist;
                artist3.Content = currentlyVotingSongs[2].artist;
                artist4.Content = currentlyVotingSongs[3].artist;

                vote1.Content = currentlyVotingSongs[0].votos;
                vote2.Content = currentlyVotingSongs[1].votos;
                vote3.Content = currentlyVotingSongs[2].votos;
                vote4.Content = currentlyVotingSongs[3].votos;

                Debug.WriteLine("Finished updating UI labels!");
            }));
        }
    }
}
