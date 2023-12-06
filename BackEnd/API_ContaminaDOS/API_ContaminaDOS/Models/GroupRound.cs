using System;
using System.Collections.Generic;

namespace API_ContaminaDOS.Models;

public partial class GroupRound
{
    public int id { get; set; }

    public string gameId { get; set; } = null!;

    public string roundId { get; set; } = null!;

    public string playerId { get; set; } = null!;

    public virtual Game game { get; set; } = null!;

    public virtual Player player { get; set; } = null!;

    public virtual Round round { get; set; } = null!;
}
