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
    public class ArticleFieldsController : ControllerBase
    {
        private readonly ArticleContext _context;

        public ArticleFieldsController(ArticleContext context)
        {
            _context = context;
        }

        // Convert fields to exclude ArticleID from the result content.
        private static readonly Expression<Func<ArticleField, FieldDTO>> AsArticleFieldDTO = x => new FieldDTO
        {
            ID = x.FieldID,
            Name = x.Name,
            Value = x.Value
        };

        // GET: api/ArticleFields
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ArticleField>>> GetArticleField()
        //{
        //    return await _context.ArticleField.ToListAsync();
        //}

        // GET: api/ArticleFields/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FieldDTO>> GetArticleField(int id)
        {
            var articleField = await _context.ArticleField.FindAsync(id);

            if (articleField == null)
            {
                return NotFound();
            }

            return Ok(new FieldDTO
            {
                ID = articleField.FieldID,
                Name = articleField.Name,
                Value = articleField.Value
            });
        }

        // PUT: api/ArticleFields
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutArticleField(ArticleFieldEditTemplateDTO ArticleField)
        {
            var TargetField = await _context.ArticleField.FindAsync(ArticleField.FieldID);

            if(TargetField == null)
            {
                return NotFound();
            }

            // Grab the article and author associated with the field.
            var OriginalArticle = await _context.Article.FindAsync(TargetField.ArticleID);
            var OriginalUser = await _context.User.FindAsync(OriginalArticle.UserID);

            // Ensure that only the article author is allowed to edit their fields.
            if(OriginalUser.UserName != ArticleField.UserName || OriginalUser.PassWord != ArticleField.PassWord)
            {
                return StatusCode(403);
            }

            TargetField.Name = ArticleField.Name;
            TargetField.Value = ArticleField.Value;
            _context.Entry(TargetField).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleFieldExists(ArticleField.FieldID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ArticleFields
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ArticleField>> PostArticleField(ArticleFieldCreateTemplateDTO ArticleField)
        {
            var TargetArticle = await _context.Article.FindAsync(ArticleField.ArticleID);

            if(TargetArticle == null)
            {
                return BadRequest("The article itself does not exist.");
            }

            var ArticleAuthor = await _context.User.Where(p => p.UserName == ArticleField.UserName).FirstOrDefaultAsync();

            // Ensure that only the article author is allowed to add fields to their articles.
            if (ArticleAuthor.UserID != TargetArticle.UserID || ArticleAuthor.PassWord != ArticleField.PassWord)
            {
                return StatusCode(403);
            }

            ArticleField NewField = new ArticleField
            {
                ArticleID = ArticleField.ArticleID,
                Name = ArticleField.Name,
                Value = ArticleField.Value
            };

            _context.ArticleField.Add(NewField);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticleField", new { id = NewField.FieldID }, NewField);
        }

        // DELETE: api/ArticleFields/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ArticleField>> DeleteArticleField(int id)
        {
            var articleField = await _context.ArticleField.FindAsync(id);
            if (articleField == null)
            {
                return NotFound();
            }

            _context.ArticleField.Remove(articleField);
            await _context.SaveChangesAsync();

            return articleField;
        }

        private bool ArticleFieldExists(int id)
        {
            return _context.ArticleField.Any(e => e.FieldID == id);
        }
    }
}
