//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class DepartmentReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DepartmentReport()
        {
            this.DepartmentReportDetails = new HashSet<DepartmentReportDetail>();
        }
    
        public int department_report_id { get; set; }
        public int report_period_id { get; set; }
        public string file_path { get; set; }
        public string status { get; set; }
        public string account_id { get; set; }
        public Nullable<int> comment_id { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual ReportPeriod ReportPeriod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartmentReportDetail> DepartmentReportDetails { get; set; }
    }
}
