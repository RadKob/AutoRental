namespace AutoRental.Models
{
    using System;
    public partial class Car
    {
        public int IDCar { get; set; }
        public string CarNo { get; set; }
        public string TheBrand { get; set; }
        public string TheModel { get; set; }
        public int Year { get; set; }
        public string Mileage { get; set; }
        public string Engine { get; set; }
        public Nullable<int> MaxPower { get; set; }
        public string ABS { get; set; }
        public Nullable<int> Doors { get; set; }
        public Nullable<int> Mass { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }
    }
}
