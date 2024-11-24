using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridMapToolset.Util {
public class RectGridPassableMap {
        public readonly bool[,] PassableMap;
        public bool CanDiagonallyPassByObstacle;
        public readonly int Width;
        public readonly int Height;
        public readonly Vector2Int Size;
        public readonly int Area;

        public RectGridPassableMap(bool[,] passableMap, bool canDiagonallyPassByObstacle = false) {
            PassableMap = passableMap;
            CanDiagonallyPassByObstacle = canDiagonallyPassByObstacle;
            Width = PassableMap.GetLength(0);
            Height = PassableMap.GetLength(1);
            Size = new Vector2Int(Width, Height);
            Area = Width * Height;
        }
        
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        
        public bool IsPassable(int x, int y, bool checkEdge = true) {
            if (checkEdge) {
                return x >= 0 && x < Width && y >= 0 && y < Height && PassableMap[x, y];
            }
            return x < 0 || x >= Width || y < 0 || y >= Height || PassableMap[x, y];
        }

        public bool CanMoveTo(int x, int y, Vector2Int dir) => CanMoveTo(x, y, dir.x, dir.y);
        public bool CanMoveTo(int x, int y, int dx, int dy) {
            int npx = x + dx;
            int npy = y + dy;
            if(!IsPassable(npx, npy)) return false;
            if (dx == 0 || dy == 0) {
                return true;
            }
            
            bool hPassable = IsPassable(npx, y);
            bool vPassable = IsPassable(x, npy);
            if(!hPassable && !vPassable) return false;
            if (!CanDiagonallyPassByObstacle && (!hPassable || !vPassable)) {
                return false;
            }
            return true;
        }

        public void UpdateMap(RectInt bounds, bool passable) {
            for (int x = bounds.xMin; x < bounds.xMax; x++) {
                for (int y = bounds.yMin; y < bounds.yMax; y++) {
                    PassableMap[x, y] = passable;
                }
            }
        }
        
        // Bresenham's Line Algorithm（布雷森汉姆线段算法）
        public bool IsLineOfSight(Vector2Int start, Vector2Int end) {
            int x0 = start.x;
            int y0 = start.y;
            int x1 = end.x;
            int y1 = end.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true) {
                // 如果当前格子不可通过，返回 false
                if (!IsPassable(x0, y0)) return false;
                // 如果到达目标节点，返回 true
                if (x0 == x1 && y0 == y1) return true;

                int cdx = 0, cdy = 0;
                int e2 = 2 * err;
                if (e2 > -dy) {
                    err -= dy;
                    cdx = sx;
                }
                if (e2 < dx) {
                    err += dx;
                    cdy = sy;
                }
                if (!CanMoveTo(x0, y0, cdx, cdy)) return false;
                x0 += cdx;
                y0 += cdy;
            }
        }
        
        public override string ToString() {
            return $"SourceMap> Width: {Width},Height: {Height},\tCanDiagonallyPassByObstacle: {CanDiagonallyPassByObstacle}";
        }
        
        public enum Directions { Up, Down, Left, Right, LeftUp, RightUp, LeftDown, RightDown }
        public static readonly Dictionary<Directions, Vector2Int> Direction8Dict = new() {
            { Directions.Up, Vector2Int.up },
            { Directions.Down, Vector2Int.down },
            { Directions.Left, Vector2Int.left },
            { Directions.Right, Vector2Int.right },
            { Directions.LeftUp, new Vector2Int(-1, 1) },
            { Directions.RightUp, new Vector2Int(1, 1) },
            { Directions.LeftDown, new Vector2Int(-1, -1) },
            { Directions.RightDown, new Vector2Int(1, -1) }
        };
        public static readonly Dictionary<Directions, Vector2Int> Direction4Dict = new() {
            { Directions.Up, Vector2Int.up },
            { Directions.Down, Vector2Int.down },
            { Directions.Left, Vector2Int.left },
            { Directions.Right, Vector2Int.right },
        };

        public static bool Is8Neighbor(Vector2Int a, Vector2Int b) => Direction8Dict.Values.Any(dir => b == a + dir);
    }
}