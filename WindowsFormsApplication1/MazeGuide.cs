using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class cMazeGuide
    {
        // Audio Files created with
        // http://www.spanishdict.com
        // http://www.spanishdict.com/translate/
        // http://audio.online-convert.com/convert-to-wav

        protected enum GuideResponses
        {
            Good, Bad, Win, None
        }
        protected readonly Dictionary<GuideResponses, string[]> dsaResponses = new Dictionary<GuideResponses, string[]>
                {
                    { GuideResponses.Good , new string[] { "Caliente", "Mejor"} },
                    { GuideResponses.Bad , new string[] { "Frio", "Peor"} },
                    { GuideResponses.Win , new string[] { "Victoria" } }
                    //{ GuideResponses.Good , new string[] { "Warmer", "Closer", "Good", "Better", "Not Bad"} },
                    //{ GuideResponses.Bad , new string[] { "Colder", "Wrong Way", "Nope", "Go Back", "Incorrect" } },
                    //{ GuideResponses.Win , new string[] { "Victory", "You Win" } }
                };

        protected List<Point> plSolution = new List<Point>();
        protected Point pLastSquare;
        protected int iResponseCount = 0;
        protected GuideResponses grLastResponse = GuideResponses.None;

        public cMazeGuide(List<Point> plNewSolution)
        {
            plSolution = new List<Point>(plNewSolution);
            pLastSquare = plSolution[0];
            plSolution.RemoveAt(0);
        }
        public string GetHint(Point pCurrentLocation, cOrientation oPlayerOrientation, bool bPlayAudio = false)
        {
            GuideResponses grCurrentResponse;

            if (plSolution.Count() == 1)
                grCurrentResponse = GuideResponses.Win;
            else if (plSolution[0] == pCurrentLocation)
            {
                plSolution.RemoveAt(0);
                pLastSquare = pCurrentLocation;
                grCurrentResponse = GuideResponses.Good;
            }
            else
            {
                plSolution.Insert(0, pLastSquare);
                pLastSquare = pCurrentLocation;
                grCurrentResponse = GuideResponses.Bad;
            }

            int iResponseIndex = new Random().Next(dsaResponses[grCurrentResponse].Count());

            if ( (grLastResponse == grCurrentResponse) && (iResponseCount < 4))
            {
                iResponseCount++;
                return "";
            }/*
            else if (bPlayAudio)
            {
                string sLocation = dsaResponses[grCurrentResponse][iResponseIndex].Replace(" ", "") + ".wav";
                sLocation = System.IO.Directory.GetCurrentDirectory() + "\\" + sLocation.ToLower();
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(sLocation);
                player.Play();
            }*/

            grLastResponse = grCurrentResponse;
            iResponseCount = 1;
            return ReturnResponse(dsaResponses[grCurrentResponse][iResponseIndex], bPlayAudio);
        }
        public string GetDirectionHint(Point pCurrentLocation, cOrientation oPlayerOrientation, bool bPlayAudio = false)
        {
            return "";//Child Classes use this
        }
        public List<Point> GetSolution()
        {
            //This is for debugging purposes
            return plSolution;
        }
        public string ReturnResponse(String sResponse, bool bPlayAudio)
        {
            if (bPlayAudio)
            {
                string sLocation = sResponse.Replace(" ", "") + ".wav";
                sLocation = System.IO.Directory.GetCurrentDirectory() + "\\" + sLocation.ToLower();
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(sLocation);
                player.Play();
            }
            return sResponse;
        }
    }
    class cMazeDirections : cMazeGuide
    {/*
        enum GuideDirections
        {
            Forward, Left, Right, TurnAround, Arrived, None
        }*/
        protected readonly Dictionary<RelativeDirection, string[]> dsaDirections = new Dictionary<RelativeDirection, string[]>
                {
                    { RelativeDirection.Forward , new string[] { " Derecho"} },
                    { RelativeDirection.Left , new string[] { "Izquierda "} },
                    { RelativeDirection.Right , new string[] { "Derecha" } }
                };
        protected readonly string[] saArrivedText = new string[] { "Victoria" };
        protected readonly string[] saTurnAroundText = new string[] { "Girar" };        

        //protected readonly Dictionary<GuideDirections, string[]> dsaDirections = new Dictionary<GuideDirections, string[]>;
        protected RelativeDirection rdCurrentInstruction = RelativeDirection.None;

        public cMazeDirections(List<Point> plNewSolution) : base(plNewSolution)
        {
        }
        public new string GetHint(Point pCurrentLocation, cOrientation oPlayerOrientation, bool bPlayAudio = false)
        {
            if (plSolution.Count() == 1)
                return ReturnResponse(saArrivedText[0], bPlayAudio);
            else
            {
                if (plSolution[0] == pCurrentLocation)
                {
                    plSolution.RemoveAt(0);
                    pLastSquare = pCurrentLocation;
                }
                else
                {
                    plSolution.Insert(0, pLastSquare);
                    pLastSquare = pCurrentLocation;
                }
                Direction dVectorDirection = cOrientation.GetDirectionBetweenTwoAdjacentPoints(pCurrentLocation, plSolution[0]);
                RelativeDirection dRelativeDirection = cOrientation.GetRelativeDirection(oPlayerOrientation.GetCurrentOrientation(),dVectorDirection);
                
                if (dRelativeDirection == RelativeDirection.Backward)
                    return ReturnResponse(saTurnAroundText[0], bPlayAudio);
                if (dRelativeDirection == RelativeDirection.None)
                    return "";
                return ReturnResponse(dsaDirections[dRelativeDirection][0], bPlayAudio);
            }
        }
        public new string GetDirectionHint(Point pCurrentLocation, cOrientation oPlayerOrientation, bool bPlayAudio = false)
        {
            return "";
        }
    }
}
