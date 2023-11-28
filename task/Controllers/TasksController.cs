using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using task.Data;
using task.Models;

namespace task.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<AccountUser> _usermanager;

        public TasksController(TaskContext context,UserManager<AccountUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var taskContext = _context.Tasks.Include(t => t.Category).Include(t => t.Priority).Include(t => t.TaskFor).Include(t => t.Owner);
            return View(await taskContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TaskFor)
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TasksId == id);
                
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "PriorityId", "Name");
            ViewData["TaskForId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create([Bind("TasksId,Title,Description,CategoryId,PriorityId,TaskForId")] Tasks tasks)
        {
            var currentUser = await _usermanager.GetUserAsync(User);
            
            if (ModelState.IsValid)
            {
                tasks.DateCreated = DateTime.Now;
                tasks.DateEdited = DateTime.Now;
                tasks.Owner = currentUser;
                tasks.OwnerId = currentUser.Id;

                tasks.TaskFor = await _usermanager.FindByIdAsync(tasks.TaskForId);

                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", tasks.CategoryId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "PriorityId", "Name", tasks.PriorityId);
            ViewData["TaskForId"] = new SelectList(_context.Users, "Id", "UserName",tasks.TaskForId);
            return View(tasks);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", tasks.CategoryId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "PriorityId", "Name", tasks.PriorityId);
            ViewData["TaskForId"] = new SelectList(_context.Users, "Id", "UserName",tasks.TaskForId);
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TasksId,Title,Description,CategoryId,PriorityId,TaskForId")] Tasks tasks)
        {
            if (id != tasks.TasksId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the original entity from the database
            var originalTasks = await _context.Tasks.FindAsync(id);

            if (originalTasks == null)
            {
                return NotFound();
            }

            // Update only the necessary properties
            originalTasks.TaskForId = tasks.TaskForId;
            originalTasks.Title = tasks.Title;
            originalTasks.Description = tasks.Description;
            originalTasks.CategoryId = tasks.CategoryId;
            originalTasks.PriorityId = tasks.PriorityId;
            
            // Update the DateEdited property
            originalTasks.DateEdited = DateTime.Now;

            _context.Update(originalTasks);
            await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TasksId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", tasks.CategoryId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "PriorityId", "PriorityId", tasks.PriorityId);
            ViewData["TaskForId"] = new SelectList(_context.Users, "Id", "UserName",tasks.TaskForId);
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.TaskFor)
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TasksId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks
        .Include(t => t.Comments) // Include the Comments navigation property
        .FirstOrDefaultAsync(t => t.TasksId == id);

    if (tasks == null)
    {
        return NotFound();
    }

    // Delete related comments
    _context.Comments.RemoveRange(tasks.Comments);

    // Delete the Tasks record
    _context.Tasks.Remove(tasks);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TasksId == id);
        }
    }
}
