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
        SqlCommand komut;

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
        public ActionResult ProfileEdit(string tc,string name, string surname, string adress, string phonenumber)
        {
            var context = new MyContext();

                string studentNumber = User.Identity.Name;
                int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
                var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
                Student student = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

                student.Name = name;
                student.Surname = surname;
                student.PhoneNumber = phonenumber;
                student.Adress = adress;

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
                        student.InternCaseId = 6;
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
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            Student kl = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
            kl.InternCaseId = 4;
            context.InternBookToGives.Remove(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

        [HttpGet]
        [ActionName("PasswordEdit")]
        public ActionResult PasswordEdit()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();

            var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        [ActionName("PasswordEdit")]
        public ActionResult PasswordEdit(string password, string confirmPassword)
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
            Student student = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            student.Password = password;

            if (confirmPassword==password)
            {
                context.Entry(student).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                ViewBag.Mesaj = "Parolanız güncellendi";

                return View(data);
            }
            else
            {
                ViewBag.Hata = "Parola uyumlu değil.";
                return View();
            }

        }

        [HttpGet]
        [ActionName("ConfirmPassword")]
        public ActionResult ConfirmPassword()
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();

            var data = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        [ActionName("ConfirmPassword")]
        public ActionResult ConfirmPassword(string password)
        {
            var context = new MyContext();
            string studentNumber = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == studentNumber).Select(x => x.StudentId).FirstOrDefault();
            var studentPassword = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();

            if (password==studentPassword.Password)
            {
                return RedirectToAction("PasswordEdit", "Student");
            }
            else
            {
                ViewBag.Mesaj = "Hatalı Parola";
                return View();
            }
        }

        [HttpGet]
        [ActionName("FormUpload")]
        public ActionResult FormUpload()
        {
            var context = new MyContext();
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternForms.Where(x => x.StudentId == studentId).OrderByDescending(x => x.FileDate).Take(5);
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
        [ActionName("FormUpload")]
        public ActionResult FormUpload(IEnumerable<HttpPostedFileBase> files, InternForm sb)
        {
            var context = new MyContext();
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            var list = context.InternForms.Where(x => x.StudentId == studentId).OrderByDescending(x => x.FileDate).Take(5);
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

                        context.InternForms.Add(sb);
                        context.SaveChanges();

                        kl.InternCaseId = 6;
                        context.Entry(kl).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                    }
                }
            }
            return View(list);
        }
        public FileResult FormDownload(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/InternFiles/Staj/" + file + ""));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }

        public ActionResult FormShow(string dosya)
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

        public void FormRemove(int id)
        {
            var context = new MyContext();
            var data = context.InternForms.Where(m => m.FileId == id).FirstOrDefault();
            var bookName = context.InternForms.Where(m => m.FileId == id).Select(x => x.FileName).FirstOrDefault();
            if (bookName != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/InternBooks/Books/" + bookName)))
                {
                    System.IO.File.Delete(Server.MapPath("~/InternBooks/Books/" + bookName));
                }
            }
            string number = User.Identity.Name;
            int studentId = context.Students.Where(x => x.StudentNumber == number).Select(x => x.StudentId).FirstOrDefault();
            Student kl = context.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
            kl.InternCaseId = 4;
            context.InternForms.Remove(data);
            context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
            context.SaveChanges();
        }

    }
}