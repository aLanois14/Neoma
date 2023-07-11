﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Neoma.Data;

namespace Neoma.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190911094749_Initialize")]
    partial class Initialize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Neoma.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("EstAficionado");

                    b.Property<bool>("EstCandidat");

                    b.Property<bool>("EstPorteur");

                    b.Property<DateTime>("LastAction");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Nom")
                        .IsRequired();

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<int>("OrganismeId");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<byte[]>("Photo");

                    b.Property<string>("Prenom")
                        .IsRequired();

                    b.Property<string>("RoleActuel");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<bool>("Valide");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganismeId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Neoma.Models.Besoins", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("BesoinId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ProjetId");

                    b.Property<int>("RoleId");

                    b.Property<string>("UtilisateurId");

                    b.HasKey("Id");

                    b.HasIndex("ProjetId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Besoins");
                });

            modelBuilder.Entity("Neoma.Models.BesoinsSpecialite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BesoinsId");

                    b.Property<int>("SpecialiteId");

                    b.HasKey("Id");

                    b.HasIndex("BesoinsId");

                    b.HasIndex("SpecialiteId");

                    b.ToTable("BesoinsSpecialite");
                });

            modelBuilder.Entity("Neoma.Models.Candidature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CandidatureId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BesoinsId");

                    b.Property<DateTime>("DateCreation");

                    b.Property<string>("Motif");

                    b.Property<int>("ProjetId");

                    b.Property<string>("Statut")
                        .IsRequired();

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BesoinsId");

                    b.HasIndex("ProjetId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Candidature");
                });

            modelBuilder.Entity("Neoma.Models.Conversation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ConversationId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreation");

                    b.Property<DateTime>("DateLastMessage");

                    b.HasKey("Id");

                    b.ToTable("Conversation");
                });

            modelBuilder.Entity("Neoma.Models.ConversationUtilisateur", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ConversationUtilisateurId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConversationId");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("ConversationUtilisateur");
                });

            modelBuilder.Entity("Neoma.Models.Membre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MembreId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Mail")
                        .IsRequired();

                    b.Property<string>("Nom")
                        .IsRequired();

                    b.Property<string>("Prenom")
                        .IsRequired();

                    b.Property<int>("ProjetId");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("ProjetId");

                    b.HasIndex("RoleId");

                    b.ToTable("Membre");
                });

            modelBuilder.Entity("Neoma.Models.MembreSpecialite", b =>
                {
                    b.Property<int>("MembreSpecialiteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MembreId");

                    b.Property<int>("SpecialiteId");

                    b.HasKey("MembreSpecialiteId");

                    b.HasIndex("MembreId");

                    b.HasIndex("SpecialiteId");

                    b.ToTable("MembreSpecialite");
                });

            modelBuilder.Entity("Neoma.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MessageId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConversationId");

                    b.Property<DateTime>("DateEnvoi");

                    b.Property<string>("FileName");

                    b.Property<string>("Files");

                    b.Property<bool>("MessageLu");

                    b.Property<string>("Text");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Neoma.Models.Organisme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OrganismeId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adresse");

                    b.Property<string>("CodePostal");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("LibelleOrganisme");

                    b.Property<bool>("Valide");

                    b.Property<string>("Ville");

                    b.HasKey("Id");

                    b.ToTable("Organisme");
                });

            modelBuilder.Entity("Neoma.Models.Projet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjetId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Actif");

                    b.Property<string>("CommentaireProjet");

                    b.Property<bool>("Complet");

                    b.Property<DateTime>("DateCreation");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NomProjet");

                    b.Property<string>("PresentationProjet")
                        .IsRequired();

                    b.Property<DateTime>("ProjectEnd");

                    b.Property<DateTime>("ProjectStart");

                    b.Property<int>("RolesNonPourvus");

                    b.Property<int>("RolesPourvus");

                    b.Property<int>("RolesProposes");

                    b.Property<bool>("Termine");

                    b.Property<int>("TypeProjetId");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("TypeProjetId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Projet");
                });

            modelBuilder.Entity("Neoma.Models.Proposition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PropositionId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BesoinsId");

                    b.Property<DateTime>("DateCreation");

                    b.Property<string>("Motif");

                    b.Property<int>("ProjetId");

                    b.Property<string>("Statut")
                        .IsRequired();

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BesoinsId");

                    b.HasIndex("ProjetId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Proposition");
                });

            modelBuilder.Entity("Neoma.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("LibelleRole");

                    b.Property<bool>("Valide");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Neoma.Models.RoleUtilisateur", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleUtilisateurId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoleId");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("RoleUtilisateur");
                });

            modelBuilder.Entity("Neoma.Models.Selection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SelectionId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BesoinsId");

                    b.Property<string>("Commentaire");

                    b.Property<DateTime>("DateCreation");

                    b.Property<decimal>("Note")
                        .HasColumnType("DECIMAL(2,1)");

                    b.Property<int>("ProjetId");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BesoinsId");

                    b.HasIndex("ProjetId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Selection");
                });

            modelBuilder.Entity("Neoma.Models.Specialite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SpecialiteId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("LibelleSpecialite");

                    b.Property<int>("RoleId");

                    b.Property<string>("UtilisateurId");

                    b.Property<bool>("Valide");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("Specialite");
                });

            modelBuilder.Entity("Neoma.Models.SpecialiteUtilisateur", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SpecialiteUtilisateurId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("SpecialiteId");

                    b.Property<string>("UtilisateurId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("SpecialiteId");

                    b.HasIndex("UtilisateurId");

                    b.ToTable("SpecialiteUtilisateur");
                });

            modelBuilder.Entity("Neoma.Models.TypeProjet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TypeProjetId")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("LibelleTypeProjet");

                    b.Property<bool>("Valide");

                    b.HasKey("Id");

                    b.ToTable("TypeProjet");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Neoma.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Neoma.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Neoma.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.ApplicationUser", b =>
                {
                    b.HasOne("Neoma.Models.Organisme", "Organisme")
                        .WithMany()
                        .HasForeignKey("OrganismeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Besoins", b =>
                {
                    b.HasOne("Neoma.Models.Projet", "Projet")
                        .WithMany()
                        .HasForeignKey("ProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId");
                });

            modelBuilder.Entity("Neoma.Models.BesoinsSpecialite", b =>
                {
                    b.HasOne("Neoma.Models.Besoins", "Besoins")
                        .WithMany()
                        .HasForeignKey("BesoinsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Specialite", "Specialite")
                        .WithMany()
                        .HasForeignKey("SpecialiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Candidature", b =>
                {
                    b.HasOne("Neoma.Models.Besoins", "Besoins")
                        .WithMany()
                        .HasForeignKey("BesoinsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Projet", "Projet")
                        .WithMany()
                        .HasForeignKey("ProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.ConversationUtilisateur", b =>
                {
                    b.HasOne("Neoma.Models.Conversation", "Conversation")
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Membre", b =>
                {
                    b.HasOne("Neoma.Models.Projet", "Projet")
                        .WithMany()
                        .HasForeignKey("ProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.MembreSpecialite", b =>
                {
                    b.HasOne("Neoma.Models.Membre", "Membre")
                        .WithMany()
                        .HasForeignKey("MembreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Specialite", "Specialite")
                        .WithMany()
                        .HasForeignKey("SpecialiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Message", b =>
                {
                    b.HasOne("Neoma.Models.Conversation", "Conversation")
                        .WithMany()
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Projet", b =>
                {
                    b.HasOne("Neoma.Models.TypeProjet", "TypeProjet")
                        .WithMany()
                        .HasForeignKey("TypeProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Proposition", b =>
                {
                    b.HasOne("Neoma.Models.Besoins", "Besoins")
                        .WithMany()
                        .HasForeignKey("BesoinsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Projet", "Projet")
                        .WithMany()
                        .HasForeignKey("ProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.RoleUtilisateur", b =>
                {
                    b.HasOne("Neoma.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Selection", b =>
                {
                    b.HasOne("Neoma.Models.Besoins", "Besoins")
                        .WithMany()
                        .HasForeignKey("BesoinsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.Projet", "Projet")
                        .WithMany()
                        .HasForeignKey("ProjetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Neoma.Models.Specialite", b =>
                {
                    b.HasOne("Neoma.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId");
                });

            modelBuilder.Entity("Neoma.Models.SpecialiteUtilisateur", b =>
                {
                    b.HasOne("Neoma.Models.Specialite", "Specialite")
                        .WithMany()
                        .HasForeignKey("SpecialiteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Neoma.Models.ApplicationUser", "Utilisateur")
                        .WithMany()
                        .HasForeignKey("UtilisateurId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
