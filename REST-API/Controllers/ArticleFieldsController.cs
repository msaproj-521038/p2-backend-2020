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
            ID = x.FieldsID,
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
        public async Task<ActionResult<ArticleField>> GetArticleField(int id)
        {
            var articleField = await _context.ArticleField.FindAsync(id);

            if (articleField == null)
            {
                return NotFound();
            }

            return articleField;
        }

        // PUT: api/ArticleFields/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticleField(int id, ArticleField articleField)
        {
            if (id != articleField.FieldsID)
            {
                return BadRequest();
            }

            _context.Entry(articleField).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleFieldExists(id))
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
        public async Task<ActionResult<ArticleField>> PostArticleField(ArticleField articleField)
        {
            _context.ArticleField.Add(articleField);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticleField", new { id = articleField.FieldsID }, articleField);
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
            return _context.ArticleField.Any(e => e.FieldsID == id);
        }
    }
}
