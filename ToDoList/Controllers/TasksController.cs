using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplactionDbContext _Context =new ApplactionDbContext();

        public IActionResult Index()
        {
            var tasks = _Context.ToDoItems.AsQueryable();
            return View(tasks.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ToDoItem todoitem)
        {
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
        public IActionResult Update(ToDoItem todoitem )
        {
            var taskInDbContext= _Context.ToDoItems.AsNoTracking().SingleOrDefault(c=>c.Id == todoitem.Id);
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
    }
}