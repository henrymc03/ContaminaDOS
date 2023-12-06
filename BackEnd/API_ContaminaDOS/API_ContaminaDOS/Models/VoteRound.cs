using System;
using System.Collections.Generic;

namespace API_ContaminaDOS.Models;

public partial class VoteRound
{
    public int id { get; set; }

    public string? gameId { get; set; }

    public string? roundId { get; set; }

    public string? playerId { get; set; }

    public bool? vote { get; set; }

    public virtual Game? game { get; set; }

    public virtual Player? player { get; set; }

    public virtual Round? round { get; set; }
}
