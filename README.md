# Unity2DGridMapToolset

# English

**UnityGridMapToolset2D** is a powerful modular toolkit designed for Unity2D projects, offering a variety of **pathfinding algorithms**, **maze generation**, **2D tile management**, and **grid map tools** to simplify grid-based game mechanics.

------

## Features ✨

### **1. GridMap**

**Description:** Manages grid-based maps, including map creation, manipulation, and data storage.

**Highlights:**

- Supports dynamic grid obstacle placement.
- Provides data serialization and storage for saving and loading.
- Extensibility: Supports different types of grid data structures.

------

### **2. MazeGenerate**

**Description:** Implements various maze generation algorithms.

**Supported Algorithms:**

- Depth-First Algorithm
- Randomized Prim's Algorithm (in development)

------

### **3. PathFinding**

**Description:** Offers grid-based pathfinding solutions.

**Supported Algorithms:**

- **A\***: The classic and efficient pathfinding algorithm.
- **Jump Point Search (JPS/JPS+)**: Optimized for faster path calculations.
- **Breadth-First Search (BFS)**: Comprehensive but computationally expensive.
- **Depth-First Search (DFS)**: Suitable for simple maps.
- **Dijkstra's Algorithm**: A classic method for finding the shortest path.

**Advanced Features:**

- Supports diagonal movement and pass-through detection.
- Provides frame-by-frame processing for complex scenarios.

------

### **4. Tile**

**Description:** Manages tile-based components and behaviors.

**Highlights:**

- Supports dynamic modifications to tile content.
- Tile animation support (in development).

------

### **5. Util**

**Description:** Provides utility functions and optimization modules.

**Highlights:**

- **Frame-Splitting Task Scheduler**: Balances performance for large-scale calculations.
- **Priority Queue**: Optimized for pathfinding algorithms.
- **Hash Buckets with Radix Sort**: Efficiently handles large data sets.

------

## Getting Started 🛠️

### 1. Installation

**Option 1:** Clone this repository and use it directly.

```
bash


复制代码
git clone https://github.com/MycroftCooper/UnityGridMapToolset.git
```

**Option 2:** Download from Unity Asset Store (in development).

------

### 2. Example: Pathfinding

```
csharp


复制代码
// Placeholder: Example code coming soon
```

### 3. Example: Maze Generation

```
csharp


复制代码
// Placeholder: Example code coming soon
```

------

## Technical Documentation 📚

For detailed documentation, visit the [Wiki](https://github.com/MycroftCooper/UnityGridMapToolset2D/wiki).

------

## Roadmap 🗺️

- Expand maze generation methods (e.g., Eller's Algorithm).
- Develop more grid-based tools.
- Extend grid map editor features.
- Add support for hexagonal, triangular, and circular grids.
- Include performance benchmarking and optimization tools.

------

## Contributions 🤝

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

------

## License 📜

This project is licensed under the MIT License. For more details, see the LICENSE.

------

## Acknowledgments 🙏

- Inspired by various resources on pathfinding and maze generation.
- Special thanks to the Unity community for their ongoing support.

------

## Related Libraries

[MycroftToolkit](https://github.com/MycroftCooper/MycroftToolkit): A general-purpose utility library offering optimized foundational support.

------

> Note: Certain debugging features may rely on the Odin Inspector plugin.

# 中文

**UnityGridMapToolset2D** 是一个为 Unity2D 项目设计的强大模块化工具集，提供多种 **寻路算法**、**迷宫生成** 、**2D瓦片**和 **网格地图管理工具**，旨在简化基于网格的游戏机制的实现。

## 功能 ✨

### **1. GridMap**

**功能**: 管理基于网格的地图，包括地图的创建、操作和数据存储。

**特点:**

- 支持网格障碍物的动态设置。
- 提供数据序列化与存储方法，便于存档与加载。
- 可扩展性：支持不同类型的网格数据结构。

------

### **2. MazeGenerate**

**功能**: 实现多种迷宫生成算法。

**支持的算法:**

- 深度优先算法
- 随机 Prim 算法（待开发）

------

### **3. PathFinding**

**功能**: 提供网格地图中的寻路解决方案。

**支持的算法:**

- **A\*（A星）算法**：经典的高效寻路算法。
- **跳点寻路（JPS/JPS+）**：优化路径计算性能。
- **广度优先搜索（BFS）**：全面但性能开销大的算法。
- **深度优先搜索（DFS）**：适合简单地图。
- **Dijkstra 算法**：寻找最短路径的经典方法。

**高级功能:**

- 支持对角线穿透检测。
- 提供分帧处理，适配复杂场景。

------

### **4. Tile**

**功能**: 管理瓦片数据和行为。

**特点:**

- 支持动态修改瓦片内容。
- 提供瓦片动画支持（开发中）。

------

### **5. Util**

**功能**: 提供通用工具函数和优化模块。

**特点:**

- **分帧任务调度器**：平衡性能，适合大规模运算。
- **优先级队列**：用于优化寻路算法。
- **带基数排序的哈希桶**：高效处理海量数据。

------

## 快速开始 🛠️

### 1. 安装

**方法 1:** 克隆本仓库直接使用

```
bash


复制代码
git clone https://github.com/MycroftCooper/UnityGridMapToolset.git
```

**方法 2**: Unity 资源商店中下载（开发中）

------

### 

### 2. 寻路示例

```
// 待填充
```

### 3. 迷宫生成示例

```
// 待填充
```

------

## 技术细节文档 📚

完整文档请参考 [Wiki](https://github.com/MycroftCooper/UnityGridMapToolset2D/wiki)。

------

## 开发路线 🗺️

-  扩展迷宫生成方法（如 Eller's 算法）
-  扩展更多地图网格化工具
-  扩展网格地图编辑器
-  扩展六边形，三角形，圆形等其他网格化地图
-  性能基准测试和优化工具

------

## 贡献 🤝

欢迎贡献代码！请查看 [CONTRIBUTING.md](CONTRIBUTING.md) 获取相关指南。

------

## 许可 📜

本项目基于 MIT 许可协议。详细信息请参考 LICENSE。

------

## 致谢 🙏

- 感谢多种寻路算法与迷宫生成资源的启发。
- 特别感谢 Unity 社区的持续支持。

## 其他库

[MycroftToolkit](https://github.com/MycroftCooper/MycroftToolkit): 通用工具库，提供优化的基础功能支持。



> 部分debug功能会依赖odin插件
