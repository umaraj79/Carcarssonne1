using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    Graph g;
    int[] tilesList;
    int nbrOfVertices = 85;
    bool[] visited;
    int counter;
    int roadBlocks;
    int finalScore;
    bool broken = false;

    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST,
        CENTER,
        SELF
    }

    void Start()
    {
        g = new Graph(nbrOfVertices);
    }

    public bool testIfMeepleCantBePlaced(int Vindex, TileScript.geography weight)
    {
        broken = false;
        counter = 0;
        visited = new bool[85];
        dfs(Vindex, weight, false);

        return broken;
    }

    public bool testIfMeepleCantBePlacedDirection(int Vindex, TileScript.geography weight, Direction direction)
    {
        broken = false;
        counter = 0;
        visited = new bool[85];
        dfsDirection(Vindex, weight, direction, false);

        //Debug.Log(counter);

        return broken;
    }

    public int startDfs(int Vindex, TileScript.geography weight, bool GameEnd)
    {
        counter = 1;
        roadBlocks = 0;
        finalScore = 0;
        visited = new bool[85];
        dfs(Vindex, weight, GameEnd);
        //Debug.Log(finalScore);

        return finalScore;
    }

    /// <summary>
    /// startDFS takes an index, a weight and a direction to calculate the number of points the finished set is worth. Mainly used by tiles with a town as a centerpiece.
    /// The direction starts the depth first search, but only in the specified direction.
    /// </summary>
    /// <param name="Vindex"></param>
    /// <param name="weight"></param>
    /// <param name="direction"></param>
    public int startDfsDirection(int Vindex, TileScript.geography weight, Direction direction, bool GameEnd)
    {
        counter = 1;
        roadBlocks = 0;
        finalScore = 0;
        visited = new bool[85];
        dfsDirection(Vindex, weight, direction, GameEnd);
        Debug.Log(finalScore);
        //Temporary fix
        if (counter > 2)
        {
            counter--;
        }
        return finalScore;
    }

    private void dfsDirection(int Vindex, TileScript.geography weight, Direction direction, bool GameEnd)
    {
        if (!visited[Vindex])
        {
            counter++;
            //Debug.Log("DFS Direction " + counter);
            visited[Vindex] = true;
            
            LinkedList<Edge> neighbours = g.getNeighbours(Vindex, weight, direction);
            for (int i = 0; i < neighbours.Count; i++)
            {
                LinkedList<Edge> tmp = g.getGraph().ElementAt(neighbours.ElementAt(i).endVertex);
                for(int j = 0; j < tmp.Count; j++)
                {
                    if(tmp.ElementAt(j).endVertex == Vindex)
                    {
                        Debug.Log("Meeple set on " + weight);
                        tmp.ElementAt(j).hasMeeple = true;
                    }
                }
                if (!neighbours.ElementAt(i).hasMeeple)
                {
                    Debug.Log("Meeple set on " + weight);
                    neighbours.ElementAt(i).hasMeeple = true;
                }
                else
                {
                    broken = true;
                }
                if (weight == TileScript.geography.Road)
                {
                    if (neighbours.ElementAt(i).center == TileScript.geography.Village || neighbours.ElementAt(i).center == TileScript.geography.Cloister || neighbours.ElementAt(i).center == TileScript.geography.City)
                    {
                        roadBlocks++;
                        if (roadBlocks == 2)
                        {
                            finalScore = counter;
                            //Debug.Log(finalScore);
                        }
                        //Debug.Log("RoadBlock hit");
                    }
                }
                if (neighbours.ElementAt(i).center == TileScript.geography.Village || neighbours.ElementAt(i).center == TileScript.geography.Grass)
                {
                    counter++;
                }
                else
                {
                    dfs(neighbours.ElementAt(i).endVertex, weight, GameEnd);
                }
            }
        }
        if (GameEnd)
        {
            finalScore = counter;
        }
    }

    public void printEverything()
    {
        for (int i = 0; i < g.getGraph().Count; i++)
        {
            for (int j = 0; j < g.getGraph().ElementAt(i).Count; j++)
            {
                Debug.Log("Vertex: " + i + " " + g.getGraph().ElementAt(i).ElementAt(j).direction);
            }
        }
    }

    public void RemoveVertex(int Vindex)
    {
        if (g.getGraph().ElementAt(Vindex) != null)
        {
            g.getGraph().ElementAt(Vindex).Clear();
        }
        for (int i = 0; i < g.getGraph().Count; i++)
        {
            for (int j = 0; j < g.getGraph().ElementAt(i).Count; j++)
            {
                if (g.getGraph().ElementAt(i).ElementAt(j).endVertex == Vindex)
                {
                    g.getGraph().ElementAt(i).Remove(g.getGraph().ElementAt(i).ElementAt(j));
                }
            }
        }
    }

    private void dfs(int Vindex, TileScript.geography weight, bool GameEnd)
    {
        if (!visited[Vindex])
        {
            if (weight == TileScript.geography.Road)
            {
                counter++;
            }
            if (weight == TileScript.geography.City)
            {
                counter += 2;
                //Debug.Log("Hit Town " + counter);
            }
            visited[Vindex] = true;
            
            LinkedList<Edge> neighbours = g.getNeighbours(Vindex, weight);
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (!neighbours.ElementAt(i).hasMeeple)
                {
                    //Debug.Log("Meeple set on " + weight);
                    neighbours.ElementAt(i).hasMeeple = true;
                }
                else
                {
                    if(weight == TileScript.geography.Road) { 
                        if (!visited[neighbours.ElementAt(i).endVertex]) { 
                            broken = true;
                        }
                    }
                    else
                    {
                        broken = true;
                    }
                }
                if (weight == TileScript.geography.Road)
                {
                    if (neighbours.ElementAt(i).center == TileScript.geography.Village || neighbours.ElementAt(i).center == TileScript.geography.Cloister || neighbours.ElementAt(i).center == TileScript.geography.City)
                    {
                        roadBlocks++;
                        if (roadBlocks == 2)
                        {
                            finalScore = counter;
                            //Debug.Log(finalScore);
                        }
                       //Debug.Log("RoadBlock hit");
                        
                    }
                }
                if (neighbours.ElementAt(i).center == TileScript.geography.Village || neighbours.ElementAt(i).center == TileScript.geography.Grass)
                {
                    counter++;
                }
                else
                {
                    dfs(neighbours.ElementAt(i).endVertex, weight, GameEnd);
                }
            }
        }
        if (GameEnd)
        {
            finalScore = counter;
        }
    }

    public void placeVertex(int Vindex, int[] Vindexes, TileScript.geography[] weights, TileScript.geography startCenter, TileScript.geography[] endCenters, Direction[] directions)
    {
        for (int i = 0; i < Vindexes.Length; i++)
        {
            if (Vindexes[i] != 0)
            {
                g.addEdge(Vindex, Vindexes[i], weights[i], startCenter, endCenters[i], directions[i]);
            }
        }
    }

    public Graph getGraph()
    {
        return g;
    }

    public class Graph
    {
        LinkedList<LinkedList<Edge>> graph;
        public Graph(int nbrOfVertices)
        {
            graph = new LinkedList<LinkedList<Edge>>();
            for (int i = 0; i < nbrOfVertices; i++)
            {
                graph.AddLast(new LinkedList<Edge>());
            }
        }
        private Direction getReverseDirection(Direction direction)
        {
            Direction res;
            switch (direction)
            {
                case Direction.EAST:
                    res = Direction.WEST;
                    break;
                case Direction.WEST:
                    res = Direction.EAST;
                    break;
                case Direction.NORTH:
                    res = Direction.SOUTH;
                    break;
                case Direction.SOUTH:
                    res = Direction.NORTH;
                    break;
                default:
                    res = Direction.NORTH;
                    break;
            }
            return res;
        }
        public void addEdge(int startVertex, int endVertex, TileScript.geography weight, TileScript.geography startCenter, TileScript.geography endCenter, Direction direction)
        {
            graph.ElementAt(startVertex).AddLast(new Edge(endVertex, weight, endCenter, getReverseDirection(direction)));
            graph.ElementAt(endVertex).AddLast(new Edge(startVertex, weight, startCenter, direction));
        }
        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < graph.Count; i++)
            {
                result += i + ": " + "\n";
                foreach (Edge e in graph.ElementAt(i))
                {
                    result += i + " --> " + e + "\n";
                }
            }
            return result;
        }

        
        public LinkedList<Edge> getNeighbours(int Vindex, TileScript.geography weight)
        {
            LinkedList<Edge> neighbours = new LinkedList<Edge>();
            if (weight == TileScript.geography.Road)
            {
                for (int i = 0; i < graph.ElementAt(Vindex).Count; i++)
                {
                    if (graph.ElementAt(Vindex).ElementAt(i).weight == weight)
                    {
                        neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                    }
                }
            }
            if (weight == TileScript.geography.City)
            {
                for (int i = 0; i < graph.ElementAt(Vindex).Count; i++)
                {
                    if (graph.ElementAt(Vindex).ElementAt(i).weight == weight)
                    {
                        neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                    }
                }
            }
            return neighbours;
        }

        public LinkedList<Edge> getNeighbours(int Vindex, TileScript.geography weight, Direction direction)
        {
            //Debug.Log("Checking tiles to the " + direction);
            int counter = 0;
            LinkedList<Edge> neighbours = new LinkedList<Edge>();
            if (weight == TileScript.geography.Road || weight == TileScript.geography.City)
            {
                //Debug.Log("Looking for tiles going " + direction);
                for (int i = 0; i < graph.ElementAt(Vindex).Count; i++)
                {
                    if (graph.ElementAt(Vindex).ElementAt(i).weight == weight && graph.ElementAt(Vindex).ElementAt(i).direction == getReverseDirection(direction))
                    {
                        neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                        counter++;
                    }
                }
            }
            return neighbours;
        }

        public LinkedList<LinkedList<Edge>> getGraph()
        {
            return graph;
        }
    }

    public class Edge
    {
        public int endVertex;
        public TileScript.geography weight;
        public TileScript.geography center;
        public Direction direction;
        public bool hasMeeple;

        public Edge(int endVertex, TileScript.geography weight, TileScript.geography center, Direction direction)
        {
            hasMeeple = false;
            this.endVertex = endVertex;
            this.weight = weight;
            this.center = center;
            this.direction = direction;
        }

        public override string ToString()
        {
            return "(" + endVertex + ")";
        }
    }
}
