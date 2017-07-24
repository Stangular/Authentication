using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthUser;

namespace UserRegistration.Controllers
{
    [Produces("application/json")]
    [Route("api/registration")]
    public class RegistrationController : Controller
    {
        private readonly UserManager<AUser> _userManager;
        private readonly SignInManager<AUser> _signInManager;
        private ILogger<RegistrationController> _logger;

        public RegistrationController(UserManager<AUser> userManager,
              SignInManager<AUser> signInManager,
                ILogger<RegistrationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: api/registration
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new string[] { "value1sss", "value2222" });
        }

        // GET: api/registration/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            return Ok(new string[] { "value1", "value2" });
        }

        // POST: api/registration
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try{
                    var user = new AUser() { UserName = loginViewModel.UserName };
                    var result = await _userManager.CreateAsync(user, loginViewModel.Password);
                    //      var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if( result.Succeeded)
                    {
                        foreach(string c in loginViewModel.Claims )
                        {
                            var claimResult = await _userManager.AddClaimAsync(user, new Claim(c, "True"));
                            if(!claimResult.Succeeded)
                            {
                                LogMessage("Claim failed: {c}");
                                break;
                            }
                        }
                        return Ok();
                    }

                }
                catch ( System.Exception ex)
                {
                    LogMessage("Exception thrown while Registering User  in: {ex}");
                }
            }
            return BadRequest("Failed to register");
        }

        // PUT: api/registration/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private void LogMessage(string message)
        {
            if (null != _logger)
            {
                _logger.LogError(message);
            }
        }
    }
}
