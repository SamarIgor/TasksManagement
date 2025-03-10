using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class CommentController : Controller
    {
        private readonly TaskContext _context;
        private readonly UserManager<AccountUser> _usermanager;

        public CommentController(TaskContext context,UserManager<AccountUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Comment
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
    {
        return NotFound();
    }

    // Retrieve the task along with its comments
    var taskWithComments = await _context.Tasks
        .Include(t => t.Comments)
        .ThenInclude(c => c.Owner)
        .FirstOrDefaultAsync(m => m.TasksId == id);

    if (taskWithComments == null)
    {
        return NotFound();
    }

    // Pass the task with comments to the view
    return View(taskWithComments);
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            ViewData["TasksId"] = new SelectList(_context.Tasks, "TasksId", "Title");
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CommentId,TasksId,Text")] Comment comment)
        {
            var currentUser = await _usermanager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                comment.DateCreated = DateTime.Now;
                comment.DateEdited = DateTime.Now;
                comment.Owner = currentUser;
                comment.OwnerId = currentUser.Id;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = comment.TasksId });
            }
            ViewData["TasksId"] = new SelectList(_context.Tasks, "TasksId", "TasksId", comment.TasksId);
            return View(comment);
        }

        // GET: Comment/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["TasksId"] = new SelectList(_context.Tasks, "TasksId", "Title", comment.TasksId);
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       // [Authorize(Roles ="Admin, Staff")]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,TasksId,Text")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalComment = await _context.Comments.FindAsync(id);

                    if (originalComment == null)
                    {
                        return NotFound();
                    }

                    // Update only the necessary properties
                    originalComment.Text = comment.Text;

                    // Update the DateEdited property
                    originalComment.DateEdited = DateTime.Now;

                    _context.Update(originalComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = comment.TasksId });
            }
            ViewData["TasksId"] = new SelectList(_context.Tasks, "TasksId", "TasksId", comment.TasksId);
            return View(comment);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       // [Authorize(Roles ="Admin, Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = comment.TasksId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
