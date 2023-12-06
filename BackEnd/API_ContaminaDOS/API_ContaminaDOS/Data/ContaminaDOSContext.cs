using System;
using System.Collections.Generic;
using API_ContaminaDOS.Models;
using Microsoft.EntityFrameworkCore;

namespace API_ContaminaDOS.Data;

public partial class ContaminaDOSContext : DbContext
{
    public ContaminaDOSContext()
    {
    }

    public ContaminaDOSContext(DbContextOptions<ContaminaDOSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionRound> ActionRounds { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GroupRound> GroupRounds { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<VoteRound> VoteRounds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=163.178.173.130;Database=GrupoBRedes2;user id=basesdedatos;password=rpbases.2022;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionRound>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ActionRo__3213E83F51A85D43");

            entity.ToTable("ActionRound");

            entity.Property(e => e.actionRound).HasColumnName("actionRound");
            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.roundId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.game).WithMany(p => p.ActionRounds)
                .HasForeignKey(d => d.gameId)
                .HasConstraintName("FK__ActionRou__gameI__534D60F1");

            entity.HasOne(d => d.player).WithMany(p => p.ActionRounds)
                .HasForeignKey(d => d.playerId)
                .HasConstraintName("FK__ActionRou__playe__5535A963");

            entity.HasOne(d => d.round).WithMany(p => p.ActionRounds)
                .HasForeignKey(d => d.roundId)
                .HasConstraintName("FK__ActionRou__round__5441852A");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.gameId).HasName("PK__Game__DA90B452198965AA");

            entity.ToTable("Game");

            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.createdAt)
                .HasMaxLength(24)
                .IsUnicode(false);
            entity.Property(e => e.currentRound)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.gameName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.gameOwner)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.gamePassword)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.gameStatus)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.updatedAt)
                .HasMaxLength(24)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GroupRound>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__GroupRou__3213E83FCB2DB6EE");

            entity.ToTable("GroupRound");

            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.roundId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.game).WithMany(p => p.GroupRounds)
                .HasForeignKey(d => d.gameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupRoun__gameI__4E88ABD4");

            entity.HasOne(d => d.player).WithMany(p => p.GroupRounds)
                .HasForeignKey(d => d.playerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupRoun__playe__4F7CD00D");

            entity.HasOne(d => d.round).WithMany(p => p.GroupRounds)
                .HasForeignKey(d => d.roundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupRoun__round__5070F446");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.playerId).HasName("PK__Players__2CDA01F10734DDFC");

            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.playerName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.playerType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.game).WithMany(p => p.Players)
                .HasForeignKey(d => d.gameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Players__gameId__3F466844");
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.roundId).HasName("PK__Rounds__EA0947154B9C49FA");

            entity.Property(e => e.roundId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.leader)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.phase)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.result)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.roundStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.game).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.gameId)
                .HasConstraintName("FK__Rounds__gameId__440B1D61");
        });

        modelBuilder.Entity<VoteRound>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__VoteRoun__3213E83F84D4E0F7");

            entity.ToTable("VoteRound");

            entity.Property(e => e.gameId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.roundId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.game).WithMany(p => p.VoteRounds)
                .HasForeignKey(d => d.gameId)
                .HasConstraintName("FK__VoteRound__gameI__49C3F6B7");

            entity.HasOne(d => d.player).WithMany(p => p.VoteRounds)
                .HasForeignKey(d => d.playerId)
                .HasConstraintName("FK__VoteRound__playe__4BAC3F29");

            entity.HasOne(d => d.round).WithMany(p => p.VoteRounds)
                .HasForeignKey(d => d.roundId)
                .HasConstraintName("FK__VoteRound__round__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
