﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MonthlyStatement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CP25Team04Entities : DbContext
    {
        public CP25Team04Entities()
            : base("name=CP25Team04Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DepartmentReport> DepartmentReports { get; set; }
        public virtual DbSet<DepartmentReportDetail> DepartmentReportDetails { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<FormDepartmentReport> FormDepartmentReports { get; set; }
        public virtual DbSet<FormDepartmentReportDetail> FormDepartmentReportDetails { get; set; }
        public virtual DbSet<FormPersonalReport> FormPersonalReports { get; set; }
        public virtual DbSet<FormPersonalReportDetail> FormPersonalReportDetails { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<PersonalReport> PersonalReports { get; set; }
        public virtual DbSet<PersonalReportDetail> PersonalReportDetails { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<ReportPeriod> ReportPeriods { get; set; }
        public virtual DbSet<ReportYear> ReportYears { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}
