#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkOrders.Data;
using WorkOrders.Models;
using WorkOrders.Models.ViewModels;

namespace WorkOrders.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private readonly WorkOrdersContext _context;

        public ProjectsController(WorkOrdersContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index(Project_Filter filter)
        {
            List<Project> projects = await _context.Projects
                .Include(p => p.Customer)
                .ToListAsync();


            if (filter.CustomerId != null)
            {
                projects = projects.Where(x => x.CustomerId == filter.CustomerId).ToList();
            }
            if (filter.ProjectId != null)
            {
                projects = projects.Where(x => x.ProjectId == filter.ProjectId).ToList();
            }
            //if (filter.IsActive != null)
            //{
            //    projects = projects.Where(x => x.IsActive == filter.IsActive).ToList();
            //}



            filter.Project = projects;
            filter.Projects = new SelectList(_context.Projects.ToList(), "ProjectId", "Name");
             

            filter.Customers = new SelectList(_context.Customers.ToList(), "CustomerId", "Name");
            






            //ViewBag.Poject = projects;
            return View(filter); 

            //var workOrdersContext = _context.Projects.Include(p => p.Customer);
            //return View(await workOrdersContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Name,CustomerId,IsActive,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", project.CustomerId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", project.CustomerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name,CustomerId,IsActive,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
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
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", project.CustomerId);
            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "CustomerId", project.CustomerId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //------ How to delete: -------
            //project.IsActive = false;
            //_context.Projects.Update(project);


            var project = await _context.Projects.FindAsync(id);
            project.IsActive = false;
            _context.Projects.Update(project);
            //_context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
