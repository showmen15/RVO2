using RVO3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private ConcurrentDictionary<int, State> AllRobotStates = new ConcurrentDictionary<int, State>();
        private List<SimRVO3> rvo = new List<SimRVO3>();
        private List<Thread> thr = new List<Thread>();

        List<Vector2> robotStartPos = new List<Vector2>();
        List<Vector2> robotEndPos = new List<Vector2>();

        List<IList<Vector2>> obst = GetObstacles();

        Pen pen = new Pen(Color.Green, 1);
        Font drawFont = new Font("Arial", 10);
        SolidBrush drawBrush = new SolidBrush(Color.Black);

        private bool working = false;

        private int licznik;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            killAll();

            //Block
            //initRobot();

            //            configurAgent(@"1;-1.57; 0;3.5;4; 0;3.5;1
            //2;1.57; 0;3.5;1; 0;3.5;4");

            //configurAgent(@"0;2.615; 0;3;1; 0;3;5
            //1;2.092; 0;2;1.27; 0;4;4.73
            //2;1.57; 0;1.27;2; 0;4.73;4");
            configurAgent(@"1;-1.57; 0;2.5;4; 0;2.5;1
2; -1.57; 0; 2; 4; 0; 2; 1
3; -1.57; 0; 3; 4; 0; 3; 1");



            //CAPO Circle
            //configurAgent(@"0;2.615; 0;3;1; 0;3;5
            //1;2.092; 0;2;1.27; 0;4;4.73
            //2;1.57; 0;1.27;2; 0;4.73;4
            //3;1.046; 0;1;3; 0;5;3
            //4;0.523; 0;1.27;4; 0;4.73;2
            //5;0; 0;2;4.73; 0;4.01;1.27
            //6;-0.525; 0;3;5; 0;3;1
            //7;-1.048; 0;4;4.73; 0;2;1.27
            //8;-1.57; 0;4.73;4; 0;1.27;2
            //9;-2.094; 0;5;3; 0;1;3
            //10;-2.617; 0;4.73;2; 0;1.26;3.99
            //11;-3.14; 0;4.01;1.27; 0;1.99;4.73");

            //CAPO
            //            configurAgent(@"1;-1.57; 0;2.5;4; 0;2.5;1
            //2;1.57; 0;2.5;1; 0;2.5;4");

            //0; -2.36; 0; 265; 265; 0; 135; 135
            //1; -0.79; 0; 155; 265; 0; 285; 135
            //1;-0.79; 0;155;265; 0;285;135
            //2;0.79; 0;265;155; 0;135;285
            //3;0.79; 0;155;155; 0;285;285
            //4;-2.36; 0;265;275; 0;135;145
            //5;-0.79; 0;155;275; 0;285;145
            //6;0.79; 0;265;165; 0;135;295
            //7;0.79; 0;155;165; 0;285;295
            //8;-2.36; 0;265;285; 0;135;155
            //9;-0.79; 0;155;285; 0;285;155
            //10;0.79; 0;265;175; 0;135;305
            //11;0.79; 0;155;175; 0;285;305
            //12;-2.36; 0;265;295; 0;135;165
            //13;-0.79; 0;155;295; 0;285;165
            //14;0.79; 0;265;185; 0;135;315
            //15;0.79; 0;155;185; 0;285;315
            //16;-2.36; 0;275;265; 0;145;135
            //17;-0.79; 0;165;265; 0;295;135
            //18;0.79; 0;275;155; 0;145;285
            //19;0.79; 0;165;155; 0;295;285
            //20;-2.36; 0;275;275; 0;145;145
            //21;-0.79; 0;165;275; 0;295;145
            //22;0.79; 0;275;165; 0;145;295
            //23;0.79; 0;165;165; 0;295;295
            //24;-2.36; 0;275;285; 0;145;155
            //25;-0.79; 0;165;285; 0;295;155
            //26;0.79; 0;275;175; 0;145;305
            //27;0.79; 0;165;175; 0;295;305
            //28;-2.36; 0;275;295; 0;145;165
            //29;-0.79; 0;165;295; 0;295;165
            //30;0.79; 0;275;185; 0;145;315
            //31;0.79; 0;165;185; 0;295;315
            //32;-2.36; 0;285;265; 0;155;135
            //33;-0.79; 0;175;265; 0;305;135
            //34;0.79; 0;285;155; 0;155;285
            //35;0.79; 0;175;155; 0;305;285
            //36;-2.36; 0;285;275; 0;155;145
            //37;-0.79; 0;175;275; 0;305;145
            //38;0.79; 0;285;165; 0;155;295
            //39;0.79; 0;175;165; 0;305;295
            //40;-2.36; 0;285;285; 0;155;155
            //41;-0.79; 0;175;285; 0;305;155
            //42;0.79; 0;285;175; 0;155;305
            //43;0.79; 0;175;175; 0;305;305
            //44;-2.36; 0;285;295; 0;155;165
            //45;-0.79; 0;175;295; 0;305;165
            //46;0.79; 0;285;185; 0;155;315
            //47;0.79; 0;175;185; 0;305;315
            //48;-2.36; 0;295;265; 0;165;135
            //49;-0.79; 0;185;265; 0;315;135
            //50;0.79; 0;295;155; 0;165;285
            //51;0.79; 0;185;155; 0;315;285
            //52;-2.36; 0;295;275; 0;165;145
            //53;-0.79; 0;185;275; 0;315;145
            //54;0.79; 0;295;165; 0;165;295
            //55;0.79; 0;185;165; 0;315;295
            //56;-2.36; 0;295;285; 0;165;155
            //57;-0.79; 0;185;285; 0;315;155
            //58;0.79; 0;295;175; 0;165;305
            //59;0.79; 0;185;175; 0;315;305
            //60;-2.36; 0;295;295; 0;165;165
            //61;-0.79; 0;185;295; 0;315;165
            //62;0.79; 0;295;185; 0;165;315
            //63;0.79; 0;185;185; 0;315;315");

            initThred();

            foreach (var item in thr)
                item.Start();
        }

        private void initRobot()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    robotStartPos.Add(new Vector2(55.0f + i * 10.0f, 55.0f + j * 10.0f));
                    robotEndPos.Add(new Vector2(-75.0f + i * 10.0f, -75.0f + j  * 10.0f));


                    robotStartPos.Add(new Vector2(-55.0f - i * 10.0f, 55.0f + j * 10.0f));
                    robotEndPos.Add(new Vector2(75.0f + i * 10.0f, -75.0f + j * 10.0f));
                    //createRobot(idRobota, new Vector2(-55.0f - i * 10.0f, 55.0f + j * 10.0f), new Vector2(75.0f, -75.0f));
                    //idRobota++;

                    robotStartPos.Add(new Vector2(55.0f + i * 10.0f, -55.0f - j * 10.0f));
                    robotEndPos.Add(new Vector2(-75.0f + i * 10.0f, 75.0f + j * 10.0f));
                    //createRobot(idRobota, new Vector2(55.0f + i * 10.0f, -55.0f - j * 10.0f),new Vector2(-75.0f, 75.0f));
                    //idRobota++;

                    robotStartPos.Add(new Vector2(-55.0f - i * 10.0f, -55.0f - j * 10.0f));
                    robotEndPos.Add(new Vector2(75.0f + i * 10.0f, 75.0f + j * 10.0f));
                }
            }
        }

        private void initThred()
        {
            int RobotID = 9;

            for (int i = 0; i < robotStartPos.Count; i++)
            {
                SimRVO3 sim = new SimRVO3(robotEndPos[i], RobotID, obst);

                rvo.Add(sim);

                State state = new State();
                state.location = robotStartPos[i];
                state.velocity = new Vector2(0, 0);
                state.robotId = RobotID;

                AllRobotStates.TryAdd(RobotID, state);
                RobotID++;

                thr.Add(new Thread(() => run(sim)));
            }
            working = true;
        }

        private void killAll()
        {
            licznik = 0;
            working = false;

            foreach (var item in thr)
                item.Abort();


            AllRobotStates = new ConcurrentDictionary<int, State>();
            rvo = new List<SimRVO3>();
            thr = new List<Thread>();

            robotStartPos = new List<Vector2>();
            robotEndPos = new List<Vector2>();

            obst = GetObstacles();
        }

        private void run(SimRVO3 sim)
        {
            int robotID = sim.RobotID;

            while (working)
            {
                State currentState = AllRobotStates[robotID];
                List<State> AllState = new List<State>();

                foreach (var item in AllRobotStates.Values)
                    AllState.Add(item);

                Vector2 currentVelocity = sim.compute(AllState, currentState.location);

                Console.WriteLine(string.Format("ID: {0} X: {1} Y: {2}", robotID, currentVelocity.x(), currentVelocity.y()));

                currentState.location += currentVelocity * sim.timeStep;
                currentState.velocity = currentVelocity;

                if(sim.IsCollide())
                {
                    int i = 4343;
                }

                 System.Threading.Thread.Sleep(50);
               // System.Threading.Thread.Sleep(500);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            double dlugosc;

            if (AllRobotStates != null)
            {
                foreach (var item in AllRobotStates)
                {
                    //Block
                    //e.Graphics.DrawEllipse(pen, 200 + 2 * item.Value.location.x(), 200 + 2 * item.Value.location.y(), 5.0f, 5.0f);

                    //CAPO
                    e.Graphics.DrawEllipse(pen, 200 + 30 * item.Value.location.x(), 200 + 30 * item.Value.location.y(), 5.0f, 5.0f);

                    //dlugosc = Math.Sqrt(Math.Pow(item.Value.velocity.x(), 2) + Math.Pow(item.Value.velocity.y(), 2));
                    ////e.Graphics.DrawString(dlugosc.ToString("0.0000"), drawFont, drawBrush, 200 + 30 * item.Value.location.x(), 200 + 30 * item.Value.location.y());
                    //e.Graphics.DrawString(string.Format("X: {0} Y: {1}", item.Value.velocity.x().ToString("0.0000"), item.Value.velocity.y().ToString("0.0000")), drawFont, drawBrush, 200 + 30 * item.Value.location.x(), 200 + 30 * item.Value.location.y());
                    //if (Math.Abs(item.Value.velocity.y()).ToString("0.0000") != "0,2000")
                    //    e.Graphics.DrawString(licznik.ToString(), drawFont, drawBrush, 100, 100);
                    
                    //Circle
                    //e.Graphics.DrawEllipse(pen, 50 + 2 * item.Value.location.x(), 50 + 2 * item.Value.location.y(), 5.0f, 5.0f);


                }

                //if (item.Value.velocity.x() != 0.0)
                //{

                //    e.Graphics.DrawString(licznik.ToString(), drawFont, drawBrush, 100, 100);
                    licznik++;
               // }

                drawObstacle(e);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                checkDistanceRobot();
                Refresh();
            });
        }

        public static List<IList<Vector2>> GetObstacles()
        {
            ////////////////  Blocks ////////////
            List<IList<Vector2>> obstacles = new List<IList<Vector2>>();

            //test pionwej przeszkody
            //IList<Vector2> obstacle11 = createObstacle(200, 10, 200, 250);
            //obstacles.Add(obstacle11);

            //test poziomej przeszkody
            //IList<Vector2> obstacle12 = createObstacle(10, 200, 250, 200);
            //obstacles.Add(obstacle12);

            //IList<Vector2> obstacle1 = new List<Vector2>();
            //obstacle1.Add(new Vector2(-10.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 10.0f));
            //obstacle1.Add(new Vector2(-10.0f, 10.0f));
            //obstacles.Add(obstacle1);

            //IList<Vector2> obstacle2 = new List<Vector2>();
            //obstacle2.Add(new Vector2(10.0f, 40.0f));
            //obstacle2.Add(new Vector2(10.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 40.0f));
            //obstacles.Add(obstacle2);

            //IList<Vector2> obstacle3 = new List<Vector2>();
            //obstacle3.Add(new Vector2(10.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -10.0f));
            //obstacle3.Add(new Vector2(10.0f, -10.0f));
            //obstacles.Add(obstacle3);

            //IList<Vector2> obstacle4 = new List<Vector2>();
            //obstacle4.Add(new Vector2(-10.0f, -40.0f));
            //obstacle4.Add(new Vector2(-10.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -40.0f));
            //obstacles.Add(obstacle4);



            return obstacles;

            //////////////// CAPO ////////////
            //List<IList<Vector2>> obstacles = new List<IList<Vector2>>();

            //return obstacles;
        }

        private void drawObstacle(PaintEventArgs e)
        {
            List<IList<Vector2>> obst = GetObstacles();

            foreach (var item in obst)
                printObsctcle(item, e);
        }

        private void printObsctcle(IList<Vector2> obstacle, PaintEventArgs e)
        {
            int stala = 200;
            int zmienna = 2;

            float x0 = stala + zmienna * obstacle[0].x();
            float y0 = stala + zmienna * obstacle[0].y();
            float x1;
            float y1;

            for (int i = 1; i < obstacle.Count; i++)
            {
                x1 = stala + zmienna * obstacle[i].x();
                y1 = stala + zmienna * obstacle[i].y();

                e.Graphics.DrawLine(pen, x0, y0, x1, y1);

                x0 = x1;
                y0 = y1;
            }

            x1 = stala + zmienna * obstacle[0].x();
            y1 = stala + zmienna * obstacle[0].y();

            e.Graphics.DrawLine(pen, x0, y0, x1, y1);
        }

        public void checkDistanceRobot()
        {
            double tmpDistance;
            double minDistance = 0.3; //wielkosc robota

            foreach (var i in AllRobotStates.Values)
            {
                foreach (var j in AllRobotStates.Values)
                {
                    if (i.robotId != j.robotId)
                    {
                        tmpDistance = getDistance(i.location,j.location);

                        //System.Console.WriteLine(tmpDistance);

                        if (tmpDistance <= minDistance)
                        {
                            int jj = 9999;
                        }
                    }
                }
            }
        }

        private double getDistance(Vector2 v1, Vector2 v2)
        {
            return Math.Sqrt(Math.Pow(v2.x() - v1.x(), 2) + Math.Pow(v2.y() - v1.y(), 2));
        }

        private void configurAgent(string sConfig)
        {
            string[] temp = sConfig.Split('\n');

            foreach (var item in temp)
            {
                string[] tmp = item.Split(';');

                float xStart = float.Parse(tmp[3].Replace(".", ","));
                float yStart = float.Parse(tmp[4].Replace(".", ","));

                float xEnd = float.Parse(tmp[6].Replace(".", ","));
                float yEnd = float.Parse(tmp[7].Replace(".", ",").Replace("\n", ""));

                robotStartPos.Add(new Vector2(xStart, yStart));
                robotEndPos.Add(new Vector2(xEnd, yEnd));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Vector2> list = createObstacle(0,0,8,0);
        }

        private static List<Vector2> createObstacle(float x_begin, float y_begin, float x_end, float y_end)
        {
            float x_max = Math.Max(x_begin, x_end);
            float y_max = Math.Max(y_begin, y_end);

            float x_min = Math.Min(x_begin, x_end);
            float y_min = Math.Min(y_begin, y_end);

            //return addObstacle(new List<Vector2>() { new Vector2(x_max, y_min), new Vector2(x_max, y_max), new Vector2(x_min, y_max), new Vector2(x_min, y_min) });

            return new List<Vector2>() { new Vector2(x_min, y_max), new Vector2(x_min, y_min), new Vector2(x_max, y_min), new Vector2(x_max, y_max) };
        }
    }
}
