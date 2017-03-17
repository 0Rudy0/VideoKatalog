using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Web;
using System.Net;
using System.Collections.ObjectModel;

namespace Video_katalog.Converters {
    class PersonsToStringConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            ObservableCollection<Person> personList;
            try {
                personList = (ObservableCollection<Person>) value;
            }
            catch {
                return "nije prosao converter";
            }
            if (personList.Count == 0)
                return "";
            string personsString = "";
            foreach (Person tempPerson in personList) 
                personsString += HttpUtility.HtmlDecode (tempPerson.Name) + ", ";

            //makni ", " na kraju
            personsString = personsString.Substring (0, personsString.Length - 2);

            return personsString;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
