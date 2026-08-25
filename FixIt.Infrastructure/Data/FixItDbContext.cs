using FixIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Data;

public class FixItDbContext : DbContext
{
    public FixItDbContext(DbContextOptions<FixItDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<PrestadorCategoria> PrestadorCategorias => Set<PrestadorCategoria>();
    public DbSet<Orden> Ordenes => Set<Orden>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    public DbSet<Mensaje> Mensajes => Set<Mensaje>();
    public DbSet<FotoTrabajo> FotosTrabajo => Set<FotoTrabajo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- Usuario ----
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Rol);
            entity.HasIndex(u => u.UbicacionGeo).HasMethod("GIST"); // índice espacial para búsquedas por radio
            entity.Property(u => u.UbicacionGeo).HasColumnType("geography (point)");
            entity.Property(u => u.Rol)
                .HasConversion<string>() // guarda el enum como texto legible en la DB, no como número
                .HasMaxLength(20);
        });

        // ---- Categoria ----
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        // ---- PrestadorCategoria ----
        modelBuilder.Entity<PrestadorCategoria>(entity =>
        {
            entity.HasIndex(pc => new { pc.PrestadorId, pc.CategoriaId }).IsUnique();

            entity.HasOne(pc => pc.Prestador)
                .WithMany(u => u.PrestadorCategorias)
                .HasForeignKey(pc => pc.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pc => pc.Categoria)
                .WithMany(c => c.PrestadorCategorias)
                .HasForeignKey(pc => pc.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Orden ----
        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasIndex(o => o.ClienteId);
            entity.HasIndex(o => o.PrestadorId);

            entity.Property(o => o.Estado)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(o => o.MontoTotal).HasPrecision(10, 2);
            entity.Property(o => o.ComisionPlataforma).HasPrecision(10, 2);

            // Cliente y Prestador son ambos FK a Usuario, hay que indicarle a EF
            // que NO borre en cascada (si borrás un usuario, no queremos que se
            // borren automáticamente órdenes de otro usuario relacionado)
            entity.HasOne(o => o.Cliente)
                .WithMany(u => u.OrdenesComoCliente)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Prestador)
                .WithMany(u => u.OrdenesComoPrestador)
                .HasForeignKey(o => o.PrestadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---- Pago ----
        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasIndex(p => p.OrdenId).IsUnique(); // 1 a 1 con Orden

            entity.Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.Monto).HasPrecision(10, 2);

            entity.HasOne(p => p.Orden)
                .WithOne(o => o.Pago)
                .HasForeignKey<Pago>(p => p.OrdenId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Calificacion ----
        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.HasIndex(c => c.OrdenId).IsUnique(); // 1 a 1 con Orden

            entity.HasOne(c => c.Orden)
                .WithOne(o => o.Calificacion)
                .HasForeignKey<Calificacion>(c => c.OrdenId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Mensaje ----
        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasIndex(m => m.OrdenId);

            entity.HasOne(m => m.Orden)
                .WithMany(o => o.Mensajes)
                .HasForeignKey(m => m.OrdenId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Emisor)
                .WithMany(u => u.Mensajes)
                .HasForeignKey(m => m.EmisorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<FotoTrabajo>(entity =>
        {
            entity.HasIndex(f => f.PrestadorId);

            entity.HasOne(f => f.Prestador)
                .WithMany(u => u.FotosTrabajo)
                .HasForeignKey(f => f.PrestadorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}