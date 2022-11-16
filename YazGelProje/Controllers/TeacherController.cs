using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YazGelProje.Models;

namespace YazGelProje.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        public class studentModel
        {
            public List<Student> students { get; set; }
            public List<InternCase> internCases { get; set; }

        }
        // GET: Teacher
        public ActionResult Home()
        {
            var context = new MyContext();
            var data = context.Students.ToList();
            return View(data);
        }

        [HttpGet]
        [ActionName("ProfileEdit")]
        public ActionResult ProfileEdit()
        {
            var context = new MyContext();
            string sicilNo = User.Identity.Name;
            int teacherId = context.Teachers.Where(x => x.SicilNo == sicilNo).Select(x => x.TeacherId).FirstOrDefault();

            var data = context.Teachers.Where(x => x.TeacherId == teacherId).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        [ActionName("ProfileEdit")]
        public ActionResult ProfileEdit(string name, string surname,string email)
        {
            var context = new MyContext();
            string sicilNo = User.Identity.Name;
            int teacherId = context.Teachers.Where(x => x.SicilNo == sicilNo).Select(x => x.TeacherId).FirstOrDefault();
            var data = context.Teachers.Where(x => x.TeacherId == teacherId).FirstOrDefault();
            Teacher teacher = context.Teachers.Where(x => x.TeacherId == teacherId).FirstOrDefault();

            teacher.Name = name;
            teacher.SurName = surname;
            teacher.Email = email;

            context.Entry(teacher).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            ViewBag.Mesaj = "Bilgileriniz güncellendi";

            return View(data);
        }

        [HttpGet]
        [ActionName("InternStartFiles")]
        public ActionResult InternStartFiles()
        {
            var context = new MyContext();
            var kullaniciresult = context.Students.Where(x => x.ToApply == false || x.ToApply==null).ToList();

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