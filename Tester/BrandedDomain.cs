//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tester
{
    using System;
    using System.Collections.Generic;
    
    public partial class BrandedDomain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BrandedDomain()
        {
            this.Urls = new HashSet<Url>();
        }
    
        public int BrandedDomainId { get; set; }
        public int CompanyId { get; set; }
        public string Domain { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Url> Urls { get; set; }
    }
}
