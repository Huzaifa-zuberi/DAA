# MST Visualizer — Kruskal's Algorithm

[![CI](https://github.com/Huzaifa-zuberi/DAA/actions/workflows/ci.yml/badge.svg)](https://github.com/Huzaifa-zuberi/DAA/actions/workflows/ci.yml)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-success)](LICENSE)

Interactive **Windows Forms** application demonstrating **Kruskal's Greedy Algorithm** for finding the Minimum Spanning Tree (MST) using **Union-Find (Disjoint Set Union)**.

## Features
- Click to place nodes, click to add weighted edges
- Dark theme with purple/cyan accent colors
- Kruskal's MST with green edge highlighting
- Union-Find with path compression & union by rank
- Real-time edge list with total MST cost
- Right-click to delete nodes

## Complexity
| Operation | Time |
|-----------|------|
| Edge sorting | O(E log E) |
| Union-Find operations | O(E α(V)) ≈ O(E) |
| **Overall** | **O(E log E)** |

## Quick Start
```bash
git clone https://github.com/Huzaifa-zuberi/DAA.git
cd DAA
dotnet run --project src
```

## Test Case
```
Nodes: A, B, C, D, E
Edges: A-B(4), A-C(2), B-C(1), B-D(5), C-D(8), C-E(10), D-E(2)

MST: B-C(1), A-C(2), D-E(2), A-B(4)
Total Cost: 9
```

## Structure
```
DAA/
├── src/           # C# .NET 8 Windows Forms app
├── docs/          # Algorithm documentation
├── MSTVisualizer.sln
└── README.md
```

## License
MIT
