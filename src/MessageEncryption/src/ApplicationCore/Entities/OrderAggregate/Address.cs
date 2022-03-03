namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate
{
    public class Address // ValueObject
    {
        public string Street { get; private set; }

        public string City { get; private set; }

        public string State { get; private set; }

        public string Country { get; private set; }

        public string ZipCode { get; private set; }

        private Address() { }

        public Address(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }
    }

    public class CreditCard
    {
        public CreditCard(string number, string expiry, string cvv)
        {
            Number = number;
            Expiry = expiry;
            Cvv = cvv;
        }

        public string Number { get; set; }
        public string Expiry { get; set; }
        public string Cvv { get; set; }
    }
}
