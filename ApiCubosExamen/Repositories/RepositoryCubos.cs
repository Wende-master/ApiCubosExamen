using ApiCubosExamen.Data;
using ApiCubosExamen.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCubosExamen.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos
                .ToListAsync();
        }

        public async Task<List<Cubo>> FindCubosbyMarca(string marca)
        {
            var consulta = this.context.Cubos.
                Where(z => z.Maca == marca).ToListAsync();
            return await consulta;
        }

        public async Task<UsuariosCubo> LoginAsync(string email, string pass)
        {
            return await this.context.usuariosCubos
                .Where(z => z.Email == email && z.Password == pass).FirstOrDefaultAsync();
        }

        public async Task<List<CompraCubo>> GetPedidosUsuario(int iduser)
        {
          return await this.context.CompraCubos
                .Where(x => x.IdUsuario == iduser).ToListAsync();
        }
    }
}
