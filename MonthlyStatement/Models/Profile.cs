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
    
    public partial class Profile
    {
        public int profile_id { get; set; }
        public string user_name { get; set; }
        public Nullable<int> faculty_id { get; set; }
        public string account_id { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> last_login { get; set; }
        public Nullable<System.DateTime> last_logout { get; set; }
        public Nullable<int> department_id { get; set; }
        public string user_code { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Department Department { get; set; }
        public virtual Faculty Faculty { get; set; }
    }
}
