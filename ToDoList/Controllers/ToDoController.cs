using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Infrastructure;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoContext context;

        public ToDoController(ToDoContext context) 
        {
            this.context = context;
        }

        //GET /  - All To Do List Items
        public async Task<ActionResult> Index()
        {
            IQueryable<ToDoListModel> items = from i in context.ToDoList orderby i.Id select i;
            List<ToDoListModel> todoList = await items.ToListAsync();

            return View(todoList);
        }

        //GET /todo/create - Create View Path
        public IActionResult create() => View();

        
        //POST /todo/create - Add new Item to the Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> create(ToDoListModel item)
        {
            if (ModelState.IsValid) 
            {
                context.Add(item);
                await context.SaveChangesAsync();

                TempData["Success"] = "The Item has been added successfully!";
                return RedirectToAction("Index");
            }
            return View(item);
        }

        //GET /todo/edit  - Edit View for Specific Item
        public async Task<ActionResult> Edit(int id)
        {
            ToDoListModel item = await context.ToDoList.FindAsync(id);
            if (item == null) 
            {
                TempData["error"] = "Item to edit does not exist";
                return RedirectToAction("Index");
            }

            return View(item);
        }

        //POST /todo/edit/id - Updet item in the Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ToDoListModel item)
        {
            if (ModelState.IsValid)
            {
                context.Update(item);
                await context.SaveChangesAsync();

                TempData["success"] = "The Item has been updated successfully!";
                return RedirectToAction("Index");
            }
            return View(item);
        }

        //POST /todo/delete/id - Delete item in the Database
        public async Task<ActionResult> Delete(int id)
        {
            ToDoListModel item = await context.ToDoList.FindAsync(id);
            if (item == null)
            {
                TempData["error"] = "The item has not been found!";
            }
            else
            {
                context.ToDoList.Remove(item);
                await context.SaveChangesAsync();

                TempData["success"] = "The item has been deleted successfully";
            }

            return RedirectToAction("Index");
        }
    }
}
