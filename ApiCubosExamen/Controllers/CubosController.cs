using ApiCubosExamen.Models;
using ApiCubosExamen.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiCubosExamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> GetCubos()
        {
            return await this.repo.GetCubosAsync();
        }

        [HttpGet]
        [Route("[action]/{marca}")]
        public async Task<ActionResult<List<Cubo>>> FindCubosByMarca(string marca)
        {
            List<Cubo> cubos =
                await this.repo.FindCubosbyMarca(marca);
            return Ok(cubos);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<UsuariosCubo>> PerfilUsuario()
        {
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UserData");
            string jsonUsuario = claim.Value;
            UsuariosCubo usuario = JsonConvert.DeserializeObject<UsuariosCubo>(jsonUsuario);
            return Ok(usuario);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CompraCubo>>> PedidosUsuario()
        {
            string jsonCompras = HttpContext.User.FindFirst(x => x.Type == "UserData").Value;
            UsuariosCubo usuario = JsonConvert.DeserializeObject<UsuariosCubo>(jsonCompras);
            List<CompraCubo> compras =
                await this.repo.GetPedidosUsuario(usuario.IdUsuario);
            return Ok(compras);
        }
    }
}
