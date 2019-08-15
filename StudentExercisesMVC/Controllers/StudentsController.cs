using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using StudentExercises.Models.ViewModels;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        // GET: Students
        public ActionResult Index()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, SlackHandle, CohortId
                        FROM Student
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        students.Add(new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                        });
                    }

                    reader.Close();
                }
            }

            return View(students);
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            var student = GetOneStudent(id);
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            var viewModel = new StudentCreateViewModel();
            var cohorts = GetAllCohorts();
            var selectItems = cohorts
                .Select(cohort => new SelectListItem
                {
                    Text = cohort.Name,
                    Value = cohort.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
            viewModel.Cohorts = selectItems;
            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   INSERT INTO Student (
                                   FirstName,
                                   LastName,
                                   SlackHandle,
                                   CohortId
                                  )  Values (
                                    @firstName,
                                    @lastName,
                                    @slackHandle,
                                    @cohortId
                                   )
                                ";

                        cmd.Parameters.AddWithValue("@firstName", student.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", student.LastName);
                        cmd.Parameters.AddWithValue("@slackHandle", student.SlackHandle);
                        cmd.Parameters.AddWithValue("@cohortId", student.CohortId);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //GET
        public ActionResult AddExercise(int id)
        {
            var viewModel = new StudentAddExerciseViewModel();

            var student = GetOneStudent(id);
            var instructors = GetAllInstructors();
            var iselectItems = instructors
                .Select(instructor => new SelectListItem
                {
                    Text = instructor.FullName,
                    Value = instructor.Id.ToString()
                })
                .ToList();

            iselectItems.Insert(0, new SelectListItem
            {
                Text = "Choose instructor...",
                Value = "0"
            });

            var exercises = GetAllExercises();
            var eselectItems = exercises
                .Select(exercise => new SelectListItem
                {
                    Text = exercise.Name,
                    Value = exercise.Id.ToString()
                })
                .ToList();

            eselectItems.Insert(0, new SelectListItem
            {
                Text = "Choose exercise...",
                Value = "0"
            });

            viewModel.Exercises = eselectItems;
            viewModel.Student = student;
            viewModel.Instructors = iselectItems;

            return View(viewModel);
        }

        //CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExercise(int id, StudentExercise table)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   INSERT INTO StudentExercise (
                                   ExerciseId,
                                   StudentId,
                                   InstructorId
                                  )  Values (
                                    @studentId,
                                    @exerciseId,
                                    @instructorId
                                   )
                                ";

                        cmd.Parameters.AddWithValue("@studentId", id);
                        cmd.Parameters.AddWithValue("@exerciseId", table.ExerciseId);
                        cmd.Parameters.AddWithValue("@instructorId", table.InstructorId);

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new StudentCreateViewModel();
            var cohorts = GetAllCohorts();
            var selectItems = cohorts
                .Select(cohort => new SelectListItem
                {
                    Text = cohort.Name,
                    Value = cohort.Id.ToString()
                })
                .ToList();

            selectItems.Insert(0, new SelectListItem
            {
                Text = "Choose cohort...",
                Value = "0"
            });
            viewModel.Cohorts = selectItems;
            var student = GetOneStudent(id);
            viewModel.Student = student;
            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   UPDATE Student 
                                    SET FirstName = @firstName,
                                        LastName = @lastName,
                                        SlackHandle = @slackHandle,
                                        CohortId = @cohortId
                                    WHERE Id = @id";

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@firstName", student.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", student.LastName);
                        cmd.Parameters.AddWithValue("@slackHandle", student.SlackHandle);
                        cmd.Parameters.AddWithValue("@cohortId", student.CohortId);
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            var student = GetOneStudent(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                    DELETE FROM StudentExercise WHERE StudentId = @id
                                    DELETE FROM Student WHERE Id = @id
                                     ";
                                    

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }

        private Student GetOneStudent(int id)
        {
            Student student = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, SlackHandle, CohortId
                        FROM Student
                        WHERE Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();



                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };
                    }

                    reader.Close();
                }
            }
            return(student);
        }

        private List<Instructor> GetAllInstructors()
        {
            List<Instructor> instructors = new List<Instructor>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, SlackHandle, Specialty, CohortId
                        FROM Instructor
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        instructors.Add(new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                        });
                    }

                    reader.Close();
                }
            }

            return instructors;
        }

        private List<Exercise> GetAllExercises()
        {
            List<Exercise> exercises = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Language
                        FROM Exercise
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        exercises.Add(new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        });
                    }

                    reader.Close();
                }
            }

            return (exercises);
        }
    }
}