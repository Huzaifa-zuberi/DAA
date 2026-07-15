namespace MSTVisualizer
{
    public class Edge
    {
        public Node Source { get; set; }
        public Node Target { get; set; }
        public int Weight { get; set; }

        public Edge(Node source, Node target, int weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }
    }
}
