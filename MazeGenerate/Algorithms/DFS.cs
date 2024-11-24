using System.Collections.Generic;
using GridMapToolset.Util;
using UnityEngine;

namespace GridMapToolset.MazeGenerate {
    public class DFS: IMazeGenerateAlgorithm {
        public MazeGenerateAlgorithms Algorithm => MazeGenerateAlgorithms.DFS;

        private readonly Stack<Vector2Int> _stack = new();
        private readonly HashSet<Vector2Int> _visited = new();
        
        public void GenerateMaze(MazeGenerateRequest request) {
            _stack.Clear();
            _visited.Clear();
            System.Random random = new System.Random(request.Seed);
            var resultPath = new List<Vector2Int>();
            RectGridPassableMap map = new RectGridPassableMap(request.ResultMaze) {
                PassableMap = {
                    [request.StartPos.x, request.StartPos.y] = true,
                    [request.EndPos.x, request.EndPos.y] = true
                }
            };
            bool isReachedEnd = false;

            _stack.Push(request.StartPos);

            while (_stack.Count > 0) {
                Vector2Int current = _stack.Pop();
                resultPath.Add(current);
                
                if (current == request.EndPos) {
                    isReachedEnd = true;
                    
                }

                List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, map);
                if (neighbors.Count <= 0) continue;
                _stack.Push(current);

                Vector2Int next = neighbors[random.Next(neighbors.Count)];
                RemoveWall(map.PassableMap, current, next);
                map.PassableMap[next.x, next.y] = true;
                _stack.Push(next);
            }

            request.ResultPath = resultPath;
            request.ResultMaze = map.PassableMap;
        }

        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int current, RectGridPassableMap passableMap) {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (var dir in RectGridPassableMap.Direction4Dict.Values) {
                Vector2Int neighbor = current + dir;
                if (passableMap.IsInBounds(neighbor.x, neighbor.y) && !passableMap.PassableMap[neighbor.x, neighbor.y]) {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private void RemoveWall(bool[,] maze, Vector2Int current, Vector2Int next) {
            Vector2Int wall = (current + next) / 2;
            maze[wall.x, wall.y] = true;
        }
    }
}