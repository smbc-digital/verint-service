namespace verint_service.Models
{
    public class Organisation
    {
        public Organisation()
        {

        }
        public Organisation(string strReference, string strDescription)
        {
            this.Reference = strReference;
            this.Description = strDescription;
        }

        public string Reference { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }

        public Address Address { get; set; }

        public SocialContact[] SocialContacts { get; set; }
    }
}
