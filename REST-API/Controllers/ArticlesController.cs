using System;
using System.Collections.Generic;
using System.Linq;
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

        public ArticlesController(ArticleContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticle()
        {
            return await _context.Article.ToListAsync();
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        [HttpGet("Author/{author}")]
        // Search articles based on author name.
        public async Task<ActionResult<IEnumerable<Article>>> GetArticleByAuthor(string Author)
        {
            var author = await _context.User.Where(p => p.UserName.Contains(Author)).FirstAsync();

            if(author == null)
            {
                return BadRequest("Author with that username does not exist!");
            }

            var article = await _context.Article.Where(p => p.UserID == author.UserID).ToListAsync();

            if (article == null)
            {
                return NotFound("Author has not made any articles.");
            }

            return article;
        }

        [HttpGet("Query/{query}")]
        // Search articles based on search result.
        public async Task<ActionResult<IEnumerable<Article>>> GetArticleByQuery(string query)
        {
            var article = await _context.Article.Where(p => p.Title.Contains(query)).ToListAsync();

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // PUT: api/Articles/5
        // Update the article contents, the fields will be modified using a seperate route. Requires authentication.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, ArticleEditTemplateDTO article)
        {
            var TargetArticle = await _context.Article.FindAsync(id);
            var Author = await _context.User.FindAsync(TargetArticle.UserID);

            if(TargetArticle == null)
            {
                return BadRequest("Article does not exist, create one instead!");
            }

            // Check credentials of author that is allowed to edit this article.
            if(article.UserName == Author.UserName)
            {
                if(article.PassWord != Author.PassWord)
                {
                    return Forbid("Invalid author password!");
                }
            }
            else
            {
                return Forbid("Invalid author username!");
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
                return Forbid("User is not registered.");
            }

            if (author.UserName != article.UserName)
            {
                return Unauthorized("You are not the author, so article not modified!");
            }

            if (author.PassWord != article.PassWord)
            {
                return Forbid("Incorrect password!");
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
        public async Task<ActionResult<Article>> DeleteArticle(int id, BasicAuthenticateDTO authentication)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var author = await _context.User.Where(p => p.UserName == authentication.UserName).FirstAsync();

            if (author.UserName != authentication.UserName)
            {
                return Unauthorized("You are not the author, so article not deleted!");
            }

            if(author.PassWord != authentication.PassWord)
            {
                return Forbid("Incorrect password!");
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return article;
        }

        //private bool ArticleExists(int id)
        //{
        //    return _context.Article.Any(e => e.ArticleID == id);
        //}
    }
}
