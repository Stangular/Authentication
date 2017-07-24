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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        private BadRequestObjectResult BadRequest(string userName, string message, string format = "")
        {
            var m = message;
            if (format.Length > 0)
            {
                m = String.Format(format, message);
            }
            return BadRequest(new
            {
                token = "",
                username = userName,
                expiration = DateTime.Now,
                message = m
            });
        }


        private Task<AUser> ValidateUser(string userName)
        {
            return _userMgr.FindByNameAsync(userName);
        }

        private bool ValidatePassword(AUser user, string password)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
        // POST api/values
        [HttpPost]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Post([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("", "Login credentials could not be validated.");
                }
                var user = await ValidateUser(model.UserName);
                if (user == null)
                {
                    return BadRequest(model.UserName ?? "", model.UserName, "User name {0} could not be validated");
                }
                if (!ValidatePassword(user, model.Password))
                {
                    return BadRequest(model.UserName, model.UserName, "Password for {0} could not be validated");
                }
                var userClaims = await _userMgr.GetClaimsAsync(user);

                var claims = new[]
                {
                              new Claim(JwtRegisteredClaimNames.Sub, user.UserName + ":" + user.Id),
                              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                              new Claim(JwtRegisteredClaimNames.GivenName, ""),
                              new Claim(JwtRegisteredClaimNames.FamilyName,  user.Id),
                              new Claim(JwtRegisteredClaimNames.Email, model.EMail)
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
                    username = model.UserName,
                    expiration = token.ValidTo,
                    message = "You have successfully logged in, " + user.UserName
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }
            return BadRequest("", "Failed to generate token for unknown reasons.");
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
