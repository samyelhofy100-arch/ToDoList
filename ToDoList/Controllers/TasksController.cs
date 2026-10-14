using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using Microsoft.AspNetCore.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoList.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplactionDbContext _Context =new ApplactionDbContext();


        public IActionResult Index(DateTime? date, int page = 1)
        {
            int pageSize = 5;

            var tasks = _Context.ToDoItems.AsQueryable();

            if (date.HasValue)
            {
                tasks = tasks.Where(t => t.Deadline.Date == date.Value.Date);
            }

            int totalTasks = tasks.Count();
            int totalPages = (int)Math.Ceiling(totalTasks / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            if (page < 1)
            {
                page = 1;
            }

            var taskList = tasks
                .OrderBy(t => t.Deadline)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(taskList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ToDoItem todoitem , IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", filename);

                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }
                todoitem.FilePath = filename;
            }
            _Context.ToDoItems.Add(todoitem);
            _Context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var task= _Context.ToDoItems.SingleOrDefault(c=>c.Id==id);
            if (task == null) 
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            return View(task);
        }
        [HttpPost]
        public IActionResult Update(ToDoItem todoitem , IFormFile ImageFile)
        {
            var taskInDbContext= _Context.ToDoItems.AsNoTracking().SingleOrDefault(c=>c.Id == todoitem.Id);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", filename);

                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }

                todoitem.FilePath = filename;
                
                var oldfilepath= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\", filename);
                if (System.IO.File.Exists(oldfilepath))
                {
                    System.IO.File.Delete(oldfilepath);
                }
                else
                {
                    todoitem.FilePath = taskInDbContext.FilePath;
                }
            }

            _Context.ToDoItems.Update(todoitem);
            _Context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
      

        public IActionResult Delete(int id)
        {
           var task=_Context.ToDoItems.SingleOrDefault(c=>c.Id == id);
            if (task == null) 
            {
                return RedirectToAction("Not Found ", "Home");
            }
            _Context.ToDoItems.Remove(task);
            _Context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        //------------------------------------------------

        public IActionResult Download(int id)
        {
            var task = _Context.ToDoItems
                .FirstOrDefault(t => t.Id == id);

            if (task == null || string.IsNullOrEmpty(task.FilePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(task.FilePath);

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                fileName
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            return PhysicalFile(
                filePath,
                "application/octet-stream",
                fileName
            );
        }

    }
}