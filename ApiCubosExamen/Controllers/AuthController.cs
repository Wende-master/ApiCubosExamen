using ApiCubosExamen.Helpers;
using ApiCubosExamen.Models;
using ApiCubosExamen.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCubosExamen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperActionService helper;

        public AuthController(RepositoryCubos repo, HelperActionService helper)
        {
            this.repo = repo;
            this.helper = helper;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            UsuariosCubo usuario = await this.repo.LoginAsync(model.UserName, model.Password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                   new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                string json =
                    JsonConvert.SerializeObject(model);
                Claim[] claims = new[]
                {
                    new Claim("UserData", json)
                };

                JwtSecurityToken token = new JwtSecurityToken(
                   claims: claims,
                   issuer: this.helper.Issuer,
                   audience: this.helper.Audience,
                   signingCredentials: credentials,
                   expires: DateTime.UtcNow.AddMinutes(30),
                   notBefore: DateTime.UtcNow
                   );
                
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler().WriteToken(token)
                    });

            }
        }

    }
}
