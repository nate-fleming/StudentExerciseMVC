﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentAddExerciseViewModel
    {
        public Student Student { get; set; }

        public int ExerciseId { get; set; }

        public int InstructorId { get; set; }

        public List<SelectListItem> Instructors { get; set; }

        public List<SelectListItem> Exercises { get; set; }
    }
}
