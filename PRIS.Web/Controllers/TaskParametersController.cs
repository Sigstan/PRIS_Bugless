﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRIS.Web.Data;
using PRIS.Web.Mappings;
using PRIS.Web.Models;

namespace PRIS.Web.Controllers
{
    public class TaskParametersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tasks = await _context.Exams.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            var setTaskParameterModel = TaskParametersMappings.ToTaskParameterViewModel(tasks);
            setTaskParameterModel.TaskString = new string[setTaskParameterModel.Tasks.Length];
            for (int i = 0; i < setTaskParameterModel.TaskString.Length; i++)
            {
                setTaskParameterModel.TaskString[i] = setTaskParameterModel.Tasks[i].ToString().Replace(",", ".");
            }
            return View(setTaskParameterModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, string[] tasksString)
        {
            int.TryParse(TempData["Count"].ToString(), out int studentsCountInAcceptancePeriod);
            int.TryParse(TempData["SelectedAcceptancePeriod"].ToString(), out int SelectedAcceptancePeriod);
            TempData["ExamId"] = id;
            double[] tasks = new double[tasksString.Length];

            if (studentsCountInAcceptancePeriod > 0)
            {
                TempData["ErrorMessage"] = "Šablono keisti negalima, nes prie jo jau yra priskirta kandidatų.";
                ModelState.AddModelError("ExamsError", "Šablono keisti negalima, nes prie jo jau yra priskirta kandidatų.");
                return RedirectToAction("Edit", "TaskParameters", new { id });
            }
            else
            {
                var exam = await _context.Exams.FindAsync(id);
                if (id != exam.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    for (int i = 0; i < tasksString.Length; i++)
                    {
                        tasks[i] = double.Parse(tasksString[i], System.Globalization.CultureInfo.InvariantCulture);
                    }
                    TaskParametersMappings.EditTaskParametersEntity(exam, tasks);
                    await _context.SaveChangesAsync();
                    return Redirect($"/Exams/Index?value={SelectedAcceptancePeriod}");
                }
                return View(TaskParametersMappings.ToTaskParameterViewModel(tasks));
            }
        }
    }
}