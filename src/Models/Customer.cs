using System;
using System.Collections.Generic;

namespace verint_service.Models
{
    public class Customer
    {
        public string Title { get; set; }

        public string Forename { get; set; }

        public string Initials { get; set; }

        public string Surname { get; set; }

        public string Telephone { get; set; }

        public string AlternativeTelephone { get; set; }

        public string Mobile { get; set; }

        public string FaxNumber { get; set; }

        public string Email { get; set; }

        public Address Address { get; set; }

        public Address AlternativeCorrespondenceAddress { get; set; }

        public string CustomerReference { get; set; }

        public string FullName
        {
            get
            {
                string fullName = this.Surname;

                if (!string.IsNullOrWhiteSpace(this.Forename))
                    fullName = (this.Forename + " " + fullName).TrimEnd();

                if (!string.IsNullOrWhiteSpace(this.Initials))
                    fullName = (this.Initials + " " + fullName).TrimEnd();

                if (!string.IsNullOrWhiteSpace(this.Title))
                    fullName = (this.Title + " " + fullName).TrimEnd();

                return (fullName == null) ? null : fullName.Trim();
            }
        }

        public string NationalInsuranceNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public ICollection<Customer> PreviousNames { get; set; }

        public SocialContact[] SocialContacts { get; set; }
    }
}
