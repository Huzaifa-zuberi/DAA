# Kruskal's MST — Algorithm Walkthrough

## Algorithm Steps
1. Sort all edges by weight (ascending)
2. Initialize Union-Find (each node = its own component)
3. For each edge (u, v, weight) in sorted order:
   - If Find(u) != Find(v) → add to MST, Union(u, v)
   - Else → skip (would create a cycle)
4. Stop when MST has (V-1) edges

## Union-Find
- Find(x): returns root of x's component (with path compression)
- Union(a,b): merges components of a and b (union by rank)
- Time: O(α(V)) per operation (effectively constant)
