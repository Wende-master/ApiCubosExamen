using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace ApiCubosExamen.Helpers
{
    public class HelperActionService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionService(IConfiguration configuration)
        {
            //this.Issuer = configuration.GetValue<string>("ApiOAuth:Issuer");
            //this.Audience = configuration.GetValue<string>("ApiOAuth:Audience");
            //this.SecretKey = configuration.GetValue<string>("ApiOAuth:SecretKey");

            var keyVaultUri = configuration.GetValue<string>("ApiOAuth:keys");
            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());


            Issuer = GetSecretValue(secretClient, "Issuer");
            Audience = GetSecretValue(secretClient, "Audience");
            SecretKey = GetSecretValue(secretClient, "SecretKey");
        }

        private string GetSecretValue(SecretClient secretClient, string secretName)
        {
            try
            {
                KeyVaultSecret secret = secretClient.GetSecret(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data =
                Encoding.UTF8.GetBytes(this.SecretKey);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = this.Issuer,
                    ValidAudience = this.Audience,
                    IssuerSigningKey = this.GetKeyToken(),
                };
            });
            return options;
        }

        public Action<AuthenticationOptions> GetAuthenticateSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }

    }
}
