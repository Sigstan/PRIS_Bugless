﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using PRIS.Core.Library.Entities;
using PRIS.Web.Models.InvitationToStudyModel;
using PRIS.Web.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Controllers
{
    public class StudentInvitationToStudyController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger<StudentInvitationToStudyController> _logger;
        private readonly string _user;

        public StudentInvitationToStudyController(IRepository repository, ILogger<StudentInvitationToStudyController> logger, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _logger = logger;
            _user = httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
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

            var model = StudentData.PrepareData(examId, courseId, cityId, searchString, sortOrder, ref students);
            _logger.LogInformation($"{students.Count} students on the view. Filters were used {examId}, {courseId}, {cityId}. User {_user}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int[] studentId, int[] HasInvitedToStudy)
        {
            if (ModelState.IsValid)
            {
                var students = new List<Student>();
                for (int i = 0; i < studentId.Length; i++)
                {
                    students.Add(await _repository.FindByIdAsync<Student>(studentId[i]));
                }


                students.ForEach(x => x.InvitedToStudy = false);
                for (int i = 0; i < HasInvitedToStudy.Length; i++)
                {
                    var findStudents = students.FirstOrDefault(x => x.Id == HasInvitedToStudy[i]);
                    findStudents.InvitedToStudy = true;
                    _logger.LogInformation($"Student {findStudents.Id} id was invited to study by {_user}");

                }
                await _repository.SaveAsync();
                _logger.LogInformation($"New data was saved successfully by {_user}");
                TempData["SuccessMessage"] = "Duomenys sėkmingai išsaugoti";
                var currentUrl = HttpContext.Request.Headers["Referer"];
                return Redirect(currentUrl);
            }
            _logger.LogError($"Error with ModelState validation. User {_user}");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditComment(int? id)
        {
            var student = await _repository.FindByIdAsync<Student>(id);
            StudentComment studentModel = new StudentComment
            {
                StudentFirstName = student.FirstName,
                StudentLastName = student.LastName,
                Comment = student.Comment,
            };
            return View(studentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(StudentComment model)
        {
            if (ModelState.IsValid)
            {
                var student = await _repository.FindByIdAsync<Student>(model.Id);
                student.Comment = model.Comment;
                await _repository.SaveAsync();

            _logger.LogInformation($"User {_user} edited comment of student with {student.Id}");
                var currentUrl = HttpContext.Request.Headers["Referer"];
                return Redirect(currentUrl);

            }
            _logger.LogError($"Error with ModelState validation. User {_user}");
            return RedirectToAction("Index", "Home");
        }
    }
}
