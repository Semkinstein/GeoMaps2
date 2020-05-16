using System;
using System.Collections.Generic;
using System.Linq;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace OOP_lab3
{
    class Route : MapObject
    {
        private List<PointLatLng> points;

        public Route(string title, List<PointLatLng> points) : base(title)
        {
            this.points = new List<PointLatLng>();
            foreach (PointLatLng p in points)
            {
                this.points.Add(p);
            }
        }

        public List<PointLatLng> getPoints()
        {
            return points;
        }

        public override double getDistance(PointLatLng p2)
        {
            double distance = Double.MaxValue;
            foreach (PointLatLng point in points)
            {
                GeoCoordinate c1 = new GeoCoordinate(point.Lat, point.Lng);
                GeoCoordinate c2 = new GeoCoordinate(p2.Lat, p2.Lng);
                if (distance > c1.GetDistanceTo(c2))
                {
                    distance = c1.GetDistanceTo(c2);
                }
            }
            return distance;
        }

        public override PointLatLng getFocus()
        {
            PointLatLng point = new PointLatLng();
            point.Lat = points.Sum(pts => pts.Lat) / points.Count;
            point.Lng = points.Sum(pts => pts.Lng) / points.Count;
            return point;
        }

        public override GMapMarker getMarker()
        {
            GMapMarker marker = new GMapRoute(points)
            {
                Shape = new Path()
                 {
                     Stroke = Brushes.DarkBlue,
                     Fill = Brushes.DarkBlue, 
                     StrokeThickness = 4 
                 }
            };

            marker.Position = this.getFocus();
            return marker;
        }
    }
}

