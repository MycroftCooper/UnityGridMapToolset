using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridMapToolset.MazeGenerate {
    public class MazeGenerateRequest {
        public readonly Vector2Int Size;
        public readonly Vector2Int StartPos;
        public readonly Vector2Int EndPos;
        
        public readonly int Seed;
        public readonly float E;
        public readonly float R;
        
        public bool[,] ResultMaze;
        public List<Vector2Int> ResultPath;

        public MazeGenerateRequest(Vector2Int size, Vector2Int startPos, Vector2Int endPos,
            int seed = 0, float e = 0f, float r = 0f) {
            if (size.x <= 0 || size.y <= 0) {
                throw new ArgumentException("Maze size must be greater than zero for both dimensions.");
            }

            if (startPos.x < 0 || startPos.x >= size.x || startPos.y < 0 || startPos.y >= size.y) {
                throw new ArgumentException("Start position must be within the bounds of the maze size.");
            }

            if (endPos.x < 0 || endPos.x >= size.x || endPos.y < 0 || endPos.y >= size.y) {
                throw new ArgumentException("End position must be within the bounds of the maze size.");
            }

            if (startPos == endPos) {
                throw new ArgumentException("Start position and end position cannot be the same.");
            }

            if (e is < 0f or > 1f) {
                throw new ArgumentException("E value must be between 0 and 1.");
            }

            if (r is < 0f or > 1f) {
                throw new ArgumentException("R value must be between 0 and 1.");
            }

            Size = size;
            StartPos = startPos;
            EndPos = endPos;
            Seed = seed;
            E = e;
            R = r;
        }
    }
}