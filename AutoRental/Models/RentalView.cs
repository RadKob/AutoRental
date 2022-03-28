using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoRental.Models
{
    public class RentalView
    {
        public int IDRental { get; set; }
        public string CarNo { get; set; }
        public int IDPerson { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public decimal Price { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}