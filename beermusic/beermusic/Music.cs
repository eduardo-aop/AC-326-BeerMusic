using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Music
{
    public string name { get; set; }
    public string artist { get; set; }
    public string url { get; set; }

    public static List<Music> musicDB = new List<Music>();
    public static List<Music> someSelectedMusics = new List<Music>();
    public static Dictionary<string, int> musicsVoted = new Dictionary<string, int>();
    public static Dictionary<string, Music> musicsMap = new Dictionary<string, Music>();
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

    public static void parseMusicMap()
    {
        string line;
        //Cria o leitor de arquivos e lê os dados.
        System.IO.StreamReader fileReader = new System.IO.StreamReader("musicDb.txt");
        while ((line = fileReader.ReadLine()) != null)
        {
            Debug.WriteLine(line);
            var splitResult = line.Split('|');
            Music newMusic = new Music(splitResult[0], splitResult[1], splitResult[2]);
            string[] stringSeparators = new string[] { "track/" };
            var splitUrl = splitResult[2].Split(stringSeparators, StringSplitOptions.None);
            musicsMap.Add(splitUrl[1], newMusic);
        }
        hasDBPopulated = true;
        Debug.WriteLine("Music database parsing complete.");
    }

    public static void UpdateMusicList()
    {
        someSelectedMusics.Clear();
        Random r = new Random();
        int lastRandom = r.Next(0, musicDB.Count); //for ints

        for (int i = 0; i < 4; i++)
        {
            int j = 1;
            do
            {
                lastRandom = (lastRandom * (i + j) * 19) % musicDB.Count;
                j++;
            }
            while (checkForRepeatedMusics(musicDB[lastRandom], someSelectedMusics));
                    
            someSelectedMusics.Add(musicDB[lastRandom]);
        }
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

    public static string GetJsonMusicFromList()
    {
        var json = JsonConvert.SerializeObject(someSelectedMusics);
        return json;
    }

    public static void setMusicVoted(string url)
    {
        if (musicsVoted.ContainsKey(url))
        {
            musicsVoted.Add(url, musicsVoted[url]++);
        }else
        {
            musicsVoted.Add(url, 1);
        }
    }

    public static string getMusicVoted()
    {
        int max = 0;
        string finalUrl = "";

        foreach (string url in musicsVoted.Keys.ToArray())
        {
            int n = musicsVoted[url];
            if (max <= n)
            {
                max = n;
                finalUrl = url;
            }
        }
        return finalUrl;
    }
}
