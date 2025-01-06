namespace StudentTaskManagement.Solution
{
    public class GeneralList
    {

        public static List<KeyValuePair<string, string>> GetCountries()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Brunei ", "BN"),
                new KeyValuePair<string, string>("Cambodia", "KH"),
                new KeyValuePair<string, string>("Indonesia", "ID"),
                new KeyValuePair<string, string>("Laos", "LA"),
                new KeyValuePair<string, string>("Malaysia", "MY"),
                new KeyValuePair<string, string>("Myanmar", "MM"),              
                new KeyValuePair<string, string>("Philippines", "PH"),
                new KeyValuePair<string, string>("Singapore", "SG"),
                new KeyValuePair<string, string>("Thailand", "TH"),
                new KeyValuePair<string, string>("VietNam", "VN"),
                // Add more countries as needed
            };
        }

        public static List<KeyValuePair<string, string>> GetCities()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Afghanistan", "AF"),
                new KeyValuePair<string, string>("Bangladesh", "BD"),
                new KeyValuePair<string, string>("Brunei ", "BN"),
                new KeyValuePair<string, string>("China", "CN"),
                new KeyValuePair<string, string>("Hong Kong ", "HK"),
                new KeyValuePair<string, string>("Indonesia", "ID"),
                new KeyValuePair<string, string>("Israel", "IL"),
                new KeyValuePair<string, string>("India", "IN"),
                new KeyValuePair<string, string>("Iraq ", "IR"),
                new KeyValuePair<string, string>("Jordan", "JO"),
                new KeyValuePair<string, string>("Japan", "JP"),
                new KeyValuePair<string, string>("Cambodia", "KH"),
                new KeyValuePair<string, string>("Korea", "KR"),
                new KeyValuePair<string, string>("Sri Lanka", "LK"),
                new KeyValuePair<string, string>("Myanmar", "MM"),
                new KeyValuePair<string, string>("Mongolia", "MN"),
                new KeyValuePair<string, string>("Macao", "MO"),
                new KeyValuePair<string, string>("Malaysia", "MY"),
                new KeyValuePair<string, string>("Philippines", "PH"),
                new KeyValuePair<string, string>("Pakistan", "PK"),
                new KeyValuePair<string, string>("Qatar", "QA"),
                new KeyValuePair<string, string>("Saudi Arabia", "SA"),
                new KeyValuePair<string, string>("Singapore", "SG"),
                new KeyValuePair<string, string>("Thailand", "TH"),
                new KeyValuePair<string, string>("Taiwan", "TW"),
                new KeyValuePair<string, string>("VietNam", "VN"),
                // Add more countries as needed
            };
        }

    }
}
