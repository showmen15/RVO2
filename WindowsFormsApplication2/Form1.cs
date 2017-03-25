using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RVO2;
using System.Collections.Concurrent;
using System.Threading;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        private int timeStamp = 10;

        private ConcurrentDictionary<int, State> AllRobotStates;

        private ConcurrentDictionary<int, CollisionFreeVelocityGenerator> robots;

        private List<Thread> robotThr;

        Thread monitorStates;
        bool working;

        Pen pen = new Pen(Color.Green, 1);

        List<Vector2> goals = new List<Vector2>(); // { new Vector2(5, 1), new Vector2(1, 1) };

        Random random = new Random();


        private void monitorRun()
        {
            while(true)
            {
                foreach (var robot in robots.Values)
                {
                    foreach (var state in AllRobotStates.Values)
                        robot.handle(state);
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    Refresh();
                });

                System.Threading.Thread.Sleep(timeStamp);
            }
        }

        private void addObstacle(CollisionFreeVelocityGenerator col)
        {
            List<IList<Vector2>> obst = GetObstacles();

            foreach (var item in obst)
                col.addObstacle(item);
        }

        private void runRobot(int id)
        {
            CollisionFreeVelocityGenerator col = robots[id];
            double distance = double.MaxValue;

            addObstacle(col);

            while (distance > 0.01)
            {
                State currentRobotState = AllRobotStates[id];
                Vector2 currentVelocity;
                Vector2 gole = goals[id];

                currentVelocity = getPreferredVelocities(id, currentRobotState.location);

                currentVelocity = col.GetVelocityCollisionFree(currentRobotState.location, currentVelocity);

                currentRobotState.location += currentVelocity * col.timeStep;
                currentRobotState.velocity = currentVelocity;

                System.Threading.Thread.Sleep(timeStamp);

                // System.Console.WriteLine("R: {0} X: {1} Y: {2}",id, currentRobotState.location.x(), currentRobotState.location.y());

              //  System.Console.WriteLine("R: {0} X: {1} Y: {2}", id, currentVelocity.x(), currentVelocity.y());

                distance = getDistance(gole, currentRobotState.location);
            }
        }

        private double getDistance(Vector2 v1, Vector2 v2)
        {
            return Math.Sqrt(Math.Pow(v2.x() - v1.x(), 2) + Math.Pow(v2.y() - v1.y(), 2));
        }

        private void createRobot(int id, Vector2 location, Vector2 gole)
        {
            Thread thr;
            CollisionFreeVelocityGenerator col;
            int robotId;
            State state;

            robotId = id;
            state = new State();
            state.location = location;
            state.velocity = new Vector2(0,0);
            state.robotId = id;

            AllRobotStates.TryAdd(robotId, state);
            col = new CollisionFreeVelocityGenerator(robotId);
            thr = new Thread(() => runRobot(robotId));
            robots.TryAdd(robotId, col);
            robotThr.Add(thr);

            goals.Add(gole);
        }


        private Vector2 getPreferredVelocities(int id, Vector2 location)
        {
            Vector2 goalVector = goals[id] - location;

            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }

            float angle = (float)random.NextDouble() * 2.0f * (float)Math.PI;
            float dist = (float)random.NextDouble() * 0.0001f;

            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
            goalVector = goalVector + dist * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            return goalVector;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void initStates()
        {
            robots = new ConcurrentDictionary<int, CollisionFreeVelocityGenerator>();
            AllRobotStates = new ConcurrentDictionary<int, State>();
            monitorStates = new Thread(new ThreadStart(monitorRun));
            robotThr = new List<Thread>();
            working = true;

            int idRobota = 0;


            /*for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 5; ++j)
                {

                    createRobot(idRobota, new Vector2(55.0f + i * 10.0f, 55.0f + j * 10.0f), new Vector2(-75.0f, -75.0f));

                    idRobota++;

                    //Simulator.Instance.addAgent();
                    //goals.Add(new Vector2(-75.0f, -75.0f));

                    createRobot(idRobota, new Vector2(-55.0f - i * 10.0f, 55.0f + j * 10.0f),new Vector2(75.0f, -75.0f));
                    idRobota++;


                    createRobot(idRobota, new Vector2(55.0f + i * 10.0f, -55.0f - j * 10.0f),new Vector2(-75.0f, 75.0f));
                    idRobota++;

                    createRobot(idRobota, new Vector2(-55.0f - i * 10.0f, -55.0f - j * 10.0f),new Vector2(75.0f, 75.0f));
                    idRobota++;
                }
            }*/


            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    createRobot(idRobota, new Vector2(55.0f + i * 10.0f, 55.0f + j * 10.0f),new Vector2(-75.0f, -75.0f));
                    idRobota++;

                    //createRobot(idRobota, new Vector2(-55.0f - i * 10.0f, 55.0f + j * 10.0f), new Vector2(75.0f, -75.0f));
                    //idRobota++;

                    //createRobot(idRobota, new Vector2(55.0f + i * 10.0f, -55.0f - j * 10.0f),new Vector2(-75.0f, 75.0f));
                    //idRobota++;

                    createRobot(idRobota, new Vector2(-55.0f - i * 10.0f, -55.0f - j * 10.0f), new Vector2(75.0f, 75.0f));
                    idRobota++;
                }
            }



            //  createRobot(0, new Vector2(55.0f + 1 * 10.0f, 55.0f + 1 * 10.0f), new Vector2(-75.0f, -75.0f));

            // createRobot(0, new Vector2(41.41f, 41.41f), new Vector2(-75.0f, -75.0f));

            //createRobot(0, new Vector2(48f, 48f), new Vector2(-75.0f, -75.0f));

            //   createRobot(0, new Vector2(1, 5), new Vector2(5, 5));
            //   createRobot(1, new Vector2(5, 1), new Vector2(1, 1));

            //            configurAgent(@"1;-1.57; 0;2.5;4; 0;2.5;1
            //2;1.57; 0;2.5;1; 0;2.5;4");

            //configurAgent(@"1;-1.57; 0;2.25;4; 0;2.25;1
            //2;-1.57; 0;2.75;4; 0;2.75;1
            //3;-1.57; 0;1.75;4; 0;1.75;1
            //4;-1.57; 0;3.25;4; 0;3.25;1
            //5;-1.57; 0;1.25;4; 0;1.25;1
            //6;-1.57; 0;3.75;4; 0;3.75;1
            //7;-1.57; 0;0.75;4; 0;0.75;1
            //8;-1.57; 0;4.25;4; 0;4.25;1
            //9;-1.57; 0;0.25;4; 0;0.25;1
            //10;-1.57; 0;4.75;4; 0;4.75;1
            //11;1.57; 0;2.25;1; 0;2.25;4
            //12;1.57; 0;2.75;1; 0;2.75;4
            //13;1.57; 0;1.75;1; 0;1.75;4
            //14;1.57; 0;3.25;1; 0;3.25;4
            //15;1.57; 0;1.25;1; 0;1.25;4
            //16;1.57; 0;3.75;1; 0;3.75;4
            //17;1.57; 0;0.75;1; 0;0.75;4
            //18;1.57; 0;4.25;1; 0;4.25;4
            //19;1.57; 0;0.25;1; 0;0.25;4
            //20;1.57; 0;4.75;1; 0;4.75;4");

            foreach (var th in robotThr)
                th.Start();

            monitorStates.Start();
        }

        private void configurAgent(string sConfig)
        {
            string[] temp = sConfig.Split('\n');

            foreach (var item in temp)
            {
                string[] tmp = item.Split(';');

                int robotID = int.Parse(tmp[0]) - 1;

                float xStart = float.Parse(tmp[3].Replace(".", ","));
                float yStart = float.Parse(tmp[4].Replace(".", ","));

                float xEnd = float.Parse(tmp[6].Replace(".", ","));
                float yEnd = float.Parse(tmp[7].Replace(".", ",").Replace("\n", ""));

                createRobot(robotID, new Vector2(xStart, yStart), new Vector2(xEnd, yEnd));
                //Simulator.Instance.addAgent(new Vector2(xStart, yStart));
                //goals.Add(new Vector2(xEnd, yEnd));
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            initStates();


         //   CollisionFreeVelocityGenerator gen = new CollisionFreeVelocityGenerator(0);

            //  gen.GetVelocityCollisionFree(new Vector2(0, 0), new Vector2(1, 2));

            //gen.handle()

            //gen.GetVelocityCollisionFree()

            /*   Agent defaultAgent = setAgentDefaults(15.0f, 10, 5.0f, 5.0f, 2.0f, 2.0f, new Vector2(0.0f, 0.0f));

               My nowy = new My(defaultAgent);

               nowy.addAgent(new Vector2(5, 5));
               nowy.addAgent(new Vector2(1, 5));

               for(int i = 0; i < 2;i++)
               {
                   Vector2 agentPos = nowy.getAgentPosition(i);
                   Vector2 goal = GetGoalVector(i, agentPos);

                   nowy.setAgentPrefVelocity(i, goal);

                   nowy.step(i);


               }*/

        }

        public Vector2 GetGoalVector(int agentId,Vector2 agentPos)
        {
            Vector2 goalVector = goals[agentId] - agentPos;

            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }

            return goalVector;
        }

        public Agent setAgentDefaults(float neighborDist, int maxNeighbors, float timeHorizon, float timeHorizonObst, float radius, float maxSpeed, Vector2 velocity)
        {
            Agent defaultAgent_ = new Agent();


            defaultAgent_.maxNeighbors_ = maxNeighbors;
            defaultAgent_.maxSpeed_ = maxSpeed;
            defaultAgent_.neighborDist_ = neighborDist;
            defaultAgent_.radius_ = radius;
            defaultAgent_.timeHorizon_ = timeHorizon;
            defaultAgent_.timeHorizonObst_ = timeHorizonObst;
            defaultAgent_.velocity_ = velocity;

            return defaultAgent_;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (AllRobotStates != null)
            {

                foreach (var item in AllRobotStates)
                {
                    e.Graphics.DrawEllipse(pen, 200 + 2 * item.Value.location.x(), 200 + 2 * item.Value.location.y(), 5.0f, 5.0f);
                    //e.Graphics.DrawEllipse(pen, 200 + item.x(), 200 + item.y(), 5.0f, 5.0f);
                }

                drawObstacle(e);
            }
        }

        private void drawObstacle(PaintEventArgs e)
        {
            int stala = 200;
            int zmienna = 3;

            List<IList<Vector2>> obst = GetObstacles();

            foreach (var item in obst)
                printObsctcle(item, e);

        }

        private void printObsctcle(IList<Vector2> obstacle, PaintEventArgs e)
        {
            int stala = 200;
            int zmienna = 2;


            /*   float x0 = stala + zmienna * obstacle[0].x();
               float y0 = stala + zmienna * obstacle[0].y();
               float x1 = stala + zmienna * obstacle[1].x();
               float y1 = stala + zmienna * obstacle[1].y();
               float x2 = stala + zmienna * obstacle[2].x();
               float y2 = stala + zmienna * obstacle[2].y();
               float x3 = stala + zmienna * obstacle[3].x();
               float y3 = stala + zmienna * obstacle[3].y();

               e.Graphics.DrawLine(pen, x0, y0, x1, y1);

               e.Graphics.DrawLine(pen, x1, y1, x2, y2);
               e.Graphics.DrawLine(pen, x2, y2, x3, y3);
               e.Graphics.DrawLine(pen, x3, y3, x0, y0);*/

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

        public static List<IList<Vector2>> GetObstacles()
        {
            List<IList<Vector2>> obstacles = new List<IList<Vector2>>();

            //IList<Vector2> obstacle1 = new List<Vector2>();

            //obstacle1.Add(new Vector2(-10.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 10.0f));
            //obstacle1.Add(new Vector2(-10.0f, 10.0f));

            //printObsctcle(obstacle1, e);

            //IList<Vector2> obstacle2 = new List<Vector2>();
            //obstacle2.Add(new Vector2(10.0f, 40.0f));
            //obstacle2.Add(new Vector2(10.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 40.0f));

            //printObsctcle(obstacle2, e);

            //IList<Vector2> obstacle3 = new List<Vector2>();
            //obstacle3.Add(new Vector2(10.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -10.0f));
            //obstacle3.Add(new Vector2(10.0f, -10.0f));

            //printObsctcle(obstacle3, e);

            //IList<Vector2> obstacle4 = new List<Vector2>();
            //obstacle4.Add(new Vector2(-10.0f, -40.0f));
            //obstacle4.Add(new Vector2(-10.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -40.0f));

            //printObsctcle(obstacle4, e);



            //IList<Vector2> obstacle0 = new List<Vector2>();

            //float startX = -70f;
            //float startY = -70f;
            //float endX = -70f;
            //float endY = 70f;

            //obstacle0.Add(new Vector2(startX, startY));
            //obstacle0.Add(new Vector2(endX, startY));
            //obstacle0.Add(new Vector2(endX, endY));
            //obstacle0.Add(new Vector2(startX, endY));
            //printObsctcle(obstacle0, e);

            //IList<Vector2> obstacle1 = new List<Vector2>();

            //startX = 70f;
            //startY = -70f;
            //endX = -70f;
            //endY = -70f;

            //obstacle1.Add(new Vector2(startX, startY));
            //obstacle1.Add(new Vector2(endX, startY));
            //obstacle1.Add(new Vector2(endX, endY));
            //obstacle1.Add(new Vector2(startX, endY));
            //printObsctcle(obstacle1, e);


            //IList<Vector2> obstacle1 = new List<Vector2>();

            //obstacle1.Add(new Vector2(20f, -20f));
            //obstacle1.Add(new Vector2(-20f, -20f));
            //obstacle1.Add(new Vector2(-20f, 0f));
            //obstacle1.Add(new Vector2(-30f, 0f));
            //obstacle1.Add(new Vector2(-30f, 20f));
            //obstacle1.Add(new Vector2(30f, 20f));
            //obstacle1.Add(new Vector2(30f, 0f));
            //obstacle1.Add(new Vector2(20f, 0f));
            //printObsctcle(obstacle1, e);

            //IList<Vector2> obstacle1 = new List<Vector2>();

            //obstacle1.Add(new Vector2(-10.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 10.0f));
            //obstacle1.Add(new Vector2(-10.0f, 10.0f));

            //printObsctcle(obstacle1, e);


            //IList<Vector2> obstacle2 = new List<Vector2>();
            //obstacle2.Add(new Vector2(10.0f, 40.0f));
            //obstacle2.Add(new Vector2(10.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 40.0f));

            //printObsctcle(obstacle2, e);

            //IList<Vector2> obstacle3 = new List<Vector2>();
            //obstacle3.Add(new Vector2(10.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -10.0f));
            //obstacle3.Add(new Vector2(10.0f, -10.0f));

            //printObsctcle(obstacle3, e);

            //IList<Vector2> obstacle4 = new List<Vector2>();
            //obstacle4.Add(new Vector2(-10.0f, -40.0f));
            //obstacle4.Add(new Vector2(-10.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -40.0f));

            //printObsctcle(obstacle4, e);

            //  IList<Vector2> obstacle1 = new List<Vector2>();

            //obstacle1.Add(new Vector2(-10.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 40.0f));
            //obstacle1.Add(new Vector2(-40.0f, 10.0f));
            //obstacle1.Add(new Vector2(-10.0f, 10.0f));

            //col.addObstacle(obstacle1);

            //IList<Vector2> obstacle2 = new List<Vector2>();
            //obstacle2.Add(new Vector2(10.0f, 40.0f));
            //obstacle2.Add(new Vector2(10.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 40.0f));

            //col.addObstacle(obstacle2);

            //IList<Vector2> obstacle3 = new List<Vector2>();
            //obstacle3.Add(new Vector2(10.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -40.0f));
            //obstacle3.Add(new Vector2(40.0f, -10.0f));
            //obstacle3.Add(new Vector2(10.0f, -10.0f));

            //col.addObstacle(obstacle3);

            //IList<Vector2> obstacle4 = new List<Vector2>();
            //obstacle4.Add(new Vector2(-10.0f, -40.0f));
            //obstacle4.Add(new Vector2(-10.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -10.0f));
            //obstacle4.Add(new Vector2(-40.0f, -40.0f));

            //IList<Vector2> obstacle2 = new List<Vector2>();
            //obstacle2.Add(new Vector2(10.0f, 40.0f));
            //obstacle2.Add(new Vector2(10.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 10.0f));
            //obstacle2.Add(new Vector2(40.0f, 40.0f));

            //col.addObstacle(obstacle2);




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
        }

    }
}
