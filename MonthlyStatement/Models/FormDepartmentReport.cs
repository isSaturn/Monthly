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
    
    public partial class FormDepartmentReport
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormDepartmentReport()
        {
            this.FormDepartmentReportDetails = new HashSet<FormDepartmentReportDetail>();
        }
    
        public int form_department_report_id { get; set; }
        public int report_period_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormDepartmentReportDetail> FormDepartmentReportDetails { get; set; }
        public virtual ReportPeriod ReportPeriod { get; set; }
    }
}
