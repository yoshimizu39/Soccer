﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Soccer.Web.Data;

namespace Soccer.Web.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Soccer.Web.Data.Entities.GroupDetailEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GoalsAgaints");

                    b.Property<int>("GoalsFor");

                    b.Property<int?>("GroupId");

                    b.Property<int>("MatchesLost");

                    b.Property<int>("MatchesPlayed");

                    b.Property<int>("MatchesTied");

                    b.Property<int>("MatchesWon");

                    b.Property<int?>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("TeamId");

                    b.ToTable("GroupDetails");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.GroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("TournamentId");

                    b.HasKey("Id");

                    b.HasIndex("TournamentId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.MatchEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<int?>("GoalsLocal");

                    b.Property<int?>("GoalsVisitor");

                    b.Property<int?>("GroupId");

                    b.Property<bool>("IsClosed");

                    b.Property<int?>("LocalId");

                    b.Property<int?>("VisitorId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("LocalId");

                    b.HasIndex("VisitorId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.TeamEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LogoPath");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.TournamentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LogoPath");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.GroupDetailEntity", b =>
                {
                    b.HasOne("Soccer.Web.Data.Entities.GroupEntity", "Group")
                        .WithMany("GroupDetails")
                        .HasForeignKey("GroupId");

                    b.HasOne("Soccer.Web.Data.Entities.TeamEntity", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.GroupEntity", b =>
                {
                    b.HasOne("Soccer.Web.Data.Entities.TournamentEntity", "Tournament")
                        .WithMany("Groups")
                        .HasForeignKey("TournamentId");
                });

            modelBuilder.Entity("Soccer.Web.Data.Entities.MatchEntity", b =>
                {
                    b.HasOne("Soccer.Web.Data.Entities.GroupEntity", "Group")
                        .WithMany("Matches")
                        .HasForeignKey("GroupId");

                    b.HasOne("Soccer.Web.Data.Entities.TeamEntity", "Local")
                        .WithMany()
                        .HasForeignKey("LocalId");

                    b.HasOne("Soccer.Web.Data.Entities.TeamEntity", "Visitor")
                        .WithMany()
                        .HasForeignKey("VisitorId");
                });
#pragma warning restore 612, 618
        }
    }
}
