using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using REST_API.Controllers;
using REST_API.Data;
using REST_API.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REST_API_TEST
{
    class ArticleTest : TestBase
    {
        // Attempt to edit the article content on different scenarios.
        [Test]
        public async Task EditArticleContent()
        {
            using (var context = new ArticleContext(options))
            {
                ArticlesController controller = new ArticlesController(context);

                // Attempt to edit the article title as a different user.
                ArticleEditTemplateDTO article = new ArticleEditTemplateDTO()
                {
                    UserName = "Naruto",
                    PassWord = "Uzumaki",
                    Title = "Why we procastinate",
                    Introduction = "Why do we spend time doing other things unrelated to the matter?"
                };

                // Ensure a forbidden status code is returned.
                var response = await controller.EditArticle(1, article);
                StatusCodeResult result = (StatusCodeResult)response;
                Assert.AreEqual(result.StatusCode, 403);

                // Attempt to edit the article title as the author with wrong password.
                article.UserName = "JSON";

                // Ensure a forbidden status code is returned because password supplied is wrong although with the correct author name.
                response = await controller.EditArticle(1, article);
                result = (StatusCodeResult)response;
                Assert.AreEqual(result.StatusCode, 403);

                // Attempt to edit the article title as the author with correct password.
                article.PassWord = "Jason";
                    
                // This time should be able to change the contents of article.
                response = await controller.EditArticle(1, article);
                result = (StatusCodeResult)response;
                Assert.AreEqual(result.StatusCode, 204);
            }
        }

        // Attempt to create an article with its fields.
        [Test]
        public async Task CreateEntireArticle()
        {
            using (var context = new ArticleContext(options))
            {
                ArticlesController controller = new ArticlesController(context);

                // Attempt to post with invalid credentials.
                ArticleEditTemplateDTO NewArticle = new ArticleEditTemplateDTO
                {
                    UserName = "Naruto",
                    PassWord = "Usumaki",
                    Title = "Why C#?",
                    Introduction = "C# is a programming language but is it really that good?"
                };
                var response = await controller.PostArticle(NewArticle);
                Assert.IsNull((response.Result as CreatedAtActionResult));

                // Now with correct authentication.
                NewArticle.PassWord = "Uzumaki";
                response = await controller.PostArticle(NewArticle);
                Assert.IsNotNull((response.Result as CreatedAtActionResult));
            }
        }
    }
}
