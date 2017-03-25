/*
 * Circle.cs
 * RVO2 Library C#
 *
 * Copyright 2008 University of North Carolina at Chapel Hill
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Please send all bug reports to <geom@cs.unc.edu>.
 *
 * The authors may be contacted via:
 *
 * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
 * Dept. of Computer Science
 * 201 S. Columbia St.
 * Frederick P. Brooks, Jr. Computer Science Bldg.
 * Chapel Hill, N.C. 27599-3175
 * United States of America
 *
 * <http://gamma.cs.unc.edu/RVO2/>
 */

/*
 * Example file showing a demo with 250 agents initially positioned evenly
 * distributed on a circle attempting to move to the antipodal position on the
 * circle.
 */

#define RVO_OUTPUT_TIME_AND_POSITIONS

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RVO
{
    public class Circle
    {
        /* Store the goals of the agents. */
        IList<Vector2> goals;

        public Circle()
        {
            goals = new List<Vector2>();
        }

        public void setupScenario()
        {
            //orginalne ustawienia Circle
            //Simulator.Instance.setTimeStep(0.25f);
            //Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 1.5f, 2.0f, new Vector2(0.0f, 0.0f));

            //for (int i = 0; i < 5; ++i)
            //{
            //    for (int j = 0; j < 5; ++j)
            //    {
            //        Simulator.Instance.addAgent(new Vector2(55.0f + i * 10.0f, 55.0f + j * 10.0f));
            //        goals.Add(new Vector2(-75.0f, -75.0f));

            //        Simulator.Instance.addAgent(new Vector2(-55.0f - i * 10.0f, 55.0f + j * 10.0f));
            //        goals.Add(new Vector2(75.0f, -75.0f));

            //        Simulator.Instance.addAgent(new Vector2(55.0f + i * 10.0f, -55.0f - j * 10.0f));
            //        goals.Add(new Vector2(-75.0f, 75.0f));

            //        Simulator.Instance.addAgent(new Vector2(-55.0f - i * 10.0f, -55.0f - j * 10.0f));
            //        goals.Add(new Vector2(75.0f, 75.0f));
            //    }
            //}


            /* Specify the global time step of the simulation. */
            //   Simulator.Instance.setTimeStep(0.2f); //zakladam [s]

            /*
             * Specify the default parameters for agents that are subsequently
             * added.
             */
            //  Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 1.5f, 2.0f, new Vector2(0.0f, 0.0f));


            Simulator.Instance.setTimeStep(0.2f); //zakladam [s]

            float timeHorizon = 0.0f; // 2.0 odlegosc do przeszkody //parametr zbêdny

            float neighborDist = 1.0f; //1.0 odleglosc do sasiada //kiedy mam brac pod uwagê s¹siada 

            //wybieranie obiektow do ominiecia  Agentow oraz przeszkod
            //timeHorizonObst * maxSpeed + radius

            float timeHorizonObst = 0f; //
            float maxSpeed = 0.25f; //0.5 predkosc robota
            float radius = 0.3f; //0.3wielkosc robota    


            //Simulator.Instance.setTimeStep(0.2f); //zakladam [s]

            //float neighborDist = 1.0f; 
            //float timeHorizon = 2.5f; //2.0 //2.5
            //float timeHorizonObst = 0.0f;
            //float radius = 0.3f;
            //float maxSpeed = 0.2f; //0.25 //0.2
            //Vector2 velocity = new Vector2(0.0f, 0.0f);


            Simulator.Instance.setAgentDefaults(neighborDist, 1000/* parametr nie znaczacy*/, timeHorizon, timeHorizonObst, radius, maxSpeed, new Vector2(0.0f, 0.0f));

            //            configurAgent(@"1;-1.57; 0;2.5;4; 0;2.5;1
            //2;1.57; 0;2.5;1; 0;2.5;4");


            //            configurAgent(@"1;-1.57; 0;2.25;4; 0;2.25;1
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


            //  Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 1.5f, 2.0f, new Vector2(0.0f, 0.0f));

            /*
             * Add agents, specifying their start position, and store their
             * goals on the opposite side of the environment.
             */

            /* Simulator.Instance.addAgent(new Vector2(1f, 1));
             goals.Add(new Vector2(5f, 1));

             Simulator.Instance.addAgent(new Vector2(5f, 1));
             goals.Add(new Vector2(1f, 1));
             */

            /*
            float robots = 250;
            int sizeRobot = 250;

            for (int i = 0; i < sizeRobot; ++i)
            {
                Simulator.Instance.addAgent(5.0f * new Vector2((float)Math.Cos(i * 2.0f * Math.PI / robots),(float)Math.Sin(i * 2.0f * Math.PI / robots)));

                goals.Add(-Simulator.Instance.getAgentPosition(i));
            }*/

            /*
            Simulator.Instance.setTimeStep(0.25f);
             Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 1.5f, 2.0f, new Vector2(0.0f, 0.0f)); 
            
            for (int i = 0; i < 250; ++i)
            {
                Simulator.Instance.addAgent(200.0f *
                    new Vector2((float)Math.Cos(i * 2.0f * Math.PI / 250.0f),
                        (float)Math.Sin(i * 2.0f * Math.PI / 250.0f)));

                goals.Add(-Simulator.Instance.getAgentPosition(i));
            }*/

        }

        private void configurAgent(string sConfig)
        {
            string[] temp = sConfig.Split('\n');

            foreach (var item in temp)
            {
                string[] tmp = item.Split(';');

                float xStart = float.Parse(tmp[3].Replace(".",","));
                float yStart = float.Parse(tmp[4].Replace(".", ","));

                float xEnd = float.Parse(tmp[6].Replace(".", ","));
                float yEnd = float.Parse(tmp[7].Replace(".", ",").Replace("\n",""));

                Simulator.Instance.addAgent(new Vector2(xStart, yStart));
                goals.Add(new Vector2(xEnd, yEnd));
            }
        }



#if RVO_OUTPUT_TIME_AND_POSITIONS
        public void updateVisualization()
        {
            /* Output the current global time. */
            //Console.Write(Simulator.Instance.getGlobalTime() + ";");

            /* Output the current position of all the agents. */
            for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
            {
                Vector2 vec = Simulator.Instance.getAgentPosition(i);

                Console.WriteLine("R: {0} X: {1};{2};", i, Simulator.Instance.getAgentPosition(i).x(), Simulator.Instance.getAgentPosition(i).y());
            }

            //Console.WriteLine();
        }

        public Vector2 getAgentPosition(int index)
        {
            return Simulator.Instance.getAgentPosition(index);
        }

        public Vector2 getAgentVelocity(int index)
        {
            return Simulator.Instance.getAgentNewVelocity(index);
        }

        public int getNumAgents()
        {
            return Simulator.Instance.getNumAgents();
        }

#endif

        public void setPreferredVelocities()
        {
            /*
             * Set the preferred velocity to be a vector of unit magnitude
             * (speed) in the direction of the goal.
             */
            for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
            {
                Vector2 goalVector = goals[i] - Simulator.Instance.getAgentPosition(i);

                if (RVOMath.absSq(goalVector) > 1.0f)
                {
                    goalVector = RVOMath.normalize(goalVector);
                }

                Simulator.Instance.setAgentPrefVelocity(i, goalVector);
            }
        }


        public bool reachedGoal()
        {
            /* Check if all agents have reached their goals. */
            for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
            {
                if (RVOMath.absSq(Simulator.Instance.getAgentPosition(i) - goals[i]) > Simulator.Instance.getAgentRadius(i) * Simulator.Instance.getAgentRadius(i))
                {
                    return false;
                }
            }

            return true;
        }


        //public bool reachedGoal()
        //{
        //    /* Check if all agents have reached their goals. */
        //    for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
        //    {
        //        //  if (RVOMath.absSq(Simulator.Instance.getAgentPosition(i) - goals[i]) > Simulator.Instance.getAgentRadius(i) * Simulator.Instance.getAgentRadius(i))

        //        double dist = getDistance(Simulator.Instance.getAgentPosition(i), goals[i]);

        //        if (dist < 0.1)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        public static void Main(string[] args)
        {
            Circle circle = new Circle();

            /* Set up the scenario. */
            circle.setupScenario();

            /* Perform (and manipulate) the simulation. */
            do
            {
                circle.checkDistanceRobot();

#if RVO_OUTPUT_TIME_AND_POSITIONS
                circle.updateVisualization();
#endif


                circle.setPreferredVelocities();

                Simulator.Instance.doStep();

               
            }
            while (!circle.reachedGoal());
        }

        private double getDistance(Vector2 v1, Vector2 v2)
        {
            return Math.Sqrt(Math.Pow(v2.x() - v1.x(), 2) + Math.Pow(v2.y() - v1.y(), 2));
        }

        public void checkDistanceRobot()
        {
            double tmpDistance;
            double radius = Simulator.Instance.getAgentRadius(0);

            for (int i = 0; i < Simulator.Instance.getNumAgents(); i++)
            {
                for (int j = i + 1; j < Simulator.Instance.getNumAgents(); j++)
                {
                    tmpDistance = getDistance(Simulator.Instance.getAgentPosition(i), Simulator.Instance.getAgentPosition(j));

                    if(tmpDistance <= radius)
                    {
                        int jj = 9999;
                    }
                }
            }



        }
    }
}
