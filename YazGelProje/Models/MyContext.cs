using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class MyContext:DbContext
    {
        public MyContext()
        {

        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<SemesterStart> SemesterStarts { get; set; }
        public DbSet<InternFile> InternFiles { get; set; }
        public DbSet<InternCase> InternCases { get; set; }
        public DbSet<InternBookToGive> InternBookToGives { get; set; }
        public DbSet<FileModel> FileModels { get; set; }
        public DbSet<InternForm> InternForms { get; set; }
    }
}