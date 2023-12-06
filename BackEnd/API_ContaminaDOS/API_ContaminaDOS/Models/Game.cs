using System;
using System.Collections.Generic;

namespace API_ContaminaDOS.Models;

public partial class Game
{
    public string gameId { get; set; } = null!;

    public string gameName { get; set; } = null!;

    public string gameOwner { get; set; } = null!;

    public string? gameStatus { get; set; }

    public string? gamePassword { get; set; }

    public string currentRound { get; set; } = null!;

    public string createdAt { get; set; } = null!;

    public string updatedAt { get; set; } = null!;

    public virtual ICollection<ActionRound> ActionRounds { get; set; } = new List<ActionRound>();

    public virtual ICollection<GroupRound> GroupRounds { get; set; } = new List<GroupRound>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();

    public virtual ICollection<VoteRound> VoteRounds { get; set; } = new List<VoteRound>();
}
