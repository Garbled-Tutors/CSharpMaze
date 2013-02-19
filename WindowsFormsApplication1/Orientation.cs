using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class cOrientation
    {
        public Direction dOrientation;

        private static readonly Dictionary<Direction, int> DirectionIndexTable = new Dictionary<Direction, int>
        {
            { Direction.North, 0},
            { Direction.East, 1},
            { Direction.South, 2},
            { Direction.West, 3}
        };
        private static readonly Dictionary<int, Direction> ReverseDirectionIndexTable = DirectionIndexTable.ToDictionary(x => x.Value, x => x.Key);
        private static readonly Dictionary<RelativeDirection, int> RelativeDirectionConversionTable = new Dictionary<RelativeDirection, int>
        {
            { RelativeDirection.Forward, 0 },
            { RelativeDirection.Backward, 2 },
            { RelativeDirection.Right, 1 },
            { RelativeDirection.Left, 3 }
        };
        private static readonly Dictionary<int, RelativeDirection> ReverseRelativeDirectionIndexTable = RelativeDirectionConversionTable.ToDictionary(x => x.Value, x => x.Key);
        private static readonly Dictionary<Direction, Point> AdjacentPointXYAdjustmentTable = new Dictionary<Direction, Point>
        {
            { Direction.North, new Point(0,-1)},
            { Direction.East, new Point(1,0)},
            { Direction.South, new Point(0,1)},
            { Direction.West, new Point(-1,0)}
        };
        private static readonly Dictionary<Point, Direction> ReverseAdjacentPointAdjustmentTable = AdjacentPointXYAdjustmentTable.ToDictionary(x => x.Value, x => x.Key);

        public cOrientation()
        {
            dOrientation = Direction.North;
        }
        public static RelativeDirection GetRelativeDirection(Direction dFrom, Direction dTo)
        {
            return ReverseRelativeDirectionIndexTable[(DirectionIndexTable[dTo] - DirectionIndexTable[dFrom] + 4) % 4];
        }
        private Direction GetNewDirection(RelativeDirection rdTurnDirection)
        {
            return ReverseDirectionIndexTable[(DirectionIndexTable[dOrientation] + RelativeDirectionConversionTable[rdTurnDirection]) % 4];
        }
        public void Turn(RelativeDirection rdTurnDirection)
        {
            dOrientation = GetNewDirection(rdTurnDirection);
        }
        public Point GetAdjacentPoint(RelativeDirection rdDirection, Point pCurrentLocation)
        {
            Direction dDesiredDirection = GetNewDirection(rdDirection);
            Point pPointAdjustment = AdjacentPointXYAdjustmentTable[dDesiredDirection];
            return new Point(pCurrentLocation.X + pPointAdjustment.X, pCurrentLocation.Y + pPointAdjustment.Y);
        }
        public static Direction GetDirectionBetweenTwoAdjacentPoints(Point pStart, Point pEnd)
        {
            Point pDiff = new Point(pEnd.X - pStart.X, pEnd.Y - pStart.Y);
            return ReverseAdjacentPointAdjustmentTable[pDiff];            
        }
        public Direction GetCurrentOrientation()
        {
            return dOrientation;
        }
    }
}
