﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PRIS.Core.Library.Entities;
using PRIS.Web.Data;
using PRIS.Web.Mappings;
using PRIS.Web.Models;

namespace PRIS.Web.Controllers
{
    [Authorize]
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            var result = await _context.Exams.Include(exam => exam.City).ToListAsync();
            List<ExamViewModel> examViewModels = new List<ExamViewModel>();
            result.ForEach(x => examViewModels.Add(ExamMappings.ToViewModel(x)));
            examViewModels.ForEach(x => x.SelectedCity = result.FirstOrDefault(y => y.Id == x.Id).City.Name);
            return View(examViewModels);
        }

        // GET: Exams/Create
        public async Task<IActionResult> Create()
        {
            ExamViewModel examViewModel = new ExamViewModel();
            List<City> cities = await _context.Cities.ToListAsync();

            var stringCities = new List<SelectListItem>();
            foreach (var city in cities)
            {
                stringCities.Add(new SelectListItem { Value = city.Name, Text = city.Name });
            }
            examViewModel.Cities = stringCities;

            return View(examViewModel);
        }

        // POST: Exams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityId,Date,Comment,Id,Created,SelectedCity")] ExamViewModel examViewModel)
        {
            if (ModelState.IsValid)
            {
                var exam = ExamMappings.ToEntity(examViewModel);
                exam.CityId = _context.Cities.FirstOrDefault(x => x.Name == examViewModel.SelectedCity).Id;
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }
            var exams = await _context.Exams.FindAsync(id);
            _context.Exams.Remove(exams);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
