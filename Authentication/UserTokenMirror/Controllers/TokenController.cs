using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using AuthUser;

namespace UserTokenMirror.Controllers
{
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private UserIdentityContext _context;
        private ILogger<TokenController> _logger;
        private SignInManager<AUser> _signInMgr;
        private UserManager<AUser> _userMgr;
        private IPasswordHasher<AUser> _hasher;
        private IConfigurationRoot _config;

        public TokenController(UserIdentityContext context,
               SignInManager<AUser> signInMgr,
               UserManager<AUser> userMgr,
               IPasswordHasher<AUser> hasher,
               ILogger<TokenController> logger,
               IConfigurationRoot config)
        {
            _context = context;
            _signInMgr = signInMgr;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
        }

        // GET api/values
        [HttpGet]
        [EnableCors("MyPolicy")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [EnableCors("MyPolicy")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Post([FromBody] LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userMgr.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                        {
                            var userClaims = await _userMgr.GetClaimsAsync(user);

                            var claims = new[]
                            {
              new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.GivenName, "Stanley"),
              new Claim(JwtRegisteredClaimNames.FamilyName, "Shannon"),
              new Claim(JwtRegisteredClaimNames.Email, "ssshannon1026@gmail.com")
            }.Union(userClaims);

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                              issuer: _config["Tokens:Issuer"],
                              audience: _config["Tokens:Audience"],
                              claims: claims,
                              expires: DateTime.UtcNow.AddMinutes(50),
                              signingCredentials: creds
                              );

                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expiration = token.ValidTo
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }
            return BadRequest("Failed to generate token");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [EnableCors("MyPolicy")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [EnableCors("MyPolicy")]
        public void Delete(int id)
        {
        }
    }
}
