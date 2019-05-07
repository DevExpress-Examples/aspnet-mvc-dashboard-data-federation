using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DataFederationExample
{
    public class SalesPersonData
    {
        public int ID { get; set; }
        public string SalesPerson { get; set; }
        public int Weight { get; set; }
        public bool Checked { get; set; }
    }

    public static class DataGenerator
    {
        private static List<SalesPersonData> _data;

        public static List<SalesPersonData> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = CreateSourceData();
                }
                return _data;
            }
        }
        public static List<SalesPersonData> CreateSourceData()
        {
            List<SalesPersonData> data = new List<SalesPersonData>();
            string[] salesPersons = { "Andrew Fuller", "Michael Suyama",
                                    "Robert King", "Nancy Davolio",
                                    "Margaret Peacock", "Laura Callahan",
                                    "Steven Buchanan", "Janet Leverling" };
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            Random rand = new Random(seed);

            for (int i = 0; i < salesPersons.Length; i++)
            {
                SalesPersonData record = new SalesPersonData();
                record.ID = i;
                record.SalesPerson = salesPersons[i];
                record.Weight = rand.Next(0, 100);
                data.Add(record);
            }
            return data;
        }
    }
}