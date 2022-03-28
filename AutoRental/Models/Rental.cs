namespace AutoRental.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rental
    {
        public int IDRental { get; set; }
        public string CarNo { get; set; }
        public int IDPerson { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
    }
}
