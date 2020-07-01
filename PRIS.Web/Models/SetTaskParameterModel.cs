﻿using PRIS.Core.Library.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Models
{
    public class SetTaskParameterModel
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public DateTime Date { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task1_1 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task1_2 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task1_3 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task2_1 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task2_2 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task2_3 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task3_1 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task3_2 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task3_3 { get; set; }
        [Range(double.MinValue, 10, ErrorMessage = "Įveskite balą iki 10")]
        [Required]
        public double Task3_4 { get; set; }
    }
}
