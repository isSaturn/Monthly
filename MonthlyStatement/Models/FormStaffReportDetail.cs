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
    
    public partial class FormStaffReportDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormStaffReportDetail()
        {
            this.StaffReportDetails = new HashSet<StaffReportDetail>();
        }
    
        public int form_staff_report_detail_id { get; set; }
        public int category_id { get; set; }
        public int form_staff_report_id { get; set; }
        public string status { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual FormStaffReport FormStaffReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffReportDetail> StaffReportDetails { get; set; }
    }
}
