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
    class ArticleFieldTest : TestBase
    {
        // Attempt to add a field to an article.
        [Test]
        public async Task CreateAnExtraField()
        {
            using (var context = new ArticleContext(options))
            {
                ArticleFieldsController controller = new ArticleFieldsController(context);

                // Attempt to add a field with wrong password, but correct author.
                ArticleFieldCreateTemplateDTO NewField = new ArticleFieldCreateTemplateDTO
                {
                    ArticleID = 1,
                    UserName = "JSON",
                    PassWord = "Json",
                    Name = "When do we Procastinate?",
                    Value = "Most likely when you want to take a break."
                };
                var response = await controller.PostArticleField(NewField);
                Assert.IsNull(response.Result as CreatedAtActionResult);

                // Now with correct credentials.
                NewField.PassWord = "Jason";
                response = await controller.PostArticleField(NewField);
                Assert.IsNotNull(response.Result as CreatedAtActionResult);
            }
        }

        [Test]
        public async Task EditAnExistingField()
        {
            using (var context = new ArticleContext(options))
            {
                ArticleFieldsController controller = new ArticleFieldsController(context);

                // Edit the first field of the article with the wrong credentials.
                ArticleFieldEditTemplateDTO ModField = new ArticleFieldEditTemplateDTO
                {
                    FieldID = 1,
                    UserName = "JSON",
                    PassWord = "Json",
                    Name = "When do we Procastinate?",
                    Value = "Most likely when you want to take a break."
                };

                var response = await controller.PutArticleField(ModField);
                var result = (StatusCodeResult)response;
                Assert.AreEqual(result.StatusCode, 403);

                // Now with correct credentials.
                ModField.PassWord = "Jason";
                response = await controller.PutArticleField(ModField);
                result = (StatusCodeResult)response;
                Assert.AreEqual(result.StatusCode, 204);
            }
        }
    }
}
