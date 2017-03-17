using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace Video_katalog.Converters {
    class HddIdToNameConverter : IValueConverter {        
        SqlCeConnection connection = new SqlCeConnection (ComposeConnectionString.ComposeCEString());
        List<HDD> hddList = new List<HDD> ();

        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            HDD hdd = (HDD) value;
            return (object) hdd.Name;            
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string hddName = (string) value.ToString ();
            foreach (HDD tempHDD in hddList) {
                if (tempHDD.Name == hddName) {
                    return tempHDD;
                }
            }
            return null;            
        }

        void FillHddList () {
            SqlCeDataReader reader = null;
            SqlCeCommand getHddsCommand = new SqlCeCommand ("SELECT * FROM HDD", connection);
            connection.Open ();
            reader = getHddsCommand.ExecuteReader ();
            while (reader.Read ()) {
                HDD tempHdd = new HDD ();
                tempHdd.ID = (int) reader["hddID"];
                tempHdd.Name = (string) reader["name"];
                tempHdd.PurchaseDate = (DateTime) reader["purchaseDate"];
                tempHdd.WarrantyLength = (int) reader["warrantyLength"];
                hddList.Add (tempHdd);                
            }
            reader.Close ();
            connection.Close ();
        }

        public HddIdToNameConverter () {
            FillHddList ();
        }

        #endregion
    }
}
