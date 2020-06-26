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
            ProgramViewModel programViewModel = new ProgramViewModel();
            var programResult = await _context.Programs.ToListAsync();
            programViewModel.ProgramNames = programResult;

            var cityResult = await _context.Cities.ToListAsync();
            programViewModel.CityNames = cityResult;

            return View(programViewModel);
        }

        // GET: Programs/Create
        public IActionResult Create()
        {
            var model = new ProgramCreateModel();
            return View(model);
        }

        // POST: Programs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramName")] ProgramCreateModel programCreateModel)
        {
            if (ModelState.IsValid)
            {
                var result = ProgramMappings.ToProgramEntity(programCreateModel);
                _context.Programs.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(programCreateModel);
        }
        //Create new city
        public IActionResult CreateNewCity()
        {
            var model = new CityCreateModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewCity([Bind("CityName")] CityCreateModel cityCreateModel)
        {
            if (ModelState.IsValid)
            {
                var result = ProgramMappings.ToCityEntity(cityCreateModel);
                _context.Cities.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cityCreateModel);
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
            if (program == null)
            {
                return NotFound();
            }
            var programs = await _context.Programs.FindAsync(id);
            _context.Programs.Remove(programs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(ProgramMappings.ToProgramViewModel(Index));
        }
        ////delete city
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }
            var cities = await _context.Cities.FindAsync(id);
            _context.Cities.Remove(cities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //POST: Programs/Delete/5
        /*        [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var program = await _context.Programs.FindAsync(id);
                    _context.Programs.Remove(program);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }*/

        private bool ProgramExists(int id)
        {
            return _context.Programs.Any(e => e.Id == id);
        }
        ////delete city comfirmed
        //[HttpPost, ActionName("DeleteCity")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmedCity(int id)
        //{
        //    var city = await _context.Cities.FindAsync(id);
        //    _context.Cities.Remove(city);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }

    }
    }
