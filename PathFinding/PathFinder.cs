using System;
using System.Collections.Generic;
using System.Linq;
using Unity2DGridMapToolset.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GridMapToolset.PathFinding {
    public partial class PathFinder : FrameTaskScheduler<PathFindingFrameTask> {
        #region 地图处理相关
        public bool canDiagonallyPassByObstacle;
        private PathFinderMap _map;
        
        public void SetPassableMap(bool[,] map) {
            _map = new PathFinderMap(map, canDiagonallyPassByObstacle);
            if(_algorithms.Count != 0) {
                foreach (var a in _algorithms.Values) {
                    a.InitMap(_map);
                }
            }
        }
        
        public void UpdatePassableMap(RectInt bounds, bool passable) {
            if (!_map.IsInBounds(bounds.min.x, bounds.min.y) || !_map.IsInBounds(bounds.max.x, bounds.max.y)) {
                Debug.LogError($"Bounds{bounds} is not in Map bounds!");
                return;
            }
            _map.UpdateMap(bounds, passable);
            if(_algorithms.Count != 0) {
                foreach (var a in _algorithms.Values) {
                    a.UpdateMap(bounds, passable);
                }
            }
        }
        #endregion

        #region 算法相关
        public bool useLineOfSightFirstCheck;
        private readonly Dictionary<PathFinderAlgorithms, IPathFinderAlgorithm> _algorithms = new ();
        private readonly Dictionary<PathReprocesses, IPathReprocess> _reprocesses = new ();
        private readonly HeuristicFunctions.CommonHeuristicFunction _commonHeuristicFunction = new(HeuristicTypes.Manhattan);
        
        private bool FirstCheck(PathFindingRequest request) {
            if (!useLineOfSightFirstCheck || !_map.IsLineOfSight(request.StartPos, request.EndPos)) return false;
            request.ResultPath = new List<Vector2Int>{request.StartPos ,request.EndPos};
            request.ReprocessedPath = new List<Vector2Int>(request.ResultPath);
            return true;
        }

        private IPathFinderAlgorithm GetAlgorithm(PathFinderAlgorithms algorithmType, HeuristicTypes heuristicFunction) {
            if (_algorithms.TryGetValue(algorithmType, out var a)) {
                return a;
            }
            
            a = algorithmType switch {
                PathFinderAlgorithms.JPS => new JPS(),
                PathFinderAlgorithms.JPSPlus => new JPSPlus(),
                PathFinderAlgorithms.AStar => new AStart(),
                PathFinderAlgorithms.Dijkstra => new Dijkstra(),
                PathFinderAlgorithms.BFS => new BFS(),
                PathFinderAlgorithms.DFS => new DFS(),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithmType), algorithmType, null)
            };
            
            if (heuristicFunction == HeuristicTypes.None) {
                a.HeuristicFunction = null;
            } else {
                _commonHeuristicFunction.HeuristicType = heuristicFunction;
                a.HeuristicFunction = _commonHeuristicFunction;
            }
            
            a.InitMap(_map);
            _algorithms.Add(algorithmType, a);
            return a;
        }

        private IPathReprocess GetReprocess(PathReprocesses reprocessType) {
            if (reprocessType == PathReprocesses.None) {
                return null;
            }
            if (_reprocesses.TryGetValue(reprocessType, out var p)) {
                return p;
            }

            p = reprocessType switch {
                PathReprocesses.Default => new DefaultPathSmooth(),
                PathReprocesses.Theta => new Theta(),
                _ => throw new ArgumentOutOfRangeException(nameof(reprocessType), reprocessType, null)
            };
            _reprocesses.Add(reprocessType ,p);
            return p;
        }
        #endregion
        
        private Queue<PathFindingRequest> _pathCache;
        public void AddFindPathRequest(PathFindingRequest request, int priority = 1) {
            if (!IsRequestValid(request)) return;
            if (useLineOfSightFirstCheck && FirstCheck(request)) return;
            
            float h = HeuristicFunctions.CalculateHeuristic(request.HeuristicType, request.StartPos, request.EndPos);
            float mapMaxH = HeuristicFunctions.CalculateHeuristic(request.HeuristicType, Vector2Int.zero, _map.Size - Vector2Int.one);
            float p = priority + h / mapMaxH;
            
            var newTask = new PathFindingFrameTask(this, request, p);
            AddTask(newTask);
        }

        public void ExecuteRequest(PathFindingRequest request) {
            if (request.CanUseCache) {
                // todo:使用缓存加速寻路
            }
            else {
                var a = GetAlgorithm(request.Algorithm, request.HeuristicType);
                a.NeedBestSolution = request.NeedBestSolution;
                var resultPath = a.FindPath(request.StartPos, request.EndPos);
                request.ResultPath = resultPath;
                var p = GetReprocess(request.Reprocess);
                request.ReprocessedPath = p != null ? p.ReprocessPath(request.ResultPath, _map) : new List<Vector2Int>(request.ResultPath);
            }
        }
        
        public void CancelRequest(PathFindingRequest request) {
            var task = Tasks.First(t => t.Request == request );
            CancelTask(task);
        }

        private bool IsRequestValid(PathFindingRequest request) {
            if (request == null) {
                Debug.LogError("PathFinder: request is null");
                return false;
            }
            if (request.StartPos == request.EndPos) {
                Debug.LogError($"PathFinder: StartPos cant equal EndPos{request.EndPos.x}");
                return false;
            }
            if (!_map.IsInBounds(request.StartPos.x, request.StartPos.y) ||
                !_map.IsInBounds(request.EndPos.x, request.EndPos.y)) {
                Debug.LogError($"PathFinder: StartPos{request.StartPos} or EndPos{request.EndPos.x} is out of range");
                return false;
            }
            if (!_map.IsPassable(request.StartPos.x, request.StartPos.y, false) || 
                !_map.IsPassable(request.EndPos.x, request.EndPos.y, false)) {
                Debug.LogError($"PathFinder: StartPos{request.StartPos} or EndPos{request.EndPos.x} is not passable");
                return false;
            }
            return true;
        }
    }
}