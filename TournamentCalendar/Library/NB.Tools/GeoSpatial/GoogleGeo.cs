using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace NB.Tools.GeoSpatial
{
	public class GoogleGeo
	{
		/// <summary>
		/// Gets the geo data for a postal address.
		/// </summary>
		/// <param name="address">Full postal address, including country</param>
		/// <returns></returns>
		public static Location GetLocation(string address)
		{
			// Docs for Google Geocoding API v3:
			// https://developers.google.com/maps/documentation/geocoding/


			var request = (HttpWebRequest)WebRequest.Create(string.Format("https://maps.googleapis.com/maps/api/geocode/xml?region=DE&address={0}&key={1}", address.Trim(), GoogleApiKey));
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:62.0) Gecko/20100101 Firefox/62.0";

		    if (!(request.GetResponse() is HttpWebResponse webResponse)) return null;

			Location location = null;
			if (webResponse.StatusCode == HttpStatusCode.OK)
			{
				var streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.ASCII);
				var responseString = streamReader.ReadToEnd();

				var xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(responseString);

				var latNode = xmlDoc.SelectSingleNode("/GeocodeResponse/result/geometry/location/lat");
				var lngNode = xmlDoc.SelectSingleNode("/GeocodeResponse/result/geometry/location/lng");

				if (latNode != null && lngNode != null)
				{
					location = new Location(
						new Latitude(Angle.FromDegrees(double.Parse(latNode.InnerText, CultureInfo.InvariantCulture.NumberFormat))),
						new Longitude(Angle.FromDegrees(double.Parse(lngNode.InnerText, CultureInfo.InvariantCulture.NumberFormat))));
				}
			}

			return location;
		}

	    public static string GoogleApiKey { get; set; } = string.Empty;
	}
}
