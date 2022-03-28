namespace AutoRental.Models
{
    using System;
    public partial class Return
    {
        public int IDReturn { get; set; }
        public string CarNo { get; set; }
        public Nullable<int> IDPerson { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public Nullable<int> elsp { get; set; }
        public Nullable<int> fine { get; set; }
    }
}
