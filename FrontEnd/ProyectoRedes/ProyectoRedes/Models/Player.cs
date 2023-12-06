using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace ProyectoRedes.Models
{
    public class Player
    {

        public List<PlayerCheck> otherPlayers { get; set; }

        public string player { get;set; }

        public string leader { get; set; }

        public List<bool> votes { get; set; }

        public List<string> group { get; set; }

        public string status { get; set; }

        public string result { get; set; }

        public string password { get; set; }

        public string gameId { get; set; }

        public int decade { get; set; }

        
        public string owner { get; set; }

        public string server { get; set; }

        public GameEnded gameEnded { get; set; }

        public List<Rounds> rounds { get; set; }

        public string gameName { get; set; }


    }
}
