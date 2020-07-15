using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SellingSolutions.Areas.Identity.Data
{
    // --- Identity User Database Model ---
    public class SellingSolutionsUser : IdentityUser
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public int Age { get; set; }

        public string Street { get; set; }

        public string Suburb { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        [DataType(DataType.PostalCode)]
        public string Postcode { get; set; }
    }
}
