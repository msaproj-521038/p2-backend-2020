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
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleContext _context;

        // Convert fields to exclude ArticleID from the result content.
        public static readonly Expression<Func<ArticleField, FieldDTO>> AsArticleFieldDTO = x => new FieldDTO
        {
            ID = x.FieldsID,
            Name = x.Name,
            Value = x.Value
        };

        public ArticlesController(ArticleContext context)
        {
            _context = context;
        }

        //// GET: api/Articles
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Article>>> GetArticle()
        //{
        //    return await _context.Article.ToListAsync();
        //}

        // GET: api/Articles/Summary
        [HttpGet("Summary")]
        public async Task<ActionResult<IEnumerable<ArticleContentSimpleDTO>>> GetArticleSummary()
        {
            var results = await _context.Article.ToListAsync();
            List<ArticleContentSimpleDTO> ArticleSummaries = new List<ArticleContentSimpleDTO>();

            // Change to Author Names from Author ID.
            results.ForEach(x =>
            {
                ArticleSummaries.Add(new ArticleContentSimpleDTO
                {
                    ArticleID = x.ArticleID,
                    Author = _context.User.Where(u => u.UserID.Equals(x.UserID)).FirstOrDefault().UserName,
                    CreatedDate = x.CreatedDate,
                    Title = x.Title,
                    Introduction = x.Introduction
                });
            });

            return Ok(ArticleSummaries);
        }

        // GET: api/Articles/Full/5
        // Since we are just sending 1 full details of this article along with the fields associated with this article.
        // Gives enough information to embed an article on a webpage.
        [HttpGet("Full/{id}")]
        public async Task<ActionResult<ArticleContentFullDTO>> GetArticle(int id)
        {
            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            var author = await _context.User.FindAsync(article.UserID);
            var fields = await _context.ArticleField.Where(p => p.ArticleID == id).Select(AsArticleFieldDTO).ToListAsync();

            ArticleContentFullDTO DTO = new ArticleContentFullDTO()
            {
                ArticleID = article.ArticleID,
                Author = author.UserName,
                CreatedDate = article.CreatedDate,
                Title = article.Title,
                Introduction = article.Introduction,
                Fields = new List<FieldDTO>(fields)
            };

            return Ok(DTO);
        }

        //[HttpGet("Summary/Author/{author}")]
        //// Search articles based on author name.
        //public async Task<ActionResult<IEnumerable<Article>>> GetArticleByAuthor(string Author)
        //{
        //    var author = await _context.User.Where(p => p.UserName.Contains(Author)).FirstAsync();

        //    if(author == null)
        //    {
        //        return BadRequest("Author with that username does not exist!");
        //    }

        //    var article = await _context.Article.Where(p => p.UserID == author.UserID).ToListAsync();

        //    if (article == null)
        //    {
        //        return NotFound("Author has not made any articles.");
        //    }

        //    return article;
        //}

        [HttpGet("Summary/Query/{query}")]
        // Search articles based on search result.
        public async Task<ActionResult<IEnumerable<ArticleContentSimpleDTO>>> GetArticleByQuery(string query)
        {
            // Ensure to check title without worrying about case sensitivity.
            var results = await _context.Article.Where(p => p.Title.ToLower().Contains(query.ToLower())).ToListAsync();

            if (results == null)
            {
                return NotFound();
            }

            List<ArticleContentSimpleDTO> ArticleSummaries = new List<ArticleContentSimpleDTO>();

            // Change to Author Names from Author ID.
            results.ForEach(x =>
            {
                ArticleSummaries.Add(new ArticleContentSimpleDTO
                {
                    ArticleID = x.ArticleID,
                    Author = _context.User.Where(u => u.UserID.Equals(x.UserID)).FirstOrDefault().UserName,
                    CreatedDate = x.CreatedDate,
                    Title = x.Title,
                    Introduction = x.Introduction
                });
            });

            return Ok(ArticleSummaries);
        }

        // PUT: api/Articles/5
        // Update the article contents, the fields will be modified using a seperate route. Requires authentication.
        [HttpPut("{id}")]
        public async Task<IActionResult> EditArticle(int id, ArticleEditTemplateDTO article)
        {
            var TargetArticle = await _context.Article.FindAsync(id);

            if (TargetArticle == null)
            {
                return BadRequest("Article does not exist, create one instead!");
            }

            var Author = await _context.User.FindAsync(TargetArticle.UserID);

            // Check credentials of author that is editing this article.
            if(article.UserName != Author.UserName || article.PassWord != Author.PassWord)
            {
                return StatusCode(403);
            }

            TargetArticle.Title = article.Title;
            TargetArticle.Introduction = article.Introduction;

            _context.Entry(TargetArticle).State = EntityState.Modified;

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

        // POST: api/Articles
        // Create article. Requires Authentication.
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(ArticleEditTemplateDTO article)
        {
            var author = await _context.User.Where(p => p.UserName == article.UserName).FirstAsync();

            if(author == null)
            {
                return StatusCode(403);
            }

            // Ensure that the user entered the correct credentials.
            if (author.UserName != article.UserName || author.PassWord != article.PassWord)
            {
                return StatusCode(403);
            }

            Article TargetArticle = new Article
            {
                UserID = author.UserID,
                CreatedDate = DateTime.UtcNow,
                Title = article.Title,
                Introduction = article.Introduction
            };

            _context.Article.Add(TargetArticle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = TargetArticle.ArticleID }, article);
        }

        // DELETE: api/Articles/5
        // Delete article. Requires authentication.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id, BasicAuthenticateDTO authentication)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var author = await _context.User.Where(p => p.UserName == authentication.UserName).FirstAsync();

            // Ensure that it is the author trying to delete the article.
            if(author.UserName != authentication.UserName || author.PassWord != authentication.PassWord)
            {
                return StatusCode(403);
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }

        //private bool ArticleExists(int id)
        //{
        //    return _context.Article.Any(e => e.ArticleID == id);
        //}
    }
}
