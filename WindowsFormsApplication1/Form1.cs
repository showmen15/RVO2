using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RVO;

using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Thread thr;
        Thread thr2;

        Pen pen = new Pen(Color.Green, 1);

        int x = 10;
        int y = 30;

        List<Vector2> listaAgentwo = new List<Vector2>();


        public Form1()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            
           // listaAgentwo.Add(new Vector2(120, 210));
           // listaAgentwo.Add(new Vector2(20, 20));
           // listaAgentwo.Add(new Vector2(44, 333));

            if (thr != null)
                thr.Abort();

            thr = new Thread(new ThreadStart(run));
            thr.Start();

            x += 10;
            y += 10;
            //Refresh();
        }

        private void run()
        {
            Blocks circle = new Blocks();

            /* Set up the scenario. */
            circle.setupScenario();

            int numberRobots = circle.getNumAgents();

            listaAgentwo.Clear();

            for (int i = 0; i < numberRobots; i++)
            {
                listaAgentwo.Add(new Vector2(0, 0));
            }


            while (!circle.reachedGoal())
            {
                Monitor.Enter(listaAgentwo);
                
                for (int i = 0; i < circle.getNumAgents(); i++)
                {
                    Vector2 vec = circle.getAgentPosition(i);
                    Vector2 valo = circle.getAgentVelocity(i);

                   // if(i == 1)
                   //     System.Console.Write(string.Format("{0};{1}&", valo.x(), valo.y()));

                    listaAgentwo[i] = vec;
                }

                Monitor.Exit(listaAgentwo);

                this.Invoke((MethodInvoker)delegate()
                  {
                      Refresh();
                  });
                Thread.Sleep(100);

                circle.checkDistanceRobot();
                circle.setPreferredVelocities();
                Simulator.Instance.doStep();

                //circle.RunTest();

            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Monitor.Enter(listaAgentwo);
            
            e.Graphics.Clear(this.BackColor);

            if(listaAgentwo.Count > 0)
            {

           // Console.WriteLine(String.Format("Robot 0: X : {0} Y: {1}",listaAgentwo[0].x(),listaAgentwo[0].y()));
            //Console.WriteLine(String.Format("Robot 1: X : {0} Y: {1}", listaAgentwo[1].x(), listaAgentwo[1].y()));
            }

            foreach (var item in listaAgentwo)
            {
                // e.Graphics.DrawEllipse(pen, 100 + 20 * item.x(), 100 + 20 * item.y(), 5.0f, 5.0f);
                // e.Graphics.DrawEllipse(pen, 200 + item.x(), 200 + item.y(), 5.0f, 5.0f);

                e.Graphics.DrawEllipse(pen, 200 + 2 *  item.x(), 200 + 2 * item.y(), 5.0f, 5.0f);
            }

            Monitor.Exit(listaAgentwo);

            drawObstacle(e);
        }


        private void drawObstacle(PaintEventArgs e)
        {
            int stala = 200;
            int zmienna = 3;

            List<IList<Vector2>> obst = Blocks.GetObstacles();

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

        private void button2_Click(object sender, EventArgs e)
        {

            if (thr2 != null)
                thr2.Abort();

            thr2 = new Thread(new ThreadStart(run2));
            thr2.Start();

        }

        private void run2()
        {
         

        }

    }
}
