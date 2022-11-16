using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YazGelProje.Models;

namespace YazGelProje.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AdminLogin()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            var context = new MyContext();
            var adm = context.Admins.FirstOrDefault(x => x.AdminName == admin.AdminName && x.Password == admin.Password);
            if (adm != null)
            {
                FormsAuthentication.SetAuthCookie(admin.AdminName, false);
                Session["AdminName"] = admin.AdminName;
                return RedirectToAction("Home", "Admin");
            }
            else
            {
                ViewData["error"] = "Kullanıcı Adı veya Parola Hatalı";
                return View();
            }
        }

        public ActionResult AdminLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("AdminLogin", "Login");
        }

        [HttpGet]
        public ActionResult SuperAdminLogin()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SuperAdminLogin(SuperAdmin superAdmin)
        {
            var context = new MyContext();
            var spradmin = context.SuperAdmins.FirstOrDefault(x => x.Name == superAdmin.Name && x.Password == superAdmin.Password);
            if (spradmin != null)
            {
                FormsAuthentication.SetAuthCookie(superAdmin.Name, false);
                Session["Name"] = superAdmin.Name;
                return RedirectToAction("Home", "SuperAdmin");
            }
            else
            {
                ViewData["error"] = "Kullanıcı Adı veya Parola Hatalı";
                return View();
            }
        }

        public ActionResult SuperAdminLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SuperAdminLogin", "Login");
        }

        [HttpGet]
        public ActionResult TeacherLogin()
        {
            return View();
        }


        [HttpPost]
        public ActionResult TeacherLogin(Teacher teacher)
        {
            var context = new MyContext();
            var teacherIn = context.Teachers.FirstOrDefault(x => x.SicilNo == teacher.SicilNo && x.Password == teacher.Password);

            if (teacherIn==null)
            {
                ViewData["error"] = "Sicil No veya Parola Hatalı";
                return View();
            }
            else
            {
                var teacherRole = context.Teachers.Where(x => x.TeacherId == teacherIn.TeacherId).Select(x => x.Commission.Title).FirstOrDefault();
                var off = context.SemesterStarts.Select(x => x.Case).FirstOrDefault();
                if (off == "Kapalı")
                {
                    return RedirectToAction("Alert", "Login");
                }
                else
                {
                    if (teacherRole=="Komisyon")
                    {
                        FormsAuthentication.SetAuthCookie(teacher.SicilNo, false);
                        Session["SicilNo"] = teacher.SicilNo;
                        return RedirectToAction("Home", "Teacher");
                    }
                    else
                    {
                        return RedirectToAction("TeacherAlert", "Login");
                    }
                }
            }
            return View();
        }

        public ActionResult TeacherLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("TeacherLogin", "Login");
        }

        [HttpGet]
        public ActionResult StudentLogin()
        {
            return View();
        }


        [HttpPost]
        public ActionResult StudentLogin(Student student)
        {
            var context = new MyContext();
            var off = context.SemesterStarts.Select(x => x.Case).FirstOrDefault();
            if (off=="Kapalı")
            {
                return RedirectToAction("Alert","Login");
            }
            else
            {
                var adm = context.Students.FirstOrDefault(x => x.StudentNumber == student.StudentNumber && x.Password == student.Password);
                if (adm != null)
                {
                    FormsAuthentication.SetAuthCookie(student.StudentNumber, false);
                    Session["StudentNumber"] = student.StudentNumber;
                    return RedirectToAction("Home", "Student");
                }
                else
                {
                    ViewData["error"] = "Öğrenci No veya Parola Hatalı";
                    return View();
                }
            }
            return View();
        }

        public ActionResult StudentLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("StudentLogin", "Login");
        }

        public ActionResult Alert()
        {
            return View();
        }

        public ActionResult TeacherAlert()
        {
            return View();
        }

    }
}