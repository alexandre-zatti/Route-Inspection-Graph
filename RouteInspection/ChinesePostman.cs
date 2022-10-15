namespace RouteInspection;

public class Edge
{
   private int _id;
   private int _from;
   private int _to;
   private int _weight;
   
   public Edge(int id, int from, int to, int weight)
   {
      this._id = id;
      this._to = to;
      this._from = from;
      this._weight = weight;
   }

   public int GetId()
   {
      return _id;
   }
   
   public int GetSource()
   {
      return _from;
   }
   
   public int GetDestination()
   {
      return _to;
   }
   
   public int GetWeigth()
   {
      return _weight;
   }
}

public class Node
{
   private int _id;
   private List<Edge> _edges;

   public Node(int id, List<Edge> edges)
   {
      this._id = id;
      this._edges = edges;
   }

   public int GetId()
   {
      return _id;
   }

   public List<Edge> GetRelations()
   {
      return _edges;
   }

   public void addRelation(Edge relation)
   {
      _edges.Add(relation);
   }
}

public class ChinesePostman
{
   private List<Node> _graph;
   public ChinesePostman(List<Node> graph)
   {
      _graph = graph;
   }

   public string Execute()
   {
      if (HasEulirianCycle())
      {
         var simpleTour = HierHolzer(
            _graph.First(),
            new List<Edge>(),
            new Queue<Node>()
         );

         return printPath(simpleTour);
      }

      var oddVertices = GetOddVertices();

      var oddPairs = CreateOddPairs(oddVertices);

      var groupPairs = GroupPairs(oddPairs);

      var shortestPathGroup = FindShortestPaths(groupPairs);

      AddEdgesToGraph(shortestPathGroup.Item1);

      var tour = HierHolzer(
         _graph.First(),
         new List<Edge>(),
         new Queue<Node>()
      );
      
      return printPath(tour); 
   }
   
   private bool HasEulirianCycle()
   {
      if (!IsGraphConnected())
      {
         return false;
      }

      foreach (var node in _graph)
      {
         if (node.GetRelations().Count % 2 != 0)
         {
            return false;
         }    
      }
      
      return true;
   }

   private bool IsGraphConnected()
   {
      var nonZeroDegreeNodes = _graph.FindAll((node) => node.GetRelations().Count > 0);
      
      var visited = Dfs(nonZeroDegreeNodes.First(), new List<Node>());

      return nonZeroDegreeNodes.All(visited.Contains) && visited.All(nonZeroDegreeNodes.Contains);
   }
   
   private List<Node> Dfs(Node at, List<Node> visited)
   {
      visited.Add(at);

      foreach (var edge in at.GetRelations())
      {
         var node = _graph.Find((_ => _.GetId() == edge.GetDestination()));

         if (node != null && !visited.Contains(node))
         {
            Dfs(node, visited);           
         }
      }

      return visited;
   }
   
   private Queue<Node> HierHolzer(Node startNode, List<Edge> visited, Queue<Node> path)
   {
      foreach (var edge in startNode.GetRelations())
      {
         if (!visited.Contains(edge))
         {
            visited.Add(edge);

            var node = _graph.Find((_ => _.GetId() == edge.GetDestination()));
            
            visited.Add(node.GetRelations().Find(_ => _.GetId() == edge.GetId()));
            
            HierHolzer(node, visited, path); 
         }
      }
      
      path.Enqueue(startNode);      
      
      return path;
   }

   private string printPath(Queue<Node> tour)
   {
      string tourPath = "";
         
      foreach (var node in tour)
      {
         tourPath +=  node.GetId() + " -> ";
      }
      
      var tourCost = CalculateTourCost(tour);
      
      return tourPath + " Valor total do percurso = " + tourCost; 
   }
   
   private int CalculateTourCost(Queue<Node> tour)
   {
      var totalTourCost = 0;
      
      while (tour.Count != 1)
      {
         var currentNode = tour.Dequeue();
         var nextNode = tour.Peek();

         var edge = currentNode.GetRelations().Find(_ => _.GetDestination() == nextNode.GetId());

         if (edge != null)
         {
             totalTourCost += edge.GetWeigth();           
         }
      }

      return totalTourCost;
   }
   
   private List<Node> GetOddVertices()
   {
      return _graph.FindAll(_ => _.GetRelations().Count % 2 != 0);
   }

   private List<Tuple<Node, Node>> CreateOddPairs(List<Node> oddVertices)
   {
      var pairs = new List<Tuple<Node, Node>>();

      foreach (var i in oddVertices)
      {
         foreach (var j in oddVertices)
         {
            if (i.GetId() != j.GetId() && !pairs.Contains(new Tuple<Node, Node>(j, i)))
            {
               pairs.Add(new Tuple<Node, Node>(i, j));
            }
         }
      }

      return pairs;
   }
   
   private List<List<Tuple<Node, Node>>> GroupPairs(List<Tuple<Node, Node>> oddPairs)
   {
      var groupPairs = new List<List<Tuple<Node, Node>>>();

      if (oddPairs.Count == 1)
      {
         groupPairs.Add(oddPairs);

         return groupPairs;
      }
      
      foreach (var i in oddPairs)
      {
         if (CheckIfAlreadyInGroup(groupPairs, i))
         {
            continue;
         }
         
         var auxGroup = new List<Tuple<Node, Node>>();

         foreach (var j in oddPairs)
         {
            if((i.Item1 != j.Item1 && i.Item1 != j.Item2) && (i.Item2 != j.Item1 && i.Item2 != j.Item2)) 
            {
               auxGroup.Add(j);
            }
         }
         
         groupPairs.AddRange(createGroups(auxGroup, i));
      }
      
      return groupPairs;
   }
   
   private List<List<Tuple<Node, Node>>> createGroups(List<Tuple<Node, Node>> auxGroup, Tuple<Node, Node> pair)
   {
      var groups = new List<List<Tuple<Node, Node>>>();
      
      foreach (var i in auxGroup)
      {
         if (CheckIfAlreadyInGroup(groups, i))
         {
            continue;
         }
         
         var group = new List<Tuple<Node, Node>>(){i};

         foreach (var j in auxGroup)
         {
            if (CheckIfCanEnterGroup(group, j))
            {
               group.Add(j);
            }
         }

         group.Insert(0, pair);
         groups.Add(group);
      }

      return groups;
   }
   
   private bool CheckIfCanEnterGroup(List<Tuple<Node, Node>> group, Tuple<Node, Node> pair)
   {
      var canEnter = true;
      
      foreach (var member in group)
      {
         if ((member.Item1 == pair.Item1 || member.Item1 == pair.Item2) ||
              member.Item2 == pair.Item1 || member.Item2 == pair.Item2)
         {
            canEnter = false;
         }
      }

      return canEnter;
   }
   
   private bool CheckIfAlreadyInGroup(List<List<Tuple<Node, Node>>> groupPairs, Tuple<Node, Node> pair)
   {
      var isInGroup = false;
      
      foreach (var group in groupPairs)
      {
         if (group.Contains(pair))
         {
            isInGroup = true;
         }
      }
      
      return isInGroup;
   }

   private Tuple<List<Tuple<int, List<int>, Node, Node>>, int> FindShortestPaths(List<List<Tuple<Node, Node>>> groupPairs)
   {
      var groupsInfo = new List<Tuple<List<Tuple<int, List<int>, Node, Node>>, int>>();
      
      foreach (var groupsList in groupPairs)
      {
         var auxList = new List<Tuple<int, List<int>, Node, Node>>();
         var auxTotalPathCost = 0;
         
         foreach (var group in groupsList)
         {
            var aux = Dijkstra(group);
            auxTotalPathCost += aux.Item1;
            auxList.Add(new Tuple<int, List<int>, Node, Node>(aux.Item1, aux.Item2, group.Item1, group.Item2));
         }
         
         groupsInfo.Add(new Tuple<List<Tuple<int, List<int>, Node, Node>>, int>(auxList, auxTotalPathCost));
      }

      return GetGroupWithMinimumCost(groupsInfo);
   }

   private Tuple<List<Tuple<int, List<int>, Node, Node>>, int> GetGroupWithMinimumCost(List<Tuple<List<Tuple<int, List<int>, Node, Node>>, int>> groupInfo)
   {
      var minimumCost = 0;
      var minimumCostGroup = new Tuple<List<Tuple<int, List<int>, Node, Node>>, int>(new List<Tuple<int, List<int>, Node, Node>>(), 0);
      
      foreach (var group in groupInfo)
      {
         if (minimumCost == 0 || group.Item2 < minimumCost)
         {
            minimumCost = group.Item2;
            minimumCostGroup = group;
         }
      }

      return minimumCostGroup;
   }
   
   private Tuple<int, List<int>> Dijkstra(Tuple<Node, Node> nodes)
   {
      const int infinity = 9999;

      var dist = Enumerable.Repeat(infinity, _graph.Count + 1).ToArray();
      dist[nodes.Item1.GetId()] = 0;

      var previous = Enumerable.Repeat(infinity, _graph.Count + 1).ToArray();

      var visited = new List<Node>();
      
      var pq = new PriorityQueue<Node, int>();
      pq.Enqueue(nodes.Item1, 0);
      
      while (pq.Count != 0)
      {
         var node = pq.Dequeue();
         visited.Add(node);

         foreach (var edge in node.GetRelations())
         {
            var neighbourNode = _graph.Find(_ => _.GetId() == edge.GetDestination());
            
            if (visited.Contains(neighbourNode))
            {
               continue;
            }

            var newDist = dist[node.GetId()] + edge.GetWeigth();
            
            if (newDist < dist[neighbourNode.GetId()])
            {
               dist[neighbourNode.GetId()] = newDist;
               previous[neighbourNode.GetId()] = node.GetId();
               pq.Enqueue(neighbourNode, newDist);
            }
         }
      }

      var path = GeneratePath(previous,
                                     nodes.Item2.GetId(),
                                     nodes.Item1.GetId(),
                                     new List<int>());
      path.Add(nodes.Item1.GetId());
      path.Reverse();
      
      return new Tuple<int, List<int>>(dist[nodes.Item2.GetId()], path);
   }

   private List<int> GeneratePath(int[] previous, int start, int destination, List<int> path)
   {
      path.Add(start);
      
      if (previous[start] != destination)
      {
         GeneratePath(previous, previous[start], destination, path);
      }

      return path;
   }

   private void AddEdgesToGraph(List<Tuple<int, List<int>, Node, Node>> shortestPathGroup)
   {
      var rnd = new Random();
      
      foreach (var group in shortestPathGroup)
      {
         foreach (var pathNode in group.Item2)
         {
            if (group.Item2.IndexOf(pathNode) + 1 < group.Item2.Count)
            {
               var nextPathNode = group.Item2.ElementAt(group.Item2.IndexOf(pathNode) + 1);

               var graphNode = _graph.Find(_ => _.GetId() == pathNode);
               var nextGraphNode = _graph.Find(_ => _.GetId() == nextPathNode);

               var graphNodeRelation = graphNode.GetRelations()
                  .Find(_ => _.GetSource() == pathNode && _.GetDestination() == nextPathNode);
               
               var edgeId = rnd.Next(1000000, 100000000);
               graphNode.addRelation(new Edge(edgeId,pathNode, nextPathNode, graphNodeRelation.GetWeigth()));                
               nextGraphNode.addRelation(new Edge(edgeId,nextPathNode, pathNode, graphNodeRelation.GetWeigth()));
            }
         } 
      }
   }
}





