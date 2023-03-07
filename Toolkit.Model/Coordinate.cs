using System;
using System.Collections.Generic;
using System.Text;

namespace Toolkit.Model
{
	public class Coordinate
	{		
		public double Lat { get; set; }
		
		public double Lng { get; set; }

        public Coordinate()
        {

        }
        public Coordinate(double p_Lat, double p_Lng)
		{
			Lat = p_Lat;
			Lng = p_Lng;
		}

		public bool IsEqual(Coordinate c)
		{
			return (c.Lat == Lat && c.Lng == Lng);
		}

     }

}
