using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    public enum Direction
    {
        North, South, East, West, None
    }
    public enum RelativeDirection
    {
        Forward, Left, Right, Backward, None
    }
    public enum ShapeTypes
    {
        Polygon, Rectangle, Ellipse, Text, Line, JaggedLine
    }
    public enum CellPosition
    {
        Left, Right, Front, Ground
    }
    public class cShapeOutline
    {
        private ShapeTypes stShapeType;
        private List<Point> plShapeCoordinates;
        private Color cShapeColor;
        private String sText;
        public cShapeOutline(ShapeTypes stNewShapeType, List<Point> plNewShapeCoordinates, Color cNewShapeColor)
        {
            stShapeType = stNewShapeType;
            plShapeCoordinates = new List<Point>(plNewShapeCoordinates);
            cShapeColor = cNewShapeColor;
            sText = "Error";
        }
        public cShapeOutline(string sNewText, Point pLocation, Color cNewShapeColor)
        {
            stShapeType = ShapeTypes.Text;
            plShapeCoordinates = new List<Point>();
            plShapeCoordinates.Add(pLocation);
            cShapeColor = cNewShapeColor;
            sText = sNewText;
        }
        public ShapeTypes GetShapeType()
        {
            return stShapeType;
        }
        public string GetText()
        {
            return sText;
        }
        public Color GetColor()
        {
            return cShapeColor;
        }
        public Font GetFont()
        {
            return new Font("Tahoma", 15);
        }
        public Rectangle GetRectangle()//Converts the shape into a rectangle. Assumes at least one point exists
        {
            List<int> ilXValues = new List<int>();
            List<int> ilYValues = new List<int>();
            foreach (Point pShapeAxis in plShapeCoordinates)
            {
                ilXValues.Add(pShapeAxis.X);
                ilYValues.Add(pShapeAxis.Y);
            }
            int iWidth = ilXValues.Max() - ilXValues.Min();
            int iHeight = ilYValues.Max() - ilYValues.Min();
            return new Rectangle(plShapeCoordinates[0].X, plShapeCoordinates[0].Y, iWidth, iHeight);
        }
        public Point[] GetAllPoints()
        {
            return plShapeCoordinates.ToArray();
        }
    }
    public class cStaticShapeOutlines
    {
        public static int GetPictureBounds()
        {
            return 16;
        }
        public static List<Point> GetWallOutline(CellPosition cpWallLocation)
        {
            if (cpWallLocation == CellPosition.Left)
                return new List<Point>(new Point[] { new Point(0, 8), new Point(4, 0), new Point(4, 8), new Point(0, 16), new Point(0, 8) });
            else if (cpWallLocation == CellPosition.Front)
                return new List<Point>(new Point[] { new Point(4, 0), new Point(4, 8), new Point(12, 8), new Point(12, 0), new Point(4, 0) });
            else if (cpWallLocation == CellPosition.Right)
                return new List<Point>(new Point[] { new Point(12, 0), new Point(16, 8), new Point(16, 16), new Point(12, 8), new Point(12, 0) });
            return new List<Point>(new Point[] { new Point(0, 0) });
        }
        public static List<Point> GetArchOutline(CellPosition cpArchLocation)
        {
            if (cpArchLocation == CellPosition.Left)
                return new List<Point>(new Point[] { new Point ( 0, 8 ), new Point ( 4, 0 ), new Point ( 4, 8 ), new Point ( 3, 10 ), new Point ( 3, 4 ), new Point ( 1, 8 ), new Point ( 1, 14 ), 
                    new Point ( 0, 16 ), new Point ( 0, 8 ) });

            else if (cpArchLocation == CellPosition.Front)
                return new List<Point>(new Point[] { new Point ( 4,8), new Point ( 4,0), new Point (12, 0), new Point (12, 8), new Point (10,8), new Point (10,2), new Point ( 6, 2), 
                    new Point (6, 8), new Point ( 4,8) });

            else if (cpArchLocation == CellPosition.Right)
                return new List<Point>(new Point[] { new Point ( 12, 0 ), new Point ( 16, 8 ), new Point ( 16, 16 ), new Point ( 15, 14 ), new Point ( 15, 8 ), new Point ( 13, 4 ), new Point ( 13, 10 ), 
                    new Point ( 12, 8 ), new Point ( 12, 0 ) });

            return new List<Point>(new Point[] { new Point(0, 0) });
        }
    }
    class cLinePoints
    {
        public Point pStart, pEnd;
        public cLinePoints(Point pPassedStart, Point pPassedEnd)
        {
            pStart = pPassedStart;
            pEnd = pPassedEnd;
        }
        public List<Point> GetPointList()
        {
            List<Point> plAllPoints = new List<Point>();
            plAllPoints.Add(pStart);
            plAllPoints.Add(pEnd);
            return plAllPoints;
        }
        public Point GetStartPoint() { return pStart; }
        public Point GetEndPoint() { return pEnd; }
    }
    class cGameObject
    {
        public readonly int[, ,] IA_BORDER;
        public readonly int I_PICTURE_BOUNDS = 16;
        public cGameObject(int[, ,] iaBorders) { IA_BORDER = iaBorders; }
        public int GetPictureBounds() { return I_PICTURE_BOUNDS; }
        public int GetPointCount() { return IA_BORDER.GetUpperBound(1); }
        public Point GetPoint(int side, int index) { return new Point(IA_BORDER[side, index, 0], IA_BORDER[side, index, 1]); }
    }
    class cPointPositionCalculator
    {
        public readonly int[] iaOffsets;
        public readonly int[] iaMultipliers;
        public cPointPositionCalculator(int iXOffset, int iYOffset, int iXMultiplier, int iYMultiplier)
        {
            iaOffsets = new int[] { iXOffset, iYOffset };
            iaMultipliers = new int[] { iXMultiplier, iYMultiplier };
        }
        public Point GetAdjustedPoint(Point CurrentPosition)
        {
            CurrentPosition.X = (CurrentPosition.X * iaMultipliers[0]) + iaOffsets[0];
            CurrentPosition.Y = (CurrentPosition.Y * iaMultipliers[1]) + iaOffsets[1];
            return CurrentPosition;
        }
    }
    class cPointCalculator
    {
        private readonly int[] iaOffsets;
        private readonly int[] iaMultipliers;

        public readonly int I_TOP_OFFSET = 250;
        public readonly int I_BOTTOM_OFFSET = 50;
        public readonly int I_LEFT_OFFSET = 50;
        public readonly int I_RIGHT_OFFSET = 50;

        public cPointCalculator(int iScreenWidth, int iStreenHeight)
        {
            int iMaxValue = cStaticShapeOutlines.GetPictureBounds();
            int iXMultiplier = (iScreenWidth - I_LEFT_OFFSET - I_RIGHT_OFFSET) / iMaxValue;
            int iYMultiplier = (iStreenHeight - I_TOP_OFFSET - I_BOTTOM_OFFSET) / iMaxValue;

            iaOffsets = new int[] { I_LEFT_OFFSET, I_TOP_OFFSET };
            iaMultipliers = new int[] { iXMultiplier, iYMultiplier };
        }
        public Point GetAdjustedPoint(Point CurrentPosition)
        {
            CurrentPosition.X = (CurrentPosition.X * iaMultipliers[0]) + iaOffsets[0];
            CurrentPosition.Y = (CurrentPosition.Y * iaMultipliers[1]) + iaOffsets[1];
            return CurrentPosition;
        }
        public cShapeOutline GetAdjustedEllipse(Point pCenter, Color cShapeColor)
        {
            List<Point> lpShapePoints = new List<Point>();
            lpShapePoints.Add(pCenter);
            lpShapePoints.Add(new Point(pCenter.X +1, pCenter.Y + 1));

            return GetAdjustedShape(lpShapePoints,cShapeColor,ShapeTypes.Ellipse);
        }
        public cShapeOutline GetAdjustedSolutionLine(List<Point> lpLinePoints, Color cShapeColor)
        {
            for (int i = 0; i < lpLinePoints.Count; i++)
            {
                //get the top left position of the cell
                lpLinePoints[i] = GetAdjustedPoint(lpLinePoints[i]);

                //move the point to the center
                lpLinePoints[i] = new Point(lpLinePoints[i].X + (iaMultipliers[0] / 2), lpLinePoints[i].Y + (iaMultipliers[1] / 2));
            }
            return new cShapeOutline(ShapeTypes.JaggedLine, lpLinePoints, cShapeColor);
        }
        public cShapeOutline GetAdjustedArrow(Point pArrowPosition, Direction dArrowDirection, Color cShapeColor)
        {
            List<Point> lpArrowVertices = new List<Point>();
            Point pTopLeft = GetAdjustedPoint(pArrowPosition);
            int iYMargin = iaMultipliers[1] / 10; int iXMargin = iaMultipliers[0] / 10; int iCellHeight = iaMultipliers[1]; int iCellWidth = iaMultipliers[0];

            Dictionary<string, int> ArrowPoints = new Dictionary<string, int>
                {
                    { "Top", pTopLeft.Y + iYMargin}, {"Bottom", pTopLeft.Y + iCellHeight - iYMargin}, 
                    { "Left", pTopLeft.X + iXMargin}, {"Right", pTopLeft.X + iCellWidth - iXMargin}, 
                    { "XCenter", pTopLeft.X + (iCellWidth / 2)}, {"YCenter", pTopLeft.Y + (iCellHeight / 2)}
                };
            Dictionary<Direction, Point[]> PlayerArrowPoints = new Dictionary<Direction, Point[]>
                {
                    { Direction.North, new Point[] {new Point(ArrowPoints["XCenter"], ArrowPoints["Top"]), new Point(ArrowPoints["Left"], ArrowPoints["Bottom"]), new Point(ArrowPoints["Right"], ArrowPoints["Bottom"]) } },
                    { Direction.South, new Point[] {new Point(ArrowPoints["XCenter"], ArrowPoints["Bottom"]), new Point(ArrowPoints["Left"], ArrowPoints["Top"]), new Point(ArrowPoints["Right"], ArrowPoints["Top"]) } },
                    { Direction.East, new Point[] {new Point(ArrowPoints["Right"], ArrowPoints["YCenter"]), new Point(ArrowPoints["Left"], ArrowPoints["Top"]), new Point(ArrowPoints["Left"], ArrowPoints["Bottom"]) } },
                    { Direction.West, new Point[] {new Point(ArrowPoints["Left"], ArrowPoints["YCenter"]), new Point(ArrowPoints["Right"], ArrowPoints["Top"]), new Point(ArrowPoints["Right"], ArrowPoints["Bottom"]) } }
                };
            foreach (Point pArrowVertex in PlayerArrowPoints[dArrowDirection])
            {
                lpArrowVertices.Add(pArrowVertex);
            }

            return new cShapeOutline(ShapeTypes.Polygon, lpArrowVertices, cShapeColor);
        }
        public cShapeOutline GetAdjustedShape(List<Point> lpShapePoints, Color cShapeColor, ShapeTypes stShapeType)
        {
            for (int i = 0; i < lpShapePoints.Count; i++)
                lpShapePoints[i] = GetAdjustedPoint(lpShapePoints[i]);
            return new cShapeOutline(stShapeType, lpShapePoints, cShapeColor);
        }
        public cShapeOutline[] GetWallShapes(ref cMaze mMaze, Point pPosition, ref cOrientation oPlayerOrientation)
        {
            List<cShapeOutline> solWallShapes = new List<cShapeOutline>();
            if (mMaze.PathExist(pPosition, oPlayerOrientation.GetAdjacentPoint(RelativeDirection.Left, pPosition)))
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetArchOutline(CellPosition.Left), Color.Black,ShapeTypes.Polygon));
            else
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetWallOutline(CellPosition.Left), Color.Black, ShapeTypes.Polygon));

            if (mMaze.PathExist(pPosition, oPlayerOrientation.GetAdjacentPoint(RelativeDirection.Forward, pPosition)))
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetArchOutline(CellPosition.Front), Color.Black, ShapeTypes.Polygon));
            else
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetWallOutline(CellPosition.Front), Color.Black, ShapeTypes.Polygon));

            if (mMaze.PathExist(pPosition, oPlayerOrientation.GetAdjacentPoint(RelativeDirection.Right, pPosition)))
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetArchOutline(CellPosition.Right), Color.Black, ShapeTypes.Polygon));
            else
                solWallShapes.Add(GetAdjustedShape(cStaticShapeOutlines.GetWallOutline(CellPosition.Right), Color.Black, ShapeTypes.Polygon));
            
            return solWallShapes.ToArray();
        }
        private cShapeOutline GetQuick32Line(Point pStart, Point pEnd)
        {
            List<Point> plPoints = new List<Point>();
            plPoints.Add(new Point((pStart.X * iaMultipliers[0] / 2) + iaOffsets[0], (pStart.Y * iaMultipliers[1] / 2) + iaOffsets[1]));
            plPoints.Add(new Point((pEnd.X * iaMultipliers[0] / 2) + iaOffsets[0], (pEnd.Y * iaMultipliers[1] / 2) + iaOffsets[1]));
            return new cShapeOutline(ShapeTypes.Line, plPoints, Color.Black);
        }
        private cShapeOutline GetQuick32Text(string sText, Point pTopLeft)
        {
            return new cShapeOutline(sText, new Point((pTopLeft.X * iaMultipliers[0] / 2) + iaOffsets[0], (pTopLeft.Y * iaMultipliers[1] / 2) + iaOffsets[1]), Color.Black);
        }
        public cShapeOutline[] GetStreet()
        {
            List<cShapeOutline> solStreetShapes = new List<cShapeOutline>();

            //Left Side
            solStreetShapes.Add(GetQuick32Line(new Point(8, 32), new Point(12, 24)));
            solStreetShapes.Add(GetQuick32Line(new Point(13, 22), new Point(16, 16)));
            solStreetShapes.Add(GetQuick32Line(new Point(0, 24), new Point(12, 24)));
            solStreetShapes.Add(GetQuick32Line(new Point(0, 22), new Point(13, 22)));

            //Right Side
            solStreetShapes.Add(GetQuick32Line(new Point(24, 32), new Point(20, 24)));
            solStreetShapes.Add(GetQuick32Line(new Point(19, 22), new Point(16, 16)));
            solStreetShapes.Add(GetQuick32Line(new Point(32, 24), new Point(22, 24)));
            solStreetShapes.Add(GetQuick32Line(new Point(21, 24), new Point(20, 24)));
            solStreetShapes.Add(GetQuick32Line(new Point(32, 22), new Point(22, 22)));
            solStreetShapes.Add(GetQuick32Line(new Point(21, 22), new Point(19, 22)));

            //Light Pole
            solStreetShapes.Add(GetQuick32Line(new Point(21, 25), new Point(21, 0)));
            solStreetShapes.Add(GetQuick32Line(new Point(22, 25), new Point(22, 0)));
            solStreetShapes.Add(GetQuick32Line(new Point(21, 0), new Point(22, 0)));

            solStreetShapes.Add(GetQuick32Line(new Point(22, 2), new Point(32, 2)));
            solStreetShapes.Add(GetQuick32Line(new Point(22, 5), new Point(32, 5)));
            solStreetShapes.Add(GetQuick32Line(new Point(32, 5), new Point(32, 2)));

            solStreetShapes.Add(GetQuick32Text("Oso Cll", new Point(22, 2)));
            //solStreetShapes.Add(new cShapeOutline("Cheese Street",new Point(22, 2),Color.Black));

            return solStreetShapes.ToArray();
        }
        public cShapeOutline[] GetMap(ref cMaze mMaze, Point pPosition, ref cOrientation oPlayerOrientation)
        {
            List<cShapeOutline> solMapShapes = new List<cShapeOutline>();
            cLinePoints[] lpaMazeWalls = mMaze.GetMazeMap();
            
            foreach (cLinePoints lpLinePoints in lpaMazeWalls)
            {
                solMapShapes.Add(GetAdjustedShape(lpLinePoints.GetPointList(), Color.Black, ShapeTypes.Line));
            }
            solMapShapes.Add(GetAdjustedEllipse(mMaze.GetMazeEnd(), Color.Red));

            if (mMaze.ShowSolution() == true)
                solMapShapes.Add(GetAdjustedSolutionLine(mMaze.GetSolution(), Color.Purple));
            solMapShapes.Add(GetAdjustedArrow(pPosition, oPlayerOrientation.GetCurrentOrientation(), Color.Green));

            return solMapShapes.ToArray();
        }
    }
}
