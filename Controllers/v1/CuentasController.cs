using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using webAPIAutores.DTOs;
using webAPIAutores.Servicios;

namespace webAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_ultra_mega_secreto");
        }

        [HttpPost]
        [Route("ingresar", Name ="ingresar")]
        public async Task<ActionResult<AutenticationResponse>> Ingresar(UsserCredential usserCredential)
        {
            var resultado = await signInManager.PasswordSignInAsync(usserCredential.Email, usserCredential.PassWord, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await CrearToken(usserCredential);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        //Porque siempre inyectamos la dependencia por el constructor?, Can i create and asing instances from endpoints which aren't ctors?  
        [HttpPost]
        [Route("registrar", Name ="registrar")]
        public async Task<ActionResult<AutenticationResponse>> Registrar(UsserCredential usserCredential)
        {

            var usuario = new IdentityUser { UserName = usserCredential.Email, Email = usserCredential.Email };

            var resultado = await userManager.CreateAsync(usuario, usserCredential.PassWord);

            if (resultado.Succeeded)
            {
                return await CrearToken(usserCredential);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpGet("RenovarToken", Name ="renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AutenticationResponse>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new UsserCredential
            {
                Email = email
            };

            return await CrearToken(credencialesUsuario);
        }
        private async Task<AutenticationResponse> CrearToken(UsserCredential usserCredential)
        {
            var claims = new List<Claim>
                {
                    new Claim("email", usserCredential.Email),
                    //new Claim("cancion", "Tú no mete cabra tú no lúce")
                };

            var usuario = await userManager.FindByEmailAsync(usserCredential.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new AutenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiracion
                //what if variables instead propierties
            };


        }
        [HttpPost("HacerAdmin", Name ="hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("joderSoyAdmin", "No importa"));
            return NoContent();

        }
        [HttpPost("RemoverAdmin", Name ="removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("joderSoyAdmin", "No importa"));
            return NoContent();

        }
         [HttpPost("RemoverClaims", Name ="removerClaims")]
        public async Task<ActionResult> RemoverClaims(EditarClaims editarClaims)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaims.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdminNoBulto", "joderSoyAdmin"));
            return NoContent();

        }
    }
}
