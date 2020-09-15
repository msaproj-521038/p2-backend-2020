using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_API.Data;
using REST_API.DTO;
using REST_API.Models;

namespace REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ArticleContext _context;

        public UsersController(ArticleContext context)
        {
            _context = context;
        }

        // UserDTO is used for READ functionality as to not expose the password.
        public static readonly Expression<Func<User, UserDTO>> AsUserDTO = x => new UserDTO
        {
            UserID = x.UserID,
            UserName = x.UserName
        };

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.User.Select(AsUserDTO).ToListAsync();
        }

        // GET: api/Users/Verify
        [HttpGet("Verify")]
        public async Task<IActionResult> Login(BasicAuthenticateDTO user)
        {
            var checkUser = await _context.User.Where(p => p.UserName == user.UserName).FirstOrDefaultAsync();

            if(checkUser == null)
            {
                return NotFound();
            }

            if(checkUser.PassWord == user.PassWord)
            {
                return Ok("Successfully authenticated");
            }
            else
            {
                return BadRequest("Wrong Password!");
            }
        }

        // GET: api/Users/Username/Naruto
        [HttpGet("Username/{UserName}")]
        public async Task<ActionResult<UserDTO>> GetUser(string UserName)
        {
            var user = await _context.User.Select(AsUserDTO).Where(p => p.UserName == UserName).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/Naruto
        // Changing password feature.
        [HttpPut("{UserName}")]
        public async Task<IActionResult> ResetPassword(string UserName, PasswordResetDTO PasswordReset)
        {
            var user = await _context.User.Where(p => p.UserName == UserName).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User " + UserName + " does not exist!");
            }

            if(user.PassWord != PasswordReset.OldPassWord)
            {
                return BadRequest("Passwords do not match!");
            }

            user.PassWord = PasswordReset.NewPassWord;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // Create accounts, no 2 users can have the same usernames.
        [HttpPost]
        public async Task<ActionResult<string>> CreateAccount(BasicAuthenticateDTO user)
        {
            // Enforce unique usernames.
            if (UserExists(user.UserName))
            {
                return BadRequest("Username " + user.UserName + " is already used by someone else, please choose another.");
            }

            User NewUser = new User
            {
                UserName = user.UserName,
                PassWord = user.PassWord
            };

            _context.User.Add(NewUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = NewUser.UserID }, NewUser.UserName);
        }

        // DELETE: api/Users/Naruto
        // Must supply the correct password for the account to be deleted.
        [HttpDelete("{UserName}")]
        public async Task<IActionResult> DeleteUser(string UserName, string PassWord)
        {
            var user = await _context.User.Where(p => p.UserName == UserName).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if(PassWord != user.PassWord)
            {
                return BadRequest("Incorrect password!");
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User " + user.UserName + " has been deleted.");
        }

        private bool UserExists(string UserName)
        {
            return _context.User.Any(e => e.UserName == UserName);
        }
    }
}
