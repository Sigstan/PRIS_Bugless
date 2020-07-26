﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRIS.Core.Library.Entities;
using PRIS.Web.Mappings;
using PRIS.Web.Models;
using PRIS.Web.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Controllers
{
    [Authorize]
    public class ProgramsController : Controller
    {
        private readonly IRepository _repository;

        public ProgramsController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            ProgramViewModel programViewModel = new ProgramViewModel();

            var programResult = await _repository.Query<Core.Library.Entities.Program>().ToListAsync();
            List<ProgramCreateModel> programsList = new List<ProgramCreateModel>();
            programResult.ForEach(program => programsList.Add(ProgramMappings.ToProgramListViewModel(program)));
            programViewModel.ProgramNames = programsList;

            var cities = await _repository.Query<City>().ToListAsync();
            List<CityCreateModel> citiesList = new List<CityCreateModel>();
            cities.ForEach(city => citiesList.Add(ProgramMappings.ToCityListViewModel(city)));
            programViewModel.CityNames = citiesList;

            return View(programViewModel);
        }

        public IActionResult Create()
        {
            var model = new ProgramCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramName")] ProgramCreateModel programCreateModel)
        {
            if (ModelState.IsValid)
            {
                var result = ProgramMappings.ToProgramEntity(programCreateModel);
                var beforeCreatedProgam = _repository.Query<Core.Library.Entities.Program>().FirstOrDefault(x => x.Name == result.Name);
                if (beforeCreatedProgam != null)
                {
                    TempData["ErrorMessage"] = "Ši programa jau yra sukurta";
                    return RedirectToAction(nameof(Create));
                }
                await _repository.InsertAsync<Core.Library.Entities.Program>(result);
                return RedirectToAction(nameof(Index));
            }
            return View(programCreateModel);
        }
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
                var beforeCreatedCity = _repository.Query<City>().FirstOrDefault(x => x.Name == result.Name);
                if (beforeCreatedCity != null)
                {
                    TempData["ErrorMessage"] = "Šis miestas jau yra sukurtas";
                    return RedirectToAction(nameof(CreateNewCity));
                }
                await _repository.InsertAsync<City>(result);
                return RedirectToAction(nameof(Index));
            }
            return View(cityCreateModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var program = await _repository.Query<Core.Library.Entities.Program>()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (program == null)
            {
                return NotFound();
            }
            var programById = await _repository.FindByIdAsync<Core.Library.Entities.Program>(id);
            var course = await _repository.Query<Course>().FirstOrDefaultAsync(x => x.Title == program.Name);
            if (course != null)
            {
                return await BadRequest(course, "Programos negalima ištrinti, nes prie jos jau yra priskirta kandidatų.");
            }
            else
            {
                await _repository.DeleteAsync<Core.Library.Entities.Program>(programById.Id);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var city = await _repository.FindByIdAsync<City>(id);
            if (city == null)
            {
                return NotFound();
            }

            var exam = await _repository.Query<Exam>().FirstOrDefaultAsync(x => x.CityId == city.Id);
            if (exam != null)
            {
                var exams = await _repository.Query<Exam>().ToListAsync();
                var studentCourse = exams.Where(x => x.Id == exam.Id);
                if (studentCourse.Any())
                {
                    TempData["ErrorMessage"] = "Miesto negalima ištrinti, nes prie jo jau yra priskirta kandidatų.";
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                await _repository.DeleteAsync<City>(city.Id);
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<IActionResult> BadRequest(Course course, string errorMessage)
        {
            var studentsCourses = await _repository.Query<StudentCourse>().ToListAsync();
            var studentCourse = studentsCourses.Where(x => x.CourseId == course.Id);
            if (studentCourse.Any())
            {
                ModelState.AddModelError("AssignedStudent", errorMessage);
                TempData["ErrorMessage"] = errorMessage;
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
