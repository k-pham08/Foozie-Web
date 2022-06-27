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

    public partial class VOUCHER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VOUCHER()
        {
            this.ORDERs = new HashSet<ORDER>();
        }
    
        public System.Guid voucher_id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string name { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string description { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Range(0, Int64.MaxValue, ErrorMessage = "Không được nhỏ hơn 0")]
        public int max_used { get; set; }
        public int used { get; set; }
        public Nullable<DateTime> expired { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Range(1000, Int64.MaxValue, ErrorMessage = "Giá tối thiểu là 1000đ")]
        public int reduced_price { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [Range(1000, Int64.MaxValue, ErrorMessage = "Giá tối thiểu là 1000đ")]
        public int order_price { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDER> ORDERs { get; set; }
    }
}