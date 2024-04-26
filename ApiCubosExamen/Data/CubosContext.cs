using ApiCubosExamen.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCubosExamen.Data
{
    public class CubosContext : DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options) : base(options) { }
        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<CompraCubo> CompraCubos { get; set; }
        public DbSet<UsuariosCubo> usuariosCubos { get; set; }
    }
}
