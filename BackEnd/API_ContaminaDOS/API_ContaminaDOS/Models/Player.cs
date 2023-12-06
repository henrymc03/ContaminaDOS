using System;
using System.Collections.Generic;

namespace API_ContaminaDOS.Models;

public partial class Player
{
    public string playerId { get; set; } = null!;

    public string gameId { get; set; } = null!;

    public string playerName { get; set; } = null!;

    public string? playerType { get; set; }

    public virtual ICollection<ActionRound> ActionRounds { get; set; } = new List<ActionRound>();

    public virtual ICollection<GroupRound> GroupRounds { get; set; } = new List<GroupRound>();

    public virtual ICollection<VoteRound> VoteRounds { get; set; } = new List<VoteRound>();

    public virtual Game game { get; set; } = null!;
}
