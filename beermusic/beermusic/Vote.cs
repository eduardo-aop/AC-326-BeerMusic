using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

class Vote
{
    private static Vote instance;

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

    public void computeVote(HttpListenerContext context)
    {

    }
}
