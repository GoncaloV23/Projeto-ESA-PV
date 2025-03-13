using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EasyFitHub.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Biometrics",
                columns: table => new
                {
                    BiometricsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weigth = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    WaterPercentage = table.Column<double>(type: "float", nullable: false),
                    FatMass = table.Column<double>(type: "float", nullable: false),
                    LeanMass = table.Column<double>(type: "float", nullable: false),
                    BodyMassIndex = table.Column<double>(type: "float", nullable: false),
                    MetabolicAge = table.Column<int>(type: "int", nullable: false),
                    VisceralFat = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biometrics", x => x.BiometricsId);
                });

            migrationBuilder.CreateTable(
                name: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gym", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformStats",
                columns: table => new
                {
                    PlatFormStatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GymCount = table.Column<int>(type: "int", nullable: false),
                    UserCount = table.Column<int>(type: "int", nullable: false),
                    AvgAge = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformStats", x => x.PlatFormStatsId);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RecoverCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    BankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GymId = table.Column<int>(type: "int", nullable: true),
                    GymSubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GymSubscriptionPrice = table.Column<double>(type: "float", nullable: false),
                    StripeBankId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripePlanId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.BankAccountId);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GymStats",
                columns: table => new
                {
                    GymStatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientCount = table.Column<int>(type: "int", nullable: false),
                    PTCount = table.Column<int>(type: "int", nullable: false),
                    NutricionistCount = table.Column<int>(type: "int", nullable: false),
                    SecretaryCount = table.Column<int>(type: "int", nullable: false),
                    ShopItemCount = table.Column<int>(type: "int", nullable: false),
                    MachineCount = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymStats", x => x.GymStatsId);
                    table.ForeignKey(
                        name: "FK_GymStats_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    HubImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.HubImageId);
                    table.ForeignKey(
                        name: "FK_Images_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientData",
                columns: table => new
                {
                    ClientDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageHubImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientData", x => x.ClientDataId);
                    table.ForeignKey(
                        name: "FK_ClientData_Images_ImageHubImageId",
                        column: x => x.ImageHubImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                });

            migrationBuilder.CreateTable(
                name: "GymMachines",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymMachines", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_GymMachines_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymMachines_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    ClientDataId = table.Column<int>(type: "int", nullable: false),
                    BiometricsId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_Account_UserId",
                        column: x => x.UserId,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_Biometrics_BiometricsId",
                        column: x => x.BiometricsId,
                        principalTable: "Biometrics",
                        principalColumn: "BiometricsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_ClientData_ClientDataId",
                        column: x => x.ClientDataId,
                        principalTable: "ClientData",
                        principalColumn: "ClientDataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    MachineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_Exercises_GymMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "GymMachines",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exercises_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                });

            migrationBuilder.CreateTable(
                name: "ClientStats",
                columns: table => new
                {
                    ClientStatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weigth = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    FatMass = table.Column<double>(type: "float", nullable: false),
                    LeanMass = table.Column<double>(type: "float", nullable: false),
                    BodyMassIndex = table.Column<double>(type: "float", nullable: false),
                    VisceralFat = table.Column<double>(type: "float", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStats", x => x.ClientStatsId);
                    table.ForeignKey(
                        name: "FK_ClientStats_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DebitCard",
                columns: table => new
                {
                    DebitCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    StripeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripePaymentMethodId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitCard", x => x.DebitCardId);
                    table.ForeignKey(
                        name: "FK_DebitCard_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeStats",
                columns: table => new
                {
                    EmployeeStatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientCount = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeStats", x => x.EmployeeStatsId);
                    table.ForeignKey(
                        name: "FK_EmployeeStats_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GymClients",
                columns: table => new
                {
                    GymClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymClients", x => x.GymClientId);
                    table.ForeignKey(
                        name: "FK_GymClients_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymClients_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GymEmployees",
                columns: table => new
                {
                    GymEmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymEmployees", x => x.GymEmployeeId);
                    table.ForeignKey(
                        name: "FK_GymEmployees_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymEmployees_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GymRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    GymId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_GymRequests_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GymRequests_Gym_GymId",
                        column: x => x.GymId,
                        principalTable: "Gym",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanType = table.Column<int>(type: "int", nullable: false),
                    HubImageId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.PlanId);
                    table.ForeignKey(
                        name: "FK_Plans_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plans_Images_HubImageId",
                        column: x => x.HubImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                });

            migrationBuilder.CreateTable(
                name: "Buyables",
                columns: table => new
                {
                    BuyableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyableType = table.Column<int>(type: "int", nullable: false),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    DebitCardId = table.Column<int>(type: "int", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buyables", x => x.BuyableId);
                    table.ForeignKey(
                        name: "FK_Buyables_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId");
                    table.ForeignKey(
                        name: "FK_Buyables_DebitCard_DebitCardId",
                        column: x => x.DebitCardId,
                        principalTable: "DebitCard",
                        principalColumn: "DebitCardId");
                });

            migrationBuilder.CreateTable(
                name: "StatisticEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeStatsId = table.Column<int>(type: "int", nullable: true),
                    EmployeeStatsId1 = table.Column<int>(type: "int", nullable: true),
                    GymStatsId = table.Column<int>(type: "int", nullable: true),
                    GymStatsId1 = table.Column<int>(type: "int", nullable: true),
                    PlatFormStatsId = table.Column<int>(type: "int", nullable: true),
                    PlatFormStatsId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticEntity_EmployeeStats_EmployeeStatsId",
                        column: x => x.EmployeeStatsId,
                        principalTable: "EmployeeStats",
                        principalColumn: "EmployeeStatsId");
                    table.ForeignKey(
                        name: "FK_StatisticEntity_EmployeeStats_EmployeeStatsId1",
                        column: x => x.EmployeeStatsId1,
                        principalTable: "EmployeeStats",
                        principalColumn: "EmployeeStatsId");
                    table.ForeignKey(
                        name: "FK_StatisticEntity_GymStats_GymStatsId",
                        column: x => x.GymStatsId,
                        principalTable: "GymStats",
                        principalColumn: "GymStatsId");
                    table.ForeignKey(
                        name: "FK_StatisticEntity_GymStats_GymStatsId1",
                        column: x => x.GymStatsId1,
                        principalTable: "GymStats",
                        principalColumn: "GymStatsId");
                    table.ForeignKey(
                        name: "FK_StatisticEntity_PlatformStats_PlatFormStatsId",
                        column: x => x.PlatFormStatsId,
                        principalTable: "PlatformStats",
                        principalColumn: "PlatFormStatsId");
                    table.ForeignKey(
                        name: "FK_StatisticEntity_PlatformStats_PlatFormStatsId1",
                        column: x => x.PlatFormStatsId1,
                        principalTable: "PlatformStats",
                        principalColumn: "PlatFormStatsId");
                });

            migrationBuilder.CreateTable(
                name: "GymRelations",
                columns: table => new
                {
                    GymRelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GymClientId = table.Column<int>(type: "int", nullable: false),
                    GymEmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymRelations", x => x.GymRelationId);
                    table.ForeignKey(
                        name: "FK_GymRelations_GymClients_GymClientId",
                        column: x => x.GymClientId,
                        principalTable: "GymClients",
                        principalColumn: "GymClientId");
                    table.ForeignKey(
                        name: "FK_GymRelations_GymEmployees_GymEmployeeId",
                        column: x => x.GymEmployeeId,
                        principalTable: "GymEmployees",
                        principalColumn: "GymEmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "PlanItems",
                columns: table => new
                {
                    PlanItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: true),
                    HubImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanItems", x => x.PlanItemId);
                    table.ForeignKey(
                        name: "FK_PlanItems_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanItems_Images_HubImageId",
                        column: x => x.HubImageId,
                        principalTable: "Images",
                        principalColumn: "HubImageId");
                    table.ForeignKey(
                        name: "FK_PlanItems_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "PlanId");
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItem_Buyables_CartId",
                        column: x => x.CartId,
                        principalTable: "Buyables",
                        principalColumn: "BuyableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItem_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    PaymentDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BuyableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.PaymentDetailsId);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Buyables_BuyableId",
                        column: x => x.BuyableId,
                        principalTable: "Buyables",
                        principalColumn: "BuyableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountId", "AccountType", "Email", "Password", "PhoneNumber", "RecoverCode", "Token", "UserName" },
                values: new object[,]
                {
                    { 1, 1, "202100296@estudantes.ips.pt", "8e945bf66837dcea295857efe792db4eaaa951e0b5edaca97c757ad41a9ad1d8", null, null, null, "GVieira" },
                    { 2, 1, "202100299@estudantes.ips.pt", "8e945bf66837dcea295857efe792db4eaaa951e0b5edaca97c757ad41a9ad1d8", null, null, null, "RBarroso" },
                    { 3, 1, "202100984@estudantes.ips.pt", "8e945bf66837dcea295857efe792db4eaaa951e0b5edaca97c757ad41a9ad1d8", null, null, null, "FSilva" },
                    { 4, 1, "201901953@estudantes.ips.pt", "8e945bf66837dcea295857efe792db4eaaa951e0b5edaca97c757ad41a9ad1d8", null, null, null, "APauli" }
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountId", "AccountType", "BirthDate", "Email", "Name", "Password", "PhoneNumber", "RecoverCode", "Surname", "Token", "UserName" },
                values: new object[,]
                {
                    { 7, 3, new DateOnly(2002, 1, 6), "test1@gmail.com", "Sofia", "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014", null, null, "Almeida", null, "test1" },
                    { 8, 3, new DateOnly(2002, 1, 6), "test2@gmail.com", "Tiago", "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752", null, null, "Martins", null, "test2" },
                    { 9, 3, new DateOnly(2002, 1, 6), "test3@gmail.com", "Joana", "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13", null, null, "Silva", null, "test3" },
                    { 10, 3, new DateOnly(2002, 1, 6), "test4@gmail.com", "Pedro", "a4e624d686e03ed2767c0abd85c14426b0b1157d2ce81d27bb4fe4f6f01d688a", null, null, "Santos", null, "test4" },
                    { 11, 3, new DateOnly(2002, 1, 6), "test5@gmail.com", "Mariana", "a140c0c1eda2def2b830363ba362aa4d7d255c262960544821f556e16661b6ff", null, null, "Costa", null, "test5" },
                    { 12, 3, new DateOnly(2002, 1, 6), "test6@gmail.com", "Miguel", "ed0cb90bdfa4f93981a7d03cff99213a86aa96a6cbcf89ec5e8889871f088727", null, null, "Ferreira", null, "test6" },
                    { 13, 3, new DateOnly(2002, 1, 6), "test7@gmail.com", "Carolina", "bd7c911264aae15b66d4291b6850829aa96986b1d3ead34d1fdbfef27056c112", null, null, "Sousa", null, "test7" },
                    { 14, 3, new DateOnly(2002, 1, 6), "test8@gmail.com", "Ricardo", "1f9bfeb15fee8a10c4d0711c7eb0c083962123e1918e461b6a508e7146c189b2", null, null, "Carvalho", null, "test8" },
                    { 15, 3, new DateOnly(2002, 1, 6), "test9@gmail.com", "Ana", "b4451034d3b6590060ce9484a28b88dd332a80a22ae8e39c9c5cb7357ab26c9f", null, null, "Rodrigues", null, "test9" },
                    { 16, 3, new DateOnly(2002, 1, 6), "test10@gmail.com", "Bruno", "ec2738feb2bbb0bc783eb4667903391416372ba6ed8b8dddbebbdb37e5102473", null, null, "Gonçalves", null, "test10" },
                    { 17, 3, new DateOnly(2002, 1, 6), "test11@gmail.com", "Beatriz", "744ea9ec6fa0a83e9764b4e323d5be6b55a5accfc7fe4c08eab6a8de1fca4855", null, null, "Lima", null, "test11" },
                    { 18, 3, new DateOnly(2002, 1, 6), "test12@gmail.com", "Diogo", "a98ec5c5044800c88e862f007b98d89815fc40ca155d6ce7909530d792e909ce", null, null, "Ribeiro", null, "test12" }
                });

            migrationBuilder.InsertData(
                table: "Biometrics",
                columns: new[] { "BiometricsId", "BodyMassIndex", "FatMass", "Height", "LeanMass", "MetabolicAge", "VisceralFat", "WaterPercentage", "Weigth" },
                values: new object[,]
                {
                    { 1, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 2, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 3, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 4, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 5, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 6, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 7, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 8, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 9, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 10, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 11, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 },
                    { 12, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0.0 }
                });

            migrationBuilder.InsertData(
                table: "ClientData",
                columns: new[] { "ClientDataId", "ImageHubImageId", "Location" },
                values: new object[,]
                {
                    { 1, null, "Not set" },
                    { 2, null, "Not set" },
                    { 3, null, "Not set" },
                    { 4, null, "Not set" },
                    { 5, null, "Not set" },
                    { 6, null, "Not set" },
                    { 7, null, "Not set" },
                    { 8, null, "Not set" },
                    { 9, null, "Not set" },
                    { 10, null, "Not set" },
                    { 11, null, "Not set" },
                    { 12, null, "Not set" }
                });

            migrationBuilder.InsertData(
                table: "Gym",
                columns: new[] { "Id", "Description", "IsConfirmed", "Location", "Name", "RegisterDate" },
                values: new object[,]
                {
                    { 1, "O seu destino definitivo para fitness e bem-estar à beira do rio Tejo. O FitTejo oferece uma experiência de treino premium com vistas deslumbrantes do rio e uma atmosfera energética.", true, "Vale de Milhacos, Corroios, Seixal, Setúbal, Portugal", "FitTejo", new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2754) },
                    { 2, "FitnessUp é um ginásio moderno e acolhedor, oferecendo uma ampla variedade de equipamentos de última geração e aulas de fitness emocionantes. Nossa equipe dedicada está aqui para ajudá-lo a alcançar seus objetivos de saúde e fitness, independentemente de seu nível de condicionamento físico atual.", true, "Alto do Moinho, Corroios, Seixal, Setúbal, Portugal", "FitnessUp", new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2759) },
                    { 3, "Bem-vindo ao CorroiosGym, o seu destino para uma vida saudável e ativa. Localizado no coração de Corroios, nosso ginásio oferece uma atmosfera amigável e motivadora, equipamentos de alta qualidade e treinadores experientes.", false, "Marialva, Corroios, Seixal, Setúbal, Portugal", "CorroiosGym", new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2763) },
                    { 4, "IPSGym é mais do que apenas um ginásio - é uma comunidade dedicada ao fitness e ao bem-estar. Com instalações modernas e uma variedade de programas de treinamento, desde musculação até aulas de grupo, estamos aqui para apoiar você em sua jornada de saúde. Junte-se a nós e descubra o poder da transformação pessoal.", false, "Estafanilho, Praias do Sado, Setúbal, Portugal", "IPSGym", new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2768) }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "HubImageId", "Description", "GymId", "Height", "Name", "Path", "Width" },
                values: new object[,]
                {
                    { 1, null, null, 0, "proteinshaker.webp", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fproteinshaker.webp?alt=media&token=57690fda-ea9e-4ad6-ab39-c286b3355af6", 0 },
                    { 2, null, null, 0, "OIP.jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP.jpg?alt=media&token=b4cb3cd0-ffef-4db1-8ba5-155bd52823e1", 0 },
                    { 3, null, null, 0, "R.jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR.jpg?alt=media&token=01fa4186-8a33-4a0c-b850-950274753e6e", 0 },
                    { 4, null, null, 0, "OIP (1).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(1).jpg?alt=media&token=f7694b1c-8c1a-49ff-a498-d9d18346fb6d", 0 },
                    { 5, null, null, 0, "faixa_elastica.webp", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Ffaixa_elastica.webp?alt=media&token=e47ab5ac-17d9-4d42-a4da-65dd8866bb0c", 0 },
                    { 6, null, null, 0, "5b6NR6XbOeQ3nhKpx3MJ3aFwiTVR0A.jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2F5b6NR6XbOeQ3nhKpx3MJ3aFwiTVR0A.jpg?alt=media&token=fb078d90-1b1b-4087-a757-1de6e710b262", 0 },
                    { 7, null, null, 0, "R (1).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(1).jpg?alt=media&token=46a110dd-a5f1-4faf-bc4e-8f77c49bbdc7", 0 },
                    { 8, null, null, 0, "R (2).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(2).jpg?alt=media&token=693271cb-5daa-499f-a9a4-322ebc44ec59", 0 },
                    { 9, null, null, 0, "R (3).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(3).jpg?alt=media&token=1030b9ff-9cf5-4276-a89f-a1ca38d7ed8e", 0 },
                    { 10, null, null, 0, "550848_3_proform-passadeira-corrida-705-cst.jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2F550848_3_proform-passadeira-corrida-705-cst.jpg?alt=media&token=35c2d330-1af0-416c-8aba-757fb498da83", 0 },
                    { 11, null, null, 0, "OIP (2).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(2).jpg?alt=media&token=3807105f-a18e-4d6b-86b4-8039065b2b51", 0 },
                    { 12, null, null, 0, "bike-esteira-transport-escada-como-usar.webp", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fbike-esteira-transport-escada-como-usar.webp?alt=media&token=81", 0 },
                    { 13, null, null, 0, "R (4).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(4).jpg?alt=media&token=f8421d2f-e05b-4fbf-a7ee-3dfff219cabb", 0 },
                    { 14, null, null, 0, "A.jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FA.jpg?alt=media&token=ea06d202-3dcf-4d2c-9913-fdad6e610fc4", 0 },
                    { 15, null, null, 0, "OIP (3).jpg", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(3).jpg?alt=media&token=b87b5398-e7a5-4cc2-9d67-e018e914e008", 0 }
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "AccountId", "AccountType", "Email", "GymId", "Password", "PhoneNumber", "RecoverCode", "Token", "UserName" },
                values: new object[,]
                {
                    { 5, 2, "GymTest1@email.pt", 1, "46070d4bf934fb0d4b06d9e2c46e346944e322444900a435d7d9a95e6d7435f5", null, null, null, "GymTest1" },
                    { 6, 2, "GymTest2@email.pt", 2, "46070d4bf934fb0d4b06d9e2c46e346944e322444900a435d7d9a95e6d7435f5", null, null, null, "GymTest2" },
                    { 19, 2, "GymTest3@email.pt", 3, "46070d4bf934fb0d4b06d9e2c46e346944e322444900a435d7d9a95e6d7435f5", null, null, null, "GymTest3" },
                    { 20, 2, "GymTest4@email.pt", 4, "46070d4bf934fb0d4b06d9e2c46e346944e322444900a435d7d9a95e6d7435f5", null, null, null, "GymTest4" }
                });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "BankAccountId", "GymId", "GymSubscriptionName", "GymSubscriptionPrice", "StripeBankId", "StripePlanId" },
                values: new object[,]
                {
                    { 1, 1, "Subscription Plan", 40.0, "acct_1Ow4beRpgJmjBFH7", "plan_Prg8MwDUJOj1e3" },
                    { 2, 2, "Subscription Plan", 25.0, "acct_1Ow4bhRs2igcN0GT", "plan_Pld1tSPdt0Qqid" }
                });

            migrationBuilder.InsertData(
                table: "Client",
                columns: new[] { "ClientId", "BiometricsId", "ClientDataId", "Description", "Gender", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 1, "Descrição do Cliente 1", 0, 7 },
                    { 2, 2, 2, "Descrição do Cliente 2", 1, 8 },
                    { 3, 3, 3, "Descrição do Cliente 3", 1, 9 },
                    { 4, 4, 4, "Descrição do Cliente 4", 1, 10 },
                    { 5, 5, 5, "Descrição do Cliente 5", 1, 11 },
                    { 6, 6, 6, "Descrição do Cliente 6", 1, 12 },
                    { 7, 7, 7, "Descrição do Cliente 7", 0, 13 },
                    { 8, 8, 8, "Descrição do Cliente 8", 0, 14 },
                    { 9, 9, 9, "Descrição do Cliente 9", 0, 15 },
                    { 10, 10, 10, "Descrição do Cliente 10", 0, 16 },
                    { 11, 11, 11, "Descrição do Cliente 11", 1, 17 },
                    { 12, 12, 12, "Descrição do Cliente 12", 0, 18 }
                });

            migrationBuilder.InsertData(
                table: "GymMachines",
                columns: new[] { "MachineId", "Description", "GymId", "ImageId", "Name", "Quantity" },
                values: new object[,]
                {
                    { 1, "Perfeita para corridas intensas e caminhadas, com inclinação ajustável e monitor de ritmo cardíaco integrado.", 1, 7, "Esteira Elétrica", 2 },
                    { 2, "Proporciona um treino cardiovascular eficaz, com resistência ajustável e tela LCD para acompanhar o progresso do treino.", 1, 8, "Bicicleta Ergométrica", 2 },
                    { 3, "Ótima para exercícios de cardio e fortalecimento muscular, com ajuste de resistência e monitor de desempenho integrado.", 1, 9, "Máquina de Remo", 2 },
                    { 4, "Perfeita para corridas intensas e caminhadas, com inclinação ajustável e monitor de ritmo cardíaco integrado.", 2, null, "Esteira Elétrica", 2 },
                    { 5, "Proporciona um treino cardiovascular eficaz, com resistência ajustável e tela LCD para acompanhar o progresso do treino.", 2, null, "Bicicleta Ergométrica", 2 },
                    { 6, "Ótima para exercícios de cardio e fortalecimento muscular, com ajuste de resistência e monitor de desempenho integrado.", 2, null, "Máquina de Remo", 2 }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "HubImageId", "Description", "GymId", "Height", "Name", "Path", "Width" },
                values: new object[,]
                {
                    { 16, null, 1, 0, "image_gym1ed18486e-14d1-4a09-8f6d-891442e7f3bf", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1ed18486e-14d1-4a09-8f6d-891442e7f3bf?alt=media&token=cae942b0-700e-4fd2-89b5-000dbe1a28dd", 0 },
                    { 17, null, 1, 0, "image_gym1168bad0b-8e82-4eea-8b7f-95ea63104ed5", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1168bad0b-8e82-4eea-8b7f-95ea63104ed5?alt=media&token=cf584d38-6dd9-41a4-945b-74c6721ef20c", 0 },
                    { 18, null, 1, 0, "image_gym1e4406c1c-43e6-48a3-b12e-994ed75ee3b3", "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1e4406c1c-43e6-48a3-b12e-994ed75ee3b3?alt=media&token=222687e0-b665-42e2-a3a7-1d4effb28d5f", 0 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "Description", "GymId", "ImageId", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, "Este shaker de proteína é perfeito para preparar seus shakes pós-treino. Com design ergonômico e capacidade de mistura eficaz, você pode desfrutar de uma bebida deliciosa e nutritiva sempre que precisar.", 1, 1, "Shaker de Proteína", 5.9900000000000002, 6 },
                    { 2, "Esta esteira de yoga oferece aderência superior e conforto durante suas práticas de yoga ou alongamento. Feita com material antiderrapante de alta qualidade, é ideal para uso em casa ou no estúdio.", 1, 2, "Esteira de Yoga Antiderrapante", 7.9900000000000002, 6 },
                    { 3, "Proteja suas mãos durante os exercícios de levantamento de peso com estas luvas acolchoadas. Feitas com material durável e almofadado, proporcionam aderência e suporte, permitindo que você se concentre em seu treino.", 1, 3, "Luvas de Levantamento de Peso Acolchoadas", 9.9900000000000002, 6 },
                    { 4, "Mantenha-se hidratado durante o dia com esta garrafa de água isolada. Feita de aço inoxidável durável e com isolamento a vácuo, mantém suas bebidas frias por até 24 horas ou quentes por até 12 horas.", 1, 4, "Garrafa de Água Isolada de Aço Inoxidável", 12.99, 6 },
                    { 5, "Aumente a intensidade do seu treino com estas faixas de resistência de látex. Disponíveis em diferentes níveis de resistência, são ideais para exercícios de fortalecimento muscular, alongamento e reabilitação.", 1, 5, "Faixas de Resistência de Látex", 14.99, 6 },
                    { 6, "Esta pulseira inteligente rastreia sua atividade diária, frequência cardíaca, qualidade do sono e muito mais. Com design elegante e conectividade Bluetooth, é o companheiro perfeito para ajudá-lo a alcançar seus objetivos de fitness.", 1, 6, "Pulseira Inteligente de Monitoramento de Atividade", 17.989999999999998, 6 },
                    { 7, "Este shaker de proteína é perfeito para preparar seus shakes pós-treino. Com design ergonômico e capacidade de mistura eficaz, você pode desfrutar de uma bebida deliciosa e nutritiva sempre que precisar.", 2, null, "Shaker de Proteína", 5.9900000000000002, 6 },
                    { 8, "Esta esteira de yoga oferece aderência superior e conforto durante suas práticas de yoga ou alongamento. Feita com material antiderrapante de alta qualidade, é ideal para uso em casa ou no estúdio.", 2, null, "Esteira de Yoga Antiderrapante", 7.9900000000000002, 6 },
                    { 9, "Proteja suas mãos durante os exercícios de levantamento de peso com estas luvas acolchoadas. Feitas com material durável e almofadado, proporcionam aderência e suporte, permitindo que você se concentre em seu treino.", 2, null, "Luvas de Levantamento de Peso Acolchoadas", 9.9900000000000002, 6 },
                    { 10, "Mantenha-se hidratado durante o dia com esta garrafa de água isolada. Feita de aço inoxidável durável e com isolamento a vácuo, mantém suas bebidas frias por até 24 horas ou quentes por até 12 horas.", 2, null, "Garrafa de Água Isolada de Aço Inoxidável", 12.99, 6 },
                    { 11, "Aumente a intensidade do seu treino com estas faixas de resistência de látex. Disponíveis em diferentes níveis de resistência, são ideais para exercícios de fortalecimento muscular, alongamento e reabilitação.", 2, null, "Faixas de Resistência de Látex", 14.99, 6 },
                    { 12, "Esta pulseira inteligente rastreia sua atividade diária, frequência cardíaca, qualidade do sono e muito mais. Com design elegante e conectividade Bluetooth, é o companheiro perfeito para ajudá-lo a alcançar seus objetivos de fitness.", 2, null, "Pulseira Inteligente de Monitoramento de Atividade", 17.989999999999998, 6 }
                });

            migrationBuilder.InsertData(
                table: "DebitCard",
                columns: new[] { "DebitCardId", "ClientId", "StripeCustomerId", "StripePaymentMethodId" },
                values: new object[,]
                {
                    { 1, 1, "cus_PlbprNroroGeFr", "pm_1Ow57fRseCm2355tph3WmrRE" },
                    { 2, 2, "cus_Plbp64oawQ62uP", "pm_1Ow58dRseCm2355tiOre9byR" },
                    { 3, 3, "cus_Plbp2N5j4PKj3t", "pm_1Ow59XRseCm2355te3uzVWWf" },
                    { 7, 7, "cus_PlbpHhHFVJJZgn", "pm_1Ow5AORseCm2355tQ25Jge06" },
                    { 8, 8, "cus_PlbpUx559W0bqL", "pm_1Ow5AyRseCm2355tgHg7cyRG" },
                    { 9, 9, "cus_Plbpb7N2yNwzgm", "pm_1Ow5BaRseCm2355thRNhZIui" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "ExerciseId", "Description", "ImageId", "MachineId", "Name" },
                values: new object[,]
                {
                    { 1, "Desafie-se com uma corrida em uma esteira inclinada para um treino cardiovascular intenso.", 10, 1, "Corrida Inclinada" },
                    { 2, "Um exercício de caminhada relaxante para melhorar a saúde cardiovascular e queimar calorias.", 11, 1, "Caminhada Moderada" },
                    { 3, "Experimente um treino de bicicleta estável com resistência ajustável para fortalecer as pernas e queimar gordura.", 12, 2, "Ciclismo de Resistência" },
                    { 4, "Alternância entre períodos de alta e baixa intensidade em uma bicicleta ergométrica para melhorar o condicionamento físico.", 13, 2, "Treino Intervalado de Bicicleta" },
                    { 5, "Execute movimentos de remo controlados em uma máquina de remo para trabalhar todo o corpo e melhorar a resistência.", 14, 3, "Remada Longa e Lenta" },
                    { 6, "Aumente a resistência e a velocidade para um treino de remo de alta intensidade que desafia o corpo e queima calorias.", 15, 3, "Remada Intensa" },
                    { 7, "Desafie-se com uma corrida em uma esteira inclinada para um treino cardiovascular intenso.", null, 4, "Corrida Inclinada" },
                    { 8, "Um exercício de caminhada relaxante para melhorar a saúde cardiovascular e queimar calorias.", null, 4, "Caminhada Moderada" },
                    { 9, "Experimente um treino de bicicleta estável com resistência ajustável para fortalecer as pernas e queimar gordura.", null, 5, "Ciclismo de Resistência" },
                    { 10, "Alternância entre períodos de alta e baixa intensidade em uma bicicleta ergométrica para melhorar o condicionamento físico.", null, 5, "Treino Intervalado de Bicicleta" },
                    { 11, "Execute movimentos de remo controlados em uma máquina de remo para trabalhar todo o corpo e melhorar a resistência.", null, 6, "Remada Longa e Lenta" },
                    { 12, "Aumente a resistência e a velocidade para um treino de remo de alta intensidade que desafia o corpo e queima calorias.", null, 6, "Remada Intensa" }
                });

            migrationBuilder.InsertData(
                table: "GymClients",
                columns: new[] { "GymClientId", "ClientId", "EnrollmentDate", "GymId", "Role" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2960), 1, 0 },
                    { 2, 2, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2965), 1, 0 },
                    { 3, 3, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2967), 1, 0 },
                    { 4, 4, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2969), 2, 0 },
                    { 5, 5, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2971), 2, 0 },
                    { 6, 6, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2973), 2, 0 }
                });

            migrationBuilder.InsertData(
                table: "GymEmployees",
                columns: new[] { "GymEmployeeId", "ClientId", "EnrollmentDate", "GymId", "Role" },
                values: new object[,]
                {
                    { 1, 7, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2918), 1, 1 },
                    { 2, 8, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2924), 1, 2 },
                    { 3, 9, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2926), 1, 3 },
                    { 4, 10, new DateTime(2024, 5, 4, 18, 47, 56, 791, DateTimeKind.Local).AddTicks(2928), 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "GymRequests",
                columns: new[] { "RequestId", "ClientId", "GymId" },
                values: new object[,]
                {
                    { 1, 11, 2 },
                    { 2, 12, 2 }
                });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "PlanId", "ClientId", "Description", "HubImageId", "PlanType", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Este plano nutricional foi projetado para ajudá-lo a aumentar a massa muscular e ganhar peso de forma saudável. Inclui uma variedade de alimentos ricos em proteínas e carboidratos para apoiar o crescimento muscular.", null, 0, "Bulking Plan" },
                    { 2, 1, "Este plano de exercícios foi desenvolvido para maximizar o ganho de massa muscular e força. Inclui uma combinação de exercícios de levantamento de peso e treinamento de resistência para estimular o crescimento muscular.", null, 1, "Bulking Plan" }
                });

            migrationBuilder.InsertData(
                table: "Buyables",
                columns: new[] { "BuyableId", "BankAccountId", "BuyableType", "DebitCardId", "StripeSubscriptionId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "sub_1P1wmYRseCm2355teKZtrJLj" },
                    { 2, 1, 1, 2, "sub_1P1wmbRseCm2355tRI7Ee9iR" },
                    { 3, 1, 1, 3, "sub_1P1wmeRseCm2355t542tsUZV" }
                });

            migrationBuilder.InsertData(
                table: "GymRelations",
                columns: new[] { "GymRelationId", "GymClientId", "GymEmployeeId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 2, 1 },
                    { 4, 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "PlanItems",
                columns: new[] { "PlanItemId", "Description", "HubImageId", "Name", "PlanId", "PlanType" },
                values: new object[,]
                {
                    { 1, "Esta refeição é especialmente formulada para fornecer uma dose nutritiva e energética para começar o dia com energia. Inclui uma mistura equilibrada de proteínas, carboidratos e gorduras saudáveis para sustentar suas atividades matinais.", null, "Refeição Matinal", 1, 0 },
                    { 2, "Esta refeição é projetada para ajudar na recuperação muscular e reabastecer o corpo após um treino intenso. Rica em proteínas de alta qualidade e carboidratos de rápida absorção, esta refeição promove a regeneração muscular e a reposição de energia.", null, "Refeição Pós-Treino", 1, 0 },
                    { 3, "Esta refeição noturna é projetada para promover a recuperação muscular durante o sono e manter um metabolismo saudável durante a noite. Contendo nutrientes essenciais e de digestão lenta, esta refeição ajuda a manter a saciedade e suporta o reparo muscular durante o descanso.", null, "Refeição Noturna", 1, 0 }
                });

            migrationBuilder.InsertData(
                table: "PlanItems",
                columns: new[] { "PlanItemId", "Description", "ExerciseId", "Name", "PlanId", "PlanType" },
                values: new object[,]
                {
                    { 4, "Este exercício de treino de força é focado no desenvolvimento muscular e na melhoria da força e resistência. Incorporando uma variedade de movimentos e técnicas de levantamento de peso, este treino visa fortalecer os principais grupos musculares do corpo.", 1, "Treino de Força", 2, 1 },
                    { 5, "Este exercício de cardio intenso é projetado para elevar sua frequência cardíaca e queimar calorias de forma eficaz. Com uma combinação de movimentos aeróbicos de alta intensidade, este treino melhora a capacidade cardiovascular e promove a perda de peso.", 1, "Cardio Intenso", 2, 1 },
                    { 6, "Este exercício de alongamento e flexibilidade é ideal para melhorar a amplitude de movimento, reduzir a rigidez muscular e prevenir lesões. Concentrando-se em esticar os principais grupos musculares, este treino ajuda a aumentar a mobilidade e a flexibilidade geral do corpo.", 2, "Alongamento e Flexibilidade", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "PaymentDetails",
                columns: new[] { "PaymentDetailsId", "BuyableId", "Description", "PaymentDate", "Status" },
                values: new object[,]
                {
                    { 1, 1, "Subscription 1", new DateOnly(2024, 5, 4), 2 },
                    { 2, 2, "Subscription 2", new DateOnly(2024, 5, 4), 2 },
                    { 3, 3, "Subscription 3", new DateOnly(2024, 5, 4), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_GymId",
                table: "Account",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_Token",
                table: "Account",
                column: "Token",
                unique: true,
                filter: "[Token] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserName",
                table: "Account",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_GymId",
                table: "BankAccounts",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Buyables_BankAccountId",
                table: "Buyables",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Buyables_DebitCardId",
                table: "Buyables",
                column: "DebitCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ItemId",
                table: "CartItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_BiometricsId",
                table: "Client",
                column: "BiometricsId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_ClientDataId",
                table: "Client",
                column: "ClientDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_UserId",
                table: "Client",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientData_ImageHubImageId",
                table: "ClientData",
                column: "ImageHubImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientStats_ClientId",
                table: "ClientStats",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitCard_ClientId",
                table: "DebitCard",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStats_ClientId",
                table: "EmployeeStats",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ImageId",
                table: "Exercises",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_MachineId",
                table: "Exercises",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Gym_Name",
                table: "Gym",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GymClients_ClientId",
                table: "GymClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GymClients_GymId",
                table: "GymClients",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_GymEmployees_ClientId",
                table: "GymEmployees",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GymEmployees_GymId",
                table: "GymEmployees",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_GymMachines_GymId",
                table: "GymMachines",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_GymMachines_ImageId",
                table: "GymMachines",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_GymRelations_GymClientId",
                table: "GymRelations",
                column: "GymClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GymRelations_GymEmployeeId",
                table: "GymRelations",
                column: "GymEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_GymRequests_ClientId",
                table: "GymRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GymRequests_GymId",
                table: "GymRequests",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_GymStats_GymId",
                table: "GymStats",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_GymId",
                table: "Images",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_GymId",
                table: "Items",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ImageId",
                table: "Items",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_BuyableId",
                table: "PaymentDetails",
                column: "BuyableId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanItems_ExerciseId",
                table: "PlanItems",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanItems_HubImageId",
                table: "PlanItems",
                column: "HubImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanItems_PlanId",
                table: "PlanItems",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_ClientId",
                table: "Plans",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_HubImageId",
                table: "Plans",
                column: "HubImageId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_EmployeeStatsId",
                table: "StatisticEntity",
                column: "EmployeeStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_EmployeeStatsId1",
                table: "StatisticEntity",
                column: "EmployeeStatsId1");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_GymStatsId",
                table: "StatisticEntity",
                column: "GymStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_GymStatsId1",
                table: "StatisticEntity",
                column: "GymStatsId1");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_PlatFormStatsId",
                table: "StatisticEntity",
                column: "PlatFormStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticEntity_PlatFormStatsId1",
                table: "StatisticEntity",
                column: "PlatFormStatsId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "ClientStats");

            migrationBuilder.DropTable(
                name: "GymRelations");

            migrationBuilder.DropTable(
                name: "GymRequests");

            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "PlanItems");

            migrationBuilder.DropTable(
                name: "StatisticEntity");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "GymClients");

            migrationBuilder.DropTable(
                name: "GymEmployees");

            migrationBuilder.DropTable(
                name: "Buyables");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "EmployeeStats");

            migrationBuilder.DropTable(
                name: "GymStats");

            migrationBuilder.DropTable(
                name: "PlatformStats");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "DebitCard");

            migrationBuilder.DropTable(
                name: "GymMachines");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Biometrics");

            migrationBuilder.DropTable(
                name: "ClientData");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Gym");
        }
    }
}
