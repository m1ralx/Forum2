using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Forum.Models;
using Forum2.Models;
using PagedList;

namespace Forum2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BoardsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const int PageSize = 5;

        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            var boards = from board in db.Boards
                         orderby board.Id
                         select board;
            return View(boards.ToPagedList(page ?? 1, PageSize));
        }

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            ViewBag.PageSize = PageSize;
            ViewBag.Page = page ?? 1;
            return View(board);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description")] Board board)
        {
            if (ModelState.IsValid)
            {
                db.Boards.Add(board);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(board);
        }
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description")] Board board)
        {
            if (ModelState.IsValid)
            {
                var oldBoard = db.Boards.First(b => b.Id == board.Id);
                oldBoard.Name = board.Name;
                oldBoard.Description = board.Description;
                db.Entry(oldBoard).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(board);
        }
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Board board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Board board = await db.Boards.FindAsync(id);
            db.Messages.RemoveRange(db.Threads.Where(t => t.Board.Id == id).SelectMany(t => t.Messages).ToList());
            db.Threads.RemoveRange(db.Threads.Where(t => t.Board.Id == id).ToList());
            db.Boards.Remove(board);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Thread(int? id)
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

            return RedirectToAction("Details", "ForumThreads", new { id });
        }
    }
}
