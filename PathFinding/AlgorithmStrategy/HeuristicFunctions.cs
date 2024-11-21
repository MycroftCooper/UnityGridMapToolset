using System;
using UnityEngine;

namespace GridMapToolset2D.PathFinding {
    public static class HeuristicFunctions {
        public class CommonHeuristicFunction : HeuristicFunctionBase {
            public CommonHeuristicFunction(HeuristicTypes types) {
                HeuristicType = types;
            }
            public override int CalculateHeuristic(int aX, int aY, int bX, int bY) {
                return HeuristicFunctions.CalculateHeuristic(HeuristicType, aX, aY, bX, bY);
            }
        }

        public const int DiagonalCost = 2;
        public const int StraightCost = 1;

        public static int CalculateHeuristic(HeuristicTypes type, Vector2Int a, Vector2Int b) =>
            CalculateHeuristic(type, a.x, a.y, b.x, b.y);
        
        public static int CalculateHeuristic(HeuristicTypes type, int aX, int aY, int bX, int bY) {
            switch (type) {
                case HeuristicTypes.Manhattan:
                    return Mathf.Abs(aX - bX) + Mathf.Abs(aY - bY); // 曼哈顿距离
                case HeuristicTypes.Euclidean:
                    return (int)Math.Sqrt(Math.Pow(aX - bX, 2) + Math.Pow(aY - bY, 2));// 欧式
                case HeuristicTypes.SquaredEuclidean:
                    return (aX - bX) * (aX - bX) + (aY - bY) * (aY - bY);// 欧式平方
                case HeuristicTypes.Diagonal:
                    return Math.Max(Math.Abs(aX - bX), Math.Abs(aY - bY));
                case HeuristicTypes.WeightedDiagonal:
                    int deltaX = Math.Abs(aX - bX);
                    int deltaY = Math.Abs(aY - bY);
                    return DiagonalCost * Math.Min(deltaX, deltaY) + StraightCost * Math.Abs(deltaX - deltaY);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}