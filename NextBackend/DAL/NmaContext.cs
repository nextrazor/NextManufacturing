using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Configuration.Json;

namespace NextBackend.DAL
{
    /// <summary>
    /// Контекст базы NextApsMes
    /// </summary>
    public class NmaContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<CalendarState> CalendarStates { get; set; } = null!;
        public DbSet<CalendarTemplate> CalendarTemplates { get; set; } = null!;
        public DbSet<CalendarTemplateSpan> CalendarTemplateSpans { get; set; } = null!;
        public DbSet<ResourceCalendar> ResourceCalendars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            dbContextOptionsBuilder.UseNpgsql(configuration.GetConnectionString("NmaDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Relations

            modelBuilder.Entity<CalendarTemplate>()
                .HasOne(cs => cs.DefaultState)
                .WithMany()
                .HasForeignKey(cs => cs.DefaultStateGuid);

            modelBuilder.Entity<CalendarTemplateSpan>()
                .HasOne(cts => cts.CalendarTemplate)
                .WithMany(ct => ct.Spans)
                .HasForeignKey(cts => cts.CalendarTemplateGuid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CalendarTemplateSpan>()
                .HasOne(cts => cts.State)
                .WithMany()
                .HasForeignKey(cts => cts.StateGuid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourceCalendar>()
                .HasOne(rc => rc.Resource)
                .WithMany(r => r.CalendarSpans)
                .HasForeignKey(rc => rc.ResourceGuid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourceCalendar>()
                .HasOne(rc => rc.CalendarTemplate)
                .WithMany()
                .HasForeignKey(rc => rc.CalendarTemplateGuid);

            #endregion

            #region Unique indexes and constraints

            modelBuilder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<CalendarState>()
                .HasIndex(cs => cs.Name)
                .IsUnique();

            modelBuilder.Entity<CalendarTemplate>()
                .HasIndex(ct => ct.Name)
                .IsUnique();

            modelBuilder.Entity<CalendarTemplateSpan>()
                .HasCheckConstraint("CK_CalendarTemplates_FromTo", "from_time < to_time");

            modelBuilder.Entity<ResourceCalendar>()
                .HasIndex(rc => new { rc.ResourceGuid, rc.FromTime })
                .IsUnique();

            #endregion

            #region Data seed

            var resources = new Resource[]
            {
                new Resource()
                {
                    Guid = Guid.Parse("7BB90A66-F18B-4ACC-AE91-49D535E0D189"),
                    Name = "Станок токарный (инв. 120)"
                },
                new Resource()
                {
                    Guid = Guid.Parse("9E1D6EC8-5875-4DD7-B0C3-8134006D25A1"),
                    Name = "Станок токарный (инв. 122)"
                },
                new Resource()
                {
                    Guid = Guid.Parse("BBFCF611-5234-4F61-9502-991E340006BB"),
                    Name = "Станок фрезерный (инв. 212)"
                }
            };

            var calendarStateOnShift = new CalendarState()
            {
                Guid = Guid.Parse("17C94115-07CC-4326-9AB4-66D4155CFE1A"),
                Name = "Работает"
            };
            var calendarStateOffShift = new CalendarState()
            {
                Guid = Guid.Parse("68ED1833-DE39-435F-9E7B-E87325A35AD5"),
                Name = "Перерыв"
            };

            var calendar24x7 = new CalendarTemplate()
            {
                Guid = Guid.Parse("B462C5B6-CECB-41F7-922D-A62A565A85EA"),
                Name = "Круглосуточный",
                DefaultStateGuid = calendarStateOnShift.Guid,
                PeriodDuration = TimeSpan.FromDays(1),
                ReferenceDate = new DateTime(2020, 1, 1)
            };

            var resourceCalendars = new ResourceCalendar[]
            {
                new ResourceCalendar()
                {
                    Guid = Guid.Parse("8B92DC58-6F6A-42A8-A4FF-6FDEC803D94C"),
                    ResourceGuid = resources[0].Guid,
                    CalendarTemplateGuid = calendar24x7.Guid,
                    FromTime = new DateTime(2020, 1, 1)
                },
                new ResourceCalendar()
                {
                    Guid = Guid.Parse("F4FB391B-0519-4E38-98B5-C8CE6829E893"),
                    ResourceGuid = resources[1].Guid,
                    CalendarTemplateGuid = calendar24x7.Guid,
                    FromTime = new DateTime(2020, 1, 1)
                },
                new ResourceCalendar()
                {
                    Guid = Guid.Parse("800037D5-CD8A-4D19-958E-3732B1864B91"),
                    ResourceGuid = resources[2].Guid,
                    CalendarTemplateGuid = calendar24x7.Guid,
                    FromTime = new DateTime(2020, 1, 1)
                },
            };

            modelBuilder.Entity<Resource>()
                .HasData(resources);
            modelBuilder.Entity<CalendarState>()
                .HasData(calendarStateOnShift, calendarStateOffShift);
            modelBuilder.Entity<CalendarTemplate>()
                .HasData(calendar24x7);
            modelBuilder.Entity<ResourceCalendar>()
                .HasData(resourceCalendars);

            #endregion
        }
    }
}
