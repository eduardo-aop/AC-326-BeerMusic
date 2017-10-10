using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

class Vote
{
    private static Vote instance;
    Dictionary<string, int> musicsMap = new Dictionary<string, int>();
    private Vote() { }

    public static Vote Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Vote();
            }
            return instance;
        }
    }

    public void computeVote(string url)
    {
        int count = musicsMap[url];
        musicsMap.Add(url, count+1);

        var sortedDict = from entry in musicsMap orderby entry.Value ascending select entry;
    }
}
