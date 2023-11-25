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
            var taskContext = _context.Tasks.Include(t => t.Category).Include(t => t.Priority);
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
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create([Bind("TasksId,Title,Description,CategoryId,PriorityId")] Tasks tasks)
        {
            var currentUser = await _usermanager.GetUserAsync(User);
            
            if (ModelState.IsValid)
            {
                tasks.DateCreated = DateTime.Now;
                tasks.DateEdited = DateTime.Now;
                tasks.Owner = currentUser;

                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", tasks.CategoryId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "PriorityId", "Name", tasks.PriorityId);
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
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TasksId,Title,Description,CategoryId,PriorityId")] Tasks tasks)
        {
            if (id != tasks.TasksId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
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
            var tasks = await _context.Tasks.FindAsync(id);
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
