namespace Osm2Png {
    public struct Point {
        public float x = 0f;
        public float y = 0f;
        public Point() {}
        public Point(float x, float y) 
        {
            this.x = x;
            this.y = y;
        }

        public float Distance(Point point)
        {
            return MathF.Sqrt((x-point.x)*(x-point.x) + (y-point.y)*(y-point.y));
        }

        public static Point operator +(Point op1, Point op2)
        {
            return new Point(op1.x + op2.x, op1.y + op2.y);
        }

        public static Point operator -(Point op1, Point op2)
        {
            return new Point(op1.x - op2.x, op1.y - op2.y);
        }

        public static Point operator *(Point op1, float op2)
        {
            return new Point(op1.x*op2, op1.y*op2);
        }

        public Point normalized()
        {
            float len = MathF.Sqrt(x*x + y*y);
            return new Point(x/len, y/len);
        }
    }

    public struct BoundingBox {
        public Point minCorner = new Point();
        public Point maxCorner = new Point();
        public bool valid = false;
        public BoundingBox() {}
        public void Add(Point point)
        {
            if(!valid)
            {
                maxCorner = minCorner = point;
                valid = true;
            }
            else
            {
                if (point.x < minCorner.x) {
                    minCorner.x = point.x;
                }
                else if (point.x > maxCorner.x) {
                    maxCorner.x = point.x;
                }

                if (point.y < minCorner.y) {
                    minCorner.y = point.y;
                }
                else if (point.y > maxCorner.y) {
                    maxCorner.y = point.y;
                }
            }
        }

        public void Clear() => valid = false;
    }

    public class Grid {
        private BoundingBox bbox = new BoundingBox();
        private float step = 1f;
        private ulong rowNum = 0;
        private ulong colNum = 0;
        public List<List<byte>> grid = new();
        public Grid(BoundingBox box, float step)
        {
            if(box.valid == false || step <= 0f)
            {
                throw(new ArgumentException());
            }

            bbox = box;
            this.step = step;

            rowNum = GetRowNum();
            colNum = GetColNum();

            GenerateGrid();
        }
        private void GenerateGrid()
        {
            grid.Clear();
            for(ulong row = 0; row < rowNum; row++)
            {
                var line = new List<byte>();
                for(ulong col = 0; col < colNum; col++)
                {
                    line.Add(0);
                }
                grid.Add(line);
            }
        }

        public ulong GetRowNum()
        {
            return (ulong)((bbox.maxCorner.y - bbox.minCorner.y + 0.5)/step);
        }

        public ulong GetColNum()
        {
            return (ulong)((bbox.maxCorner.x - bbox.minCorner.x + 0.5)/step);
        }
        public void AddPoint(Point point, bool line = false)
        {
            ulong col = (ulong)((point.x - bbox.minCorner.x) / step);
            ulong row = (ulong)((bbox.maxCorner.y - point.y) / step);
            byte color = line ? (byte)2 : (byte)1;

            if(col >= 0 && col < colNum && row >= 0 && row < rowNum && grid[(int)row][(int)col] != 1)
            {
                grid[(int)row][(int)col] = color;
            }
        }

        public void AddLine(List<Point> points)
        {
            for(int i = 0; i < points.Count()-1; i++)
            {
                float dist = points[i].Distance(points[i+1]);
                var dir = (points[i+1] - points[i]).normalized();

                float t = 0;

                while(t < dist)
                {
                    var p = points[i] + dir*t;
                    AddPoint(p, true);
                    t += step;
                }
            }
        }
    }
}