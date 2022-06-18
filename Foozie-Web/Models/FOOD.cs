﻿//------------------------------------------------------------------------------
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
    using System.ComponentModel.DataAnnotations;
    
    public partial class FOOD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FOOD()
        {
            this.ORDER_DETAIL = new HashSet<ORDER_DETAIL>();
        }
    
        public System.Guid food_id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string name { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Range(1000, Int64.MaxValue, ErrorMessage = "Giá tối thiểu là 1000đ")]
        public int price { get; set; }
        public Nullable<bool> is_delete { get; set; }
        public System.Guid type_id { get; set; }
    
        public virtual FOOD_TYPE FOOD_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDER_DETAIL> ORDER_DETAIL { get; set; }
    }
}
