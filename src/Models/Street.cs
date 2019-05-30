namespace verint_service.Models
{
    public class Street
    {
        public Street(string strReference, string strDescription)
        {
            this.Reference = strReference;
            this.Description = strDescription;
        }

        public Street(string strReference, string strDescription, string usrn)
        {
            this.Reference = strReference;
            this.Description = strDescription;
            this.USRN = usrn;
        }

        public string Reference { get; set; }

        public string Description { get; set; }

        public string USRN { get; set; }
    }
}
