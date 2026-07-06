using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KruskalMST
{
    public partial class Form1 : Form
    {
        // Graph data
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();
        private List<Edge> mstEdges = new List<Edge>();

        // Drawing state
        private Node selectedNode = null;
        private Node edgeStartNode = null;
        private bool isAddingEdge = false;
        private bool isMSTMode = false;
        private Point mousePos;

        // UI
        private Panel canvas;
        private Panel controlPanel;
        private Button btnAddNode;
        private Button btnAddEdge;
        private Button btnRunKruskal;
        private Button btnClear;
        private Button btnReset;
        private Label lblStatus;
        private Label lblInstructions;
        private ListBox lstEdges;
        private Label lblEdgeList;
        private Label lblMSTCost;

        private int nodeCounter = 0;
        private readonly Color NODE_COLOR = Color.FromArgb(70, 130, 180);
        private readonly Color NODE_SELECTED = Color.FromArgb(255, 140, 0);
        private readonly Color EDGE_COLOR = Color.FromArgb(80, 80, 80);
        private readonly Color MST_EDGE_COLOR = Color.FromArgb(34, 139, 34);
        private readonly Color NODE_BORDER = Color.FromArgb(30, 90, 140);

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Kruskal's Minimum Spanning Tree - Graph Builder";
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = Color.FromArgb(245, 245, 250);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Control Panel (left sidebar)
            controlPanel = new Panel
            {
                Width = 220,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(40, 44, 52),
                Padding = new Padding(10)
            };

            // Title label
            var lblTitle = new Label
            {
                Text = "Kruskal's MST",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = false,
                Width = 200,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 15)
            };

            var lblSubtitle = new Label
            {
                Text = "Graph Builder & MST Solver",
                ForeColor = Color.FromArgb(150, 160, 180),
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                Width = 200,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 52)
            };

            var separator1 = new Panel
            {
                BackColor = Color.FromArgb(60, 65, 75),
                Height = 1,
                Width = 200,
                Location = new Point(10, 85)
            };

            // Buttons
            btnAddNode = CreateButton("➕  Add Node", 100, Color.FromArgb(70, 130, 180));
            btnAddEdge = CreateButton("🔗  Add Edge", 140, Color.FromArgb(100, 149, 237));
            btnRunKruskal = CreateButton("🌲  Run Kruskal's", 180, Color.FromArgb(34, 139, 34));
            btnReset = CreateButton("↩  Reset View", 220, Color.FromArgb(150, 100, 50));
            btnClear = CreateButton("🗑  Clear All", 260, Color.FromArgb(178, 34, 34));

            btnAddNode.Click += BtnAddNode_Click;
            btnAddEdge.Click += BtnAddEdge_Click;
            btnRunKruskal.Click += BtnRunKruskal_Click;
            btnReset.Click += BtnReset_Click;
            btnClear.Click += BtnClear_Click;

            var separator2 = new Panel
            {
                BackColor = Color.FromArgb(60, 65, 75),
                Height = 1,
                Width = 200,
                Location = new Point(10, 305)
            };

            lblStatus = new Label
            {
                Text = "Mode: Select",
                ForeColor = Color.FromArgb(200, 210, 220),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = false,
                Width = 200,
                Height = 25,
                Location = new Point(10, 315),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblMSTCost = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(100, 220, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Width = 200,
                Height = 30,
                Location = new Point(10, 340),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblInstructions = new Label
            {
                Text = "Instructions:\n• Click canvas to add nodes\n• Select start & end node\n  to add weighted edge\n• Run Kruskal's to find MST\n• Right-click node to delete",
                ForeColor = Color.FromArgb(150, 160, 180),
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                Width = 200,
                Height = 120,
                Location = new Point(10, 378),
                TextAlign = ContentAlignment.TopLeft
            };

            var separator3 = new Panel
            {
                BackColor = Color.FromArgb(60, 65, 75),
                Height = 1,
                Width = 200,
                Location = new Point(10, 505)
            };

            lblEdgeList = new Label
            {
                Text = "Edge List:",
                ForeColor = Color.FromArgb(200, 210, 220),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = false,
                Width = 200,
                Height = 20,
                Location = new Point(10, 515)
            };

            lstEdges = new ListBox
            {
                Location = new Point(10, 538),
                Width = 200,
                Height = 120,
                BackColor = Color.FromArgb(55, 60, 70),
                ForeColor = Color.FromArgb(200, 220, 200),
                Font = new Font("Consolas", 8),
                BorderStyle = BorderStyle.None
            };

            controlPanel.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle, separator1,
                btnAddNode, btnAddEdge, btnRunKruskal, btnReset, btnClear,
                separator2, lblStatus, lblMSTCost, lblInstructions,
                separator3, lblEdgeList, lstEdges
            });

            // Canvas
            canvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 255),
                Cursor = Cursors.Cross
            };
            canvas.Paint += Canvas_Paint;
            canvas.MouseClick += Canvas_MouseClick;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseDown += Canvas_MouseDown;

            this.Controls.Add(canvas);
            this.Controls.Add(controlPanel);
        }

        private Button CreateButton(string text, int y, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(10, y),
                Width = 200,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
            return btn;
        }

        // ─── Button Handlers ───────────────────────────────────────────────────

        private void BtnAddNode_Click(object sender, EventArgs e)
        {
            isAddingEdge = false;
            edgeStartNode = null;
            selectedNode = null;
            canvas.Cursor = Cursors.Cross;
            lblStatus.Text = "Mode: Add Node";
            canvas.Invalidate();
        }

        private void BtnAddEdge_Click(object sender, EventArgs e)
        {
            isAddingEdge = true;
            edgeStartNode = null;
            selectedNode = null;
            canvas.Cursor = Cursors.Hand;
            lblStatus.Text = "Mode: Add Edge (click source)";
            canvas.Invalidate();
        }

        private void BtnRunKruskal_Click(object sender, EventArgs e)
        {
            if (nodes.Count < 2)
            {
                MessageBox.Show("Please add at least 2 nodes.", "Not Enough Nodes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (edges.Count == 0)
            {
                MessageBox.Show("Please add at least one edge.", "No Edges", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            mstEdges = KruskalAlgorithm();
            isMSTMode = true;

            int totalCost = 0;
            foreach (var e2 in mstEdges) totalCost += e2.Weight;
            lblMSTCost.Text = $"MST Cost: {totalCost}";
            lblStatus.Text = $"MST Found! ({mstEdges.Count} edges)";

            UpdateEdgeList();
            canvas.Invalidate();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            isMSTMode = false;
            mstEdges.Clear();
            lblMSTCost.Text = "";
            lblStatus.Text = "Mode: Select";
            UpdateEdgeList();
            canvas.Invalidate();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all nodes and edges?", "Confirm Clear",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                nodes.Clear();
                edges.Clear();
                mstEdges.Clear();
                nodeCounter = 0;
                isMSTMode = false;
                isAddingEdge = false;
                edgeStartNode = null;
                selectedNode = null;
                lblMSTCost.Text = "";
                lblStatus.Text = "Mode: Select";
                UpdateEdgeList();
                canvas.Invalidate();
            }
        }

        // ─── Canvas Events ─────────────────────────────────────────────────────

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
            if (isAddingEdge && edgeStartNode != null)
                canvas.Invalidate();
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Node hit = HitTestNode(e.Location);
                if (hit != null)
                {
                    edges.RemoveAll(ed => ed.Source == hit || ed.Target == hit);
                    nodes.Remove(hit);
                    if (isMSTMode) { isMSTMode = false; mstEdges.Clear(); lblMSTCost.Text = ""; }
                    UpdateEdgeList();
                    canvas.Invalidate();
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Node hit = HitTestNode(e.Location);

            if (!isAddingEdge)
            {
                // Add node mode
                if (hit == null)
                {
                    string name = ((char)('A' + (nodeCounter % 26))).ToString();
                    if (nodeCounter >= 26) name += (nodeCounter / 26).ToString();
                    nodes.Add(new Node(e.X, e.Y, name));
                    nodeCounter++;
                    if (isMSTMode) { isMSTMode = false; mstEdges.Clear(); lblMSTCost.Text = ""; }
                    UpdateEdgeList();
                    canvas.Invalidate();
                }
            }
            else
            {
                // Add edge mode
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
                    {
                        edgeStartNode = null;
                        lblStatus.Text = "Mode: Add Edge (click source)";
                        canvas.Invalidate();
                        return;
                    }

                    // Check duplicate
                    bool exists = edges.Exists(ed =>
                        (ed.Source == edgeStartNode && ed.Target == hit) ||
                        (ed.Source == hit && ed.Target == edgeStartNode));

                    if (exists)
                    {
                        MessageBox.Show("An edge between these nodes already exists.", "Duplicate Edge",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        edgeStartNode = null;
                        lblStatus.Text = "Mode: Add Edge (click source)";
                        canvas.Invalidate();
                        return;
                    }

                    // Ask for weight
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
                    lblStatus.Text = "Mode: Add Edge (click source)";
                    canvas.Invalidate();
                }
            }
        }

        // ─── Drawing ───────────────────────────────────────────────────────────

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawGrid(g);

            // Draw all edges
            foreach (var edge in edges)
            {
                bool inMST = isMSTMode && mstEdges.Contains(edge);
                DrawEdge(g, edge, inMST);
            }

            // Draw rubber-band line when adding edge
            if (isAddingEdge && edgeStartNode != null)
            {
                using (var pen = new Pen(Color.OrangeRed, 2) { DashStyle = DashStyle.Dash })
                    g.DrawLine(pen, edgeStartNode.X, edgeStartNode.Y, mousePos.X, mousePos.Y);
            }

            // Draw nodes
            foreach (var node in nodes)
                DrawNode(g, node);
        }

        private void DrawGrid(Graphics g)
        {
            using (var pen = new Pen(Color.FromArgb(235, 235, 245), 1))
            {
                for (int x = 0; x < canvas.Width; x += 30)
                    g.DrawLine(pen, x, 0, x, canvas.Height);
                for (int y = 0; y < canvas.Height; y += 30)
                    g.DrawLine(pen, 0, y, canvas.Width, y);
            }
        }

        private void DrawEdge(Graphics g, Edge edge, bool isInMST)
        {
            Color lineColor = isInMST ? MST_EDGE_COLOR : EDGE_COLOR;
            float lineWidth = isInMST ? 3.5f : 1.8f;

            Point src = new Point(edge.Source.X, edge.Source.Y);
            Point tgt = new Point(edge.Target.X, edge.Target.Y);

            using (var pen = new Pen(lineColor, lineWidth))
            {
                pen.CustomEndCap = new AdjustableArrowCap(5, 5);
                // Shorten line so arrow tip meets node boundary
                var (s, t) = ShortenLine(src, tgt, 22);
                g.DrawLine(pen, s, t);
            }

            // Weight label at midpoint
            float mx = (src.X + tgt.X) / 2f;
            float my = (src.Y + tgt.Y) / 2f;

            string wLabel = edge.Weight.ToString();
            var font = new Font("Segoe UI", 9, FontStyle.Bold);
            var sz = g.MeasureString(wLabel, font);

            var rect = new RectangleF(mx - sz.Width / 2 - 4, my - sz.Height / 2 - 2, sz.Width + 8, sz.Height + 4);
            using (var bgBrush = new SolidBrush(isInMST ? Color.FromArgb(200, 220, 255, 220) : Color.FromArgb(200, 255, 255, 255)))
                g.FillRectangle(bgBrush, rect);

            using (var border = new Pen(isInMST ? MST_EDGE_COLOR : Color.LightGray, 1))
                g.DrawRectangle(border, rect.X, rect.Y, rect.Width, rect.Height);

            using (var br = new SolidBrush(isInMST ? Color.DarkGreen : Color.FromArgb(60, 60, 80)))
                g.DrawString(wLabel, font, br, mx - sz.Width / 2, my - sz.Height / 2);
        }

        private void DrawNode(Graphics g, Node node)
        {
            bool isStart = node == edgeStartNode;
            int r = 22;

            // Shadow
            using (var shadow = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                g.FillEllipse(shadow, node.X - r + 3, node.Y - r + 3, r * 2, r * 2);

            // Fill
            Color fill = isStart ? NODE_SELECTED : NODE_COLOR;
            using (var br = new SolidBrush(fill))
                g.FillEllipse(br, node.X - r, node.Y - r, r * 2, r * 2);

            // Border
            using (var pen = new Pen(isStart ? Color.DarkOrange : NODE_BORDER, 2.5f))
                g.DrawEllipse(pen, node.X - r, node.Y - r, r * 2, r * 2);

            // Label
            var font = new Font("Segoe UI", 10, FontStyle.Bold);
            var sz = g.MeasureString(node.Label, font);
            using (var br = new SolidBrush(Color.White))
                g.DrawString(node.Label, font, br, node.X - sz.Width / 2, node.Y - sz.Height / 2);
        }

        // ─── Kruskal's Algorithm ───────────────────────────────────────────────

        private List<Edge> KruskalAlgorithm()
        {
            // Sort edges by weight
            var sorted = new List<Edge>(edges);
            sorted.Sort((a, b) => a.Weight.CompareTo(b.Weight));

            // Union-Find
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
                if (rank[ra] < rank[rb]) { parent[ra] = rb; }
                else if (rank[ra] > rank[rb]) { parent[rb] = ra; }
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

        // ─── Helpers ───────────────────────────────────────────────────────────

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
                lstEdges.Items.Add($"{e.Source.Label}→{e.Target.Label}  w={e.Weight}" + (isMSTMode ? " ✓" : ""));
        }
    }
}
