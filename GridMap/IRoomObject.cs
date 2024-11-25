using UnityEngine;
using System;
using System.Collections.Generic;

namespace GridMapToolset.Map {
    /// <summary>
    /// 网格物体大小
    /// </summary>
    [System.Serializable]
    public class ObjectGridSize {
        public Vector2Int size;

        public ObjectGridSize(int width = 1, int height = 1, 
            int topBoundary = 0, int bottomBoundary = 0, int leftBoundary = 0, int rightBoundary = 0) {
            if (width <= 0) {
                throw new ArgumentException("Width must be greater than 0.", nameof(width));
            }
            if (height <= 0) {
                throw new ArgumentException("Height must be greater than 0.", nameof(height));
            }
            size = new Vector2Int(width, height);
        }

        public ObjectGridSize(Vector2Int size, 
            int topBoundary = 0, int bottomBoundary = 0, int leftBoundary = 0, int rightBoundary = 0) 
            : this(size.x, size.y, topBoundary, bottomBoundary, leftBoundary, rightBoundary) {
        }
        
        // 计算可达性路径时使用,锚点为左下
        public RectInt GetRectInt(Vector2Int pos) {
            return new RectInt(pos, size);
        }
    }

    /// <summary>
    /// 物体网格数据
    /// </summary>
    [System.Serializable]
    public class ObjectGridData {
        public bool isPassable;
        public bool isStatic;
        public ObjectGridSize gridSize;
        public Vector2Int gridPos;
        public RectInt GridRect => gridSize.GetRectInt(gridPos);

        public ObjectGridData(ObjectGridSize size) {
            gridSize = size;
            gridPos = Vector2Int.zero;
            isPassable = false;
            isStatic = false;
        }

        public void SetPos(Vector2Int pos) => gridPos = pos;
    }

    [System.Serializable]
    public class ObjectLayoutOffset {
        public Vector3 offset;
        public Vector2 pivot;
    }
    
    /// <summary>
    /// 房间物体接口
    /// </summary>
    public interface IRoomObject {
        public GameObject Go { get; }
        public RoomGrid OwnerRoomGrid { get; set; }
        public ObjectGridSize GridSize { get; }
        public ObjectLayoutOffset LayoutOffset { get; }
    }
    
    /// <summary>
    /// 占位式房间物体接口
    /// </summary>
    public interface IOccupiedRoomObject : IRoomObject {
        public ObjectGridData GridData { get; }
        public void OnAddedToRoomGrid();
        public void OnRemoveFormRoomGrid(RoomGrid roomGrid);
        public void InitObject(Dictionary<string, string> parameterDict);
    }

    public class RoomObjectPlacementInfo : IRoomObject {
        public readonly GameObject Prefab;
        public bool CanBePlaced;
        public Vector2Int GridPos;
        public Vector3 WordPos => OwnerRoomGrid.GetObjectWorldLayoutPos(this, GridPos);
        public Vector3 LocalPos => OwnerRoomGrid.GetObjectLocalLayoutPos(this, GridPos);
        public GameObject Go => Prefab;
        public RoomGrid OwnerRoomGrid { get; set; }
        private readonly IRoomObject _roomObjectComponent;
        public ObjectGridSize GridSize => _roomObjectComponent.GridSize;
        public ObjectLayoutOffset LayoutOffset => _roomObjectComponent.LayoutOffset;

        public RoomObjectPlacementInfo(GameObject prefab) {
            Prefab = prefab;
            _roomObjectComponent = prefab.GetComponent<IRoomObject>();
            if (_roomObjectComponent == null) {
                Debug.LogError("Room object has no IRoomObject component.");
            }
        }

        public void SetPos(RoomGrid ownerRoomGrid, Vector2Int gridPos) {
            OwnerRoomGrid = ownerRoomGrid;
            GridPos = gridPos;
            CanBePlaced = true;
        }
    }

    public class DefaultOccupiedRoomObject : IOccupiedRoomObject {
        public GameObject Go => GameObject;
        public GameObject GameObject;
        public RoomGrid OwnerRoomGrid { get; set; }
        public ObjectGridSize GridSize => GridData.gridSize;
        public ObjectGridData GridData => _logicGridData;

        public ObjectLayoutOffset LayoutOffset => new ObjectLayoutOffset {
            offset = new Vector2(0.5f, 0),
            pivot = Vector2.zero
        };

        private ObjectGridData _logicGridData = new ObjectGridData(new ObjectGridSize()) {
            isPassable = false,
            isStatic = true
        };

        public DefaultOccupiedRoomObject(GameObject go) {
            GameObject = go;
        }

        public void OnAddedToRoomGrid() { }

        public void OnRemoveFormRoomGrid(RoomGrid roomGrid) { }
        public void InitObject(Dictionary<string, string> parameterDict) { }
    }
}