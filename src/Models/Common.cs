using System;
using System.IO;

namespace verint_service.Models
{
    public class Common
    {
        public static string AnonymousIndividualReference
        {
            get { return "101001155391"; }
        }

        public static string Channel
        {
            get { return "Web"; }
        }

        public static string IndividualObjectType
        {
            get { return "C1"; }
        }

        public static string OrganisationObjectType
        {
            get { return "C2"; }
        }

        public static string PropertyObjectType
        {
            get { return "D3"; }
        }

        public static string StreetObjectType
        {
            get { return "D4"; }
        }

        public static string FormatPostcode(string postcode)
        {
            if (postcode.Contains(" ") && (postcode.Length == 7 || postcode.Length == 8))
            {
                return postcode;
            }
            else
            {
                return postcode;
            }
        }

        public static bool CreateTextFileFromXML(string text, string filename, string folder)
        {
            try
            {
                string fileLocation = folder + filename;
                File.WriteAllText(fileLocation, text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetXMLFromTextFile(string filename, string folder)
        {
            string fileLocation = folder + filename;
            try
            {
                string xml = File.ReadAllText(fileLocation);
                return xml;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
