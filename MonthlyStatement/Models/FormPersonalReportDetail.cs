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
    
    public partial class FormPersonalReportDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormPersonalReportDetail()
        {
            this.PersonalReportDetails = new HashSet<PersonalReportDetail>();
        }
    
        public int form_personal_report_detail_id { get; set; }
        public int category_id { get; set; }
        public int form_personal_report_id { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonalReportDetail> PersonalReportDetails { get; set; }
        public virtual FormPersonalReport FormPersonalReport { get; set; }
    }
}
