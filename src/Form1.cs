using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MSTVisualizer
{
    public partial class Form1 : Form
    {
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private List<Edge> mstEdges = new List<Edge>();

        private Node selectedNode;
        private Node edgeStartNode = null;
        private bool isAddingEdge = false;
        private bool isMSTMode = false;
        private Point mousePos;

        private Panel canvas;
        private Panel topBar;
        private Button btnAddNode;
        private Button btnAddEdge;
        private Button btnRunKruskal;
        private Button btnClear;
        private Button btnReset;
        private Label lblStatus;
        private Label lblMSTCost;
        private ListBox lstEdges;
        private Label lblEdgeList;

        private int nodeCounter = 0;
        private readonly Color CANVAS_BG = Color.FromArgb(18, 18, 30);
        private readonly Color NODE_COLOR = Color.FromArgb(120, 80, 200);
        private readonly Color NODE_SELECTED = Color.FromArgb(0, 210, 200);
        private readonly Color EDGE_COLOR = Color.FromArgb(100, 100, 130);
        private readonly Color MST_EDGE_COLOR = Color.FromArgb(0, 230, 120);
        private readonly Color NODE_BORDER = Color.FromArgb(160, 120, 240);

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "MST Visualizer — Kruskal's Algorithm";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = CANVAS_BG;
            this.StartPosition = FormStartPosition.CenterScreen;

            topBar = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(26, 26, 45),
            };

            var lblTitle = new Label
            {
                Text = "MST Visualizer",
                ForeColor = Color.FromArgb(160, 120, 255),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = false,
                Width = 200,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(15, 0),
                BackColor = Color.Transparent,
            };

            btnAddNode = CreateToolbarButton("Add Node", 230, NODE_COLOR);
            btnAddEdge = CreateToolbarButton("Add Edge", 320, Color.FromArgb(100, 150, 240));
            btnRunKruskal = CreateToolbarButton("Run Kruskal", 410, Color.FromArgb(0, 180, 100));
            btnReset = CreateToolbarButton("Reset MST", 510, Color.FromArgb(200, 160, 60));
            btnClear = CreateToolbarButton("Clear All", 600, Color.FromArgb(200, 60, 60));

            btnAddNode.Click += BtnAddNode_Click;
            btnAddEdge.Click += BtnAddEdge_Click;
            btnRunKruskal.Click += BtnRunKruskal_Click;
            btnReset.Click += BtnReset_Click;
            btnClear.Click += BtnClear_Click;

            var sepLine = new Panel
            {
                Width = 1, Height = 40,
                BackColor = Color.FromArgb(60, 60, 90),
                Location = new Point(700, 10),
            };

            lblStatus = new Label
            {
                Text = "Ready",
                ForeColor = Color.FromArgb(140, 140, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                AutoSize = false,
                Width = 180, Height = 25,
                Location = new Point(715, 8),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            lblMSTCost = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(0, 230, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Width = 180, Height = 25,
                Location = new Point(715, 30),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            topBar.Controls.AddRange(new Control[] {
                lblTitle, btnAddNode, btnAddEdge, btnRunKruskal, btnReset, btnClear,
                sepLine, lblStatus, lblMSTCost
            });

            var rightPanel = new Panel
            {
                Width = 220,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(26, 26, 45),
            };

            lblEdgeList = new Label
            {
                Text = "Edge List",
                ForeColor = Color.FromArgb(160, 140, 220),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Width = 200, Height = 25,
                Location = new Point(10, 15),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            lstEdges = new ListBox
            {
                Location = new Point(10, 45),
                Width = 200,
                Height = 660,
                BackColor = Color.FromArgb(30, 30, 50),
                ForeColor = Color.FromArgb(180, 210, 180),
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None,
            };

            rightPanel.Controls.AddRange(new Control[] { lblEdgeList, lstEdges });

            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CANVAS_BG,
                Cursor = Cursors.Cross,
            };
            canvas.Paint += Canvas_Paint;
            canvas.MouseClick += Canvas_MouseClick;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseDown += Canvas_MouseDown;

            this.Controls.Add(canvas);
            this.Controls.Add(rightPanel);
            this.Controls.Add(topBar);
        }

        private Button CreateToolbarButton(string text, int x, Color accent)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Width = 80,
                Height = 36,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 40, 65),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 1, BorderColor = accent },
            };
            btn.Paint += (s, e) =>
            {
                var b = s as Button;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(accent, 1))
                    g.DrawRectangle(pen, 1, 1, b.Width - 3, b.Height - 3);
            };
            return btn;
        }

        private void BtnAddNode_Click(object sender, EventArgs e)
        {
            isAddingEdge = false; edgeStartNode = null; selectedNode = null;
            canvas.Cursor = Cursors.Cross;
            lblStatus.Text = "Mode: Add Node — click canvas";
            canvas.Invalidate();
        }

        private void BtnAddEdge_Click(object sender, EventArgs e)
        {
            isAddingEdge = true; edgeStartNode = null; selectedNode = null;
            canvas.Cursor = Cursors.Hand;
            lblStatus.Text = "Mode: Add Edge — click source node";
            canvas.Invalidate();
        }

        private void BtnRunKruskal_Click(object sender, EventArgs e)
        {
            if (nodes.Count < 2)
            { MessageBox.Show("Add at least 2 nodes.", "No Nodes", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (edges.Count == 0)
            { MessageBox.Show("Add at least one edge.", "No Edges", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            mstEdges = KruskalAlgorithm();
            isMSTMode = true;
            int totalCost = 0;
            foreach (var e2 in mstEdges) totalCost += e2.Weight;
            lblMSTCost.Text = $"MST Cost: {totalCost}  |  Edges: {mstEdges.Count}";
            lblStatus.Text = "MST computed — green edges highlight the tree";
            UpdateEdgeList();
            canvas.Invalidate();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            isMSTMode = false; mstEdges.Clear();
            lblMSTCost.Text = ""; lblStatus.Text = "Ready";
            UpdateEdgeList(); canvas.Invalidate();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all nodes and edges?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                nodes.Clear(); edges.Clear(); mstEdges.Clear(); nodeCounter = 0;
                isMSTMode = false; isAddingEdge = false; edgeStartNode = null; selectedNode = null;
                lblMSTCost.Text = ""; lblStatus.Text = "Ready";
                UpdateEdgeList(); canvas.Invalidate();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        { mousePos = e.Location; if (isAddingEdge && edgeStartNode != null) canvas.Invalidate(); }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = HitTestNode(e.Location);
                if (hit != null)
                {
                    edges.RemoveAll(ed => ed.Source == hit || ed.Target == hit);
                    nodes.Remove(hit);
                    if (isMSTMode) { isMSTMode = false; mstEdges.Clear(); lblMSTCost.Text = ""; }
                    UpdateEdgeList(); canvas.Invalidate();
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var hit = HitTestNode(e.Location);

            if (!isAddingEdge)
            {
                if (hit == null)
                {
                    string name = ((char)('A' + (nodeCounter % 26))).ToString();
                    if (nodeCounter >= 26) name += (nodeCounter / 26).ToString();
                    nodes.Add(new Node(e.X, e.Y, name));
                    nodeCounter++;
                    if (isMSTMode) { isMSTMode = false; mstEdges.Clear(); lblMSTCost.Text = ""; }
                    UpdateEdgeList(); canvas.Invalidate();
                }
            }
            else
            {
                if (hit == null) return;
                if (edgeStartNode == null)
                {
                    edgeStartNode = hit;
                    lblStatus.Text = $"Edge from {hit.Label} → click target";
                    canvas.Invalidate();
                }
                else
                {
                    if (edgeStartNode == hit)
                    { edgeStartNode = null; lblStatus.Text = "Mode: Add Edge — click source"; canvas.Invalidate(); return; }

                    bool exists = edges.Exists(ed =>
                        (ed.Source == edgeStartNode && ed.Target == hit) ||
                        (ed.Source == hit && ed.Target == edgeStartNode));
                    if (exists)
                    {
                        MessageBox.Show("Edge already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        edgeStartNode = null; lblStatus.Text = "Mode: Add Edge — click source";
                        canvas.Invalidate(); return;
                    }

                    using (var dlg = new EdgeWeightDialog(edgeStartNode.Label, hit.Label))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            edges.Add(new Edge(edgeStartNode, hit, dlg.Weight));
                            if (isMSTMode) { isMSTMode = false; mstEdges.Clear(); lblMSTCost.Text = ""; }
                            UpdateEdgeList();
                        }
                    }
                    edgeStartNode = null;
                    lblStatus.Text = "Mode: Add Edge — click source";
                    canvas.Invalidate();
                }
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawGrid(g);
            foreach (var edge in edges)
            {
                bool inMST = isMSTMode && mstEdges.Contains(edge);
                DrawEdge(g, edge, inMST);
            }
            if (isAddingEdge && edgeStartNode != null)
            {
                using (var pen = new Pen(NODE_SELECTED, 2) { DashStyle = DashStyle.Dash })
                    g.DrawLine(pen, edgeStartNode.X, edgeStartNode.Y, mousePos.X, mousePos.Y);
            }
            foreach (var node in nodes)
                DrawNode(g, node);
        }

        private void DrawGrid(Graphics g)
        {
            using (var pen = new Pen(Color.FromArgb(25, 25, 42), 1))
            {
                for (int x = 0; x < canvas.Width; x += 40)
                    g.DrawLine(pen, x, 0, x, canvas.Height);
                for (int y = 0; y < canvas.Height; y += 40)
                    g.DrawLine(pen, 0, y, canvas.Width, y);
            }
        }

        private void DrawEdge(Graphics g, Edge edge, bool isInMST)
        {
            Color lineColor = isInMST ? MST_EDGE_COLOR : EDGE_COLOR;
            float lineWidth = isInMST ? 3.5f : 1.8f;
            var src = new Point(edge.Source.X, edge.Source.Y);
            var tgt = new Point(edge.Target.X, edge.Target.Y);

            using (var pen = new Pen(lineColor, lineWidth))
            {
                pen.CustomEndCap = new AdjustableArrowCap(5, 5);
                var (s, t) = ShortenLine(src, tgt, 22);
                g.DrawLine(pen, s, t);
            }

            float mx = (src.X + tgt.X) / 2f;
            float my = (src.Y + tgt.Y) / 2f;
            string wLabel = edge.Weight.ToString();
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                var sz = g.MeasureString(wLabel, font);
                var rect = new RectangleF(mx - sz.Width / 2 - 4, my - sz.Height / 2 - 2, sz.Width + 8, sz.Height + 4);
                using (var bgBrush = new SolidBrush(isInMST ? Color.FromArgb(200, 0, 40, 20) : Color.FromArgb(200, 20, 20, 40)))
                    g.FillRectangle(bgBrush, rect);
                using (var border = new Pen(isInMST ? MST_EDGE_COLOR : Color.FromArgb(60, 60, 90), 1))
                    g.DrawRectangle(border, rect.X, rect.Y, rect.Width, rect.Height);
                using (var br = new SolidBrush(isInMST ? Color.FromArgb(0, 230, 120) : Color.FromArgb(150, 150, 200)))
                    g.DrawString(wLabel, font, br, mx - sz.Width / 2, my - sz.Height / 2);
            }
        }

        private void DrawNode(Graphics g, Node node)
        {
            bool isStart = node == edgeStartNode;
            int r = 22;

            using (var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillEllipse(shadow, node.X - r + 4, node.Y - r + 4, r * 2, r * 2);

            Color fill = isStart ? NODE_SELECTED : NODE_COLOR;
            using (var br = new SolidBrush(fill))
                g.FillEllipse(br, node.X - r, node.Y - r, r * 2, r * 2);

            using (var pen = new Pen(isStart ? Color.FromArgb(0, 230, 220) : NODE_BORDER, 2.5f))
                g.DrawEllipse(pen, node.X - r, node.Y - r, r * 2, r * 2);

            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                var sz = g.MeasureString(node.Label, font);
                using (var br = new SolidBrush(Color.White))
                    g.DrawString(node.Label, font, br, node.X - sz.Width / 2, node.Y - sz.Height / 2);
            }
        }

        private List<Edge> KruskalAlgorithm()
        {
            var sorted = new List<Edge>(edges);
            sorted.Sort((a, b) => a.Weight.CompareTo(b.Weight));

            var parent = new Dictionary<Node, Node>();
            var rank = new Dictionary<Node, int>();
            foreach (var n in nodes) { parent[n] = n; rank[n] = 0; }

            Node Find(Node x)
            {
                if (parent[x] != x) parent[x] = Find(parent[x]);
                return parent[x];
            }

            void Union(Node a, Node b)
            {
                var ra = Find(a); var rb = Find(b);
                if (ra == rb) return;
                if (rank[ra] < rank[rb]) parent[ra] = rb;
                else if (rank[ra] > rank[rb]) parent[rb] = ra;
                else { parent[rb] = ra; rank[ra]++; }
            }

            var mst = new List<Edge>();
            foreach (var edge in sorted)
            {
                if (Find(edge.Source) != Find(edge.Target))
                {
                    mst.Add(edge);
                    Union(edge.Source, edge.Target);
                    if (mst.Count == nodes.Count - 1) break;
                }
            }
            return mst;
        }

        private Node HitTestNode(Point p)
        {
            foreach (var n in nodes)
                if (Math.Sqrt(Math.Pow(p.X - n.X, 2) + Math.Pow(p.Y - n.Y, 2)) <= 22)
                    return n;
            return null;
        }

        private (PointF, PointF) ShortenLine(Point src, Point tgt, float amount)
        {
            float dx = tgt.X - src.X, dy = tgt.Y - src.Y;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 1) return (src, tgt);
            float ux = dx / len, uy = dy / len;
            return (
                new PointF(src.X + ux * amount, src.Y + uy * amount),
                new PointF(tgt.X - ux * amount, tgt.Y - uy * amount)
            );
        }

        private void UpdateEdgeList()
        {
            lstEdges.Items.Clear();
            var list = isMSTMode ? mstEdges : edges;
            foreach (var e in list)
                lstEdges.Items.Add($"{e.Source.Label} → {e.Target.Label}  w={e.Weight}" + (isMSTMode ? " ✓" : ""));
        }
    }
}
