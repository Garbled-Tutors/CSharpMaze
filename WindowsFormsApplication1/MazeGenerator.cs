using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class cMazeGen
    {
        public readonly int I_TOTAL_CELLS;
        public readonly int I_MAZE_SIZE;
        private Point pCurrentCell;
        private Random random = new Random();

        List<Point> plCurrentPath = new List<Point>();
        List<Point> plLongestPath = new List<Point>();
        private bool[,] baVisitedCells;
        public int iTotalCellsVisited = -1;

        public cMazeGen(int iMazeSize)
        {
            I_MAZE_SIZE = iMazeSize;
            I_TOTAL_CELLS = iMazeSize * iMazeSize;
        }
        public List<Point> GetSolution()
        {
            return plLongestPath;
        }
        public List<Point> GetCurrentPath()
        {
            return plCurrentPath;
        }
        private void ResetMaze(ref bool[,] ibHorizontalWalls, ref bool[,] ibVerticalWalls)
        {
            iTotalCellsVisited = 0;
            pCurrentCell = new Point(random.Next(I_MAZE_SIZE), random.Next(I_MAZE_SIZE));
            plCurrentPath.Add(pCurrentCell);
            plLongestPath.Add(pCurrentCell);

            ibHorizontalWalls = new bool[I_MAZE_SIZE + 1, I_MAZE_SIZE];
            ibVerticalWalls = new bool[I_MAZE_SIZE, I_MAZE_SIZE + 1];
            baVisitedCells = new bool[I_MAZE_SIZE, I_MAZE_SIZE];

            //Sets starting values
            for (int x = 0; x < I_MAZE_SIZE; x++) { for (int y = 0; y < I_MAZE_SIZE; y++) { baVisitedCells[x, y] = false; ibHorizontalWalls[x, y] = true; ibVerticalWalls[x, y] = true; } }
            for (int i = 0; i < I_MAZE_SIZE; i++) { ibHorizontalWalls[I_MAZE_SIZE, i] = true; ibVerticalWalls[i, I_MAZE_SIZE] = true; }
        }
        public Point[] GetAllAdacentUnvisitedSquares()
        {
            List<Point> plAdacentUnvisitedSquares = new List<Point>();

            Point[] paPossiblePoints = new Point[] { new Point(pCurrentCell.X - 1, pCurrentCell.Y), new Point(pCurrentCell.X + 1, pCurrentCell.Y), 
                new Point(pCurrentCell.X, pCurrentCell.Y - 1), new Point(pCurrentCell.X, pCurrentCell.Y + 1)  };

            foreach (Point pPoint in paPossiblePoints)
            {
                if ((pPoint.X >= 0) && (pPoint.X < I_MAZE_SIZE) && (pPoint.Y >= 0) && (pPoint.Y < I_MAZE_SIZE) && (baVisitedCells[pPoint.X, pPoint.Y] == false))
                {
                    plAdacentUnvisitedSquares.Add(pPoint);
                }
            }

            return plAdacentUnvisitedSquares.ToArray();
        }
        public void GenerateMaze(ref bool[,] ibHorizontalWalls, ref bool[,] ibVerticalWalls)
        {
            ResetMaze(ref ibHorizontalWalls, ref ibVerticalWalls);
            while (iTotalCellsVisited < I_TOTAL_CELLS)
            {
                ExtendMazeOneStep(ref ibHorizontalWalls, ref ibVerticalWalls);
            }
        }
        private void ExtendMazeIntoThisSquare(ref bool[,] ibHorizontalWalls, ref bool[,] ibVerticalWalls, Point pDestination)
        {
            //If we are moving horizontally
            if (pCurrentCell.Y == pDestination.Y)
            {
                int iDirection = (new int[] { pDestination.X - pCurrentCell.X, 0 }.Max());// needs to be a one or a zero
                ibHorizontalWalls[pCurrentCell.X + iDirection, pCurrentCell.Y] = false;
            }
            else//if we are moving vertically
            {
                int iDirection = (new int[] { pDestination.Y - pCurrentCell.Y, 0 }.Max());// needs to be a one or a zero
                ibVerticalWalls[pCurrentCell.X, pCurrentCell.Y + iDirection] = false;
            }

            pCurrentCell = pDestination;
            plCurrentPath.Insert(0, pCurrentCell);

            if (plCurrentPath.Count > plLongestPath.Count)
            {
                plLongestPath = new List<Point>(plCurrentPath);
            }
        }
        public void ExtendMazeOneStep(ref bool[,] ibHorizontalWalls, ref bool[,] ibVerticalWalls)
        {
            if (iTotalCellsVisited < 0)
            {
                ResetMaze(ref ibHorizontalWalls, ref ibVerticalWalls);
            }
            if (iTotalCellsVisited < I_TOTAL_CELLS)
            {
                if (baVisitedCells[pCurrentCell.X, pCurrentCell.Y] == false)
                {
                    baVisitedCells[pCurrentCell.X, pCurrentCell.Y] = true;
                    iTotalCellsVisited++;
                }
                Point[] paUnvisitedAdjacentSquareArray = GetAllAdacentUnvisitedSquares();

                if (paUnvisitedAdjacentSquareArray.Length > 0)
                {
                    int iNextCellIndex = random.Next(paUnvisitedAdjacentSquareArray.Length);
                    ExtendMazeIntoThisSquare(ref ibHorizontalWalls, ref ibVerticalWalls, paUnvisitedAdjacentSquareArray[iNextCellIndex]);
                }
                else
                {
                    pCurrentCell = plCurrentPath[1];
                    plCurrentPath.RemoveAt(0);
                }
            }
        }
    }
}
