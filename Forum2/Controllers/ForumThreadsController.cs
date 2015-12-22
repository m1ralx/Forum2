using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Forum.Models;
using Forum2.Models;
using Microsoft.AspNet.Identity;

namespace Forum2.Controllers
{
    [Authorize]
    public class ForumThreadsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private const int PageSize = 10;

        // GET: ForumThreads/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThread forumThread = await db.Threads.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            ViewBag.PageSize = PageSize;
            ViewBag.Page = page ?? 1;
            return View(forumThread);
        }

        // GET: ForumThreads/Create
        public ActionResult Create(int? boardId)
        {
            if (boardId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.BoardId = boardId;
            return View();
        }

        // POST: ForumThreads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description")] ForumThread forumThread, int boardId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                var owner = await db.Users.FirstOrDefaultAsync(user => user.Id == userId);
                if (owner == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

                forumThread.Owner = owner;
                var board = await db.Boards.FindAsync(boardId);
                forumThread.Board = board;
                db.Threads.Add(forumThread);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Boards", new { id = boardId });
            }

            return View(forumThread);
        }

        // GET: ForumThreads/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThread forumThread = await db.Threads.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() != forumThread.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View(forumThread);
        }

        // POST: ForumThreads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description")] ForumThread forumThread)
        {
            if (ModelState.IsValid)
            {
                var oldForumThread = db.Threads.First(t => t.Id == forumThread.Id);
                oldForumThread.Name = forumThread.Name;
                oldForumThread.Description = forumThread.Description;
                if (User.Identity.GetUserId() != oldForumThread.Owner.Id && !User.IsInRole("Admin"))
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                db.Entry(oldForumThread).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Boards", new { id = oldForumThread.Board.Id });
            }
            return View(forumThread);
        }

        // GET: ForumThreads/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumThread forumThread = await db.Threads.FindAsync(id);
            if (forumThread == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() != forumThread.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View(forumThread);
        }

        // POST: ForumThreads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ForumThread forumThread = await db.Threads.FindAsync(id);
            if (User.Identity.GetUserId() != forumThread.Owner.Id && !User.IsInRole("Admin"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var boardId = forumThread.Board.Id;
            db.Messages.RemoveRange(db.Messages.Where(m => m.Thread.Id == forumThread.Id).ToList());
            db.Threads.Remove(forumThread);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Boards", new { id = boardId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
