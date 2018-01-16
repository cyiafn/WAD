using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WAD
{
    public class Movie : IEquatable<Movie>
    {
        public String Title { get; set; }
        public String MovieType { get; set; }
        public double Price { get; set; }
        public String ImageFileName { get; set; }
        public bool Status { get; set; }
        public byte[] FileNameByte { get; set; }
        public byte[] FileData { get; set; }
        public string VideoId { get; set; }
        [XmlIgnore]
        public Dictionary<string, string[]> ShowTime { get; set; }

        string today = (DateTime.Today).ToString("dd/MM/yyyy");
        string secondDay = (DateTime.Today.AddDays(1)).ToString("dd/MM/yyyy");
        string thirdDay = (DateTime.Today.AddDays(2)).ToString("dd/MM/yyyy");

        public void initMovie(String title, String movieType, double price, String imageFileName, byte[] fileNameByte, byte[] fileData, string videoId)
        {
            this.Title = title;
            this.MovieType = movieType;
            this.Price = price;
            this.ImageFileName = imageFileName;
            this.Status = true;
            this.FileNameByte = fileNameByte;
            this.FileData = fileData;
            this.VideoId = videoId;

            Dictionary<string, string[]> showTime = new Dictionary<string, string[]>();
            string[] seats = { "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5", "C1", "C2", "C3", "C4", "C5" };
            string[] timeslot = { "12PM", "2PM", "4PM", "6PM", "8PM", "10PM", "12AM" };
            for (int a = 0; a < 3; a++)
            {
                string s = null;
                if (a == 0)
                    s = today;
                else if (a == 1)
                    s = secondDay;
                else if (a == 2)
                    s = thirdDay;
                for (int i = 0; i < 7; i++)
                {
                    showTime.Add(s + ";" + timeslot[i], seats);
                }
            }
            this.ShowTime = showTime;
        }

        public bool Equals(Movie other)
        {
            return Title.Equals(other.Title);
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}
