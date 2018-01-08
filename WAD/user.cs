using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAD
{
    public class user
    {
        private string firstName;
        private string middleName;
        private string lastName;
        private string email;
        private string password;
        private string dob;

        public void intializeUser(string fName, string mName, string lName, string Email, string pw, string date)
        {
            this.firstName = fName;
            this.middleName = mName;
            this.lastName = lName;
            this.email = Email;
            this.password = pw;
            this.dob = date;
        }
        public string getFirstName() { return firstName; }
        public void setFirstName(string fName) { this.firstName = fName; }
        public string getMiddleName() { return middleName; }
        public void setMiddleName(string mName) { this.middleName = mName; }
        public string getLastName() { return this.lastName; }
        public void setLastName(string lName) { this.lastName = lName; }
        public string getEmail() { return this.email; }
        public void setEmail(string Email) { this.email = Email; }
        public string getPassword() { return this.password; }
        public void setPassword(string pw) { this.password = pw; }
        public void setDOB(string DOB) { this.dob = DOB; }
        public string getDOB() { return this.dob; }
    }
}
