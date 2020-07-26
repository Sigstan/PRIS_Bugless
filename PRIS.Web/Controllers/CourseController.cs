﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using PRIS.Core.Library.Entities;
using PRIS.Web.Mappings;
using PRIS.Web.Models.CourseModels;
using PRIS.Web.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CourseController : Controller
    {
        private readonly IRepository _repository;

        public CourseController(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index(string examId, string courseId, string cityId, string searchString, string sortOrder)
        {
            var students = await _repository.Query<Student>()
                .Include(x => x.ConversationResult)
                .Include(x => x.Result)
                .ThenInclude(x => x.Exam)
                .ThenInclude(x => x.City)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.Course)
                .Where(x => x.PassedExam == true)
                .ToListAsync();

            var model = StudentStatisticsData.PrepareData(examId, courseId, cityId, searchString, sortOrder, ref students);

            return View(model);
        }

        public async Task<IActionResult> LockingOfStudentData(string examId, string courseId, string cityId, string searchString, string sortOrder)
        {
            var students = await _repository.Query<Student>()
                .Include(x => x.ConversationResult)
                .Include(x => x.Result)
                .ThenInclude(x => x.Exam)
                .ThenInclude(x => x.City)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.Course)
                .Where(x => x.InvitedToStudy == true)
                .ToListAsync();

            var model = StudentLockingData.PrepareData(examId, courseId, cityId, searchString, sortOrder, ref students);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockingOfStudentData(int[] studentId, int[] HasSignedAContract, int[] HasStudentDataLocked)
        {
            if (ModelState.IsValid)
            {

                var students = new List<Student>();
                for (int i = 0; i < studentId.Length; i++)
                {
                    students.Add(await _repository.FindByIdAsync<Student>(studentId[i]));
                }
                students.ForEach(x => x.SignedAContract = false);
                for (int i = 0; i < HasSignedAContract.Length; i++)
                {
                    var findStudents = students.FirstOrDefault(x => x.Id == HasSignedAContract[i]);
                    findStudents.SignedAContract = true;
                }

                var studentsForDataLock = new List<Student>();
                for (int i = 0; i < studentId.Length; i++)
                {
                    studentsForDataLock.Add(await _repository.FindByIdAsync<Student>(studentId[i]));
                }
                studentsForDataLock.ForEach(x => x.StudentDataLocked = false);
                for (int i = 0; i < HasStudentDataLocked.Length; i++)
                {
                    foreach (var student in studentsForDataLock)
                    {
                        if (student.SignedAContract == true)
                        {
                            var findStudents = studentsForDataLock.FirstOrDefault(x => x.Id == HasStudentDataLocked[i]);
                            if (findStudents.SignedAContract == true)
                            {
                                findStudents.StudentDataLocked = true;
                            }
                            else
                            {
                                ModelState.AddModelError("StudentDelete", "Kandidatas turi būti pasirašęs sutartį, kad būtų galima užrakinti kandidato duomenis");
                                TempData["ErrorMessage"] = "Kandidatas turi būti pasirašęs sutartį, kad būtų galima užrakinti kandidato duomenis";
                            }
                        }
                    }
                }
                await _repository.SaveAsync();
                var currentUrl = HttpContext.Request.Headers["Referer"];
                return Redirect(currentUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
