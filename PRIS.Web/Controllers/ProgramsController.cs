﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRIS.Web.Data;
using PRIS.Web.Mappings;
using PRIS.Web.Models;
using PRIS.Core.Library.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PRIS.Web.Controllers
{
    public class ProgramsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Programs
        public async Task<IActionResult> Index()
        {
            var programResult = await _context.Programs.ToListAsync();
            var cityResult = await _context.Cities.ToListAsync();
            List<ProgramViewModel> programViewModels = new List<ProgramViewModel>();

            foreach (var programEntity in programResult)
            {
                foreach (var cityEntity in cityResult)
                {
                    programViewModels.Add(ProgramMappings.ToProgramViewModel(programEntity, cityEntity));
                }
                
            }
            return View(programViewModels);
        }
        //public async Task<IActionResult> Index1()
        //{
        //    var result = await _context.Programs.ToListAsync();
        //    var result2 = await _context.Cities.ToListAsync();
        //    List<ProgramViewModel> programViewModels = new List<ProgramViewModel>();

        //    foreach (var item in result)
        //    {
        //        programViewModels.Add(ProgramMappings.ToProgramViewModel(item));

        //    }
        //    return View(programViewModels);
        //}

        // GET: Programs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramName,ProgramId")] ProgramViewModel programViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = ProgramMappings.ToProgramEntity(programViewModel);
                _context.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(programViewModel);
        }

        //GET: Programs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var program = await _context.Programs
                .FirstOrDefaultAsync(m => m.Id == id);
            var city = await _context.Cities.FirstOrDefaultAsync(m => m.Id == id);
            if (program == null)
            {
                return NotFound();
            }

            return View(ProgramMappings.ToProgramViewModel(program, city));
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var program = await _context.Programs.FindAsync(id);
            _context.Programs.Remove(program);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgramExists(int id)
        {
            return _context.Programs.Any(e => e.Id == id);
        }

    }
}
