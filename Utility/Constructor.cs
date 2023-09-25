
namespace WY_App.Utility
{

    public class Constructor
    {
        public class CameraParams
        {
            public double[] Gain = new double[4];
            public double[] Shutter = new double[4];
            public double[] Black_Level = new double[4];
            public double[] Gamma = new double[4];
            public string[] CameraID = new string[4];
            public CameraParams()
            {
                for (int i = 0; i < 4; i++)
                {
                    Gain[i] = 10;
                    Shutter[i] = 10;
                    Black_Level[i] = 0;
                    Gamma[i] = 0;
                    CameraID[i] = "Cam" + i;
                }
            }
        }

        public struct Position
        {
            public double X;

            public double Y;

            public double R;

            public double Z;

            public double U;
        }

        public class Standardization
        {
            public Position[] pixelCoordinates;
            public Position[] worldCoordinate;


            public Standardization()
            {
                pixelCoordinates = new Position[9];
                worldCoordinate = new Position[9];
                for (int i = 0; i < 9; i++)
                {
                    pixelCoordinates[i].X = 0;
                    pixelCoordinates[i].Y = 0;
                    pixelCoordinates[i].R = 0;
                    pixelCoordinates[i].Z = 0;
                    pixelCoordinates[i].U = 0;

                    worldCoordinate[i].X = 0;
                    worldCoordinate[i].Y = 0;
                    worldCoordinate[i].R = 0;
                    worldCoordinate[i].Z = 0;
                    worldCoordinate[i].U = 0;
                }
            }                     
        }
    }
}
