﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class StudentExercise
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        [Required]
        public int InstructorId { get; set; }
    }
}
