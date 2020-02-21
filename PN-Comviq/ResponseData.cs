namespace PN_Comviq
{
    public class ResponseData
    {
        public SsnInfo ssnInfo { get; set; }
    }

    public class Address
    {
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
    }

    public class SsnInfo
    {
        public string identificationNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Address address { get; set; }
        public string birthday { get; set; }
    }
}