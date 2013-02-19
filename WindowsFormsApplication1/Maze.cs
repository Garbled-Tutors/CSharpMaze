using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class cMaze
    {
        public readonly int I_MAZE_SIZE;
        public readonly bool B_DEBUG_MAZE_GEN;
        public bool[,] ibHorizontalWalls;
        public bool[,] ibVerticalWalls;
        List<Point> plSolution = new List<Point>();
        private cMazeGen mgMazeGenerator;

        public Point GetMazeStart() { return plSolution[0]; }
        public Point GetMazeEnd() { return plSolution[plSolution.Count - 1]; }
        public int GetMazeSize() { return I_MAZE_SIZE; }
        public cMaze(int iMazeSize = 16, bool bGenerateMaze = true)
        {
            I_MAZE_SIZE = iMazeSize;
            B_DEBUG_MAZE_GEN = !bGenerateMaze;

            ibHorizontalWalls = new bool[I_MAZE_SIZE + 1, I_MAZE_SIZE];
            ibVerticalWalls = new bool[I_MAZE_SIZE, I_MAZE_SIZE + 1];

            mgMazeGenerator = new cMazeGen(I_MAZE_SIZE);
            if (B_DEBUG_MAZE_GEN == false)
                mgMazeGenerator.GenerateMaze(ref ibHorizontalWalls, ref ibVerticalWalls);
            else
                mgMazeGenerator.ExtendMazeOneStep(ref ibHorizontalWalls, ref ibVerticalWalls);
            plSolution = mgMazeGenerator.GetSolution();
        }
        public List<Point> GetSolution()
        {
            if (B_DEBUG_MAZE_GEN == true)
                plSolution = mgMazeGenerator.GetCurrentPath();
            return plSolution;
        }
        public void ExtendMazeOneStep()
        {
            mgMazeGenerator.ExtendMazeOneStep(ref ibHorizontalWalls, ref ibVerticalWalls);
        }
        public bool PathExist(Point pFrom, Point pTo)
        {
            if (pFrom.X == pTo.X)//If we are moving Vertically
            {
                if (pFrom.Y == pTo.Y + 1)//Moving Up
                {
                    return !ibVerticalWalls[pFrom.X, pFrom.Y];
                }
                else if (pFrom.Y == pTo.Y - 1)//Moving Down
                {
                    return !ibVerticalWalls[pFrom.X, pTo.Y];
                }
            }
            else if (pFrom.Y == pTo.Y)//If we are moving Horizontally
            {
                if (pFrom.X == pTo.X + 1)//Moving Left
                {
                    return !ibHorizontalWalls[pFrom.X, pFrom.Y];
                }
                else if (pFrom.X == pTo.X - 1)//Moving Right
                {
                    return !ibHorizontalWalls[pTo.X, pFrom.Y];
                }
            }
            return false;
        }
        public cLinePoints[] GetMazeMap()
        {
            List<cLinePoints> llpMazeMap = new List<cLinePoints>();
            for (int x = 0; x <= I_MAZE_SIZE; x++)
            {
                for (int y = 0; y <= I_MAZE_SIZE; y++)
                {
                    if ((y != I_MAZE_SIZE) && (ibHorizontalWalls[x, y]))
                    {
                        llpMazeMap.Add(new cLinePoints(new Point(x, y), new Point(x, y + 1)));
                    }
                    if ((x != I_MAZE_SIZE) && (ibVerticalWalls[x, y]))
                    {
                        llpMazeMap.Add(new cLinePoints(new Point(x, y), new Point(x + 1, y)));
                    }
                }
            }
            return llpMazeMap.ToArray();
        }
        public bool ShowSolution() //debugging code
        {
            return false;
        }
    }
}
