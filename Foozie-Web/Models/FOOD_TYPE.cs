//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Foozie_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FOOD_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FOOD_TYPE()
        {
            this.FOODs = new HashSet<FOOD>();
        }
    
        public System.Guid type_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Nullable<bool> is_delete { get; set; }
        public string thumbnail { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FOOD> FOODs { get; set; }
    }
}