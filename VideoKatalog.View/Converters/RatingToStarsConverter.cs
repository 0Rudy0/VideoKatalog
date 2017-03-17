using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media;
using System.IO;

namespace Video_katalog.Converters {
    class RatingToStarsConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            float rating = float.Parse (((decimal) value).ToString ());
            
                
            Bitmap ratingImageBitmap = VideoKatalog.View.Properties.Resources.ratingStars;
            MemoryStream ms = new MemoryStream ();
            ratingImageBitmap.Save (ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            BitmapImage ratingStars = new BitmapImage ();
            ratingStars.BeginInit ();
            ratingStars.StreamSource = new MemoryStream (ms.ToArray());
            ratingStars.EndInit ();            

            System.Windows.Shapes.Rectangle newRect = new System.Windows.Shapes.Rectangle ();
            newRect.Height = 29;
            newRect.Width = (int) (rating * 29.4);

            ImageBrush myBrush = new ImageBrush ();
            myBrush.TileMode = TileMode.Tile;
            myBrush.ImageSource = ratingStars;
            newRect.Fill =  myBrush;

            //do tud radi sigurno

            Geometry rectangleGeometry = newRect.RenderedGeometry;
            //DrawingGroup group = new DrawingGroup ();
            //group.Children.Add (new GeometryDrawing (null, null, rectangleGeometry));
            //DrawingImage img = new DrawingImage (group);

            DrawingVisual drawingVisual = new DrawingVisual ();
            DrawingContext drawingContext = drawingVisual.RenderOpen ();
            drawingContext.DrawGeometry (null, null,rectangleGeometry);
            drawingContext.Close ();


            RenderTargetBitmap bmp = new RenderTargetBitmap (180, 180, 120, 96, PixelFormats.Pbgra32);
            bmp.Render (drawingVisual);


            return bmp;
            //return ratingStars;
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
