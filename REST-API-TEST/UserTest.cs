using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using REST_API.Controllers;
using REST_API.Data;
using REST_API.DTO;
using REST_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace REST_API_TEST
{
    public class UserTest : TestBase
    {
        // Someone created a new account. (Tests GET and POST methods)
        [Test]
        public async Task CreateUser()
        {
            using (var context = new ArticleContext(options))
            {
                BasicAuthenticateDTO NewUser = new BasicAuthenticateDTO
                {
                    UserName = "Hinata",
                    PassWord = "Hyuga"
                };

                UsersController controller = new UsersController(context);
                await controller.CreateAccount(NewUser);

                // Check object exists.
                var results = await controller.GetUser("Hinata");
                Assert.IsNotNull(results.Value);

                // Attempt to login to new account.
                var results2 = await controller.Login(NewUser);
                Assert.IsInstanceOf<OkObjectResult>(results2);
            }
        }

        // Password reset functionality test.
        [Test]
        public async Task ChangeUserPassword()
        {
            using (var context = new ArticleContext(options))
            {
                UsersController controller = new UsersController(context);
                PasswordResetDTO Password = new PasswordResetDTO
                {
                    OldPassWord = "Usumaki",
                    NewPassWord = "Hyuga"
                };

                // Should return BadRequest as password is wrong.
                var response = await controller.ResetPassword("Naruto", Password);
                Assert.IsInstanceOf<BadRequestObjectResult>(response);

                Password = new PasswordResetDTO
                {
                    OldPassWord = "Uzumaki",
                    NewPassWord = "Haruno"
                };

                // Should return NoContent meaning password change should be successful.
                response = await controller.ResetPassword("Naruto", Password);
                Assert.IsInstanceOf<NoContentResult>(response);

                BasicAuthenticateDTO user = new BasicAuthenticateDTO
                {
                    UserName = "Naruto",
                    PassWord = "Haruno"
                };

                // Should be able to login with new password.
                response = await controller.Login(user);
                Assert.IsInstanceOf<OkObjectResult>(response);
            }
        }

        // Check that user can be deleted correctly.
        [Test]
        public async Task DeleteUser()
        {
            using (var context = new ArticleContext(options))
            {
                UsersController controller = new UsersController(context);

                // Attempt to delete user using wrong password.
                var response = await controller.DeleteUser("JSON", "Haruno");
                Assert.IsInstanceOf<BadRequestObjectResult>(response);

                // Now using the correct password.
                response = await controller.DeleteUser("JSON", "Jason");
                Assert.IsInstanceOf<OkObjectResult>(response);

                // Check user does not exist in database.
                BasicAuthenticateDTO user = new BasicAuthenticateDTO
                {
                    UserName = "JSON",
                    PassWord = "Jason"
                };
                response = await controller.Login(user);
                Assert.IsInstanceOf<NotFoundResult>(response);
            }
        }
    }
}
