using System.Collections.Generic;
using System.Diagnostics;
using GridMapToolset2D.MazeGenerate;
using Unity2DGridMapToolset.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace GridMapToolset2D.PathFinding {
#if UNITY_EDITOR
    public partial class PathFinder {
        public bool isDebug;
        [ShowInInspector] public bool[,] TestMap;
        public bool debugNeedBestSolution = true;
        public PathFinderAlgorithms debugAlgorithm = PathFinderAlgorithms.JPS;
        public PathReprocesses debugPathReprocesses = PathReprocesses.None;
        public HeuristicTypes debugHeuristic = HeuristicTypes.Manhattan;
        private PathFindingRequest _debugRequest;
        private Stopwatch _stopwatch;
        private MazeGenerator _debugMazeGenerator;

        [Button]
        private void DebugInitTestMap(Vector2Int size) {
            TestMap = new bool[size.x, size.y];
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    TestMap[x, y] = true;
                }
            }
            SetPassableMap(TestMap);
        }

        [Button]
        private void DebugUpdateTestMap() {
            SetPassableMap(TestMap);
        }

        [Button]
        private void DebugGeneratorMaze(Vector2Int size, int seed) {
            size = new Vector2Int(101, 100);
            TestMap = new bool[size.x, size.y];
            _debugMazeGenerator = new MazeGenerator();
            TestMap = _debugMazeGenerator.GenerateMaze(size,seed, new Vector2Int(1,0), new Vector2Int(99,99));
            SetPassableMap(TestMap);
        }

        [Button]
        private void DebugFindPath(Vector2Int start, Vector2Int end) {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            PathFindingRequest request = new PathFindingRequest(start, end, 
                debugAlgorithm, debugNeedBestSolution, debugHeuristic, debugPathReprocesses);
            ExecuteRequest(request);
            _debugRequest = request;
            _stopwatch.Stop();
            Debug.Log($"Pathfinder> DebugRequest completed in {_stopwatch.Elapsed.TotalMilliseconds} ms.\n" +
                      $"{_debugRequest}\n{_map}");
        }

        [Button]
        private void DebugSplitFrameTest(Vector2Int start, Vector2Int end, int times) {
            for (int i = 0; i < times; i++) {
                PathFindingRequest request = new PathFindingRequest(start, end, 
                    debugAlgorithm, debugNeedBestSolution, debugHeuristic, debugPathReprocesses, false, 
                    r=> Debug.Log($"Pathfinder> DebugRequest completed!\n{r}"));
                AddFindPathRequest(request);
            }
            Debug.Log($"Pathfinder> All {times} DebugRequest completed!");
        }

        void OnDrawGizmos() {
            if (!isDebug) return;
            DrawMap();
            if (_debugRequest == null) return;
            DrawPath(_debugRequest.ResultPath, Color.blue, Vector3.zero);
            DrawPath(_debugRequest.ReprocessedPath, Color.green, new Vector3(0.1f, 0f, 0.1f));
        }

        private void DrawMap() {
            if(_map == null) return;
            IPathFinderAlgorithm a = null;
            if (_debugRequest != null) {
                a = GetAlgorithm(_debugRequest.Algorithm, _debugRequest.HeuristicType);
            }
            Gizmos.color = Color.gray;
            Vector3 oPos = transform.position + new Vector3(0.5f, 0.5f);
            // 遍历 passableMap 并绘制格子
            for (int x = 0; x < _map.Width; x++) {
                for (int y = 0; y < _map.Height; y++) {
                    Vector3 position = oPos + new Vector3(x, y, 0); // 将网格位置映射到世界空间
                    a?.OnDebugDrawGizmos(oPos, new Vector2Int(x, y));
                    Gizmos.color = _map.PassableMap[x, y] ? Color.clear : Color.red;
                    Gizmos.DrawCube(position, new Vector3(1, 1, 0.1f));
                    Gizmos.color = Color.black; // 边框颜色设置为黑色
                    Gizmos.DrawWireCube(position, Vector3.one);
                }
            }
        }
        
        private void DrawPath(List<Vector2Int> path, Color color, Vector3 offset) {
            if (path == null || path.Count == 0) {
                return;
            }
            Vector3 oPos = transform.position + new Vector3(0.5f, 0.5f);
            
            Gizmos.color = color;
            var p = path[0].ToVec3() + oPos + new Vector3(0, 0, 0.2f);
            Gizmos.DrawCube(p, new Vector3(0.9f, 0.9f, 0.1f));
            p = path[^1].ToVec3() + oPos + new Vector3(0, 0, 0.2f);
            Gizmos.DrawCube(p, new Vector3(0.9f, 0.9f, 0.1f));
            
            for (int i = 0; i < path.Count - 1; i++) {
                Vector3 startPos = oPos + new Vector3(path[i].x, path[i].y, 0.2f);
                Vector3 endPos = oPos + new Vector3(path[i + 1].x, path[i + 1].y, 0.2f);
                Debug.DrawLine(startPos + offset, endPos + offset, color);
            }
        }
    }
#endif
}