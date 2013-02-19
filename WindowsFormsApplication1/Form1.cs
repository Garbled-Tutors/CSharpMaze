using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//я
//Form1.MousePosition.X
//int iMaxValue = IA_WALLS.Cast<int>().Max();
//gfx.DrawString(wWidth.ToString(), new Font("Tahoma", 15), new SolidBrush(Color.Red), new Point(100, 100));

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        enum GameModes
        {
            MazeMap, MazeRoom, CityMap, CityStreet
        }
        private GameModes gmCurrentGameMode = GameModes.CityMap;
        private cOrientation oPlayerOrientation = new cOrientation();

        public readonly int I_TOP_OFFSET = 250;
        public readonly int I_BOTTOM_OFFSET = 50;
        public readonly int I_LEFT_OFFSET = 50;
        public readonly int I_RIGHT_OFFSET = 50;

        private cMaze mMaze = new cMaze(8);
        private cMazeDirections mgMazeGuide;
        private Point pPosition = new Point(-1, -1);
        public string sCurrentMessage = "";

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pPosition = mMaze.GetMazeStart();
            mgMazeGuide = new cMazeDirections(mMaze.GetSolution());
        }
        private void PaintGameObjects(ref Graphics gfx, cShapeOutline[] soaListOfObjects)
        {
            Point[] paAllPoints;
            foreach (cShapeOutline soObject in soaListOfObjects)
            {
                switch (soObject.GetShapeType())
                {
                    case ShapeTypes.Ellipse:
                        gfx.DrawEllipse(new Pen(soObject.GetColor()), soObject.GetRectangle());
                        break;
                    case ShapeTypes.Rectangle:
                        gfx.DrawRectangle(new Pen(soObject.GetColor()), soObject.GetRectangle());
                        break;
                    case ShapeTypes.Polygon:
                        gfx.DrawPolygon(new Pen(soObject.GetColor()), soObject.GetAllPoints());
                        break;
                    case ShapeTypes.Text:
                        gfx.DrawString(soObject.GetText(), soObject.GetFont(), new SolidBrush(soObject.GetColor()), soObject.GetAllPoints()[0]);
                        break;
                    case ShapeTypes.Line:
                        paAllPoints = soObject.GetAllPoints();
                        gfx.DrawLine(new Pen(soObject.GetColor()), paAllPoints[0], paAllPoints[1]);
                        break;
                    case ShapeTypes.JaggedLine:
                        paAllPoints = soObject.GetAllPoints();

                        for (int i = paAllPoints.Count() - 1; i > 0; i--)
                        {
                            gfx.DrawLine(new Pen(soObject.GetColor()), paAllPoints[i], paAllPoints[i  - 1]);
                        }
                        break;
                }
            }
        }
        protected override void OnPaint(PaintEventArgs paintEvnt)
        {
            Graphics gfx = paintEvnt.Graphics;
            cPointCalculator pcCalc = new cPointCalculator(Form1.ActiveForm.ClientSize.Width, Form1.ActiveForm.ClientSize.Height);

            if (gmCurrentGameMode == GameModes.MazeMap)
                PaintGameObjects(ref gfx, pcCalc.GetMap(ref mMaze, pPosition, ref oPlayerOrientation));
            else if (gmCurrentGameMode == GameModes.MazeRoom)
                PaintGameObjects(ref gfx, pcCalc.GetWallShapes(ref mMaze, pPosition, ref oPlayerOrientation));
            else if (gmCurrentGameMode == GameModes.CityStreet)
                PaintGameObjects(ref gfx, pcCalc.GetStreet());
            //else if (gmCurrentGameMode == GameModes.CityMap)
            
            gfx.DrawString(sCurrentMessage, new Font("Tahoma", 15), new SolidBrush(Color.Black), new Point(20, 20));
        }
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Invalidate();
        }
        public void OnClick(object sender, EventArgs e)
        {/* //Debug Maze Generator Code
            if (I_GENERATE_MAZE_STEP != -1)
            {
                for (int i = 0; i < I_GENERATE_MAZE_STEP; i++)
                {
                    mMaze.ExtendMazeOneStep();
                }
                Invalidate();
            }*/
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Point pDestination = new Point(-1, -1);
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            {
                RelativeDirection rdMoveDirection = (e.KeyCode == Keys.Up) ? RelativeDirection.Forward : RelativeDirection.Backward;
                pDestination = oPlayerOrientation.GetAdjacentPoint(rdMoveDirection, pPosition);
                if (mMaze.PathExist(pPosition, pDestination))
                {
                    pPosition = pDestination;
                    sCurrentMessage = mgMazeGuide.GetHint(pPosition, oPlayerOrientation, true);
                }
            }
            if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right))
            {
                if (e.KeyCode == Keys.Left)
                    oPlayerOrientation.Turn(RelativeDirection.Left);
                else if (e.KeyCode == Keys.Right)
                    oPlayerOrientation.Turn(RelativeDirection.Right);

                sCurrentMessage = mgMazeGuide.GetDirectionHint(pPosition, oPlayerOrientation, true);
            }
            else if (e.KeyCode == Keys.Tab)
            {
                if (gmCurrentGameMode == GameModes.CityMap)
                    gmCurrentGameMode = GameModes.CityStreet;
                else if (gmCurrentGameMode == GameModes.CityStreet)
                    gmCurrentGameMode = GameModes.CityMap;

                else if (gmCurrentGameMode == GameModes.MazeRoom)
                    gmCurrentGameMode = GameModes.MazeMap;
                else if (gmCurrentGameMode == GameModes.MazeRoom)
                    gmCurrentGameMode = GameModes.MazeMap;
            }


            Invalidate();
        }
        //Want to remove
        private void PaintGameObject(ref Graphics gfx, ref Pen myPen, ref cGameObject goGameObject, ref cPointPositionCalculator ppcCalc, int iWallIndex)
        {
            int j = 0;
            while (j < goGameObject.GetPointCount())
            {
                Point pStart = ppcCalc.GetAdjustedPoint(goGameObject.GetPoint(iWallIndex, j));
                j = j + 1;
                Point pEnd = ppcCalc.GetAdjustedPoint(goGameObject.GetPoint(iWallIndex, j));

                gfx.DrawLine(myPen, pStart, pEnd);
            }
        }

    }
}
