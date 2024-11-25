using System.Collections.Generic;
using UnityEngine;

namespace GridMapToolset.MazeGenerate {
    public enum MazeGenerateAlgorithms { BFS, DFS, Prim, Kruskal, Wilson }

    public interface IMazeGenerateAlgorithm {
        public MazeGenerateAlgorithms Algorithm { get; }
        public void GenerateMaze(MazeGenerateRequest request);
    }
}