using System.Xml;

namespace Osm2Png {
    public class OsmReader {
        private string filename = "";
        public Dictionary<ulong, Point> nodes = new();
        public Dictionary<ulong, List<ulong>> ways = new();
        private bool hasOrigin = false;
        private double xOrigin = 0;
        private double yOrigin = 0;
        public OsmReader(string name)
        {
            filename = name;

            ReadData();
        }
        private void ReadData()
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.Load(filename);

            XmlElement? xRoot = xDoc.DocumentElement;

            if (xRoot == null) {
                throw(new Exception("No root data in xml"));
            }

            foreach (XmlElement xnode in xRoot)
            {
                if(xnode.Name == "node")
                {
                    ulong id = Convert.ToUInt64(xnode.Attributes.GetNamedItem("id")?.Value);
                    double lat = Convert.ToDouble(xnode.Attributes.GetNamedItem("lat")?.Value);
                    double lon = Convert.ToDouble(xnode.Attributes.GetNamedItem("lon")?.Value);

                    double x = 0, y = 0;
                    WGS2UTMConverter.convertToUTM(lat, lon, ref x, ref y);
                    
                    if(!hasOrigin)
                    {
                        xOrigin = x;    
                        yOrigin = y;
                        hasOrigin = true;
                    }

                    x -= xOrigin;
                    y -= yOrigin;
                    var point = new Point((float)x, (float)y);
                    nodes[id] = point;
                }
                else if(xnode.Name == "way")
                {
                    ulong wayId = Convert.ToUInt64(xnode.Attributes.GetNamedItem("id")?.Value);
                    var nodesId = new List<ulong>();
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "nd")
                        {
                            ulong nodeId = Convert.ToUInt64(childnode?.Attributes?.GetNamedItem("ref")?.Value);
                            nodesId.Add(nodeId);
                        }
                    }
                    
                    ways[wayId] = nodesId;
                }
            }
        }
    }
}