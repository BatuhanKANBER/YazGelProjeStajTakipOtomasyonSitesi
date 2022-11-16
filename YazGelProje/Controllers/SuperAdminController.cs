using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YazGelProje.Models;

namespace YazGelProje.Controllers
{
    [Authorize]
    public class SuperAdminController : Controller
    {
        public class studentModel
        {
            public List<Student> students { get; set; }
            public List<InternCase> internCases { get; set; }

        }

        // GET: SuperAdmim

        [HttpGet]
        [ActionName("Home")]
        public ActionResult Home()
        {
            var context = new MyContext();
            var data = context.SemesterStarts.Where(x => x.Id == 1).FirstOrDefault();
            return View();
        }

        [HttpPost]
        [ActionName("Home")]
        public ActionResult Home(string SemesterStart, string Description)
        {
            var context = new MyContext();
            SemesterStart data = context.SemesterStarts.Where(x => x.Id == 1).FirstOrDefault();
            data.Case = SemesterStart;
            data.Description = Description;

            context.SemesterStarts.Add(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            if (SemesterStart == "Seçiniz")
            {
                ViewBag.Mesaj = "Lütfen staj dönemi bilgisini seçiniz.";
                return View(data);
            }

            ViewBag.Mesaj = "Staj Dönemi "+data.Case;
            return View(data);
        }

        public ActionResult AdminList()
        {
            var context = new MyContext();
            var adm = context.Admins.ToList();
            return View(adm);
        }

        [HttpGet]
        [ActionName("AdminAdd")]
        public ActionResult AdminAdd()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AdminAdd")]
        public ActionResult AdminAdd(Admin admin)
        {
            var context = new MyContext();
            UpdateModel(admin);
            context.Admins.Add(admin);
            context.SaveChanges();
            return RedirectToAction("AdminList", "SuperAdmin");


        }

        [HttpGet]
        [ActionName("AdminEdit")]
        public ActionResult AdminEdit(int? id)
        {
            var context = new MyContext();
            var model = context.Admins.Find(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("AdminEdit")]
        public ActionResult AdminEdit(int? id, Admin admin)
        {
            var context = new MyContext();
            var model = context.Admins.Find(id);
            model.AdminName = admin.AdminName;
            UpdateModel(model);
            context.SaveChanges();
            return RedirectToAction("AdminList", "SuperAdmin");
        }
        public ActionResult AdminRemove(int? id)
        {
            var context = new MyContext();
            var model = context.Admins.SingleOrDefault(x => x.AdminId == id);
            context.Admins.Remove(model);
            context.SaveChanges();
            return RedirectToAction("AdminList", "SuperAdmin");
        }

        public ActionResult StudentList()
        {
            var context = new MyContext();
            var student = context.Students.ToList();
            return View(student);
        }

        [HttpGet]
        [ActionName("StudentAdd")]
        public ActionResult StudentAdd()
        {
            return View();
        }

        [HttpPost]
        [ActionName("StudentAdd")]
        public ActionResult StudentAdd(Student student)
        {
            var context = new MyContext();
            UpdateModel(student);
            context.Students.Add(student);
            context.SaveChanges();
            return RedirectToAction("StudentList", "SuperAdmin");


        }

        [HttpGet]
        [ActionName("StudentEdit")]
        public ActionResult StudentEdit(int? id)
        {
            var context = new MyContext();
            var model = context.Students.Find(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("StudentEdit")]
        public ActionResult StudentEdit(int? id, Student student)
        {
            var context = new MyContext();
            var model = context.Students.Find(id);
            model.Name = student.Name;
            UpdateModel(model);
            context.SaveChanges();
            return RedirectToAction("StudentList", "SuperAdmin");
        }
        public ActionResult StudentRemove(int? id)
        {
            var context = new MyContext();
            var model = context.Students.SingleOrDefault(x => x.StudentId == id);
            context.Students.Remove(model);
            context.SaveChanges();
            return RedirectToAction("StudentList", "SuperAdmin");
        }

        public ActionResult TeacherList()
        {
            var context = new MyContext();
            var student = context.Teachers.ToList();
            return View(student);
        }

        [HttpGet]
        [ActionName("TeacherAdd")]
        public ActionResult TeacherAdd()
        {
            return View();
        }

        [HttpPost]
        [ActionName("TeacherAdd")]
        public ActionResult TeacherAdd(Teacher teacher)
        {
            var context = new MyContext();
            UpdateModel(teacher);
            context.Teachers.Add(teacher);
            context.SaveChanges();
            return RedirectToAction("TeacherList", "SuperAdmin");


        }

        [HttpGet]
        [ActionName("TeacherEdit")]
        public ActionResult TeacherEdit(int? id)
        {
            var context = new MyContext();
            var model = context.Teachers.Find(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("TeacherEdit")]
        public ActionResult TeacherEdit(int? id, Teacher teacher)
        {
            var context = new MyContext();
            var model = context.Teachers.Find(id);
            model.Name = teacher.Name;
            UpdateModel(model);
            context.SaveChanges();
            return RedirectToAction("TeacherList", "SuperAdmin");
        }
        public ActionResult TeacherRemove(int? id)
        {
            var context = new MyContext();
            var model = context.Teachers.SingleOrDefault(x => x.TeacherId == id);
            context.Teachers.Remove(model);
            context.SaveChanges();
            return RedirectToAction("TeacherList", "SuperAdmin");
        }

        [HttpGet]
        [ActionName("InternFileUpload")]
        public ActionResult InternFileUpload()
        {
            return View(GetFiles());
        }

        [HttpPost]
        [ActionName("InternFileUpload")]
        public ActionResult InternFileUpload(HttpPostedFileBase postedFile)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["MyContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO FileModels VALUES (@Name, @ContentType, @Data)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", postedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View(GetFiles());
        }

        [HttpPost]
        public FileResult DownloadFile(int? fileId)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["MyContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT Name, Data, ContentType FROM FileModels WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", fileId);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["ContentType"].ToString();
                        fileName = sdr["Name"].ToString();
                    }
                    con.Close();
                }
            }

            return File(bytes, contentType, fileName);
        }

        private static List<FileModel> GetFiles()
        {
            List<FileModel> files = new List<FileModel>();
            string constr = ConfigurationManager.ConnectionStrings["MyContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM FileModels"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new FileModel
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = sdr["Name"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return files;
        }

        public ActionResult InternFileRemove(int? id)
        {
            var context = new MyContext();
            var model = context.FileModels.SingleOrDefault(x => x.Id == id);
            context.FileModels.Remove(model);
            context.SaveChanges();
            return RedirectToAction("InternFileUpload", "SuperAdmin");
        }

        [HttpGet]
        [ActionName("InternFileEdit")]
        public ActionResult InternFileEdit(int? id)
        {
            var context = new MyContext();
            var model = context.FileModels.Find(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("InternFileEdit")]
        public ActionResult InternFileEdit(int? id, FileModel file)
        {
            var context = new MyContext();
            var model = context.FileModels.Find(id);
            model.Name = file.Name;
            UpdateModel(model);
            context.SaveChanges();
            return RedirectToAction("InternFileUpload", "SuperAdmin");
        }

        public ActionResult Authority()
        {
            var context = new MyContext();
            var teacher = context.Teachers.ToList();
            return View(teacher);
        }

        [HttpGet]
        [ActionName("SetAuthority")]
        public ActionResult SetAuthority(int id)
        {
            var context = new MyContext();
            int teacherId = context.Teachers.Where(x => x.TeacherId == id).Select(x => x.TeacherId).FirstOrDefault();
            var data = context.Teachers.Where(x => x.TeacherId == teacherId).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        [ActionName("SetAuthority")]
        public ActionResult SetAuthority(string data, int id)
        {
            if (data == "Seçiniz")
            {
                ViewBag.Mesaj = "Lütfen yetkilendirme bilgisini seçiniz";
                return View("YetkiBelirle");
            }
            var context = new MyContext();
            Teacher teacherRole = context.Teachers.Where(x => x.TeacherId == id).FirstOrDefault();
            var role = context.Commissions.Where(x => x.Title == data).Select(x => x.CommissionId).FirstOrDefault();

            context.Entry(teacherRole).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();

            teacherRole.CommissionId = role;
            teacherRole.TeacherId = id;

            context.Teachers.Add(teacherRole);
            context.SaveChanges();

            ViewBag.Mesaj1 = teacherRole.Name + " " + teacherRole.SurName + " " + "'ın yetkilendirmesi " + data + " olarak seçilmiştir.";
            return View(teacherRole);
        }

        // GET: Teacher
        public ActionResult StudentInfo()
        {
            var context = new MyContext();
            var data = context.Students.ToList();
            return View(data);
        }

        [HttpGet]
        [ActionName("InternStartFiles")]
        public ActionResult InternStartFiles()
        {
            var context = new MyContext();
            var kullaniciresult = context.Students.Where(x => x.ToApply == false || x.ToApply == null).ToList();

            studentModel result = new studentModel
            {
                students = kullaniciresult
            };

            ViewBag.list = context.InternCases.Where(x => x.Hidden == true).ToList();

            //ViewBag.liste = context.StajDurum.ToList();

            return View(result);
        }

        [HttpPost]
        [ActionName("InternStartFiles")]
        public ActionResult InternStartFiles(int? internCaseId, int? StudentID)
        {
            var context = new MyContext();
            var kullaniciresult = context.Students.Where(x => x.ToApply == false || x.ToApply == null).ToList();

            studentModel result = new studentModel
            {
                students = kullaniciresult
            };

            ViewBag.list = context.InternCases.ToList();

            Student st = context.Students.Where(x => x.StudentId == StudentID).FirstOrDefault();

            if (st != null)
            {
                st.InternCaseId = internCaseId;
                context.Entry(st).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return View(result);
            }
            else
            {
                ViewBag.Uyari = "Lütfen öğrenci bilgisini seçiniz.";
                return View(result);
            }
        }

        [HttpGet]
        [ActionName("InternBookFiles")]
        public ActionResult InternBookFiles()
        {
            var context = new MyContext();
            var kullaniciresult = context.Students.Where(x => x.ToApply == false || x.ToApply == null).ToList();

            studentModel result = new studentModel
            {
                students = kullaniciresult
            };

            ViewBag.list = context.InternCases.Where(x => x.Hidden == true).ToList();

            //ViewBag.liste = context.StajDurum.ToList();

            return View(result);
        }

        [HttpPost]
        [ActionName("InternBookFiles")]
        public ActionResult InternBookFiles(int? internCaseId, int? StudentID)
        {
            var context = new MyContext();
            var kullaniciresult = context.Students.Where(x => x.ToApply == false || x.ToApply == null).ToList();

            studentModel result = new studentModel
            {
                students = kullaniciresult
            };

            ViewBag.list = context.InternCases.ToList();

            Student st = context.Students.Where(x => x.StudentId == StudentID).FirstOrDefault();

            if (st != null)
            {
                st.InternCaseId = internCaseId;
                context.Entry(st).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return View(result);
            }
            else
            {
                ViewBag.Uyari = "Lütfen öğrenci bilgisini seçiniz.";
                return View(result);
            }
        }

        [HttpPost]
        public JsonResult InternStartFileTake(int id)
        {
            var context = new MyContext();
            var file = context.InternFiles.Include("Student").Where(m => m.StudentId == id).OrderByDescending(x => x.FileDate).Take(5);

            List<InternFile> son = new List<InternFile>();

            ViewBag.list = context.InternCases.ToList();

            foreach (var item in file)
            {
                son.Add(new InternFile
                {
                    FileId = item.FileId,
                    FileName = item.FileName,
                    StudentId = item.StudentId,
                    FileDate = item.FileDate
                });
            }
            return Json(son);
        }

        [HttpPost]
        public JsonResult InternBookFileTake(int id)
        {
            var context = new MyContext();
            var file = context.InternBookToGives.Include("Student").Where(m => m.StudentId == id).OrderByDescending(x => x.Date).Take(5);

            List<InternBookToGive> son = new List<InternBookToGive>();

            ViewBag.list = context.InternCases.ToList();

            foreach (var item in file)
            {
                son.Add(new InternBookToGive
                {
                    InternBookToGiveId = item.InternBookToGiveId,
                    PageName = item.PageName,
                    StudentId = item.StudentId,
                    Date = item.Date
                });
            }
            return Json(son);
        }

        public JsonResult InternStartCase(int? id)
        {
            var context = new MyContext();
            ViewBag.list = context.InternCases.Where(x => x.Hidden == true).Select(x => x.Case).ToList();
            List<Student> studentList = context.Students.Where(f => f.StudentId == id).OrderBy(f => f.InternCase.Case).ToList();
            List<SelectListItem> itemlist = (from i in studentList
                                             select new SelectListItem
                                             {
                                                 Value = i.InternCaseId.ToString(),
                                                 Text = i.InternCase.Case

                                             }).ToList();
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("InternStudentStartInfo")]
        public ActionResult InternStudentStartInfo()
        {
            var context = new MyContext();
            var data = context.InternStudentStarts.OrderBy(x => x.Date).ToList();

            return View(data);
        }

        [HttpPost]
        [ActionName("InternStudentStartInfo")]
        public ActionResult InternStudentStartInfo(DateTime? startDate, DateTime? finishDate)
        {
            var context = new MyContext();
            var result = context.InternStudentStarts.Where(entry => entry.Date >= startDate.Value).Where(entry => entry.Date <= finishDate.Value).OrderBy(x => x.Date).ToList();

            if (result.Count() == 0)
            {
                ViewBag.Mesaj = "Seçili tarihler arasında kayıt bulunamadı.";
                return View();
            }

            ViewBag.Mesaj1 = startDate.Value.Date.ToString().TrimEnd('0', ':') + " ve " + finishDate.Value.Date.ToString().TrimEnd('0', ':') + " " + " staj başlangıç tarihli öğrencilerin kayıtları listelenmiştir.";
            return View(result);
        }


    }
}