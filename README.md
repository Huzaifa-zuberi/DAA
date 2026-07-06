# DAA — Design & Analysis of Algorithms

[![C#](https://img.shields.io/badge/C%23-11.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp)
[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-success)](LICENSE)

A collection of **Algorithm Design & Analysis** projects with interactive visualizations in C# .NET.

---

## Project: Kruskal's Minimum Spanning Tree

Interactive **Windows Forms** application demonstrating **Kruskal's Greedy Algorithm** for finding the Minimum Spanning Tree (MST) using **Union-Find (Disjoint Set Union)**.

### Features
- Interactive graph drawing — click to place nodes and add weighted edges
- Kruskal's MST with visual highlighting (green edges = MST)
- Union-Find with path compression & union by rank
- Real-time edge list panel with total MST cost
- Node deletion, edge weight dialogs, canvas reset

### Complexity
| Operation | Time |
|-----------|------|
| Edge sorting | O(E log E) |
| Union-Find operations | O(E α(V)) ≈ O(E) |
| **Overall** | **O(E log E)** |

### Quick Start
`ash
git clone https://github.com/Huzaifa-zuberi/DAA.git
cd DAA
dotnet run --project src
`

### Test Case
`
Nodes: A, B, C, D, E
Edges: A→B(4), A→C(2), B→C(1), B→D(5), C→D(8), C→E(10), D→E(2)

MST: B→C(1), A→C(2), D→E(2), A→B(4)
Total Cost: 9
`

---

## File Structure
`
DAA/
├── src/           # C# source code (.NET 6 WinForms)
├── docs/          # Algorithm documentation
├── KruskalMST.sln # Visual Studio solution
└── README.md      # Project documentation
`

## License
MIT © 2026 Huzaifa Zuberi
