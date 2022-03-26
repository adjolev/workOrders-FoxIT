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
    [Authorize]
    public class WorkOrdersController : Controller
    {
        private readonly WorkOrdersContext _context;

        public WorkOrdersController(WorkOrdersContext context)
        {
            _context = context;
        }

        //GET: WorkOrders
        public async Task<IActionResult> Index(WorkOrder_Filter filter)
        {
            //filter = new WorkOrder_Filter();
            //List<WorkOrder> workOrders = await _context.WorkOrders
            //    .Include(x => x.Customer)
            //    .Include(x => x.Project)
            //    .ToListAsync();


            List<WorkOrder> workOrders = await _context.WorkOrders
               .Include(x => x.Customer)
               .Include(x => x.Project)
                .ToListAsync();


            if (filter.CustomerId != null)
            {
               workOrders = workOrders.Where(x => x.CustomerId == filter.CustomerId).ToList();
            }
            if (filter.ProjectId != null)
            {
               workOrders = workOrders.Where(x => x.ProjectId == filter.ProjectId).ToList();
            }
            if (filter.UserId != null)
            {
              workOrders =  workOrders.Where(x => x.UserId == filter.UserId).ToList();
            }
            //if (!string.IsNullOrEmpty(filter.CustomerNote))
            //{
            //    workOrders = workOrders.Where(x => x.CustomerNote.Contains(filter.CustomerNote)).ToList();
            //}

            filter.WorkOrders = workOrders;

            filter.Customers = new SelectList(_context.Customers.ToList(), "CustomerId", "Name");
            filter.Projects = new SelectList(_context.Projects.ToList(), "ProjectId", "Name");
            filter.Users = new SelectList(_context.WorkOrders.Select(x => new SelectListItem()
            {
                Value = x.UserId,
                Text = x.UserId
            }).Distinct().ToList(), "Value", "Text"); 


            //ViewBag.WorkOrders = workOrders;

            return View(filter);

            //var workOrdersContext = _context.WorkOrders.Include(w => w.Customer).Include(w => w.Project);
            //return View(await workOrdersContext
            //    .Include(x => x.Customer)
            //    .Include(x => x.Project)
            //    .ToListAsync());
        }




        // GET: WorkOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.Customer)
                .Include(w => w.Project)
                .FirstOrDefaultAsync(m => m.WorkOrderId == id);
            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);
        }

        // GET: WorkOrders/Create
        public IActionResult Create()
        {
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");

            var customers = new SelectList(_context.Customers.Select(x => new SelectListItem
            {
                Value = x.CustomerId.ToString(),
                Text = x.Name
            }).ToList(), "Value", "Text");

            var projects = new SelectList(_context.Projects.Select(x => new SelectListItem
            {
                Value = x.ProjectId.ToString(),
                Text = x.Name
            }).ToList(), "Value", "Text");

            ViewBag.Customers = customers;
            ViewBag.Projects = projects;

            return View();
        }

        // POST: WorkOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkOrderId,CustomerId,ProjectId,UserId,Date,CustomerNote,PerformedWorks,Hours,Minutes,IsActive,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", workOrder.CustomerId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workOrder.ProjectId);
            return View(workOrder);
        }

        // GET: WorkOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", workOrder.CustomerId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workOrder.ProjectId);
            return View(workOrder);
        }

        // POST: WorkOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkOrderId,CustomerId,ProjectId,UserId,Date,CustomerNote,PerformedWorks,Hours,Minutes,IsActive,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] WorkOrder workOrder)
        {
            if (id != workOrder.WorkOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkOrderExists(workOrder.WorkOrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", workOrder.CustomerId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", workOrder.ProjectId);
            return View(workOrder);
        }

        // GET: WorkOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.Customer)
                .Include(w => w.Project)
                .FirstOrDefaultAsync(m => m.WorkOrderId == id);
            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);
        }

        // POST: WorkOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var workOrder = await _context.WorkOrders.FindAsync(id);
            workOrder.IsActive = false;
            _context.WorkOrders.Update(workOrder);
            //_context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.WorkOrderId == id);
        }
    }
}
