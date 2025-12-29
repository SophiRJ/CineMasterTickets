using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Data
{
    //Heredamos de IdentityDbContext 
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //Creación de los DBSets para las tablas de BBDD
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketSeat> TicketSeats { get; set; }
        public DbSet<SessionSeat> SessionSeats { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<BoxOffice> BoxOffices { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<TicketAddOn> TicketAddOns { get; set; }

        //Ahora definimos las relaciones entre los modelos para mayor refuerzo de la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------------
            //  Relaciones Uno-a-Muchos
            // -----------------------------
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Rows)
                .WithOne(row => row.Room)
                .HasForeignKey(row => row.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Row>()
                .HasMany(row => row.Seats)
                .WithOne(seat => seat.Row)
                .HasForeignKey(seat => seat.RowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Sessions)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Sessions)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Session)
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------
            // Relaciones Muchos-a-Muchos
            // -----------------------------

            // TicketSeat
            modelBuilder.Entity<TicketSeat>()
                .HasKey(ts => new { ts.TicketId, ts.SeatId, ts.SessionId });

            modelBuilder.Entity<TicketSeat>()
                .HasOne(ts => ts.Ticket)
                .WithMany(t => t.TicketSeats)
                .HasForeignKey(ts => ts.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketSeat>()
                .HasOne(ts => ts.Seat)
                .WithMany(s => s.TicketSeats)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketSeat>()
                .HasOne(ts => ts.Session)
                .WithMany()
                .HasForeignKey(ts => ts.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // SessionSeat
            modelBuilder.Entity<SessionSeat>()
                .HasKey(ss => new { ss.SessionId, ss.SeatId });

            modelBuilder.Entity<SessionSeat>()
                .HasOne(ss => ss.Session)
                .WithMany(s => s.SessionSeats)
                .HasForeignKey(ss => ss.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SessionSeat>()
                .HasOne(ss => ss.Seat)
                .WithMany(s => s.SessionSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketAddOn
            modelBuilder.Entity<TicketAddOn>()
                .HasKey(ta => new { ta.TicketId, ta.AddOnId });

            modelBuilder.Entity<TicketAddOn>()
                .HasOne(ta => ta.Ticket)
                .WithMany(t => t.TicketAddOns)
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TicketAddOn>()
                .HasOne(ta => ta.AddOn)
                .WithMany(a => a.TicketAddOns)
                .HasForeignKey(ta => ta.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddOn>()
        .Property(a => a.Price)
        .HasPrecision(18, 2);

            modelBuilder.Entity<Session>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.TotalPrice)
                .HasPrecision(18, 2);

            // -----------------------------
            // Índices únicos
            // -----------------------------
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.RowId, s.Number })
                .IsUnique();

            // -----------------------------
            // Seed de Rooms, Rows y Seats
                //La aplicación lanzara por defecto 10 salas con sus butacas, y los puestos de trabajo predefinidos
            // -----------------------------
            var rooms = new List<Room>();
            for (int i = 1; i <= 10; i++)
            {
                rooms.Add(new Room
                {
                    RoomId = i,
                    Capacity = 200
                });
            }
            modelBuilder.Entity<Room>().HasData(rooms);

            var filas = new List<Row>();
            int filaId = 1;
            foreach (var room in rooms)
            {
                for (char rowName = 'A'; rowName <= 'J'; rowName++)
                {
                    filas.Add(new Row
                    {
                        RowId = filaId++,
                        RoomId = room.RoomId,
                        Name = rowName
                    });
                }
            }
            modelBuilder.Entity<Row>().HasData(filas);

            var seats = new List<Seat>();
            int seatId = 1;
            foreach (var fila in filas)
            {
                for (int num = 1; num <= 20; num++)
                {
                    seats.Add(new Seat
                    {
                        SeatId = seatId++,
                        RowId = fila.RowId,
                        Number = num.ToString(),
                        Available = true
                    });
                }
            }
            modelBuilder.Entity<Seat>().HasData(seats);

            // -----------------------------
            // Seed de BoxOffices
            // -----------------------------
            var boxOffices = new List<BoxOffice>
                {
                    new BoxOffice { BoxOfficeId = 1, BoxOfficeName = "Taquilla Principal - Entrada" },
                    new BoxOffice { BoxOfficeId = 2, BoxOfficeName = "Taquilla Lateral - Parking" },
                    new BoxOffice { BoxOfficeId = 3, BoxOfficeName = "Taquilla VIP - Planta 1" },
                    new BoxOffice { BoxOfficeId = 4, BoxOfficeName = "Taquilla Online - Recogida" },
                    new BoxOffice { BoxOfficeId = 5, BoxOfficeName = "Taquilla Express - Kiosko" }
                };

            modelBuilder.Entity<BoxOffice>().HasData(boxOffices);
        }
    }
    
}
