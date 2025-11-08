namespace eVOL.Domain.ValueObjects
{
    public class Address
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string AddressName { get; private set; }
        public string AddressNumber { get; private set; }
        private Address() { }

        public Address(string country, string city, string addressName, string addressNumber)
        {
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required");
            if (string.IsNullOrWhiteSpace(addressName)) throw new ArgumentException("Address name is required");
            if (string.IsNullOrWhiteSpace(addressNumber)) throw new ArgumentException("Address number is required");

            Country = country;
            City = city;
            AddressName = addressName;
            AddressNumber = addressNumber;
        }
    }

}
