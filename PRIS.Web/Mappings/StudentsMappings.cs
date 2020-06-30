﻿using PRIS.Core.Library.Entities;
using PRIS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Mappings
{
    public class StudentsMappings
    {
        public static Student ToEntity(StudentViewModel model)
        {
            return new Student
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Comment = model.Comment,
                PassedExam = model.PassedExam
            };
        }

        public static Result ToResultEntity(StudentsResultViewModel model)
        {
            return new Result
            {
                Id = model.ResultId,
                Task1_1 = model.Task1_1,
                Task1_2 = model.Task1_2,
                Task1_3 = model.Task1_3,
                Task2_1 = model.Task2_1,
                Task2_2 = model.Task2_2,
                Task2_3 = model.Task2_3,
                Task3_1 = model.Task3_1,
                Task3_2 = model.Task3_2,
                Task3_3 = model.Task3_3,
                Task3_4 = model.Task3_4,
                Comment = model.CommentResult,
                StudentForeignKey = model.StudentForeignKey
            };

        }
        public static StudentViewModel ToViewModel(Student entity)
        {
            return new StudentViewModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Gender = entity.Gender,
                Comment = entity.Comment,
                PassedExam = entity.PassedExam
            };
        }
        public static StudentsResultViewModel ToStudentsResultViewModel(Student entity)
        {
            return new StudentsResultViewModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Gender = entity.Gender,
                Comment = entity.Comment,
                PassedExam = entity.PassedExam
            };
        }

        public static StudentsResultViewModel ToViewModel(Student studentEntity, StudentsResultViewModel studentResultViewModel)
        {
            return new StudentsResultViewModel
            {
                Id = studentEntity.Id,
                FirstName = studentEntity.FirstName,
                LastName = studentEntity.LastName,
                Email = studentEntity.Email,
                PhoneNumber = studentEntity.PhoneNumber,
                Gender = studentEntity.Gender,
                Comment = studentEntity.Comment,
                PassedExam = studentEntity.PassedExam,
                ResultId = studentResultViewModel.Id,
                Task1_1 = studentResultViewModel.Task1_1,
                Task1_2 = studentResultViewModel.Task1_2,
                Task1_3 = studentResultViewModel.Task1_3,
                Task2_1 = studentResultViewModel.Task2_1,
                Task2_2 = studentResultViewModel.Task2_2,
                Task2_3 = studentResultViewModel.Task2_3,
                Task3_1 = studentResultViewModel.Task3_1,
                Task3_2 = studentResultViewModel.Task3_2,
                Task3_3 = studentResultViewModel.Task3_3,
                Task3_4 = studentResultViewModel.Task3_4,
                CommentResult = studentResultViewModel.Comment,
                StudentForeignKey = studentResultViewModel.StudentForeignKey
            };
        }

        public static StudentsResultViewModel ToViewModel(Student studentEntity, Result resultEntity)
        {
            return new StudentsResultViewModel
            {
                Id = studentEntity.Id,
                FirstName = studentEntity.FirstName,
                LastName = studentEntity.LastName,
                Email = studentEntity.Email,
                PhoneNumber = studentEntity.PhoneNumber,
                Gender = studentEntity.Gender,
                Comment = studentEntity.Comment,
                PassedExam = studentEntity.PassedExam,
                ResultId = resultEntity.Id,
                Task1_1 = resultEntity.Task1_1,
                Task1_2 = resultEntity.Task1_2,
                Task1_3 = resultEntity.Task1_3,
                Task2_1 = resultEntity.Task2_1,
                Task2_2 = resultEntity.Task2_2,
                Task2_3 = resultEntity.Task2_3,
                Task3_1 = resultEntity.Task3_1,
                Task3_2 = resultEntity.Task3_2,
                Task3_3 = resultEntity.Task3_3,
                Task3_4 = resultEntity.Task3_4,
                CommentResult = resultEntity.Comment,
                StudentForeignKey = resultEntity.StudentForeignKey
            };
        }

        //public static StudentViewModel ToResultViewModel(Result entity)
        //{
        //    return new StudentViewModel
        //    {
        //        ResultId = entity.Id,
        //        Task1_1 = entity.Task1_1,
        //        Task1_2 = entity.Task1_2,
        //        Task1_3 = entity.Task1_3,
        //        Task2_1 = entity.Task2_1,
        //        Task2_2 = entity.Task2_2,
        //        Task2_3 = entity.Task2_3,
        //        Task3_1 = entity.Task3_1,
        //        Task3_2 = entity.Task3_2,
        //        Task3_3 = entity.Task3_3,
        //        Task3_4 = entity.Task3_4,
        //        CommentResult = entity.Comment,
        //        StudentForeignKey = entity.StudentForeignKey
        //    };
        //}
    }
}
