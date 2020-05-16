using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace OOP_lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        Car curCar;
        Human curHuman;
        Location curLocation;
        public GMapMarker carMarker;
        public GMapMarker humanMarker;

        


        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = GMapProviders.OpenStreetMap;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);

            // настройка взаимодействия с картой
         
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;
            //
            comboBox.SelectedIndex = 0;
            curHuman = new Human(textBoxName.Text, new PointLatLng(0, 0));
            Map.Markers.Add(humanMarker = curHuman.getMarker());
            mapObjects.Add(curHuman);
        }

        List<MapObject> mapObjects = new List<MapObject>();
        List<PointLatLng> activePoints = new List<PointLatLng>();

        void CreateMapObjects(PointLatLng point)
        {
            if(comboBox.SelectedIndex == 0)
            {
                Car car = new Car(textBoxName.Text, point);
                mapObjects.Add(car);
                Map.Markers.Add(car.getMarker());
                activePoints.Clear();
            }
            if (comboBox.SelectedIndex == 1)
            {
                Location location = new Location(textBoxName.Text, point);
                mapObjects.Add(location);
                Map.Markers.Add(location.getMarker());
                activePoints.Clear();
            }
            if (comboBox.SelectedIndex == 2)
            {
                curHuman.setPoint(point);
                humanMarker.Position = point;
                
                activePoints.Clear();
            }
            if (comboBox.SelectedIndex == 3)
            {
                Route route = new Route(textBoxName.Text, activePoints);
                mapObjects.Add(route);
                Map.Markers.Add(route.getMarker());
                activePoints.Clear();
            }
            if (comboBox.SelectedIndex == 4)
            {
                Area area = new Area(textBoxName.Text, activePoints);
                mapObjects.Add(area);
                Map.Markers.Add(area.getMarker());
                activePoints.Clear();
            }
        }


        GMapMarker userMarker;

        void SearchOnMap(PointLatLng point)
        {
            listBox.Items.Clear();
            Map.Markers.Remove(userMarker);

            userMarker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32,
                    Height = 32,
                    ToolTip = "User",
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/loading.png"))
                }
            };
            Map.Markers.Add(userMarker);

            var mapObjectsSorted = mapObjects.OrderBy(mo => mo.getDistance(point));
            foreach(MapObject mo in mapObjectsSorted)
            {
                if(mo.getTitle().Contains(textBoxSearch.Text))
                    listBox.Items.Add((int)mo.getDistance(point) + "m " + mo.getTitle() + " ");
            }
            if (listBox.Items.Count == 1) FocusOnMarker(0);
        }

        void FocusOnMarker(int selectedIndex)
        {
            Map.Position = mapObjects[selectedIndex].getFocus();
        } 

        void CarCall()
        {
            int carIndex = int.MaxValue;
            for(int i = 0; i < mapObjects.Count; i++)
            {
                if (mapObjects[i] is Car) carIndex = i;
            }
            int humanIndex = int.MaxValue;
            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (mapObjects[i] is Human) humanIndex = i;
            }

            int locationIndex = int.MaxValue;
            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (mapObjects[i] is Location) locationIndex = i;
            }

            if (carIndex != int.MaxValue && humanIndex != int.MaxValue && locationIndex != int.MaxValue)
            {
                curCar = (Car)mapObjects[carIndex];
                curHuman = (Human)mapObjects[humanIndex];
                curLocation = (Location)mapObjects[locationIndex];
                curHuman.setDestination(curLocation.getFocus());

                curCar.Arrived += curHuman.carArrved;
                curHuman.passengerSeated += curCar.passengerSeated;
                //carMarker = curCar.getMarker();
                //humanMarker = curHuman.getMarker();

                Route route = curCar.moveTo(curHuman.getFocus());
                mapObjects.Add(route);
                Map.Markers.Add(route.getMarker());
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////// Interface

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            activePoints.Add(point);
            
        }

        private void ButtonAddOk_Click(object sender, RoutedEventArgs e)
        {
            if (radioButtonCreate.IsChecked == true)
            {
                if (activePoints.Count != 0)
                {
                    Map.Markers.Remove(userMarker);
                    CreateMapObjects(activePoints.Last());
                }
            }
        }

        private void ButtonSearchOk_Click(object sender, RoutedEventArgs e)
        {
            if(radioButtonSearch.IsChecked == true)
            {
                if (activePoints.Count != 0)
                {
                    SearchOnMap(activePoints.Last());
                }
            }
        }

        private void ButtonAddCancel_Click(object sender, RoutedEventArgs e)
        {
            activePoints.Clear();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FocusOnMarker(listBox.SelectedIndex);
        }

        private void ButtonCall_Click(object sender, RoutedEventArgs e)
        {
            CarCall();
        }
    }
}
