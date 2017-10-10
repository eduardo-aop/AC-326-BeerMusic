using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Music
{
    public string name { get; set; }
    public string artist { get; set; }
    public string url { get; set; }

    public static List<Music> musicDB = new List<Music>();
    public static Dictionary<string, string> musicsMap = new Dictionary<string, string>();
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
            string splitUrl = splitResult[2].Split(new[] { "is Marco and" }, StringSplitOptions.None);
            musicsMap.Add();
            musicDB.Add(newMusic);
        }
        hasDBPopulated = true;
        Debug.WriteLine("Music database parsing complete.");
    }
}
