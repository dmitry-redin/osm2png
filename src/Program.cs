using Osm2Png;
public class Program {
    static void Main(string[] Args)
    {
        if(Args.Length < 3)
        {
            Console.WriteLine("run map.osm map.png step[in meters]");
            return;
        }
        string osmmap = Args[0];
        string pngmap = Args[1];
        float step = Convert.ToSingle(Args[2]);

        var reader = new OsmReader(osmmap);

        var box = new BoundingBox();

        foreach (var item in reader.nodes)
        {
            box.Add(item.Value);
        }

        var grid = new Grid(box, step);

        foreach (var item in reader.nodes)
        {
            grid.AddPoint(item.Value);
        }

        foreach (var item in reader.ways)
        {
            var points = new List<Osm2Png.Point>();

            foreach(var nodeId in item.Value)
            {
                points.Add(reader.nodes[nodeId]);
            }

            grid.AddLine(points);
        }

        var saver = new Osm2Image();

        saver.SaveImage(pngmap, grid);
    }
}
