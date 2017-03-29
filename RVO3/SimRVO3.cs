using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVO3
{
    public class SimRVO3
    {
        /////////////////  Blocks ////////////////////////
        //private float neighborDist = 15.0f;
        //private int maxNeighbors = 10;
        //private float timeHorizon = 5.0f; //2.0 //2.5
        //private float timeHorizonObst = 5.0f;
        //private float radius = 2.0f;
        //private float maxSpeed = 2.0f; //0.25 //0.2
        //private Vector2 velocity = new Vector2(0.0f, 0.0f);
        //public float timeStep = 0.25f;

        ////////////////// CAPO  /////////////////////////
        //private float neighborDist = 1.2f; //1.2f; //minimalna odleglosc kiedy robot zacznie reagowac na innego robota "zobaczy go"
        //private int maxNeighbors = 1000;
        //private float timeHorizon = 3.0f; //3.0f; //2.0 //2.5
        //private float timeHorizonObst = 0.0f;
        //private float radius = 0.3f;
        //private float maxSpeed = 0.2f; //0.25 //0.2
        //private Vector2 velocity = new Vector2(0.0f, 0.0f);
        //public float timeStep = 0.2f;

        //////////////////// CAPO 1 /////////////////////////
        //private float neighborDist = 1.0f;
        //private int maxNeighbors = 1000;
        //private float timeHorizon = 2.5f; //2.0 //2.5
        //private float timeHorizonObst = 0.0f;
        //private float radius = 0.3f;
        //private float maxSpeed = 0.2f; //0.25 //0.2
        //private Vector2 velocity = new Vector2(0.0f, 0.0f);
        //public float timeStep = 0.2f;

        //////////////////// Circle CAPO  ////////////////////
        private float neighborDist = 5.0f; //
        private int maxNeighbors = 10; //
        private float timeHorizon = 1.5f;
        private float timeHorizonObst = 1.5f;
        private float radius = 0.3f;
        private float maxSpeed = 0.25f;
        private Vector2 velocity = new Vector2(0.0f, 0.0f);
        public float timeStep = 0.2f;

        //////////////////// Circle  ////////////////////
        //private float neighborDist = 15.0f;
        //private int maxNeighbors = 10;
        //private float timeHorizon = 10.0f; 
        //private float timeHorizonObst = 10.0f;
        //private float radius = 1.5f;
        //private float maxSpeed = 2.0f; 
        //private Vector2 velocity = new Vector2(0.0f, 0.0f);
        //public float timeStep = 0.25f;

        internal IList<Agent> agents_;
        internal IList<Obstacle> obstacles_;
        internal KdTree kdTree_;

        internal Agent CurrentAgent;
        internal Vector2 GoalAgent;

        private Random random = new Random();

        public int RobotID;

        public SimRVO3(Vector2 goalAgent, int robotID, List<IList<Vector2>> obst)
        {
            GoalAgent = goalAgent;
            CurrentAgent = createAgent(robotID);
            RobotID = robotID;
            obstacles_ = new List<Obstacle>();

            foreach (var item in obst)
                addObstacle(item);           
        }

        public Vector2 compute(IList<State> sate, Vector2 currentPosition)
        {
            CurrentAgent.position_ = currentPosition;

           // CurrentAgent.velocity_ = new Vector2(0, 0); //bug bug bug gdy jest zerowane powoduje problem roboty powinny uwzgledniac velocity z poptrzedniego wyliczenia

            IList<Agent> agents = getAgents(sate);
            agents.Add(CurrentAgent);

            agents_ = agents;
            kdTree_ = new KdTree();
            kdTree_.buildAgentTree(agents_);
            kdTree_.buildObstacleTree(obstacles_);

            setAgentPrefVelocity();

            CurrentAgent.computeNeighbors(kdTree_); //Simulator.Instance.agents_[agentNo].computeNeighbors();
            CurrentAgent.computeNewVelocity(); //Simulator.Instance.agents_[agentNo].computeNewVelocity();

            CurrentAgent.update();

            return CurrentAgent.velocity_;
        }

        public bool IsCollide()
        {
            return CurrentAgent.IsCollide();
           // return CurrentAgent.IsCollide() && (CurrentAgent.velocity_.x() != CurrentAgent.prefVelocity_.x()) && (CurrentAgent.velocity_.y() != CurrentAgent.prefVelocity_.y());
        }

        private void setAgentPrefVelocity()
        {
            Vector2 goalVector = GoalAgent - CurrentAgent.position_; // Simulator.Instance.getAgentPosition(i);

            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }

            CurrentAgent.prefVelocity_ = goalVector; // Simulator.Instance.setAgentPrefVelocity(i, goalVector);

            ///* Perturb a little to avoid deadlocks due to perfect symmetry. */
            //float angle = (float)random.NextDouble() * 2.0f * (float)Math.PI;
            //float dist = (float)random.NextDouble() * 0.0001f;

            //CurrentAgent.prefVelocity_ = CurrentAgent.prefVelocity_ + dist * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        private Agent createAgent(int id)
        {
            Agent agent = new Agent();
            agent.id_ = id;

            agent.position_ = new Vector2(0, 0);

            agent.maxNeighbors_ = maxNeighbors;
            agent.maxSpeed_ = maxSpeed;
            agent.neighborDist_ = neighborDist;
            agent.radius_ = radius;
            agent.timeHorizon_ = timeHorizon;
            agent.timeHorizonObst_ = timeHorizonObst;
            agent.velocity_ = velocity;

            agent.timeStep_ = timeStep;

            return agent;
        } //

        private IList<Agent> getAgents(IList<State> sate)//
        {
            List<Agent> temp = new List<Agent>();//

            foreach (var item in sate)
            {
                if (item.robotId == RobotID)
                    continue;
                else
                temp.Add(createAgent(item));//
            }

            return temp;//
        }

        private Agent createAgent(State state)//
        {
            Agent temp = createAgent(state.robotId);//

            temp.position_ = state.location;//
            temp.velocity_ = state.velocity;//

            return temp;//
        }



        /**
         * <summary>Adds a new obstacle to the simulation.</summary>
         *
         * <returns>The number of the first vertex of the obstacle, or -1 when
         * the number of vertices is less than two.</returns>
         *
         * <param name="vertices">List of the vertices of the polygonal obstacle
         * in counterclockwise order.</param>
         *
         * <remarks>To add a "negative" obstacle, e.g. a bounding polygon around
         * the environment, the vertices should be listed in clockwise order.
         * </remarks>
         */
        public int addObstacle(IList<Vector2> vertices)
        {
            if (vertices.Count < 2)
            {
                return -1;
            }

            int obstacleNo = obstacles_.Count;

            for (int i = 0; i < vertices.Count; ++i)
            {
                Obstacle obstacle = new Obstacle();
                obstacle.point_ = vertices[i];

                if (i != 0)
                {
                    obstacle.previous_ = obstacles_[obstacles_.Count - 1];
                    obstacle.previous_.next_ = obstacle;
                }

                if (i == vertices.Count - 1)
                {
                    obstacle.next_ = obstacles_[obstacleNo];
                    obstacle.next_.previous_ = obstacle;
                }

                obstacle.direction_ = RVOMath.normalize(vertices[(i == vertices.Count - 1 ? 0 : i + 1)] - vertices[i]);

                if (vertices.Count == 2)
                {
                    obstacle.convex_ = true;
                }
                else
                {
                    obstacle.convex_ = (RVOMath.leftOf(vertices[(i == 0 ? vertices.Count - 1 : i - 1)], vertices[i], vertices[(i == vertices.Count - 1 ? 0 : i + 1)]) >= 0.0f);
                }

                obstacle.id_ = obstacles_.Count;
                obstacles_.Add(obstacle);
            }

            return obstacleNo;
        }

        private int createObstacle(float x_begin,float y_begin,float x_end, float y_end)
        {
            float x_max = Math.Max(x_begin, x_end);
            float y_max = Math.Max(y_begin, y_end);

            float x_min = Math.Min(x_begin, x_end);
            float y_min = Math.Min(y_begin, y_end);

            //return addObstacle(new List<Vector2>() { new Vector2(x_max, y_min), new Vector2(x_max, y_max), new Vector2(x_min, y_max), new Vector2(x_min, y_min) });

            return addObstacle(new List<Vector2>() { new Vector2(x_min, y_max), new Vector2(x_min, y_min), new Vector2(x_max, y_min), new Vector2(x_max, y_max) });
        }

    }
}
