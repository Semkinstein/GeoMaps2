using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using GMap.NET.MapProviders;
using System.Windows;
using System.Threading;
using System.Windows.Media;

namespace OOP_lab3
{
    
    class Car : MapObject
    {
        private PointLatLng point;
        
        private Route route;
        private PointLatLng destination;
        GMapMarker carMarker;
        public List<Human> passengers = new List<Human>();

        public event EventHandler Arrived;

        public Car(string title, PointLatLng point) : base(title)
        {
            this.point = point;
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
            carMarker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 50, 
                    Height = 50, 
                    ToolTip = this.getTitle(), 
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.png")) 
                }
            };
            carMarker.Position = point;
           
            return carMarker;
        }


        public Route moveTo(PointLatLng endpoint)
        {
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            MapRoute route = routingProvider.GetRoute( point, endpoint, false, false, 15);

            List<PointLatLng> routePoints = route.Points;
            this.route = new Route("", routePoints);

            Thread newThread = new Thread(new ThreadStart(moveByRoute));
            newThread.Start();

            return this.route;
        }

        public void passengerSeated(object sender, EventArgs e)
        {
            passengers.Add((Human)sender);
            Application.Current.Dispatcher.Invoke( delegate {
                Route route = moveTo(passengers[0].getDestination());
                MainWindow.Instance.Map.Markers.Add(route.getMarker());
            });
        }

        private void moveByRoute()
        {
            PointLatLng prevPoint = route.getPoints()[0];
            foreach (var point in route.getPoints())
            {
                
                Application.Current.Dispatcher.Invoke(delegate {
                    carMarker.Position = point;

                    double latDiff = point.Lat - prevPoint.Lat;
                    double lngDiff = point.Lng - prevPoint.Lng;
                    double angle = Math.Atan2(lngDiff, latDiff) * 180.0 / Math.PI;
                    carMarker.Shape.RenderTransform = new RotateTransform { Angle = angle + 80 };

                    this.point = point;
                    if(passengers.Count != 0)
                    {
                        passengers[0].setPoint(point);
                        passengers[0].humanMarker.Position = point;
                    }
                });
                Thread.Sleep(500);
                prevPoint = point;
            }
            Arrived?.Invoke(this, null);
            
        }
    }
}
