using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace OOP_lab3
{
    abstract class MapObject
    {
        private string title;
        private DateTime creationDate;

        public MapObject(string title)
        {
            this.title = title;
            this.creationDate = new DateTime();
        }

        public string getTitle() {
            return title;
        }

        public DateTime getCreationDate()
        {
            return creationDate;
        }

        abstract public double getDistance(PointLatLng p2);

        abstract public PointLatLng getFocus();

        abstract public GMapMarker getMarker();
        
    }
}
