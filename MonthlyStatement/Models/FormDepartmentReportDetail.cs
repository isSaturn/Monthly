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
    
    public partial class FormDepartmentReportDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormDepartmentReportDetail()
        {
            this.DepartmentReportDetails = new HashSet<DepartmentReportDetail>();
        }
    
        public int form_department_report_detail_id { get; set; }
        public int category_id { get; set; }
        public int form_department_report_id { get; set; }
        public Nullable<int> row { get; set; }
        public Nullable<int> col { get; set; }
        public string status { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartmentReportDetail> DepartmentReportDetails { get; set; }
        public virtual FormDepartmentReport FormDepartmentReport { get; set; }
    }
}
