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
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Home()
        {
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

        [HttpGet]
        [ActionName("ProfileEdit")]
        public ActionResult ProfileEdit()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();

            var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        [ActionName("ProfileEdit")]
        public ActionResult ProfileEdit(string tc,string name, string surname, string adress, string email, string studentnumber, string phonenumber, string classs, string password,string faculty, string department)
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
            Student student = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            student.Tc = tc;
            student.Name = name;
            student.Surname = surname;
            student.StudentNumber = studentnumber;
            student.Email = email;
            student.PhoneNumber = phonenumber;
            student.Adress = adress;
            student.Password = password;
            student.Faculty = faculty;
            student.Department = department;
            student.Class = classs;

            context.Entry(student).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            ViewBag.Mesaj = "Bilgileriniz güncellendi";

            return View(data);
        }

        [HttpGet]
        [ActionName("FileUpload")]
        public ActionResult FileUpload()
        {
            var context = new MyContext();
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternFiles.Where(x => x.StudentId == studentId).OrderByDescending(x => x.FileDate).Take(5);
            Student kl = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            if (list.Count() == 0)
            {
                kl.InternCaseId = 5;
                context.Entry(kl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return View(list);
            }

            return View(list);
        }

        [HttpPost]
        [ActionName("FileUpload")]
        public ActionResult FileUpload(IEnumerable<HttpPostedFileBase> files, InternFile sb)
        {
            var context = new MyContext();
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternFiles.Where(x => x.StudentId == studentId).OrderByDescending(x => x.FileDate).Take(5);
            Student kl = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            string gd = Guid.NewGuid().ToString().Substring(0, 8);
            string fName = "";

            if (files != null)
            {
                foreach (var file in files)
                {
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}InternFiles\\", Server.MapPath(@"\")));

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "Staj");

                        var fileName1 = gd + "_" + Path.GetFileName(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, fileName1);
                        file.SaveAs(path);

                        sb.FileName = fileName1;
                        sb.StudentId = studentId;
                        sb.FileDate = DateTime.Now;

                        context.InternFiles.Add(sb);
                        context.SaveChanges();

                        kl.InternCaseId = 3;
                        context.Entry(kl).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                    }
                }
            }
            return View(list);
        }
        public FileResult Download(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/InternFiles/Staj/" + file + ""));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }

        public ActionResult FileShow(string dosya)
        {
            FileInfo file = new FileInfo(Server.MapPath("~/InternFiles/Staj/" + dosya + ""));

            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.Clear();
                Response.TransmitFile(file.FullName);
                Response.End();
            }
            return View();
        }

        public void FileRemove(int id)
        {
            var context = new MyContext();
            var data = context.InternFiles.Where(m => m.FileId == id).FirstOrDefault();
            Student student = context.Students.Where(x => x.StudentId == data.StudentId).FirstOrDefault();

            var file = context.InternFiles.Where(m => m.FileId == id).Select(x => x.FileName).FirstOrDefault();

            if (file != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/InternFiles/Staj/" + file)))
                {
                    System.IO.File.Delete(Server.MapPath("~/InternFiles/Staj/" + file));
                }
            }

            student.ToApply = false;
            context.Entry(student).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            context.InternFiles.Remove(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        public ActionResult InternBook()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();

            var data = context.Interns.Where(x => x.StudentId == studentId && x.Student.InternCaseId == 1).OrderBy(x => x.InternDate).ToList();
            ViewBag.data = data.Count();

            return View(data);
        }

        [HttpGet]
        [ActionName("InternBookUpload")]
        public ActionResult InternBookUpload()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternBookToGives.Where(x => x.StudentId == studentId).ToList();

            return View(list);
        }

        [HttpPost]
        [ActionName("InternBookUpload")]
        public ActionResult InternBookUpload(IEnumerable<HttpPostedFileBase> files, InternBookToGive toGive)
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternBookToGives.Where(x => x.StudentId == studentId).ToList();
            Student student = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            string gd = Guid.NewGuid().ToString().Substring(0, 8);
            string fName = "";

            if (files != null)
            {
                foreach (var file in files)
                {
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}InternBooks\\", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "Books");

                        var fileName1 =gd+"_"+ Path.GetFileName(file.FileName);

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, fileName1);
                        file.SaveAs(path);

                        toGive.PageName = fileName1;
                        toGive.StudentId = studentId;
                        toGive.Date = DateTime.Now;

                        context.InternBookToGives.Add(toGive);
                        context.SaveChanges();
                    }
                }
            }
            return View(list);
        }

        public FileResult InternBookDownload(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/InternBooks/Books/" + file + ""));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }

        public ActionResult InternBookFileShow(string dosya)
        {
            FileInfo file = new FileInfo(Server.MapPath("~/InternBooks/Books/" + dosya + ""));

            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.Clear();
                Response.TransmitFile(file.FullName);
                Response.End();
            }
            return View();
        }

        public void InternBookRemove(int id)
        {
            var context = new MyContext();
            var data = context.InternBookToGives.Where(m => m.InternBookToGiveId == id).FirstOrDefault();
            var bookName = context.InternBookToGives.Where(m => m.InternBookToGiveId == id).Select(x => x.PageName).FirstOrDefault();
            if (bookName != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/InternBooks/Books/" + bookName)))
                {
                    System.IO.File.Delete(Server.MapPath("~/InternBooks/Books/" + bookName));
                }
            }
            context.InternBookToGives.Remove(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }
























        //[HttpGet]
        //[ActionName("InternBookUpload")]
        //public ActionResult InternBookUpload()
        //{
        //    var context = new MyContext();
        //    string studentNumber = User.Identity.Name;
        //    int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
        //    var list = context.InternBookToGives.Where(x => x.StudentId == studentId).ToList();

        //    return View(list);
        //}

        //[HttpPost]
        //[ActionName("InternBookUpload")]
        //public ActionResult InternBookUpload(IEnumerable<HttpPostedFileBase> files, InternBookToGive toGive)
        //{
        //    var context = new MyContext();
        //    string studentNumber = User.Identity.Name;
        //    int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
        //    var list = context.InternBookToGives.Where(x => x.StudentId == studentId).ToList();
        //    Student student = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

        //    string gd = Guid.NewGuid().ToString().Substring(0, 8);
        //    string fName = "";

        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {
        //            fName = file.FileName;
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var originalDirectory = new DirectoryInfo(string.Format("{0}InternBooks\\", Server.MapPath(@"\")));

        //                string pathString = Path.Combine(originalDirectory.ToString(), "Books");

        //                var fileName1 = gd + "_" + Path.GetFileName(file.FileName);

        //                bool isExists = Directory.Exists(pathString);

        //                if (!isExists)
        //                    System.IO.Directory.CreateDirectory(pathString);

        //                var path = string.Format("{0}\\{1}", pathString, fileName1);
        //                file.SaveAs(path);

        //                toGive.PageName = fileName1;
        //                toGive.StudentId = studentId;
        //                toGive.Date = DateTime.Now;

        //                context.InternBookToGives.Add(toGive);
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    return View(list);
        //}

        //public FileResult InternBookDownload(string file)
        //{
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/InternBooks/Books/" + file + ""));
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        //}

        //public ActionResult InternBookFileShow(string dosya)
        //{
        //    FileInfo file = new FileInfo(Server.MapPath("~/InternBooks/Books/" + dosya + ""));

        //    if (file.Exists)
        //    {
        //        Response.ContentType = "application/pdf";
        //        Response.Clear();
        //        Response.TransmitFile(file.FullName);
        //        Response.End();
        //    }
        //    return View();
        //}

        //public void InternBookRemove(int id)
        //{
        //    var context = new MyContext();
        //    var data = context.InternBookToGives.Where(m => m.InternBookToGiveId == id).FirstOrDefault();
        //    var bookName = context.InternBookToGives.Where(m => m.InternBookToGiveId == id).Select(x => x.PageName).FirstOrDefault();
        //    if (bookName != null)
        //    {
        //        if (System.IO.File.Exists(Server.MapPath("~/InternBooks/Books/" + bookName)))
        //        {
        //            System.IO.File.Delete(Server.MapPath("~/InternBooks/Books/" + bookName));
        //        }
        //    }
        //    context.InternBookToGives.Remove(data);
        //    context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
        //    context.SaveChanges();
        //}


























        public static int WorkingCalculation(DateTime startDate, DateTime endDate)
        {
            DateTime date = startDate;
            int dayCount = 0;
            string gun = string.Empty;
            while (date <= endDate)
            {
                gun = date.ToString("dddd");
                if (gun != "Cumartesi" && gun != "Pazar")
                {
                    dayCount++;
                }
                date = date.AddDays(1);
            }
            return dayCount;
        }

        public static int WorkingCalculation1(DateTime startDate, DateTime endDate)
        {
            DateTime date = startDate;
            int dayCount = 0;
            string gun = string.Empty;
            while (date <= endDate)
            {
                gun = date.ToString("dddd");
                if (gun != "Cumartesi" && gun != "Pazar")
                {
                    dayCount++;
                }
                date = date.AddDays(1);
            }
            return dayCount;
        }

        [HttpGet]
        [ActionName("InternStartForm")]
        public ActionResult InternStartForm()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var list = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            return View(list);
        }

        [HttpPost]
        [ActionName("InternStartForm")]
        public ActionResult InternStartForm(string workAdress, string workName, DateTime? startDate, DateTime? endDate, string saturdayWork, InternStudentStart internstdstart)
        {
            var context =new MyContext();
            var holiday = context.Holidays.Select(x => x.HolidayDate).ToList();
            DateTime firstDate = Convert.ToDateTime(startDate);
            DateTime lastDate = Convert.ToDateTime(endDate);
            int holidayCount = 0;

            if (saturdayWork == "Hayır")
            {
                foreach (DateTime isHoliday in holiday) 
                {
                    if ((isHoliday.ToString("dddd") != "Cumartesi" && isHoliday.ToString("dddd") != "Pazar") && (isHoliday >= firstDate && isHoliday <= lastDate))
                    {
                        holidayCount++;
                    }
                }
            }

            if (saturdayWork == "Evet")
            {
                foreach (DateTime isHoliday in holiday)
                {
                    if ((isHoliday.ToString("dddd") != "Pazar") && (isHoliday >= firstDate && isHoliday <= lastDate))
                    {
                        holidayCount++;
                    }
                }
            }
            string studentNumber = User.Identity.Name;
            var studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var list = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
            if (saturdayWork == "Hayır")
            {
                int sonuc = WorkingCalculation(firstDate, lastDate);
                int tsonuc = holidayCount;
                int calismasuresi = sonuc - tsonuc;

                if (calismasuresi > 30 || calismasuresi < 20)
                {
                    ViewBag.Mesaj1 = "Lütfen staj başlangıç ve bitiş tarihinizi yeniden seçiniz. Toplam çalışma gününüz hafta sonu ve resmi tatiller haricinde 20 günden az 30 günden fazla olmamalıdır.";
                    return View(list);
                }
                internstdstart.WeekDayCount = sonuc;
                internstdstart.HolidayCount = tsonuc;
                internstdstart.WorkTime = calismasuresi;
            }
            if (saturdayWork == "Evet")
            {
                int sonuc = WorkingCalculation1(firstDate, lastDate);
                int tsonuc = holidayCount;
                int calismasuresi = sonuc - tsonuc;
                if (calismasuresi > 30 || calismasuresi < 20)
                {
                    ViewBag.Mesaj1 = "Lütfen staj başlangıç ve bitiş tarihinizi yeniden seçiniz. Toplam çalışma gününüz pazar günü ve resmi tatiller haricinde 20 günden az 30 günden fazla olmamalıdır.";
                    return View(list);
                }
                internstdstart.WeekDayCount = sonuc;
                internstdstart.HolidayCount = tsonuc;
                internstdstart.WorkTime = calismasuresi;
            }
            internstdstart.WorkPlaceName = workName;
            internstdstart.InternStart = startDate;
            internstdstart.InternEnd = endDate;
            internstdstart.SaturdayWork = saturdayWork;
            internstdstart.StudentId = studentId;
            internstdstart.Date = DateTime.Now;
            internstdstart.WorkPlaceAdress = workAdress;
            context.InternStudentStarts.Add(internstdstart);
            context.SaveChanges();
            ViewBag.Mesaj = "Staj Başvuru Formunuz Başarıyla Kaydedilmiştir.";
            return View(list);
        }

        [ActionName("MyInternStartForm")]
        public ActionResult MyInternStartForm()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var data = context.InternStudentStarts.Where(x => x.Student.StudentId == studentId).ToList();
            if (data.Count() == 0)
            {
                ViewBag.Mesaj = "Başvurunuz bulunmamaktadır. Lütfen staj başvuru formunu doldurup kaydediniz.";
                return View(data);
            }
            ViewBag.Mesaj1 = "Staj başvuru formunuz sisteme kaydedilmiştir. Kaydedilen başvuru/başvurularınızı aşağıda görebilirsiniz.";
            return View(data);
        }
        public void InternStartFormRemove(int id)
        {
            var context = new MyContext();
            var data = context.InternStudentStarts.Where(m => m.InternStudentStartId == id).FirstOrDefault();
            context.InternStudentStarts.Remove(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

    }
}