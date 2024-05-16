using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Models;

namespace MiniMesTrainApi.Persistance
{
    public class MiniProductionDbContext(DbContextOptions options) : DbContext(options)
    {
        public virtual DbSet<Machine> Machines { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Process> Processes { get; set; } = null!;
        public virtual DbSet<ProcessParameter> ProcessParameters { get; set; } = null!;
        public virtual DbSet<Parameter> Parameters { get; set; } = null!;
        public virtual DbSet<MachineParameter> MachineParameter { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Machine>(entity =>
            {
                entity.ToTable("Machines", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.MachineId);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId);
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.ToTable("Processes", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SerialNumber).HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Processes)
                    .HasForeignKey(d => d.OrderId);
            });

            modelBuilder.Entity<ProcessParameter>(entity =>
            {
                entity.ToTable("ProcessParameters", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.ProcessParameters)
                    .HasForeignKey(d => d.ProcessId);

                entity.HasOne(d => d.Parameter)
                    .WithMany(p => p.ProcessParameters)
                    .HasForeignKey(d => d.ParameterId);
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.ToTable("Parameters", "MiniMes");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Unit).HasMaxLength(50);
            });

            modelBuilder.Entity<MachineParameter>(entity =>
            {
                entity.ToTable("MachineParameters", "Minimes"); 

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.MachineParameter)
                    .HasForeignKey(d => d.MachineId);
                entity.HasOne(d => d.Parameter)
                    .WithMany(p => p.MachineParameter)
                    .HasForeignKey(d => d.ParameterId);
            }); 
        }
    }
}
