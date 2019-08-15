using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;

namespace StudentExercisesMVC.Controllers
{
    public class CohortsController : Controller
    {

        private readonly IConfiguration _config;

        public CohortsController(IConfiguration config)
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


        // GET: Cohorts
        public ActionResult Index()
        {
            List<Cohort> cohorts = new List<Cohort>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name]
                        FROM Cohort
                        ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }

                    reader.Close();
                }
            }

            return View(cohorts);
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int id)
        {
            Cohort cohort = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name]
                        FROM Cohort
                        WHERE Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    reader.Close();
                }
            }

            return View(cohort);
        }

        // GET: Cohorts/Create
        public ActionResult Create()
        {
            Cohort cohort = new Cohort();

            return View(cohort);
        }

        // POST: Cohorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   INSERT INTO Cohort ([Name])  
                                    Values (@name)";

                        cmd.Parameters.AddWithValue("@name", cohort.Name);
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

        // GET: Cohorts/Edit/5
        public ActionResult Edit(int id)
        {
            var cohort = GetOneCohort(id);
            return View(cohort);
        }

        // POST: Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                   UPDATE Cohort 
                                    SET [Name] = @name
                                    WHERE Id = @id";

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", cohort.Name);
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

        // GET: Cohorts/Delete/5
        public ActionResult Delete(int id)
        {
            var cohort = GetOneCohort(id);
            return View(cohort);
        }

        // POST: Cohorts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var students = GetAllStudents();
            var instructors = GetAllInstructors();

            if (students.Any(student => student.CohortId == id) && instructors.Any(instructor => instructor.CohortId == id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {

                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();

                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"
                                    DELETE FROM Cohort WHERE Id = @id";

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
        }

        private Cohort GetOneCohort(int id)
        {
            Cohort cohort = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name]
                        FROM Cohort
                        WHERE Id = @id
                        ";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    reader.Close();
                }
            }

            return cohort;
        }

        private List<Student> GetAllStudents()
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

            return students;
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
    }
}