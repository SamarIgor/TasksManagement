using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task.Data;
using task.Filters;
using task.Models;

namespace task.Controllers_Api
{
    [Route("api/Comments")]
    [ApiController]
    [ApiKeyAuthAttr]
    public class CommentsApiController : ControllerBase
    {
        private readonly TaskContext _context;

        public CommentsApiController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/CommentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            var comments = await _context.Comments
            .Include(c => c.Owner)
            .Include(c => c.Tasks)
            .ToListAsync();

            var commentDtos = comments.Select(c => new
            {       
                CommentId = c.CommentId,
                TasksId = c.TasksId,
                Text = c.Text,
                OwnerUsername = c.Owner?.UserName,
                DateCreated = c.DateCreated,
                DateEdited = c.DateEdited
            });

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64, 
            };

            var serializedComments = JsonSerializer.Serialize(commentDtos, jsonOptions);

            return Content(serializedComments, "application/json");
        }

        // GET: api/CommentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments
            .Include(c => c.Owner)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return NotFound();
            }

            var commentDto = new
            {
                CommentId = comment.CommentId,
                TasksId = comment.TasksId,
                Text = comment.Text,
                OwnerUsername = comment.Owner?.UserName,
                DateCreated = comment.DateCreated,
                DateEdited = comment.DateEdited
            };

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64, 
            };

            var serializedComment = JsonSerializer.Serialize(commentDto, jsonOptions);

            return Content(serializedComment, "application/json");
        }

        // PUT: api/CommentsApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/CommentsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/CommentsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
