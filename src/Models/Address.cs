using System.Text;

namespace verint_service.Models
{
    public class Address
    {
        public Address()
        {
        }

        public Address(string strReference, string strDescription)
        {
            this.Reference = strReference;
            this.Description = strDescription;
        }

        public string Reference { get; set; }

        public string Description { get; set; }

        public string Number { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string City { get; set; }

        public string Postcode { get; set; }

        public string UPRN { get; set; }

        public string USRN { get; set; }

        public string PropertyId { get; set; }

        public string BusinessName { get; set; }

        public bool? FromCookie { get; set; }

        public bool IsFullyResolved
        {
            get
            {
                bool isFullyResolved = this.Number != null &&
                                       this.AddressLine1 != null &&
                                       this.City != null &&
                                       this.Postcode != null &&
                                       this.Number != string.Empty &&
                                       this.AddressLine1 != string.Empty &&
                                       this.City != string.Empty &&
                                       this.Postcode != string.Empty;

                return isFullyResolved;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendString(this.Number, sb);
            AppendString(this.BusinessName, sb);
            AppendString(this.AddressLine1, sb);
            AppendString(this.AddressLine2, sb);
            AppendString(this.AddressLine3, sb);
            AppendString(this.City, sb);
            AppendString(this.Postcode, sb);
            string s = sb.ToString();
            s = s.Trim();
            s = s.TrimEnd(new char[] { ',' });
            return s;
        }

        private StringBuilder AppendString(string s, StringBuilder sb)
        {
            if ((!string.IsNullOrWhiteSpace(s)))
                sb.Append(s + ", ");

            return sb;
        }
    }
}
