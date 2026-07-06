# Kruskal's Minimum Spanning Tree - Windows Forms Application
## Project 15 | C# | Greedy Algorithm

---

## Overview
This Windows Forms application allows users to interactively build a **weighted directed graph** and apply **Kruskal's Algorithm** (Greedy approach) to find the **Minimum Spanning Tree (MST)**.

---

## Features
- **Interactive Graph Drawing** — Click on the canvas to place named nodes (A, B, C, ...)
- **Weighted Directed Edges** — Click two nodes to add a directed edge with a custom weight/cost
- **Kruskal's MST Algorithm** — Finds the MST using Union-Find (Disjoint Set Union) data structure
- **Visual Highlighting** — MST edges are highlighted in green; non-MST edges remain grey
- **Edge Weight Labels** — All edge costs shown on the canvas
- **Edge List Panel** — Side panel shows all edges (or MST edges after computation)
- **Total MST Cost** — Displayed after running the algorithm
- **Delete Nodes** — Right-click any node to remove it along with its edges
- **Clear All** — Wipe the canvas and start fresh

---

## Requirements
- **Visual Studio 2022** (recommended) or VS Code with C# extension
- **.NET 6.0 SDK** or later
- **Windows OS** (Windows Forms is Windows-only)

---

## How to Run

### Option 1: Visual Studio
1. Open `KruskalMST.sln` in Visual Studio 2022
2. Press `F5` or click **Start** to build and run

### Option 2: .NET CLI
```bash
cd KruskalMST
dotnet run
```

---

## How to Use

| Action | How |
|--------|-----|
| **Add Node** | Click "Add Node" button, then click anywhere on canvas |
| **Add Edge** | Click "Add Edge" button, click source node, then target node. Enter weight in dialog |
| **Run MST** | Click "Run Kruskal's" — MST edges highlighted in green |
| **Reset View** | Click "Reset View" to clear MST highlighting |
| **Delete Node** | Right-click any node to remove it (and its edges) |
| **Clear All** | Click "Clear All" to wipe the entire graph |

---

## Algorithm Details

### Kruskal's Algorithm (Greedy Approach)
1. **Sort** all edges by weight in ascending order
2. **Initialize** Union-Find structure (each node is its own component)
3. **Iterate** through sorted edges:
   - If adding the edge does NOT create a cycle (source and target in different components) → **add to MST**
   - Otherwise, **skip** the edge
4. Stop when MST has `(V - 1)` edges where V = number of vertices

### Union-Find (Disjoint Set Union)
- Uses **path compression** and **union by rank** for near O(1) operations
- `Find(x)` → returns root of x's component
- `Union(a, b)` → merges components of a and b

### Time Complexity
- Sorting: O(E log E)
- Union-Find: O(E α(V)) ≈ O(E) with path compression
- **Overall: O(E log E)**

---

## File Structure
```
KruskalMST/
├── KruskalMST.sln          # Visual Studio solution file
└── KruskalMST/
    ├── KruskalMST.csproj   # Project file (.NET 6 WinForms)
    ├── Program.cs           # Entry point
    ├── Form1.cs             # Main form (UI + drawing + algorithm)
    ├── Form1.Designer.cs    # Designer-generated code
    ├── Node.cs              # Node model class
    ├── Edge.cs              # Edge model class
    └── EdgeWeightDialog.cs  # Dialog for entering edge weight
```

---

## Sample Test Case

Try this graph:
- Nodes: A, B, C, D, E
- Edges: A→B(4), A→C(2), B→C(1), B→D(5), C→D(8), C→E(10), D→E(2)

**Expected MST edges:** B→C(1), A→C(2), D→E(2), A→B(4)  
**Total MST Cost: 9**
