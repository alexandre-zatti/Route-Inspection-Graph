namespace RouteInspection;

public static class Program
{
    public static void Main(string[] args)
    {
        var graph = new List<Node>
        {
            new Node(1, new List<Edge>
            {
                new Edge(1,1, 2, 2),
                new Edge(2,1, 5, 2),
            }),
            new Node(2, new List<Edge>
            {
                new Edge(1,2, 1, 2),
                new Edge(3,2, 3, 3),
                new Edge(4,2, 4, 1),
                new Edge(5,2, 5, 1),
            }),
            new Node(3, new List<Edge>
            {
                new Edge(3,3, 2, 3),
                new Edge(6,3, 4, 1),
                new Edge(7,3, 7, 6),
            }),
            new Node(4, new List<Edge>
            {
                new Edge(4,4, 2, 1),
                new Edge(6,4, 3, 1),
                new Edge(8,4, 5, 5),
                new Edge(9,4, 6, 3),
                new Edge(10,4, 8, 2),
                new Edge(11,4, 9, 3),
            }),
            new Node(5, new List<Edge>
            {
                new Edge(2,5, 1, 2),
                new Edge(5,5, 2, 1),
                new Edge(8,5, 4, 5),
            }),
            new Node(6, new List<Edge>
            {
                new Edge(9,6, 4, 3),
                new Edge(12,6, 9, 5),
                new Edge(13,6, 7, 2),
            }),
            new Node(7, new List<Edge>
            {
                new Edge(7,7, 3, 6),
                new Edge(13,7, 6, 2),
                new Edge(14,7, 8, 2),   
            }),
            new Node(8, new List<Edge>
            {
                new Edge(10,8, 4, 2),
                new Edge(14,8, 7, 2),
                new Edge(15,8, 9, 1),   
            }),
            new Node(9, new List<Edge>
            {
                new Edge(11,9, 4, 3),
                new Edge(12,9, 6, 5),
                new Edge(15,9, 8, 1),               
            })
        };
        
        var chinesePostman = new ChinesePostman(graph);
        
        Console.WriteLine(chinesePostman.Execute());
    }
}
