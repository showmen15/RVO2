using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RVO2
{
    public class CollisionFreeVelocityGenerator
    {
        //////////   Stale ////// wypelnic warotsciami z algorytmu 
        //private float neighborDist = 1.0f;
        //private int maxNeighbors = 1000;
        //private float timeHorizon = 2.0f; //2.0 //2.5
        //private float timeHorizonObst = 5.0f;
        //private float radius = 0.3f;
        //private float maxSpeed = 0.5f; //0.25 //0.2
        //private Vector2 velocity = new Vector2(0.0f, 0.0f);
        //public float timeStep = 0.2f;

        //////////////////////////////////////////////////////////
        private float neighborDist = 15.0f;
        private int maxNeighbors = 10;
        private float timeHorizon = 5.0f; //2.0 //2.5
        private float timeHorizonObst = 5.0f;
        private float radius = 2.0f;
        private float maxSpeed = 2.0f; //0.25 //0.2
        private Vector2 velocity = new Vector2(0.0f, 0.0f);
        public float timeStep = 0.2f;

        private KdTree kdTree_;

        private List<Obstacle> walls = new List<Obstacle>(); //lista wszystkich Obstacles

        private List<Obstacle> obstacles = new List<Obstacle>(); //zgrupowane Obstacles

        //public void AddObstacle(Obstacle obstacle)
        //{

        //}



        // private final Map<Integer, State> states = new ConcurrentHashMap<>();
        private ConcurrentDictionary<int, State> states = new ConcurrentDictionary<int, State>();

        public void handle(State state)
        {
            //   states.AddOrUpdate(state.robotId, state, (key, oldValue) => state);

            if (state.robotId == robotId)
            {
                return;
            }
            else
            {
                states.AddOrUpdate(state.robotId, state, (key, oldValue) => state);
            }
        }

        private int robotId; //
        private Agent currentAgent; //

        //  private final WallCollisionDetector wallCollisionDetector;
        //private final CollisionFreeVelocityType collisionFreeVelocityType;

        public CollisionFreeVelocityGenerator(/*CollisionFreeVelocityType collisionFreeVelocityType,*/ int robotId/*, WallCollisionDetector wallCollisionDetector*/)
        {
            this.robotId = robotId; //
           //
            
        }

        public Vector2 GetVelocityCollisionFree(Vector2 currentlocation, Vector2 currentVelocity)//
        {
            IList<Agent> tmpAgents = getAgents();//
            currentAgent = createAgent(robotId);

            if (tmpAgents.Count > 0)
            {
                float rangeSq = RVOMath.sqr(timeHorizonObst * maxSpeed + radius);
                IList<Obstacle> tmpObstacle = getObstacles(currentlocation, rangeSq);

                currentAgent.position_ = currentlocation; //
                currentAgent.prefVelocity_ = currentVelocity; //

                kdTree_ = new KdTree();
                kdTree_.buildAgentTree(tmpAgents);

                currentAgent.computeNeighbors(kdTree_, tmpObstacle);//
                currentAgent.computeNewVelocity();//

                return currentAgent.newVelocity_; //
            }
            else
            {
                return new Vector2(0, 0);
            }

        }

        private IList<Agent> getAgents()//
        {
            List<Agent> temp = new List<Agent>();//

            foreach (var state in states.Values)//
                temp.Add(createAgent(state));//

            return temp;//
        }

        private Agent createAgent(State state)//
        {
            Agent temp = createAgent(state.robotId);//

            temp.position_ = state.location;//
            temp.velocity_ = state.velocity;//

            return temp;//
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

            agent.TimeStep = timeStep;

            return agent;
        } //

        public bool IsCurrentVelocityCollisionFree(Vector2 location, Vector2 currentVelocity)//
        {
            return false;//
        }

        public int addObstacle(IList<Vector2> vertices)
        {
            if (vertices.Count < 2)
            {
                return -1;
            }

            int obstacleNo = walls.Count;

            for (int i = 0; i < vertices.Count; ++i)
            {
                Obstacle obstacle = new Obstacle();
                obstacle.point_ = vertices[i];

                if (i != 0)
                {
                    obstacle.previous_ = walls[walls.Count - 1];
                    obstacle.previous_.next_ = obstacle;
                }

                if (i == vertices.Count - 1)
                {
                    obstacle.next_ = walls[obstacleNo];
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

                obstacle.id_ = walls.Count;
                walls.Add(obstacle);

                if (i == 0)
                    obstacles.Add(obstacle);
            }

            return obstacleNo;
        }

        private IList<Obstacle> getObstacles(Vector2 currentlocation, float rangeSq)
        {
            List<Obstacle> temp = new List<Obstacle>();

            for (int i = 0; i < walls.Count; i++)

            {
                Obstacle obstacle1 = walls[i];
                Obstacle obstacle2 = obstacle1.next_;

                float agentLeftOfLine = RVOMath.leftOf(obstacle1.point_, obstacle2.point_, currentlocation);


                float distSqLine = RVOMath.sqr(agentLeftOfLine) / RVOMath.absSq(obstacle2.point_ - obstacle1.point_);

                if (distSqLine < rangeSq)
                {
                    if (agentLeftOfLine < 0.0f)
                    {
                        temp.Add(walls[i]);

                    }
                }
            }

            return temp;
        }
    }
}
