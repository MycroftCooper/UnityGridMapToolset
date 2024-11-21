using Sirenix.OdinInspector;
using System.Collections.Generic;
using GridMapToolset.PathFinding;
using Unity2DGridMapToolset.Util;
using UnityEngine;

namespace GridMapToolset.Map {
    public class RoomGrid : MonoBehaviour {
        #region 初始化相关
        public void Init(Vector2Int fullSize, Vector2Int baseSize) {
            FullSize = fullSize;
            BaseSize = baseSize;
            RoomElements = new Dictionary<IOccupiedRoomObject, Vector2Int>();
            ObjectMap = new IOccupiedRoomObject[baseSize.x, baseSize.y];
            PassableMap = new bool[baseSize.x, baseSize.y];
            for (int i = 0; i < BaseSize.x; i++) {
                for (int j = 0; j < BaseSize.y; j++) {
                    PassableMap[i, j] = true;
                }
            }
            PathFinder = gameObject.AddComponent<PathFinder>();
            PathFinder.SetPassableMap(PassableMap);
        }
        
        #endregion
        
        #region 基础信息
        [ShowInInspector]
        public string RoomName => gameObject.transform.parent.name;
        [ShowInInspector]
        public Vector3 CenterPos {
            get => transform.position + FullSize.ToVec3() / 2;
            set => transform.position = value - FullSize.ToVec3() / 2;
        }

        [ShowInInspector] public Vector2Int FullSize { get; protected set; }
        public RectInt FullRect => new RectInt(Vector2Int.zero, FullSize);
        [ShowInInspector] public Vector2Int BaseSize { get; protected set; }
        public RectInt BaseRect => new RectInt((FullSize - BaseSize) / 2, BaseSize);
        #endregion

        #region 房间物品放置相关
        public Dictionary<IOccupiedRoomObject, Vector2Int> RoomElements;
        public IOccupiedRoomObject[,] ObjectMap { get; private set; }

        public PointSet ProhibitedZone;
        
        public bool[,] PassableMap { get; private set; }
        public PathFinder PathFinder { get; private set; }

        private void UpdateMapForChangedObject(IOccupiedRoomObject obj, bool isAdding) {
            var occupiedPositions = obj.GridData.GridRect;
            for (int x = occupiedPositions.xMin; x < occupiedPositions.xMax; x++) {
                for (int y = occupiedPositions.yMin; y < occupiedPositions.yMax; y++) {
                    if (!IsInBaseMap(new Vector2Int(x, y))) { 
                        Debug.LogError($"目标物体{obj.Go.name}在房间{RoomName}的位置({x}, {y})越界!isAdding:{isAdding}");
                        continue;
                    }
                    if (isAdding) {
                        if (ObjectMap[x, y] != null) {
                            Debug.LogError($"目标物体{obj.Go.name}在房间{RoomName}的位置({x}, {y})与物体{ObjectMap[x, y].Go.name}重叠!");
                            continue;
                        }
                        ObjectMap[x, y] = obj;
                        PassableMap[x, y] = obj.GridData.isPassable;
                    } else {
                        if (ObjectMap[x, y] == null) {
                            Debug.LogError($"目标物体{obj.Go.name}在房间{RoomName}的位置({x}, {y})已被移除，请勿重复移除!");
                            continue;
                        }
                        if (obj != ObjectMap[x, y]) {
                            Debug.LogError($"目标物体{obj.Go.name}在房间{RoomName}的位置({x}, {y})与物体{ObjectMap[x, y].Go.name}重叠!");
                        }
                        ObjectMap[x, y] = null;
                        PassableMap[x, y] = false;
                    }
                }
            }
        }
        
        public bool CanSetObjectAt(IRoomObject targetObj, Vector2Int lPos, bool isPassablePlaceable = false,  
            HashSet<Vector2Int> filter = null, bool isProhibitedZoneEnable = true) {
            if (targetObj == null) {
                Debug.LogError("目标物体为Null!");
                return false;
            }
            
            // 使用PlaceableMap检查是否可放置
            var targetRect = new RectInt(lPos, targetObj.GridSize.size);
            return IsRectPlaceable(targetRect, isPassablePlaceable, filter, isProhibitedZoneEnable);
        }

        public bool IsRectPlaceable(RectInt targetRect, bool isPassablePlaceable = false, 
            HashSet<Vector2Int> filter = null, bool isProhibitedZoneEnable = true) {
            for (int x = targetRect.xMin; x < targetRect.xMax; x++) {
                for (int y = targetRect.yMin; y < targetRect.yMax; y++) {
                    var pos = new Vector2Int(x, y);
                    if (!IsInBaseMap(pos)) {
                        return false;
                    }
                    if (filter != null && filter.Contains(pos)) {
                        return false;
                    }
                    if (isProhibitedZoneEnable && ProhibitedZone.Points.Contains(pos)) {
                        return false;
                    }
                    if (ObjectMap[x, y] != null && !(isPassablePlaceable && ObjectMap[x, y].GridData.isPassable)) {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<Vector2Int> GetAllPlaceableGridPos(Vector2Int targetSize, bool isPassablePlaceable = false, HashSet<Vector2Int> filter = null) {
            List<Vector2Int> result = new List<Vector2Int>();
            for (int x = 0; x < BaseSize.x; x++) {
                for (int y = 0; y < BaseSize.y; y++) {
                    if (ObjectMap[x, y] != null && !(isPassablePlaceable && ObjectMap[x, y].GridData.isPassable)) {
                        y += ObjectMap[x, y].GridSize.size.y - 1;
                        continue;
                    }
                    
                    Vector2Int pos = new Vector2Int(x, y);
                    if (filter != null && filter.Contains(pos)) {
                        continue;
                    }
                    if (ProhibitedZone.Points.Contains(pos)) {
                        continue;
                    }
                    
                    RectInt targetRect = new RectInt(pos, targetSize);
                    if (IsRectPlaceable(targetRect, isPassablePlaceable, filter)) {
                        result.Add(pos);
                    }
                }
            }
            return result;
        }

        public Vector2Int GetRandomGridPosition(QuickRandom random) {
            return new Vector2Int(random.GetInt(0, BaseSize.x), random.GetInt(0, BaseSize.y));
        }

        public (Vector2Int pos, bool isPlaceable) GetRandomPlaceableGridPosition(QuickRandom random) {
            List<Vector2Int> placeablePositions = GetAllPlaceableGridPos();
            if (placeablePositions == null || placeablePositions.Count == 0) {
                return (default, false);
            }
            return (placeablePositions.GetRandomObject(random), true);
        }
        
        public List<Vector2Int> GetAllPlaceableGridPos() {
            List<Vector2Int> placeablePositions = new List<Vector2Int>();
            for (int x = 0; x < BaseSize.x; x++) {
                for (int y = 0; y < BaseSize.y; y++) {
                    Vector2Int p = new Vector2Int(x, y);
                    if (ObjectMap[x, y] != null || ProhibitedZone.Points.Contains(p)) {
                        continue;
                    }
                    placeablePositions.Add(p);
                }
            }
            return placeablePositions;
        }

        public int GetPlaceableGridPosCount() {
            int count = 0;
            for (int x = 0; x < BaseSize.x; x++) {
                for (int y = 0; y < BaseSize.y; y++) {
                    var p = new Vector2Int(x, y);
                    if (ObjectMap[x, y] != null || ProhibitedZone.Points.Contains(p)) {
                        continue;
                    }
                    count++;
                }
            }
            return count;
        }

        public List<Vector2Int> GetPlaceableGridPosList(IRoomObject targetObj, bool isPassablePlaceable = false, HashSet<Vector2Int> filter = null) {
            List<Vector2Int> placeablePositions = new List<Vector2Int>();
            for (int x = 0; x < BaseSize.x; x++) {
                for (int y = 0; y < BaseSize.y; y++) {
                    Vector2Int potentialPosition = new Vector2Int(x, y);
                    if (CanSetObjectAt(targetObj, potentialPosition, isPassablePlaceable, filter)) {
                        placeablePositions.Add(potentialPosition);
                    }
                }
            }
            return placeablePositions;
        }

        public void GetPlacementInfo(RoomObjectPlacementInfo targetObj, QuickRandom random, bool isPassablePlaceable = false) {
            var canSetPosList = GetPlaceableGridPosList(targetObj, isPassablePlaceable);
            if (canSetPosList == null || canSetPosList.Count == 0) {
                return;
            }

            var targetGridPos = canSetPosList.GetRandomObject(random);
            targetObj.SetPos(this, targetGridPos);
        }

        public void GetPlacementInfos(List<RoomObjectPlacementInfo> targetObjs, QuickRandom random, bool isPassablePlaceable = false) {
            targetObjs.Shuffle(random); // 随机化放置顺序
            HashSet<Vector2Int> tempPlacementMap = new HashSet<Vector2Int>();

            foreach (var obj in targetObjs) {
                List<Vector2Int> placeableGridPositions = GetPlaceableGridPosList(obj, isPassablePlaceable, tempPlacementMap);
                if (placeableGridPositions.Count == 0) {
                    if (obj.GridSize.size == Vector2Int.one) {
                        break; // 最小的一个也放不下时break
                    }
                    continue;
                }

                var targetGridPos = placeableGridPositions.GetRandomObject(random); // 随机化放置顺序
                obj.SetPos(this, targetGridPos);
                RectInt boundaryRect = obj.GridSize.GetRectInt(targetGridPos);
                var pSet = new PointSetRect(boundaryRect);
                foreach (Vector2Int p in pSet.Points) {
                    tempPlacementMap.Add(p);
                }
            } 
        }

        public IOccupiedRoomObject GetObjectAt(Vector2Int logicPos) {
            if (IsInBaseMap(logicPos)) {
                return ObjectMap[logicPos.x, logicPos.y];
            }
            Debug.LogError($"房间{RoomName}查询位置{logicPos}越界!");
            return null;
        }
        
        public bool AddObject(IOccupiedRoomObject obj, Vector2Int lPos, bool isProhibitedZoneEnable = true) {
            if (obj == null) {
                Debug.LogError("目标物体为Null!");
                return false;
            }

            if (obj.OwnerRoomGrid != null) {
                Debug.LogWarning($"物体:{obj.Go.name}已被放置在{RoomName}房间,无法重复放置！");
                return false;
            }
            
            if (!CanSetObjectAt(obj, lPos,false, null, isProhibitedZoneEnable)) {
                Debug.LogWarning($"无法在位置 ({lPos.x}, {lPos.y}) 放置物体 {obj.Go.name}.");
                return false;
            }
            
            obj.GridData.SetPos(lPos);
            UpdateMapForChangedObject(obj, true);
            RoomElements.Add(obj, lPos);
            obj.OwnerRoomGrid = this;
            var layoutPos = GetObjectWorldLayoutPos(obj, lPos);
            if (obj.Go != null) {
                obj.Go.transform.position = layoutPos;
                Debug.Log($"物品{obj.Go.name}已被放入房间{RoomName}, 位置:{lPos}");
            }
            obj.OnAddedToRoomGrid();
            return true;
        }

        public bool AddObject(IOccupiedRoomObject obj, QuickRandom random) {
            if (obj == null) {
                Debug.LogError("目标物体为Null!");
                return false;
            }

            if (obj.OwnerRoomGrid != null) {
                Debug.LogWarning($"物体:{obj.Go.name}已被放置在{RoomName}房间,无法重复放置！");
                return false;
            }
            
            var canSetPosList = GetPlaceableGridPosList(obj);
            if (canSetPosList == null || canSetPosList.Count == 0) {
                return false;
            }

            var lPos = canSetPosList.GetRandomObject(random);
            obj.GridData.SetPos(lPos);
            UpdateMapForChangedObject(obj, true);
            RoomElements.Add(obj, lPos);
            obj.OwnerRoomGrid = this;
            var layoutPos = GetObjectWorldLayoutPos(obj, lPos);
            obj.Go.transform.position = layoutPos;
            obj.OnAddedToRoomGrid();
            Debug.Log($"物品{obj.Go.name}已被放入房间{RoomName}, 位置:{lPos}");
            return true;
        }
        
        public void RemoveObject(IOccupiedRoomObject obj) {
            if (obj == null) {
                Debug.LogError("目标物体为Null!");
                return;
            }

            if (!RoomElements.ContainsKey(obj)) {
                Debug.LogError($"目标物体{obj.Go.name}不存在房间{RoomName}中!无法移除!");
                return;
            }
            UpdateMapForChangedObject(obj, false);
            RoomElements.Remove(obj);
            obj.OwnerRoomGrid = null;
            obj.GridData.SetPos(Vector2Int.zero);
            
            obj.OnRemoveFormRoomGrid(this);
            Debug.Log($"物品{obj.Go.name}已被取出房间{RoomName}");
        }
        #endregion
        
        #region 坐标转换
        // 以下所有操作都是基于BaseSize,是房间内部可用空间的计算
        public Vector3 GetObjectLocalLayoutPos(IRoomObject obj, Vector2Int gridPos) {
            Vector2 basePos = GridPos2LocalPos(gridPos);
            Vector2 adjustedPos = basePos + obj.LayoutOffset.pivot;
            Vector3 finalPos = new Vector3(adjustedPos.x + obj.LayoutOffset.offset.x, adjustedPos.y + obj.LayoutOffset.offset.y, obj.LayoutOffset.offset.z);
            return finalPos;
        }
        
        public Vector3 GetObjectWorldLayoutPos(IRoomObject obj, Vector2Int gridPos) {
            Vector2 basePos = GridPos2WorldPos(gridPos);
            Vector2 adjustedPos = basePos + obj.LayoutOffset.pivot;
            Vector3 finalPos = new Vector3(adjustedPos.x + obj.LayoutOffset.offset.x, adjustedPos.y + obj.LayoutOffset.offset.y, obj.LayoutOffset.offset.z);
            return finalPos;
        }

        public Vector2Int GetObjectGridPosByWorldPos(IRoomObject obj, Vector3 worldPos) {
            Vector2 adjustedPos = worldPos - obj.LayoutOffset.offset;
            Vector2 basePos = adjustedPos - obj.LayoutOffset.pivot;
            Vector2Int gridPos = WorldPos2GridPos(basePos);
            return gridPos;
        }
        
        public Vector2Int GetObjectGridPosByLocalPos(IRoomObject obj, Vector3 localPos) {
            Vector2 adjustedPos = localPos - obj.LayoutOffset.offset;
            Vector2 basePos = adjustedPos - obj.LayoutOffset.pivot;
            Vector2Int gridPos = LocalPos2GridPos(basePos);
            return gridPos;
        }
        
        public Vector2Int WorldPos2GridPos(Vector3 pos) {
            Vector2 logicPos = pos - transform.position - BaseRect.min.ToVec3();
            return new Vector2Int(Mathf.FloorToInt(logicPos.x), Mathf.FloorToInt(logicPos.y));
        }

        public Vector2Int WorldPos2SafeGridPos(Vector3 pos) {
            Vector2Int logicPos = WorldPos2GridPos(pos);
            if (IsInBaseMap(logicPos)) {
                return logicPos;
            }

            if (logicPos.x >= BaseSize.x) {
                logicPos.x = BaseSize.x - 1;
            } else if (logicPos.x < 0) {
                logicPos.x = 0;
            }

            if (logicPos.y >= BaseSize.y) {
                logicPos.y = BaseSize.y - 1;
            } else if (logicPos.y < 0) {
                logicPos.y = 0;
            }

            return logicPos;
        }

        public Vector3 GridPos2WorldPos(Vector2Int pos) {
            return pos.ToVec3() + transform.position + BaseRect.min.ToVec3();
        }

        public Vector3 GridPos2WorldPosWithOffset(Vector2Int pos, Vector3 offset) {
            return pos.ToVec3() + transform.position + BaseRect.min.ToVec3() + offset;
        }

        public Vector3 GridPos2LocalPos(Vector2Int pos) {
            return GridPos2WorldPos(pos) - transform.parent.position;
        }

        public Vector2Int LocalPos2GridPos(Vector3 pos) {
            var worldPos = pos + transform.parent.position;
            return WorldPos2GridPos(worldPos);
        }

        public bool IsInBaseMap(Vector2Int logicPos) {
            return logicPos.x < BaseSize.x && logicPos.x >= 0 && logicPos.y < BaseSize.y && logicPos.y >= 0;
        }

        public bool IsInBaseMap(RectInt rectInt) {
            return rectInt.xMax <= BaseSize.x && rectInt.yMax <= BaseSize.y;
        }
        #endregion
        
#if UNITY_EDITOR
        #region Debug相关
        private bool _showDebugGizmos;

        [Button]
        public void DebugGizmosSwitch() => _showDebugGizmos = !_showDebugGizmos;
        private void OnDrawGizmos() {
            if (!_showDebugGizmos) {
                return;
            }

            Vector2Int mapSize = new Vector2Int(ObjectMap.GetLength(0), ObjectMap.GetLength(1));
            for (int x = 0; x < mapSize.x; x++) {
                for (int y = 0; y < mapSize.y; y++) {
                    var obj = ObjectMap[x, y];
                    Vector3 offsetZ;
                    Gizmos.color = Color.clear;
                    if (obj == null) {
                        if (ProhibitedZone.Points.Contains(new Vector2Int(x, y))) {
                            Gizmos.color = new Color(0,0,0,0.5f);
                            offsetZ = Vector3.forward * -1;
                        } else {
                            Gizmos.color = new Color(0, 1, 0, 0.3f);
                            offsetZ = Vector3.zero;
                        }
                    } else {
                        if (obj.GridData.isPassable) {
                            Gizmos.color = new Color(0,0,1,0.5f);
                            offsetZ = Vector3.forward;
                        } else if(obj.GridData.isStatic) {
                            Gizmos.color = new Color(1,0,0,0.5f);
                            offsetZ = Vector3.forward * 2;
                        } else {
                            Gizmos.color = new Color(1,1,0,0.5f);
                            offsetZ = Vector3.forward * 3;
                        }
                    }

                    if (Gizmos.color == Color.clear) {
                        continue;
                    }
                    
                    Vector3 worldPos = GridPos2WorldPos(new Vector2Int(x, y)) + Vector3.one / 2;
                    Gizmos.DrawCube(worldPos + offsetZ, Vector3.one);
                    Gizmos.DrawWireCube(worldPos + offsetZ, Vector3.one);
                }
            }
        }
        #endregion
#endif
    }
}