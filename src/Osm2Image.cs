using ImagePF = SixLabors.ImageSharp.PixelFormats;
using ImageIS = SixLabors.ImageSharp;

namespace Osm2Png {
    public class Osm2Image {
        public Osm2Image()
        {
        }

        public void SaveImage(string filename, Grid grid)
        {
            int width = (int)grid.GetColNum(), height = (int)grid.GetRowNum();
            using (var image = new ImageIS.Image<ImagePF.Rgb24>(width, height))
            {
                var white = new ImagePF.Rgb24(255,255,255);
                var black = new ImagePF.Rgb24(0,0,0);
                var red = new ImagePF.Rgb24(255,0,0);
                for(int row = 0; row < height; row++)
                {
                    for(int col = 0; col < width; col++)
                    {
                        byte c = grid.grid[row][col];

                        if(c == 0)
                        {
                            image[col, row] = white;
                        }
                        else if(c == 1)
                        {
                            image[col, row] = red;
                        }
                        else
                        {
                            image[col, row] = black;
                        }
                    }
                }

                image.Save(filename);
            }
        }
    }
}