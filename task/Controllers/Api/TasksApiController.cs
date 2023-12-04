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
using task.Models;
using task.Filters;

namespace task.Controllers_Api
{
    [Route("api/Tasks")]
    [ApiController]
    [ApiKeyAuthAttr]    
    public class TasksApiController : ControllerBase
    {
        private readonly TaskContext _context;

        public TasksApiController(TaskContext context)
        {
            _context = context;
        }

        // GET: api/TasksApi
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Tasks>>> GetTasks()
        {
            var tasks = await _context.Tasks
            .Include(task => task.Category)
            .Include(task => task.Priority)
            .Include(task => task.Comments)
            .Include(task => task.Owner)
            .Include(task => task.TaskFor)
            .ToListAsync();

            var taskDtos = tasks.Select(task => new
            {
                TasksId = task.TasksId,
                Title = task.Title,
                Description = task.Description,
                Category = task.Category?.Name,
                Priority = task.Priority?.Name,
                Comments = task.Comments?.Select(comment => new
                {
                    CommentId = comment.CommentId,
                    Text = comment.Text,
                    OwnerUsername = comment.Owner?.UserName,
                    DateCreated = comment.DateCreated,
                    DateEdited = comment.DateEdited
                }),
                OwnerUsername = task.Owner?.UserName,
                TaskForUsername = task.TaskFor?.UserName,
                DateCreated = task.DateCreated,
                DateEdited = task.DateEdited
            });

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64,
            };

            var serializedTasks = JsonSerializer.Serialize(taskDtos, jsonOptions);

            return Content(serializedTasks, "application/json");
        }

        // GET: api/TasksApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tasks>> GetTasks(int id)
        {
             var task = await _context.Tasks
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Comments)
            .Include(t => t.Owner)
            .Include(t => t.TaskFor)
            .FirstOrDefaultAsync(t => t.TasksId == id);

            if (task == null)
            {
                return NotFound();
            }

            var taskDto = new
            {
                TasksId = task.TasksId,
                Title = task.Title,
                Description = task.Description,
                Category = task.Category?.Name,
                Priority = task.Priority?.Name,
                Comments = task.Comments?.Select(comment => new
                {
                    CommentId = comment.CommentId,
                    Text = comment.Text,
                    OwnerUsername = comment.Owner?.UserName,
                    DateCreated = comment.DateCreated,
                    DateEdited = comment.DateEdited
                }),
                OwnerUsername = task.Owner?.UserName,
                TaskForUsername = task.TaskFor?.UserName,
                DateCreated = task.DateCreated,
                DateEdited = task.DateEdited
            };

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64,
            };

            var serializedTask = JsonSerializer.Serialize(taskDto, jsonOptions);

            return Content(serializedTask, "application/json");
        }

        // PUT: api/TasksApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasks(int id, Tasks tasks)
        {
            if (id != tasks.TasksId)
            {
                return BadRequest();
            }

            _context.Entry(tasks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TasksExists(id))
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

        // POST: api/TasksApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tasks>> PostTasks(Tasks tasks)
        {
            _context.Tasks.Add(tasks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTasks", new { id = tasks.TasksId }, tasks);
        }

        // DELETE: api/TasksApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTasks(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TasksId == id);
        }
    }
}
