﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PRIS.Web.Models
{
    public class ResultViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "1.1")]
        public double Task1_1 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "1.2")]
        public double Task1_2 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "1.3")]
        public double Task1_3 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "2.1")]
        public double Task2_1 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "2.2")]
        public double Task2_2 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "2.3")]
        public double Task2_3 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "3.1")]
        public double Task3_1 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "3.2")]
        public double Task3_2 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "3.3")]
        public double Task3_3 { get; set; }
        [Required(ErrorMessage = "Įrašykite užduoties įvertinimą")]
[Display(Name = "3.4")]
        public double Task3_4 { get; set; }
        [Display(Name = "Testo balai")]
        public double? FinalPoints { get; set; }
        public double? PercentageGrade { get; set; }
        public string CommentResult { get; set; }
        public int? StudentForeignKey { get; set; }
    }
}
