using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Neoma.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    ConversationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    DateLastMessage = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.ConversationId);
                });

            migrationBuilder.CreateTable(
                name: "Organisme",
                columns: table => new
                {
                    OrganismeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LibelleOrganisme = table.Column<string>(nullable: false),
                    Ville = table.Column<string>(nullable: true),
                    Adresse = table.Column<string>(nullable: true),
                    CodePostal = table.Column<string>(nullable: true),
                    Valide = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisme", x => x.OrganismeId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LibelleRole = table.Column<string>(nullable: false),
                    Valide = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TypeProjet",
                columns: table => new
                {
                    TypeProjetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LibelleTypeProjet = table.Column<string>(nullable: false),
                    Valide = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeProjet", x => x.TypeProjetId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Prenom = table.Column<string>(nullable: false),
                    Nom = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Photo = table.Column<byte[]>(nullable: true),
                    EstPorteur = table.Column<bool>(nullable: false),
                    EstCandidat = table.Column<bool>(nullable: false),
                    EstAficionado = table.Column<bool>(nullable: false),
                    Valide = table.Column<bool>(nullable: false),
                    RoleActuel = table.Column<string>(nullable: true),
                    LastAction = table.Column<DateTime>(nullable: false),
                    OrganismeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organisme_OrganismeId",
                        column: x => x.OrganismeId,
                        principalTable: "Organisme",
                        principalColumn: "OrganismeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationUtilisateur",
                columns: table => new
                {
                    ConversationUtilisateurId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UtilisateurId = table.Column<string>(nullable: false),
                    ConversationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationUtilisateur", x => x.ConversationUtilisateurId);
                    table.ForeignKey(
                        name: "FK_ConversationUtilisateur_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationUtilisateur_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    DateEnvoi = table.Column<DateTime>(nullable: false),
                    MessageLu = table.Column<bool>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true),
                    UtilisateurId = table.Column<string>(nullable: false),
                    ConversationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Message_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projet",
                columns: table => new
                {
                    ProjetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NomProjet = table.Column<string>(nullable: false),
                    PresentationProjet = table.Column<string>(nullable: false),
                    CommentaireProjet = table.Column<string>(nullable: true),
                    Termine = table.Column<bool>(nullable: false),
                    Complet = table.Column<bool>(nullable: false),
                    Actif = table.Column<bool>(nullable: false),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    ProjectStart = table.Column<DateTime>(nullable: false),
                    ProjectEnd = table.Column<DateTime>(nullable: false),
                    RolesProposes = table.Column<int>(nullable: false),
                    RolesPourvus = table.Column<int>(nullable: false),
                    RolesNonPourvus = table.Column<int>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: false),
                    TypeProjetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projet", x => x.ProjetId);
                    table.ForeignKey(
                        name: "FK_Projet_TypeProjet_TypeProjetId",
                        column: x => x.TypeProjetId,
                        principalTable: "TypeProjet",
                        principalColumn: "TypeProjetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projet_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUtilisateur",
                columns: table => new
                {
                    RoleUtilisateurId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UtilisateurId = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUtilisateur", x => x.RoleUtilisateurId);
                    table.ForeignKey(
                        name: "FK_RoleUtilisateur_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUtilisateur_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specialite",
                columns: table => new
                {
                    SpecialiteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LibelleSpecialite = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    Valide = table.Column<bool>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialite", x => x.SpecialiteId);
                    table.ForeignKey(
                        name: "FK_Specialite_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Specialite_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Besoins",
                columns: table => new
                {
                    BesoinId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjetId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Besoins", x => x.BesoinId);
                    table.ForeignKey(
                        name: "FK_Besoins_Projet_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projet",
                        principalColumn: "ProjetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Besoins_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Besoins_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Membre",
                columns: table => new
                {
                    MembreId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(nullable: false),
                    Prenom = table.Column<string>(nullable: false),
                    Mail = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    ProjetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membre", x => x.MembreId);
                    table.ForeignKey(
                        name: "FK_Membre_Projet_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projet",
                        principalColumn: "ProjetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membre_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialiteUtilisateur",
                columns: table => new
                {
                    SpecialiteUtilisateurId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UtilisateurId = table.Column<string>(nullable: false),
                    SpecialiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialiteUtilisateur", x => x.SpecialiteUtilisateurId);
                    table.ForeignKey(
                        name: "FK_SpecialiteUtilisateur_Specialite_SpecialiteId",
                        column: x => x.SpecialiteId,
                        principalTable: "Specialite",
                        principalColumn: "SpecialiteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialiteUtilisateur_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BesoinsSpecialite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BesoinsId = table.Column<int>(nullable: false),
                    SpecialiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BesoinsSpecialite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BesoinsSpecialite_Besoins_BesoinsId",
                        column: x => x.BesoinsId,
                        principalTable: "Besoins",
                        principalColumn: "BesoinId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BesoinsSpecialite_Specialite_SpecialiteId",
                        column: x => x.SpecialiteId,
                        principalTable: "Specialite",
                        principalColumn: "SpecialiteId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Candidature",
                columns: table => new
                {
                    CandidatureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Statut = table.Column<string>(nullable: false),
                    Motif = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: false),
                    BesoinsId = table.Column<int>(nullable: false),
                    ProjetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidature", x => x.CandidatureId);
                    table.ForeignKey(
                        name: "FK_Candidature_Besoins_BesoinsId",
                        column: x => x.BesoinsId,
                        principalTable: "Besoins",
                        principalColumn: "BesoinId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidature_Projet_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projet",
                        principalColumn: "ProjetId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Candidature_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Proposition",
                columns: table => new
                {
                    PropositionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Statut = table.Column<string>(nullable: false),
                    Motif = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: false),
                    BesoinsId = table.Column<int>(nullable: false),
                    ProjetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposition", x => x.PropositionId);
                    table.ForeignKey(
                        name: "FK_Proposition_Besoins_BesoinsId",
                        column: x => x.BesoinsId,
                        principalTable: "Besoins",
                        principalColumn: "BesoinId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Proposition_Projet_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projet",
                        principalColumn: "ProjetId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Proposition_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Selection",
                columns: table => new
                {
                    SelectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjetId = table.Column<int>(nullable: false),
                    UtilisateurId = table.Column<string>(nullable: false),
                    BesoinsId = table.Column<int>(nullable: false),
                    Commentaire = table.Column<string>(nullable: true),
                    Note = table.Column<decimal>(type: "DECIMAL(2,1)", nullable: false),
                    DateCreation = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selection", x => x.SelectionId);
                    table.ForeignKey(
                        name: "FK_Selection_Besoins_BesoinsId",
                        column: x => x.BesoinsId,
                        principalTable: "Besoins",
                        principalColumn: "BesoinId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Selection_Projet_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projet",
                        principalColumn: "ProjetId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Selection_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "MembreSpecialite",
                columns: table => new
                {
                    MembreSpecialiteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MembreId = table.Column<int>(nullable: false),
                    SpecialiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembreSpecialite", x => x.MembreSpecialiteId);
                    table.ForeignKey(
                        name: "FK_MembreSpecialite_Membre_MembreId",
                        column: x => x.MembreId,
                        principalTable: "Membre",
                        principalColumn: "MembreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembreSpecialite_Specialite_SpecialiteId",
                        column: x => x.SpecialiteId,
                        principalTable: "Specialite",
                        principalColumn: "SpecialiteId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganismeId",
                table: "AspNetUsers",
                column: "OrganismeId");

            migrationBuilder.CreateIndex(
                name: "IX_Besoins_ProjetId",
                table: "Besoins",
                column: "ProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Besoins_RoleId",
                table: "Besoins",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Besoins_UtilisateurId",
                table: "Besoins",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_BesoinsSpecialite_BesoinsId",
                table: "BesoinsSpecialite",
                column: "BesoinsId");

            migrationBuilder.CreateIndex(
                name: "IX_BesoinsSpecialite_SpecialiteId",
                table: "BesoinsSpecialite",
                column: "SpecialiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidature_BesoinsId",
                table: "Candidature",
                column: "BesoinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidature_ProjetId",
                table: "Candidature",
                column: "ProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidature_UtilisateurId",
                table: "Candidature",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUtilisateur_ConversationId",
                table: "ConversationUtilisateur",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUtilisateur_UtilisateurId",
                table: "ConversationUtilisateur",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Membre_ProjetId",
                table: "Membre",
                column: "ProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Membre_RoleId",
                table: "Membre",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MembreSpecialite_MembreId",
                table: "MembreSpecialite",
                column: "MembreId");

            migrationBuilder.CreateIndex(
                name: "IX_MembreSpecialite_SpecialiteId",
                table: "MembreSpecialite",
                column: "SpecialiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId",
                table: "Message",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_UtilisateurId",
                table: "Message",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Projet_TypeProjetId",
                table: "Projet",
                column: "TypeProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Projet_UtilisateurId",
                table: "Projet",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposition_BesoinsId",
                table: "Proposition",
                column: "BesoinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposition_ProjetId",
                table: "Proposition",
                column: "ProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposition_UtilisateurId",
                table: "Proposition",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUtilisateur_RoleId",
                table: "RoleUtilisateur",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUtilisateur_UtilisateurId",
                table: "RoleUtilisateur",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Selection_BesoinsId",
                table: "Selection",
                column: "BesoinsId");

            migrationBuilder.CreateIndex(
                name: "IX_Selection_ProjetId",
                table: "Selection",
                column: "ProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Selection_UtilisateurId",
                table: "Selection",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialite_RoleId",
                table: "Specialite",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialite_UtilisateurId",
                table: "Specialite",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialiteUtilisateur_SpecialiteId",
                table: "SpecialiteUtilisateur",
                column: "SpecialiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialiteUtilisateur_UtilisateurId",
                table: "SpecialiteUtilisateur",
                column: "UtilisateurId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BesoinsSpecialite");

            migrationBuilder.DropTable(
                name: "Candidature");

            migrationBuilder.DropTable(
                name: "ConversationUtilisateur");

            migrationBuilder.DropTable(
                name: "MembreSpecialite");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Proposition");

            migrationBuilder.DropTable(
                name: "RoleUtilisateur");

            migrationBuilder.DropTable(
                name: "Selection");

            migrationBuilder.DropTable(
                name: "SpecialiteUtilisateur");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Membre");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "Besoins");

            migrationBuilder.DropTable(
                name: "Specialite");

            migrationBuilder.DropTable(
                name: "Projet");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "TypeProjet");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Organisme");
        }
    }
}
