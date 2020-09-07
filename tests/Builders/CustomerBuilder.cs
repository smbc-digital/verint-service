using StockportGovUK.NetStandard.Models.Verint;

namespace verint_service_tests.Builders
{
    public class CustomerBuilder
    {
        private string _forename = "forename";
        private string _surname = "surname";
        private string _telephone;
        private string _email;
        private Address _address = null;

        public Customer Build()
        {
            return new Customer
            {
                Forename = _forename,
                Surname = _surname,
                Telephone = _telephone,
                Email = _email,
                Address = _address
            };
        }

        public CustomerBuilder WithForename(string value)
        {
            _forename = value;
            return this;
        }
        
        public CustomerBuilder WithSurname(string value)
        {
            _surname = value;
            return this;
        }

        public CustomerBuilder WithEmail(string value)
        {
            _email = value;
            return this;
        }

        public CustomerBuilder WithTelephone(string value)
        {
            _telephone = value;
            return this;
        }
        public CustomerBuilder WithAddress(Address value)
        {
            _address = value;
            return this;
        }
    }
}