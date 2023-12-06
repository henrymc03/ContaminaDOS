using System;
using System.Collections.Generic;

namespace API_ContaminaDOS.Models;

public partial class Round
{
    public string roundId { get; set; } = null!;

    public string? gameId { get; set; }

    public string leader { get; set; } = null!;

    public string? phase { get; set; }

    public string? roundStatus { get; set; }

    public string? result { get; set; }

    public virtual ICollection<ActionRound> ActionRounds { get; set; } = new List<ActionRound>();

    public virtual ICollection<GroupRound> GroupRounds { get; set; } = new List<GroupRound>();

    public virtual ICollection<VoteRound> VoteRounds { get; set; } = new List<VoteRound>();

    public virtual Game? game { get; set; }
}
