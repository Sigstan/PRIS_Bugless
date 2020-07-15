﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PRIS.Core.Library.Entities;
using PRIS.Web.Data;
using PRIS.Web.Mappings;
using PRIS.Web.Models;
using PRIS.Web.Storage;

namespace PRIS.Web.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IRepository _repository;


        public StudentsController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            TempData["ExamId"] = id;
            var studentRequest = _repository.Query<Student>()
                .Include(x => x.Result)
                .Include(x => x.ConversationResult)
                .Where(x => x.Id > 0);
            var students = await studentRequest.Where(x => x.Result.Exam.Id == id).ToListAsync();
            if (students == null)
            {
                return NotFound();
            }
            var studentViewModels = new List<StudentsResultViewModel>();
            students.ForEach(x => studentViewModels.Add(StudentsMappings.ToViewModel(x, x.Result)));
            foreach (var item in students)
            {
                foreach (var student in studentViewModels)
                {
                    student.FinalPoints = student.Tasks?.Sum(x => x) ?? 0;
                    var examDraft = _repository.Query<Exam>().FirstOrDefault(e => e.Id == student.ExamId);
                    double maxPoints = TaskParametersMappings.ToTaskParameterViewModel(examDraft).Tasks.Sum(x => x);
                    student.PercentageGrade = student.FinalPoints * 100 / maxPoints;
                }
            }

            studentViewModels = studentViewModels.OrderByDescending(x => x.FinalPoints).ToList();

            return View(studentViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int[] HasPassedExam)
        {
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);
            var backToExam = RedirectToAction("Index", "Students", new { id = ExamId });
            if (ModelState.IsValid)
            {
                var students = await _repository.Query<Student>()
                    .Include(x => x.Result)
                    .Include(x => x.ConversationResult)
                    .Where(x => x.Id > 0)
                    .Where(x => x.Result.Exam.Id == ExamId)
                    .ToListAsync();
                students.ForEach(x => x.PassedExam = false);

                for (int i = 0; i < HasPassedExam.Length; i++)
                {
                    var findStudents = students.FirstOrDefault(x => x.Id == HasPassedExam[i]);
                    findStudents.PassedExam = true;
                }

                await _repository.SaveAsync();
                return RedirectToAction("Index", "ConversationResults", new { id = ExamId });
            }
            return backToExam;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, FirstName, LastName, Email, PhoneNumber, Gender, Comment")] StudentViewModel studentViewModel)
        {
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);
            if (ModelState.IsValid)
            {
                var student = StudentsMappings.ToEntity(studentViewModel);
                Result result = new Result
                {
                    ExamId = ExamId,
                };
                result = await _repository.InsertAsync<Result>(result);
                result.Student = student;
                student.Result = result;
                student.ResultId = result.Id;
                await _repository.InsertAsync<Student>(student);

                return RedirectToAction("Index", "Students", new { id = ExamId });
            }
            return RedirectToAction("Index", "Students", new { id = ExamId });
        }

        public async Task<IActionResult> Delete(int? id, bool examPassed)
        {
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);
            if (id == null)
            {
                return NotFound();
            }
            if (!examPassed)
            {
                if (_repository.Exists<Student>(id))
                {
                    var student = await _repository.Query<Student>()
                                                     .Include(x => x.Result)
                                                     .SingleOrDefaultAsync(x => x.Id == id);
                    await _repository.DeleteAsync<Student>(id);
                    if (student.ResultId != null)
                        await _repository.DeleteAsync<Result>(student.ResultId);
                }
                else
                {
                    ModelState.AddModelError("StudentDelete", "Toks studentas neegzistuoja.");
                    TempData["ErrorMessage"] = "Toks studentas neegzistuoja.";
                    return RedirectToAction("Index", "Students", new { id = ExamId });
                }
                return RedirectToAction("Index", "Students", new { id = ExamId });
            }
            else
            {
                ModelState.AddModelError("StudentDelete", "Į pokalbį pakviesto kandidato ištrinti negalima.");
                TempData["ErrorMessage"] = "Į pokalbį pakviesto kandidato ištrinti negalima.";
                return RedirectToAction("Index", "Students", new { id = ExamId });
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _repository.FindByIdAsync<Student>(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(StudentsMappings.ToViewModel(student));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, Email, PhoneNumber, Gender, Comment")] StudentViewModel studentViewModel)
        {
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);
            var student = await _repository.FindByIdAsync<Student>(id);
            StudentsMappings.ToEntity(student, studentViewModel);

            if (id != student.Id)
            {
                return NotFound();
            }
            if(student.StudentDataLocked == true)
            {
                ModelState.AddModelError("StudentEdit", "Studento duomenys yra užrakinti, redaguoti studento negalima");
                TempData["ErrorMessage"] = "Studento duomenys yra užrakinti, redaguoti studento negalima";
                return RedirectToAction("Edit", "Students", new { id = student.Id });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.Exists<Student>(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Students", new { id = ExamId });
            }
            return View(student);
        }

        public async Task<IActionResult> EditResult(int? id, int? resultId)
        {
            TempData["ResultId"] = resultId;
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);

            if (id == null)
            {
                return NotFound();
            }
            var studentRequest = _repository.Query<Student>().Include(x => x.Result).Where(x => x.Id == id);
            var studentEntity = await studentRequest.FirstOrDefaultAsync();

            var resultEntity = await _repository.FindByIdAsync<Result>(studentEntity.Result.Id);

            if (studentEntity == null)
            {
                return NotFound();
            }
            var exam = await _repository.FindByIdAsync<Exam>(ExamId);
            var studentViewModel = StudentsMappings.ToViewModel(studentEntity, resultEntity);
            if (resultEntity.Tasks == null)
                studentViewModel.Tasks = new double[JsonSerializer.Deserialize<double[]>(exam.Tasks).Length];
            TempData["ExamId"] = ExamId;
            return View(studentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResult(double[] Tasks)
        {
            int.TryParse(TempData["ResultId"].ToString(), out int resultId);
            int.TryParse(TempData["ExamId"].ToString(), out int ExamId);
            if (ModelState.IsValid)
            {
                try
                {
                    var studentRequest = _repository.Query<Student>().Include(x => x.Result).ThenInclude(y => y.Exam).Where(x => x.Id > 0);
                    var result = await _repository.FindByIdAsync<Result>(resultId);
                    var student = await studentRequest.FirstOrDefaultAsync(x => x.Id == result.StudentForeignKey);
                    var studentResultViewModel = StudentsMappings.ToStudentsResultViewModel(Tasks);
                    var examTasks = StudentsMappings.ToStudentsResultViewModel(result).Tasks;
                    if (student.PassedExam)
                    {
                        TempData["ErrorMessage"] = "Studentas yra pakviestas į pokalbį, todėl jo duomenų negalima redaguoti.";
                        return RedirectToAction("EditResult", "Students", new { resultId });
                    }
                    var testToDelete = examTasks.Select((x, i) => x < Tasks[i]);

                    var isInvalid = examTasks.Select((x, i) => x < Tasks[i]).Any(x => x);
                    if (isInvalid)
                    {
                        ModelState.AddModelError("EditResult", "Užduoties balas negali būti didesnis nei testo šablono balas");
                        TempData["ErrorMessage"] = "Užduoties balas negali būti didesnis nei testo šablono balas";
                        return RedirectToAction("EditResult", "Students", new { resultId });
                    }

                    StudentsMappings.ToResultEntity(result, studentResultViewModel);
                    await _repository.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index", "Students", new { id = ExamId });
            }
            return RedirectToAction("Index", "Students", new { id = ExamId });
        }
    }
}
