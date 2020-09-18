using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using REST_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ArticleContext(
                serviceProvider.GetRequiredService<DbContextOptions<ArticleContext>>()))
            {
                // Look for any data.
                if (context.User.Count() > 0)
                {
                    return;   // DB has been populated, do not change data.
                }

                /*
                 * DECLARE USERS
                 */
                User user1 = new User
                {
                    UserName = "JSON",
                    PassWord = "Jason"
                };

                User user2 = new User
                {
                    UserName = "Naruto",
                    PassWord = "Uzumaki"
                };

                // Add items to database so we have user data to work with.
                context.User.AddRange(user1, user2);
                context.SaveChanges();

                /*
                 * DECLARE ARTICLES
                 */
                Article article1 = new Article()
                {
                    UserID = context.User.FirstOrDefault().UserID,
                    CreatedDate = DateTime.UtcNow,
                    Title = "Procastination",
                    Introduction = "Why do we spend time doing other things unrelated to the matter?"
                };
                context.Article.Add(article1);
                context.SaveChanges();

                /*
                 * DECLARE FIELDS FOR ARTICLE1
                 */
                ArticleField field1 = new ArticleField
                {
                    ArticleID = context.Article.FirstOrDefault().UserID,
                    Name = "Why do we Procastinate?",
                    Value = "We may spend this procastination time doing something else which is of interest or maybe more productive. Like for example, at the time of writing this article, I spend like 80% of the time I should do this playing Minecraft instead."
                };

                ArticleField field2 = new ArticleField
                {
                    ArticleID = context.Article.FirstOrDefault().UserID,
                    Name = "Is Procastination good?",
                    Value = "It depends, if we waste too much time procastinating, it can be bad, but it is almost always unavoidable especially when due dates are days away. This is where last minute work happens to some people."
                };

                context.ArticleField.Add(field1);
                context.ArticleField.Add(field2);

                context.SaveChanges();
            }
        }
    }
}
