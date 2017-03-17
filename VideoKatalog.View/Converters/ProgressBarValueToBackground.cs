using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;

namespace Video_katalog.Converters {
    class ProgressBarValueToBackground : System.Windows.Data.IMultiValueConverter {  
        List<Color> colorListTS1 = new List<Color>();
        List<Color> colorListTS2 = new List<Color>();
        List<Color> colorListTS3 = new List<Color>();
        List<Color> colorListTS4 = new List<Color>();

        void FillColorList () {
            colorListTS1.Clear ();
            colorListTS2.Clear ();
            colorListTS3.Clear ();
            colorListTS4.Clear ();
            for (int i = 1 ; i < 102 ; i++) {
                colorListTS1.Add (new Color ());
                colorListTS2.Add (new Color ());
                colorListTS3.Add (new Color ());
                colorListTS4.Add (new Color ());
            }
            //iz zelene u crvenu, 100 unosa koji korespondiraju sa postotkom popunjenosti HDDa            
            for (int i = 1 ; i <= 50 ; i++) {
                byte TS11 = (byte) (100 + ((int)(2.2 * (float)i)));
                byte TS12 = (byte) 210;
                byte TS13 = (byte) 100;
                colorListTS1[i] = (Color.FromRgb ( TS11, TS12, TS13));

                byte TS21 = (byte) (81 + ((int)(2.46 * (float)i)));
                byte TS22 = (byte) 204;
                byte TS23 = (byte) 81;
                colorListTS2[i] = (Color.FromRgb (TS21, TS22, TS23));

                byte TS31 = (byte) (48 + ((int) (2.8 * (float) i))); ;
                byte TS32 = (byte) 188;
                byte TS33 = (byte) 48;
                colorListTS3[i] = (Color.FromRgb (TS31, TS32, TS33));

                byte TS41 = (byte) (9 + ((int) (2.96 * (float) i)));
                byte TS42 = (byte) 157;
                byte TS43 = (byte) 9;
                colorListTS4[i] = (Color.FromRgb (TS41, TS42, TS43));
            }
            for (int i = 1; i <= 50; i++) {
                byte TS11 = (byte) 210;
                byte TS12 = (byte) (210 - ((int) (2.2 * (float) i)));
                byte TS13 = (byte) 100;
                colorListTS1[i+50] = (Color.FromRgb (TS11, TS12, TS13));

                byte TS21 = (byte) 204;
                byte TS22 = (byte) (204 - ((int) (2.46 * (float) i))); ;
                byte TS23 = (byte) 81;
                colorListTS2[i+50] = (Color.FromRgb (TS21, TS22, TS23));

                byte TS31 = (byte) 188;
                byte TS32 = (byte) (188 - ((int) (2.8 * (float) i))); ;
                byte TS33 = (byte) 48;
                colorListTS3[i+50] = (Color.FromRgb (TS31, TS32, TS33));

                byte TS41 = (byte) 157;
                byte TS42 = (byte) (157 - ((int) (2.96 * (float) i)));
                byte TS43 = (byte) 9;
                colorListTS4[i+50] = (Color.FromRgb (TS41, TS42, TS43));
            }
        }

        public object Convert (object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            double maximum =  (double)values[0];
            double value = (double) values[1];
            FillColorList ();
            BrushConverter conv = new BrushConverter ();
            double percentage = (value / maximum) * 100;
            LinearGradientBrush background = new LinearGradientBrush();
            background.StartPoint = new System.Windows.Point (0.5,0);
            background.EndPoint = new System.Windows.Point (0.5, 1);
            GradientStop stop1 = new GradientStop (colorListTS1[(int) percentage], 0);
            GradientStop stop2 = new GradientStop (colorListTS2[(int) percentage], 0.453);
            GradientStop stop3 = new GradientStop (colorListTS3[(int) percentage], 0.528);
            GradientStop stop4 = new GradientStop (colorListTS4[(int) percentage], 1);
            background.GradientStops.Add (stop1);
            background.GradientStops.Add (stop2);
            background.GradientStops.Add (stop3);
            background.GradientStops.Add (stop4);
            //System.Windows.MessageBox.Show (maximum.ToString ());
            return background;
        }

        public object[] ConvertBack (object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }
    }
}
