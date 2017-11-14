using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

class Music
{
    public string name { get; set; }
    public string artist { get; set; }
    public string url { get; set; }
    public int votos { get; set; }

    private static List<Music> currentlyVotingSongs = new List<Music>();
    private static Random rnd = new Random();

    public static List<Music> musicDB = new List<Music>();
    public static bool hasDBPopulated { get; set; } = false;

    Music(string songName, string artistName, string spotifyUrl)
    {
        this.name = songName;
        this.url = spotifyUrl;
        this.artist = artistName;
    }

    public static void parseMusicList()
    {
        string line;
        //Cria o leitor de arquivos e lê os dados.
        System.IO.StreamReader fileReader = new System.IO.StreamReader("musicDb.txt");
        while ((line = fileReader.ReadLine()) != null)
        {
            Debug.WriteLine(line);
            var splitResult = line.Split('|');
            Music newMusic = new Music(splitResult[0], splitResult[1], splitResult[2]);
            musicDB.Add(newMusic);
        }
        hasDBPopulated = true;
        Debug.WriteLine("Music database parsing complete.");
    }

    //return true case has some equal music and false case no one equal
    public static bool checkForRepeatedMusics(Music music, List<Music> musics)
    {
        for (int i = 0; i < musics.Count; i++)
        {
            if (musics[i].artist.Equals(music.artist) 
                || musics[i].name.Equals(music.name))
            {
                return true;
            }
        }
        return false;
    }

    public static string getJsonMusicFromList()
    {
        var json = JsonConvert.SerializeObject(currentlyVotingSongs);
        return json;
    }

    public static void setMusicVoted(string url)
    {
        foreach (Music music in currentlyVotingSongs)
        {
            if (music.url.Equals(url))
            {
                music.votos++;
                beermusic.MainWindow.client.resetVotingLabels();
            }
        }
    }

    public static List<Music> chooseSongsForVoting()
    {
        //Verifica se já possui a lista de músicas prontas
        if (!Music.hasDBPopulated)
        {
            //Inicializa o "banco de dados" de músicas
            Music.parseMusicList();
        }
        currentlyVotingSongs.Clear();
        for (int i = 0; i < 4; i++)
        {
            int r = rnd.Next(Music.musicDB.Count);
            if (i != 0)
            {
                for (int j = 0; j < currentlyVotingSongs.Count; j++)
                {
                    //Verifica se não há músicas repetidas
                    while (String.Compare(currentlyVotingSongs[j].name, Music.musicDB[r].name) == 0)
                    {
                        r = rnd.Next(Music.musicDB.Count);
                    }
                    //Adiciona a música na lista de músicas a serem votadas
                    if (currentlyVotingSongs.Count < 4)
                    {
                        currentlyVotingSongs.Add(Music.musicDB.ElementAt(r));
                    }
                }
            }
            else
            {
                //A primeira música não precisa ser verificada!
                currentlyVotingSongs.Add(Music.musicDB.ElementAt(r));
            }
        }
        Debug.WriteLine("Finished selecting songs for voting!");
        return currentlyVotingSongs;
    }
}
