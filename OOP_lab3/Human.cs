using System;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace OOP_lab3
{
    class Human : MapObject
    {
        private PointLatLng point;
        private PointLatLng destinationPoint;
        public GMapMarker humanMarker;

        public event EventHandler passengerSeated;

        public Human(string title, PointLatLng point) : base(title)
        {
            this.point = point;
        }

        public void setPoint(PointLatLng point)
        {
            this.point = point;
        }

        public PointLatLng getDestination()
        {
            return destinationPoint;
        }

        public void setDestination(PointLatLng point)
        {
            destinationPoint = point;
        }

        public override double getDistance(PointLatLng p2)
        {
            GeoCoordinate c1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate c2 = new GeoCoordinate(p2.Lat, p2.Lng);

            double distance = c1.GetDistanceTo(c2);
            return distance;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker getMarker()
        {
            humanMarker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32, 
                    Height = 32, 
                    ToolTip = this.getTitle(),
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/human.png")) 
                }
            };
            humanMarker.Position = point;
            return humanMarker;
        }

        public void moveTo(PointLatLng point)
        {

        }

        public void carArrved(object sender, EventArgs e)
        {
            Car car = (Car)sender;
            if (car.passengers.Count == 0)
            {
                passengerSeated?.Invoke(this, EventArgs.Empty);
            }
            else
            {

            }
        }


    }
}
