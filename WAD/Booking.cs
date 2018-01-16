using System;

namespace WAD
{
    public class Booking : IEquatable<Booking>
    {
        public String TransactionId { get; set; }
        public String Movie { get; set; }
        public String User { get; set; }
        public double Price { get; set; }
        public String Date { get; set; }
        public String Timeslot { get; set; }
        public String[] Seats { get; set; }

        // Initalize Booking object
        public void initBooking(String transactionId, String movie, String user, double price, String date, String timeslot, String[] seats)
        {
            this.TransactionId = transactionId;
            this.Movie = movie;
            this.User = user;
            this.Price = price;
            this.Date = date;
            this.Timeslot = timeslot;
            this.Seats = seats;
        }

        // Compares booking transaction id with other transaction id
        public bool Equals(Booking other)
        {
            return TransactionId.Equals(other.TransactionId);
        }

        // Overrides the hash code to return hash code for id
        public override int GetHashCode()
        {
            return TransactionId.GetHashCode();
        }
    }
}
