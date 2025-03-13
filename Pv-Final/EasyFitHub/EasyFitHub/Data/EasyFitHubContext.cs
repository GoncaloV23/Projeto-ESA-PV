using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EasyFitHub.Models.Account;
using EasyFitHub.Models.Gym;
using EasyFitHub.Models.Profile;
using EasyFitHub.Models.Inventory;
using EasyFitHub.Models.Payment;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using EasyFitHub.Models.Plan;
using EasyFitHub.Models.Statistics;
using EasyFitHub.Models.Miscalenous;
using EasyFitHub.Utils;

namespace EasyFitHub.Data
{
    public class EasyFitHubContext : DbContext
    {
        public EasyFitHubContext(DbContextOptions<EasyFitHubContext> options)
            : base(options)
        {
        }

        //Authentication
        public DbSet<Account> Account { get; set; } = default!;

        //Images
        public DbSet<HubImage> Images { get; set; } = default!;

        //Profile
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<ClientData> ClientData { get; set; } = default!;
        public DbSet<Biometrics> Biometrics { get; set; } = default!;

        //Gym
        public DbSet<Gym> Gym { get; set; } = default!;
        public DbSet<GymClient> GymClients { get; set; } = default!;
        public DbSet<GymEmployee> GymEmployees { get; set; } = default!;
        public DbSet<GymRelation> GymRelations { get; set; } = default!;
        public DbSet<GymRequest> GymRequests { get; set; } = default!;

        //Inventory
        public DbSet<Item> Items { get; set; } = default!;
        public DbSet<GymMachine> GymMachines { get; set; } = default!;
        public DbSet<Exercise> Exercises { get; set; } = default!;


        //Payment
        public DbSet<BankAccount> BankAccounts { get; set; } = default!;
        public DbSet<DebitCard> DebitCard { get; set; } = default!;
        public DbSet<PaymentDetails> PaymentDetails { get; set; } = default!;
        public DbSet<Buyable> Buyables { get; set; } = default!;

        //Plans
        public DbSet<PlanItem> PlanItems { get; set; } = default!;
        public DbSet<Plan> Plans { get; set; } = default!;

        //Stats
        public DbSet<PlatformStats> PlatformStats { get; set; } = default!;
        public DbSet<ClientStats> ClientStats { get; set; } = default!;
        public DbSet<GymStats> GymStats { get; set; } = default!;
        public DbSet<EmployeeStats> EmployeeStats { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Account
            modelBuilder.Entity<Account>()
               .HasDiscriminator<AccountType>("AccountType")
               .HasValue<Account>(AccountType.UNDEFINED)
               .HasValue<User>(AccountType.USER)
               .HasValue<Manager>(AccountType.MANAGER)
               .HasValue<Admin>(AccountType.ADMIN);

            modelBuilder.Entity<Manager>()
                .HasBaseType<Account>();

            modelBuilder.Entity<User>()
                .HasBaseType<Account>();

            modelBuilder.Entity<Account>()
               .HasIndex(a => a.UserName)
               .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Token)
                .IsUnique();


            //Gym
            modelBuilder.Entity<Gym>()
                .HasIndex(a => a.Name)
                .IsUnique();
            modelBuilder.Entity<GymRequest>()
                .HasOne(ge => ge.Gym)
                .WithMany(g => g.Requests)
                .HasForeignKey(ge => ge.GymId).OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<GymEmployee>()
                .HasOne(ge => ge.Gym)
                .WithMany(g => g.GymEmployees)
                .HasForeignKey(ge => ge.GymId).OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<GymClient>()
                .HasOne(ge => ge.Gym)
                .WithMany(g => g.GymClients)
                .HasForeignKey(ge => ge.GymId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<GymRelation>()
                .HasOne(gr => gr.GymClient)
                .WithMany(gc => gc.GymEmployees)
                .HasForeignKey(gr => gr.GymClientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GymRelation>()
                .HasOne(gr => gr.GymEmployee)
                .WithMany(ge => ge.GymClients)
                .HasForeignKey(gr => gr.GymEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);


            //Plans
            modelBuilder.Entity<PlanItem>()
                .HasOne(gr => gr.Plan)
                .WithMany(ge => ge.Items)
                .HasForeignKey(gr => gr.PlanId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<PlanItem>()
                .HasDiscriminator(p => p.PlanType)
                .HasValue<PlanMeal>(PlanType.NUTRITION)
                .HasValue<PlanExercise>(PlanType.EXERCISE)
                .HasValue<PlanItem>(PlanType.UNDEFINED);

            //Payments
            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.Gym)
                .WithMany()
                .HasForeignKey(b => b.GymId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<DebitCard>()
                .HasOne(b => b.Client)
                .WithMany()
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Buyable>()
                .HasDiscriminator(b => b.BuyableType)
                .HasValue<Subscription>(BuyableType.SUBSCRIPTION)
                .HasValue<Cart>(BuyableType.CART);
            modelBuilder.Entity<Buyable>()
                .HasOne(ge => ge.GymBank)
                .WithMany(g => g.Buyables)
                .HasForeignKey(b => b.BankAccountId).OnDelete(DeleteBehavior.ClientNoAction);
            modelBuilder.Entity<Buyable>()
                .HasOne(ge => ge.ClientDebitCard)
                .WithMany(g => g.Buyables)
                .HasForeignKey(b => b.DebitCardId).OnDelete(DeleteBehavior.ClientNoAction);

            //Stats
            modelBuilder.Entity<GymStats>()
                .HasOne(s => s.Gym)
                .WithMany()
                .HasForeignKey(s => s.GymId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeStats>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ClientStats>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            //Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AccountId = 1,
                    UserName = "GVieira",
                    Email = "202100296@estudantes.ips.pt",
                    Password = PasswordHasher.HashPassword("projesa2024")
                },
                new Admin
                {
                    AccountId = 2,
                    UserName = "RBarroso",
                    Email = "202100299@estudantes.ips.pt",
                    Password = PasswordHasher.HashPassword("projesa2024")
                },
                new Admin
                {
                    AccountId = 3,
                    UserName = "FSilva",
                    Email = "202100984@estudantes.ips.pt",
                    Password = PasswordHasher.HashPassword("projesa2024")
                },
                new Admin
                {
                    AccountId = 4,
                    UserName = "APauli",
                    Email = "201901953@estudantes.ips.pt",
                    Password = PasswordHasher.HashPassword("projesa2024")
                });


            seedTables(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void seedTables(ModelBuilder modelBuilder)
        {
            //Clients
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    AccountId = 7,
                    UserName = "test1",
                    Email = "test1@gmail.com",
                    Password = PasswordHasher.HashPassword("test1"),
                    Name = "Sofia",
                    Surname = "Almeida",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 8,
                    UserName = "test2",
                    Email = "test2@gmail.com",
                    Password = PasswordHasher.HashPassword("test2"),
                    Name = "Tiago",
                    Surname = "Martins",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 9,
                    UserName = "test3",
                    Email = "test3@gmail.com",
                    Password = PasswordHasher.HashPassword("test3"),
                    Name = "Joana",
                    Surname = "Silva",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 10,
                    UserName = "test4",
                    Email = "test4@gmail.com",
                    Password = PasswordHasher.HashPassword("test4"),
                    Name = "Pedro",
                    Surname = "Santos",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 11,
                    UserName = "test5",
                    Email = "test5@gmail.com",
                    Password = PasswordHasher.HashPassword("test5"),
                    Name = "Mariana",
                    Surname = "Costa",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 12,
                    UserName = "test6",
                    Email = "test6@gmail.com",
                    Password = PasswordHasher.HashPassword("test6"),
                    Name = "Miguel",
                    Surname = "Ferreira",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 13,
                    UserName = "test7",
                    Email = "test7@gmail.com",
                    Password = PasswordHasher.HashPassword("test7"),
                    Name = "Carolina",
                    Surname = "Sousa",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 14,
                    UserName = "test8",
                    Email = "test8@gmail.com",
                    Password = PasswordHasher.HashPassword("test8"),
                    Name = "Ricardo",
                    Surname = "Carvalho",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 15,
                    UserName = "test9",
                    Email = "test9@gmail.com",
                    Password = PasswordHasher.HashPassword("test9"),
                    Name = "Ana",
                    Surname = "Rodrigues",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 16,
                    UserName = "test10",
                    Email = "test10@gmail.com",
                    Password = PasswordHasher.HashPassword("test10"),
                    Name = "Bruno",
                    Surname = "Gonçalves",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 17,
                    UserName = "test11",
                    Email = "test11@gmail.com",
                    Password = PasswordHasher.HashPassword("test11"),
                    Name = "Beatriz",
                    Surname = "Lima",
                    BirthDate = new DateOnly(2002, 1, 6)
                },
                new User
                {
                    AccountId = 18,
                    UserName = "test12",
                    Email = "test12@gmail.com",
                    Password = PasswordHasher.HashPassword("test12"),
                    Name = "Diogo",
                    Surname = "Ribeiro",
                    BirthDate = new DateOnly(2002, 1, 6)
                }
            );

            // Biometrics
            modelBuilder.Entity<Biometrics>().HasData(
                new Biometrics
                {
                    BiometricsId = 1,
                },
                new Biometrics
                {
                    BiometricsId = 2,
                },
                new Biometrics
                {
                    BiometricsId = 3,
                },
                new Biometrics
                {
                    BiometricsId = 4,
                },
                new Biometrics
                {
                    BiometricsId = 5,
                },
                new Biometrics
                {
                    BiometricsId = 6,
                },
                new Biometrics
                {
                    BiometricsId = 7,
                },
                new Biometrics
                {
                    BiometricsId = 8,
                },
                new Biometrics
                {
                    BiometricsId = 9,
                },
                new Biometrics
                {
                    BiometricsId = 10,
                },
                new Biometrics
                {
                    BiometricsId = 11,
                },
                new Biometrics
                {
                    BiometricsId = 12,
                }
            );
            modelBuilder.Entity<ClientData>().HasData(
                new ClientData
                {
                    ClientDataId = 1
                },
                new ClientData
                {
                    ClientDataId = 2
                },
                new ClientData
                {
                    ClientDataId = 3
                },
                new ClientData
                {
                    ClientDataId = 4
                },
                new ClientData
                {
                    ClientDataId = 5
                },
                new ClientData
                {
                    ClientDataId = 6
                },
                new ClientData
                {
                    ClientDataId = 7
                },
                new ClientData
                {
                    ClientDataId = 8
                },
                new ClientData
                {
                    ClientDataId = 9
                },
                new ClientData
                {
                    ClientDataId = 10
                },
                new ClientData
                {
                    ClientDataId = 11
                },
                new ClientData
                {
                    ClientDataId = 12
                }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    ClientId = 1,
                    Description = "Descrição do Cliente 1",
                    Gender = Gender.MASCULINE,
                    UserId = 7,
                    BiometricsId = 1,
                    ClientDataId = 1,
                },
                new Client
                {
                    ClientId = 2,
                    Description = "Descrição do Cliente 2",
                    Gender = Gender.FEMININE,
                    UserId = 8,
                    BiometricsId = 2,
                    ClientDataId = 2,
                },
                new Client
                {
                    ClientId = 3,
                    Description = "Descrição do Cliente 3",
                    Gender = Gender.FEMININE,
                    UserId = 9,
                    BiometricsId = 3,
                    ClientDataId = 3,
                },
                new Client
                {
                    ClientId = 4,
                    Description = "Descrição do Cliente 4",
                    Gender = Gender.FEMININE,
                    UserId = 10,
                    BiometricsId = 4,
                    ClientDataId = 4,
                }, new Client
                {
                    ClientId = 5,
                    Description = "Descrição do Cliente 5",
                    Gender = Gender.FEMININE,
                    UserId = 11,
                    BiometricsId = 5,
                    ClientDataId = 5,
                },
                new Client
                {
                    ClientId = 6,
                    Description = "Descrição do Cliente 6",
                    Gender = Gender.FEMININE,
                    UserId = 12,
                    BiometricsId = 6,
                    ClientDataId = 6,
                },
                new Client
                {
                    ClientId = 7,
                    Description = "Descrição do Cliente 7",
                    Gender = Gender.MASCULINE,
                    UserId = 13,
                    BiometricsId = 7,
                    ClientDataId = 7,
                },
                new Client
                {
                    ClientId = 8,
                    Description = "Descrição do Cliente 8",
                    Gender = Gender.MASCULINE,
                    UserId = 14,
                    BiometricsId = 8,
                    ClientDataId = 8,
                },
                new Client
                {
                    ClientId = 9,
                    Description = "Descrição do Cliente 9",
                    Gender = Gender.MASCULINE,
                    UserId = 15,
                    BiometricsId = 9,
                    ClientDataId = 9,
                },
                new Client
                {
                    ClientId = 10,
                    Description = "Descrição do Cliente 10",
                    Gender = Gender.MASCULINE,
                    UserId = 16,
                    BiometricsId = 10,
                    ClientDataId = 10,
                },
                new Client
                {
                    ClientId = 11,
                    Description = "Descrição do Cliente 11",
                    Gender = Gender.FEMININE,
                    UserId = 17,
                    BiometricsId = 11,
                    ClientDataId = 11,
                },
                new Client
                {
                    ClientId = 12,
                    Description = "Descrição do Cliente 12",
                    Gender = Gender.MASCULINE,
                    UserId = 18,
                    BiometricsId = 12,
                    ClientDataId = 12,
                }
            );


            modelBuilder.Entity<HubImage>().HasData(

           new HubImage
           {
               HubImageId = 1,
               Name = "proteinshaker.webp",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fproteinshaker.webp?alt=media&token=57690fda-ea9e-4ad6-ab39-c286b3355af6"
           },
           new HubImage
           {
               HubImageId = 2,
               Name = "OIP.jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP.jpg?alt=media&token=b4cb3cd0-ffef-4db1-8ba5-155bd52823e1"
           },
           new HubImage
           {
               HubImageId = 3,
               Name = "R.jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR.jpg?alt=media&token=01fa4186-8a33-4a0c-b850-950274753e6e"
           },
           new HubImage
           {
               HubImageId = 4,
               Name = "OIP (1).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(1).jpg?alt=media&token=f7694b1c-8c1a-49ff-a498-d9d18346fb6d"
           },
           new HubImage
           {
               HubImageId = 5,
               Name = "faixa_elastica.webp",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Ffaixa_elastica.webp?alt=media&token=e47ab5ac-17d9-4d42-a4da-65dd8866bb0c"
           },
           new HubImage
           {
               HubImageId = 6,
               Name = "5b6NR6XbOeQ3nhKpx3MJ3aFwiTVR0A.jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2F5b6NR6XbOeQ3nhKpx3MJ3aFwiTVR0A.jpg?alt=media&token=fb078d90-1b1b-4087-a757-1de6e710b262"
           },
           new HubImage
           {
               HubImageId = 7,
               Name = "R (1).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(1).jpg?alt=media&token=46a110dd-a5f1-4faf-bc4e-8f77c49bbdc7"
           },
           new HubImage
           {
               HubImageId = 8,
               Name = "R (2).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(2).jpg?alt=media&token=693271cb-5daa-499f-a9a4-322ebc44ec59"
           },
           new HubImage
           {
               HubImageId = 9,
               Name = "R (3).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(3).jpg?alt=media&token=1030b9ff-9cf5-4276-a89f-a1ca38d7ed8e"
           },
           new HubImage
           {
               HubImageId = 10,
               Name = "550848_3_proform-passadeira-corrida-705-cst.jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2F550848_3_proform-passadeira-corrida-705-cst.jpg?alt=media&token=35c2d330-1af0-416c-8aba-757fb498da83"
           },
           new HubImage
           {
               HubImageId = 11,
               Name = "OIP (2).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(2).jpg?alt=media&token=3807105f-a18e-4d6b-86b4-8039065b2b51"
           },
           new HubImage
           {
               HubImageId = 12,
               Name = "bike-esteira-transport-escada-como-usar.webp",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fbike-esteira-transport-escada-como-usar.webp?alt=media&token=81"
           },
           new HubImage
           {
               HubImageId = 13,
               Name = "R (4).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FR%20(4).jpg?alt=media&token=f8421d2f-e05b-4fbf-a7ee-3dfff219cabb"
           },
           new HubImage
           {
               HubImageId = 14,
               Name = "A.jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FA.jpg?alt=media&token=ea06d202-3dcf-4d2c-9913-fdad6e610fc4"
           },
           new HubImage
           {
               HubImageId = 15,
               Name = "OIP (3).jpg",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2FOIP%20(3).jpg?alt=media&token=b87b5398-e7a5-4cc2-9d67-e018e914e008"
           },
           new HubImage
           {
               HubImageId = 16,
               Name = "image_gym1ed18486e-14d1-4a09-8f6d-891442e7f3bf",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1ed18486e-14d1-4a09-8f6d-891442e7f3bf?alt=media&token=cae942b0-700e-4fd2-89b5-000dbe1a28dd",
               GymId = 1
           },
           new HubImage
           {
               HubImageId = 17,
               Name = "image_gym1168bad0b-8e82-4eea-8b7f-95ea63104ed5",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1168bad0b-8e82-4eea-8b7f-95ea63104ed5?alt=media&token=cf584d38-6dd9-41a4-945b-74c6721ef20c",
               GymId = 1
           },
           new HubImage
           {
               HubImageId = 18,
               Name = "image_gym1e4406c1c-43e6-48a3-b12e-994ed75ee3b3",
               Path = "https://firebasestorage.googleapis.com/v0/b/easyfithub-a7cf3.appspot.com/o/images%2Fimage_gym1e4406c1c-43e6-48a3-b12e-994ed75ee3b3?alt=media&token=222687e0-b665-42e2-a3a7-1d4effb28d5f",
               GymId = 1
           }
           );
            //Gym
            modelBuilder.Entity<Gym>().HasData(
                new Gym
                {
                    Id = 1,
                    Name = "FitTejo",
                    Description = "O seu destino definitivo para fitness e bem-estar à beira do rio Tejo. O FitTejo oferece uma experiência de treino premium com vistas deslumbrantes do rio e uma atmosfera energética.",
                    Location = "Vale de Milhacos, Corroios, Seixal, Setúbal, Portugal",
                    IsConfirmed = true,
                    RegisterDate = DateTime.Now,
                },
                new Gym
                {
                    Id = 2,
                    Name = "FitnessUp",
                    Description = "FitnessUp é um ginásio moderno e acolhedor, oferecendo uma ampla variedade de equipamentos de última geração e aulas de fitness emocionantes. Nossa equipe dedicada está aqui para ajudá-lo a alcançar seus objetivos de saúde e fitness, independentemente de seu nível de condicionamento físico atual.",
                    Location = "Alto do Moinho, Corroios, Seixal, Setúbal, Portugal",
                    IsConfirmed = true,
                    RegisterDate = DateTime.Now
                },
                new Gym
                {
                    Id = 3,
                    Name = "CorroiosGym",
                    Description = "Bem-vindo ao CorroiosGym, o seu destino para uma vida saudável e ativa. Localizado no coração de Corroios, nosso ginásio oferece uma atmosfera amigável e motivadora, equipamentos de alta qualidade e treinadores experientes.",
                    Location = "Marialva, Corroios, Seixal, Setúbal, Portugal",
                    IsConfirmed = false,
                    RegisterDate = DateTime.Now
                },
                new Gym
                {
                    Id = 4,
                    Name = "IPSGym",
                    Description = "IPSGym é mais do que apenas um ginásio - é uma comunidade dedicada ao fitness e ao bem-estar. Com instalações modernas e uma variedade de programas de treinamento, desde musculação até aulas de grupo, estamos aqui para apoiar você em sua jornada de saúde. Junte-se a nós e descubra o poder da transformação pessoal.",
                    Location = "Estafanilho, Praias do Sado, Setúbal, Portugal",
                    IsConfirmed = false,
                    RegisterDate = DateTime.Now
                }
            );

            //Manager
            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    AccountId = 5,
                    UserName = "GymTest1",
                    Email = "GymTest1@email.pt",
                    Password = PasswordHasher.HashPassword("teste"),
                    GymId = 1
                },
                new Manager
                {
                    AccountId = 6,
                    UserName = "GymTest2",
                    Email = "GymTest2@email.pt",
                    Password = PasswordHasher.HashPassword("teste"),
                    GymId = 2
                },
                new Manager
                {
                    AccountId = 19,
                    UserName = "GymTest3",
                    Email = "GymTest3@email.pt",
                    Password = PasswordHasher.HashPassword("teste"),
                    GymId = 3
                },
                new Manager
                {
                    AccountId = 20,
                    UserName = "GymTest4",
                    Email = "GymTest4@email.pt",
                    Password = PasswordHasher.HashPassword("teste"),
                    GymId = 4
                }
            );





            //GymEmployees
            modelBuilder.Entity<GymEmployee>().HasData(
                new GymEmployee
                {
                    GymEmployeeId = 1,
                    Role = Role.PT,
                    ClientId = 7,
                    GymId = 1,
                },
                new GymEmployee
                {
                    GymEmployeeId = 2,
                    Role = Role.NUTRICIONIST,
                    ClientId = 8,
                    GymId = 1,
                },
                new GymEmployee
                {
                    GymEmployeeId = 3,
                    Role = Role.SECRETARY,
                    ClientId = 9,
                    GymId = 1,
                },
                new GymEmployee
                {
                    GymEmployeeId = 4,
                    Role = Role.PT,
                    ClientId = 10,
                    GymId = 2,
                }
            );
            //GymClients
            modelBuilder.Entity<GymClient>().HasData(
                new GymClient
                {
                    GymClientId = 1,
                    ClientId = 1,
                    GymId = 1
                },
                new GymClient
                {
                    GymClientId = 2,
                    ClientId = 2,
                    GymId = 1
                },
                new GymClient
                {
                    GymClientId = 3,
                    ClientId = 3,
                    GymId = 1,
                },
                new GymClient
                {
                    GymClientId = 4,
                    ClientId = 4,
                    GymId = 2
                },
                new GymClient
                {
                    GymClientId = 5,
                    ClientId = 5,
                    GymId = 2,
                },
                new GymClient
                {
                    GymClientId = 6,
                    ClientId = 6,
                    GymId = 2,
                }
            );
            //Requests
            modelBuilder.Entity<GymRequest>().HasData(
                new GymRequest
                {
                    ClientId = 11,
                    RequestId = 1,
                    GymId = 2,
                },
                new GymRequest
                {
                    ClientId = 12,
                    RequestId = 2,
                    GymId = 2,
                }
            );

            //Relations
            modelBuilder.Entity<GymRelation>().HasData(
                new GymRelation
                {
                    GymRelationId = 1,
                    GymClientId = 1,
                    GymEmployeeId = 1
                },
                new GymRelation
                {
                    GymRelationId = 2,
                    GymClientId = 1,
                    GymEmployeeId = 2
                },
                new GymRelation
                {
                    GymRelationId = 3,
                    GymClientId = 2,
                    GymEmployeeId = 1
                },
                new GymRelation
                {
                    GymRelationId = 4,
                    GymClientId = 4,
                    GymEmployeeId = 4
                }
            );



            //Inventory
            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    ItemId = 1,
                    Name = "Shaker de Proteína",
                    Description = "Este shaker de proteína é perfeito para preparar seus shakes pós-treino. Com design ergonômico e capacidade de mistura eficaz, você pode desfrutar de uma bebida deliciosa e nutritiva sempre que precisar.",
                    Quantity = 6,
                    Price = 5.99,
                    GymId = 1,
                    ImageId = 1
                },
                new Item
                {
                    ItemId = 2,
                    Name = "Esteira de Yoga Antiderrapante",
                    Description = "Esta esteira de yoga oferece aderência superior e conforto durante suas práticas de yoga ou alongamento. Feita com material antiderrapante de alta qualidade, é ideal para uso em casa ou no estúdio.",
                    Quantity = 6,
                    Price = 7.99,
                    GymId = 1,
                    ImageId = 2
                },
                new Item
                {
                    ItemId = 3,
                    Name = "Luvas de Levantamento de Peso Acolchoadas",
                    Description = "Proteja suas mãos durante os exercícios de levantamento de peso com estas luvas acolchoadas. Feitas com material durável e almofadado, proporcionam aderência e suporte, permitindo que você se concentre em seu treino.",
                    Quantity = 6,
                    Price = 9.99,
                    GymId = 1,
                    ImageId = 3
                },
                new Item
                {
                    ItemId = 4,
                    Name = "Garrafa de Água Isolada de Aço Inoxidável",
                    Description = "Mantenha-se hidratado durante o dia com esta garrafa de água isolada. Feita de aço inoxidável durável e com isolamento a vácuo, mantém suas bebidas frias por até 24 horas ou quentes por até 12 horas.",
                    Quantity = 6,
                    Price = 12.99,
                    GymId = 1,
                    ImageId = 4
                },
                new Item
                {
                    ItemId = 5,
                    Name = "Faixas de Resistência de Látex",
                    Description = "Aumente a intensidade do seu treino com estas faixas de resistência de látex. Disponíveis em diferentes níveis de resistência, são ideais para exercícios de fortalecimento muscular, alongamento e reabilitação.",
                    Quantity = 6,
                    Price = 14.99,
                    GymId = 1,
                    ImageId = 5
                },
                new Item
                {
                    ItemId = 6,
                    Name = "Pulseira Inteligente de Monitoramento de Atividade",
                    Description = "Esta pulseira inteligente rastreia sua atividade diária, frequência cardíaca, qualidade do sono e muito mais. Com design elegante e conectividade Bluetooth, é o companheiro perfeito para ajudá-lo a alcançar seus objetivos de fitness.",
                    Quantity = 6,
                    Price = 17.99,
                    GymId = 1,
                    ImageId = 6
                },
                new Item
                {
                    ItemId = 7,
                    Name = "Shaker de Proteína",
                    Description = "Este shaker de proteína é perfeito para preparar seus shakes pós-treino. Com design ergonômico e capacidade de mistura eficaz, você pode desfrutar de uma bebida deliciosa e nutritiva sempre que precisar.",
                    Quantity = 6,
                    Price = 5.99,
                    GymId = 2,
                },
                new Item
                {
                    ItemId = 8,
                    Name = "Esteira de Yoga Antiderrapante",
                    Description = "Esta esteira de yoga oferece aderência superior e conforto durante suas práticas de yoga ou alongamento. Feita com material antiderrapante de alta qualidade, é ideal para uso em casa ou no estúdio.",
                    Quantity = 6,
                    Price = 7.99,
                    GymId = 2,
                },
                new Item
                {
                    ItemId = 9,
                    Name = "Luvas de Levantamento de Peso Acolchoadas",
                    Description = "Proteja suas mãos durante os exercícios de levantamento de peso com estas luvas acolchoadas. Feitas com material durável e almofadado, proporcionam aderência e suporte, permitindo que você se concentre em seu treino.",
                    Quantity = 6,
                    Price = 9.99,
                    GymId = 2,
                },
                new Item
                {
                    ItemId = 10,
                    Name = "Garrafa de Água Isolada de Aço Inoxidável",
                    Description = "Mantenha-se hidratado durante o dia com esta garrafa de água isolada. Feita de aço inoxidável durável e com isolamento a vácuo, mantém suas bebidas frias por até 24 horas ou quentes por até 12 horas.",
                    Quantity = 6,
                    Price = 12.99,
                    GymId = 2,
                },
                new Item
                {
                    ItemId = 11,
                    Name = "Faixas de Resistência de Látex",
                    Description = "Aumente a intensidade do seu treino com estas faixas de resistência de látex. Disponíveis em diferentes níveis de resistência, são ideais para exercícios de fortalecimento muscular, alongamento e reabilitação.",
                    Quantity = 6,
                    Price = 14.99,
                    GymId = 2,
                },
                new Item
                {
                    ItemId = 12,
                    Name = "Pulseira Inteligente de Monitoramento de Atividade",
                    Description = "Esta pulseira inteligente rastreia sua atividade diária, frequência cardíaca, qualidade do sono e muito mais. Com design elegante e conectividade Bluetooth, é o companheiro perfeito para ajudá-lo a alcançar seus objetivos de fitness.",
                    Quantity = 6,
                    Price = 17.99,
                    GymId = 2,
                }
            );
            modelBuilder.Entity<GymMachine>().HasData(
               new GymMachine
               {
                   MachineId = 1,
                   Name = "Esteira Elétrica",
                   Description = "Perfeita para corridas intensas e caminhadas, com inclinação ajustável e monitor de ritmo cardíaco integrado.",
                   Quantity = 2,
                   GymId = 1,
                   ImageId = 7
               },
                new GymMachine
                {
                    MachineId = 2,
                    Name = "Bicicleta Ergométrica",
                    Description = "Proporciona um treino cardiovascular eficaz, com resistência ajustável e tela LCD para acompanhar o progresso do treino.",
                    Quantity = 2,
                    GymId = 1,
                    ImageId = 8
                },
                new GymMachine
                {
                    MachineId = 3,
                    Name = "Máquina de Remo",
                    Description = "Ótima para exercícios de cardio e fortalecimento muscular, com ajuste de resistência e monitor de desempenho integrado.",
                    Quantity = 2,
                    GymId = 1,
                    ImageId = 9
                },
                new GymMachine
                {
                    MachineId = 4,
                    Name = "Esteira Elétrica",
                    Description = "Perfeita para corridas intensas e caminhadas, com inclinação ajustável e monitor de ritmo cardíaco integrado.",
                    Quantity = 2,
                    GymId = 2,
                },
                new GymMachine
                {
                    MachineId = 5,
                    Name = "Bicicleta Ergométrica",
                    Description = "Proporciona um treino cardiovascular eficaz, com resistência ajustável e tela LCD para acompanhar o progresso do treino.",
                    Quantity = 2,
                    GymId = 2,
                },
                new GymMachine
                {
                    MachineId = 6,
                    Name = "Máquina de Remo",
                    Description = "Ótima para exercícios de cardio e fortalecimento muscular, com ajuste de resistência e monitor de desempenho integrado.",
                    Quantity = 2,
                    GymId = 2,
                }
            );
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    ExerciseId = 1,
                    Name = "Corrida Inclinada",
                    Description = "Desafie-se com uma corrida em uma esteira inclinada para um treino cardiovascular intenso.",
                    MachineId = 1,
                    ImageId = 10
                },
                new Exercise
                {
                    ExerciseId = 2,
                    Name = "Caminhada Moderada",
                    Description = "Um exercício de caminhada relaxante para melhorar a saúde cardiovascular e queimar calorias.",
                    MachineId = 1,
                    ImageId = 11
                },
                new Exercise
                {
                    ExerciseId = 3,
                    Name = "Ciclismo de Resistência",
                    Description = "Experimente um treino de bicicleta estável com resistência ajustável para fortalecer as pernas e queimar gordura.",
                    MachineId = 2,
                    ImageId = 12
                },
                new Exercise
                {
                    ExerciseId = 4,
                    Name = "Treino Intervalado de Bicicleta",
                    Description = "Alternância entre períodos de alta e baixa intensidade em uma bicicleta ergométrica para melhorar o condicionamento físico.",
                    MachineId = 2,
                    ImageId = 13
                },
                new Exercise
                {
                    ExerciseId = 5,
                    Name = "Remada Longa e Lenta",
                    Description = "Execute movimentos de remo controlados em uma máquina de remo para trabalhar todo o corpo e melhorar a resistência.",
                    MachineId = 3,
                    ImageId = 14
                },
                new Exercise
                {
                    ExerciseId = 6,
                    Name = "Remada Intensa",
                    Description = "Aumente a resistência e a velocidade para um treino de remo de alta intensidade que desafia o corpo e queima calorias.",
                    MachineId = 3,
                    ImageId = 15
                },
                new Exercise
                {
                    ExerciseId = 7,
                    Name = "Corrida Inclinada",
                    Description = "Desafie-se com uma corrida em uma esteira inclinada para um treino cardiovascular intenso.",
                    MachineId = 4,
                },
                new Exercise
                {
                    ExerciseId = 8,
                    Name = "Caminhada Moderada",
                    Description = "Um exercício de caminhada relaxante para melhorar a saúde cardiovascular e queimar calorias.",
                    MachineId = 4,
                },
                new Exercise
                {
                    ExerciseId = 9,
                    Name = "Ciclismo de Resistência",
                    Description = "Experimente um treino de bicicleta estável com resistência ajustável para fortalecer as pernas e queimar gordura.",
                    MachineId = 5,
                },
                new Exercise
                {
                    ExerciseId = 10,
                    Name = "Treino Intervalado de Bicicleta",
                    Description = "Alternância entre períodos de alta e baixa intensidade em uma bicicleta ergométrica para melhorar o condicionamento físico.",
                    MachineId = 5,
                },
                new Exercise
                {
                    ExerciseId = 11,
                    Name = "Remada Longa e Lenta",
                    Description = "Execute movimentos de remo controlados em uma máquina de remo para trabalhar todo o corpo e melhorar a resistência.",
                    MachineId = 6,
                },
                new Exercise
                {
                    ExerciseId = 12,
                    Name = "Remada Intensa",
                    Description = "Aumente a resistência e a velocidade para um treino de remo de alta intensidade que desafia o corpo e queima calorias.",
                    MachineId = 6,
                }
            );

            //payments
            modelBuilder.Entity<BankAccount>().HasData(
                new Models.Payment.BankAccount
                {
                    BankAccountId = 1,
                    GymId = 1,
                    GymSubscriptionPrice = 40,
                    StripePlanId = "plan_Prg8MwDUJOj1e3",
                    StripeBankId = "acct_1Ow4beRpgJmjBFH7",
                },
                new Models.Payment.BankAccount
                {
                    BankAccountId = 2,
                    GymId = 2,
                    GymSubscriptionPrice = 25,
                    StripePlanId = "plan_Pld1tSPdt0Qqid",
                    StripeBankId = "acct_1Ow4bhRs2igcN0GT",
                }
            );


            modelBuilder.Entity<DebitCard>().HasData(
                new DebitCard
                {
                    DebitCardId = 1,
                    StripeCustomerId = "cus_PlbprNroroGeFr",
                    StripePaymentMethodId = "pm_1Ow57fRseCm2355tph3WmrRE",
                    ClientId = 1,
                },
                new DebitCard
                {
                    DebitCardId = 2,
                    StripeCustomerId = "cus_Plbp64oawQ62uP",
                    StripePaymentMethodId = "pm_1Ow58dRseCm2355tiOre9byR",
                    ClientId = 2,
                },
                new DebitCard
                {
                    DebitCardId = 3,
                    StripeCustomerId = "cus_Plbp2N5j4PKj3t",
                    StripePaymentMethodId = "pm_1Ow59XRseCm2355te3uzVWWf",
                    ClientId = 3,
                },
                new DebitCard
                {
                    DebitCardId = 7,
                    StripeCustomerId = "cus_PlbpHhHFVJJZgn",
                    StripePaymentMethodId = "pm_1Ow5AORseCm2355tQ25Jge06",
                    ClientId = 7,
                },
                new DebitCard
                {
                    DebitCardId = 8,
                    StripeCustomerId = "cus_PlbpUx559W0bqL",
                    StripePaymentMethodId = "pm_1Ow5AyRseCm2355tgHg7cyRG",
                    ClientId = 8,
                },
                new DebitCard
                {
                    DebitCardId = 9,
                    StripeCustomerId = "cus_Plbpb7N2yNwzgm",
                    StripePaymentMethodId = "pm_1Ow5BaRseCm2355thRNhZIui",
                    ClientId = 9,
                }
            );
            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    BuyableId = 1,
                    StripeSubscriptionId = "sub_1P1wmYRseCm2355teKZtrJLj",
                    BankAccountId = 1,
                    DebitCardId = 1
                },
                new Subscription
                {
                    BuyableId = 2,
                    StripeSubscriptionId = "sub_1P1wmbRseCm2355tRI7Ee9iR",
                    BankAccountId = 1,
                    DebitCardId = 2
                },
                new Subscription
                {
                    BuyableId = 3,
                    StripeSubscriptionId = "sub_1P1wmeRseCm2355t542tsUZV",
                    BankAccountId = 1,
                    DebitCardId = 3
                }
            );
            modelBuilder.Entity<PaymentDetails>().HasData(
                new PaymentDetails
                {
                    PaymentDetailsId = 1,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Subscription 1",
                    Status = Status.SUBSCRIPTION,
                    BuyableId = 1

                },
                new PaymentDetails
                {
                    PaymentDetailsId = 2,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Subscription 2",
                    Status = Status.SUBSCRIPTION,
                    BuyableId = 2

                },
                new PaymentDetails
                {
                    PaymentDetailsId = 3,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    Description = "Subscription 3",
                    Status = Status.SUBSCRIPTION,
                    BuyableId = 3

                }
            );
            modelBuilder.Entity<Plan>().HasData(
                    new Plan
                    {
                        PlanId = 1,
                        Title = "Bulking Plan",
                        Description = "Este plano nutricional foi projetado para ajudá-lo a aumentar a massa muscular e ganhar peso de forma saudável. Inclui uma variedade de alimentos ricos em proteínas e carboidratos para apoiar o crescimento muscular.",
                        PlanType = PlanType.NUTRITION,
                        ClientId = 1
                    },
                    new Plan
                    {
                        PlanId = 2,
                        Title = "Bulking Plan",
                        Description = "Este plano de exercícios foi desenvolvido para maximizar o ganho de massa muscular e força. Inclui uma combinação de exercícios de levantamento de peso e treinamento de resistência para estimular o crescimento muscular.",
                        PlanType = PlanType.EXERCISE,
                        ClientId = 1
                    }
                );

            modelBuilder.Entity<PlanMeal>().HasData(
                new PlanMeal
                {
                    PlanItemId = 1,
                    PlanId = 1,
                    Name = "Refeição Matinal",
                    Description = "Esta refeição é especialmente formulada para fornecer uma dose nutritiva e energética para começar o dia com energia. Inclui uma mistura equilibrada de proteínas, carboidratos e gorduras saudáveis para sustentar suas atividades matinais.",
                },
                new PlanMeal
                {
                    PlanItemId = 2,
                    PlanId = 1,
                    Name = "Refeição Pós-Treino",
                    Description = "Esta refeição é projetada para ajudar na recuperação muscular e reabastecer o corpo após um treino intenso. Rica em proteínas de alta qualidade e carboidratos de rápida absorção, esta refeição promove a regeneração muscular e a reposição de energia.",
                },
                new PlanMeal
                {
                    PlanItemId = 3,
                    PlanId = 1,
                    Name = "Refeição Noturna",
                    Description = "Esta refeição noturna é projetada para promover a recuperação muscular durante o sono e manter um metabolismo saudável durante a noite. Contendo nutrientes essenciais e de digestão lenta, esta refeição ajuda a manter a saciedade e suporta o reparo muscular durante o descanso.",
                }
            );

            modelBuilder.Entity<PlanExercise>().HasData(
                new PlanExercise
                {
                    PlanItemId = 4,
                    PlanId = 2,
                    Name = "Treino de Força",
                    Description = "Este exercício de treino de força é focado no desenvolvimento muscular e na melhoria da força e resistência. Incorporando uma variedade de movimentos e técnicas de levantamento de peso, este treino visa fortalecer os principais grupos musculares do corpo.",
                    ExerciseId = 1
                },
                new PlanExercise
                {
                    PlanItemId = 5,
                    PlanId = 2,
                    Name = "Cardio Intenso",
                    Description = "Este exercício de cardio intenso é projetado para elevar sua frequência cardíaca e queimar calorias de forma eficaz. Com uma combinação de movimentos aeróbicos de alta intensidade, este treino melhora a capacidade cardiovascular e promove a perda de peso.",
                    ExerciseId = 1
                },
                new PlanExercise
                {
                    PlanItemId = 6,
                    PlanId = 2,
                    Name = "Alongamento e Flexibilidade",
                    Description = "Este exercício de alongamento e flexibilidade é ideal para melhorar a amplitude de movimento, reduzir a rigidez muscular e prevenir lesões. Concentrando-se em esticar os principais grupos musculares, este treino ajuda a aumentar a mobilidade e a flexibilidade geral do corpo.",
                    ExerciseId = 2
                }
            );

        }
    }

}