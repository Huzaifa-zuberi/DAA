namespace KruskalMST
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Label { get; set; }

        public Node(int x, int y, string label)
        {
            X = x;
            Y = y;
            Label = label;
        }
    }
}
