using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity2DGridMapToolset.Util {
public class BucketPriorityQueue<T> : ICollection<T> where T : BucketPriorityQueue<T>.IBucketPriorityQueueItem {
        public interface IBucketPriorityQueueItem {
            int PriorityValue { get; }
        }

        public bool NeedBestSolution = true;
        private readonly int _bucketSize;// 每个桶包含的优先级范围
        private readonly List<HashSet<T>> _buckets;// 桶集合
        private int _minIndex;// 当前优先级最小的非空桶索引
        private int _maxIndex; // 当前优先级最大的非空桶索引
        public int Count { get; private set; } // 总元素数
        public bool IsReadOnly => false; // 可变集合

        public BucketPriorityQueue(int maxPriorityValue, int bucketSize = 10) {
            _bucketSize = bucketSize;
            int bucketCount = maxPriorityValue / bucketSize + 1;
            _buckets = new List<HashSet<T>>(bucketCount);
            for (int i = 0; i < bucketCount; i++) {
                _buckets.Add(new HashSet<T>());
            }
            _minIndex = -1;
            _maxIndex = -1;
            Count = 0;
        }
        
        public void Add(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }
            
            int index = item.PriorityValue / _bucketSize;
            if (index < 0 || index >= _buckets.Count) {
                Debug.LogError($"PriorityValue {item.PriorityValue} is out of range[0,{_buckets.Count * _bucketSize}].");
                return;
            }
            if (!_buckets[index].Add(item)) {
                Debug.LogError($"PriorityValue {item.PriorityValue} is already added!");
                return;
            }
            Count++;
            if (_minIndex == -1 || index < _minIndex) {
                _minIndex = index; // 更新最小索引
            }
            if (_maxIndex == -1 || index > _maxIndex) {
                _maxIndex = index; // 更新最大索引
            }
        }

        public bool TryAdd(T item) {
            if (item == null) {
                return false;
            }
            
            int index = item.PriorityValue / _bucketSize;
            if (index < 0 || index >= _buckets.Count) {
                return false;
            }

            if (!_buckets[index].Add(item)) return false;
            Count++;
            if (_minIndex == -1 || index < _minIndex) {
                _minIndex = index; // 更新最小索引
            }
            if (_maxIndex == -1 || index > _maxIndex) {
                _maxIndex = index; // 更新最大索引
            }
            return true;
        }
        
        public bool Remove(T item) {
            if (item == null) {
                return false;
            }
            int index = item.PriorityValue / _bucketSize;
            if (index < 0 || index >= _buckets.Count) {
                Debug.LogError($"PriorityValue {item.PriorityValue} is out of range[0,{_buckets.Count * _bucketSize}].");
                return false;
            }
            if (!_buckets[index].Remove(item)) return false;
            
            Count--;
            if (_buckets[index].Count == 0 && _minIndex == index) {
                _minIndex = _buckets.FindIndex(b => b.Count > 0); // 更新最小索引
                _maxIndex = _buckets.FindLastIndex(b => b.Count > 0);
            }
            return true;
        }
        
        public T DequeueMin() {
            if (_minIndex == -1 || Count == 0) {
                throw new InvalidOperationException("The queue is empty.");
            }

            var bucket = _buckets[_minIndex];
            T item = default;
            if (NeedBestSolution) {
                foreach (var x in bucket) {
                    if (item == null || x.PriorityValue < item.PriorityValue) {
                        item = x;
                    }
                }
            } else {
                item = bucket.First();
            }
            bucket.Remove(item);
            Count--;

            if (bucket.Count == 0) {
                _minIndex = _buckets.FindIndex(b => b.Count > 0); // 更新最小索引
            }

            return item;
        }
        
        public T DequeueMax() {
            if (_maxIndex == -1 || Count == 0) {
                throw new InvalidOperationException("The queue is empty.");
            }

            var bucket = _buckets[_maxIndex];
            T item = default;
            if (NeedBestSolution) {
                foreach (var x in bucket) {
                    if (item == null || x.PriorityValue > item.PriorityValue) {
                        item = x;
                    }
                }
            } else {
                item = bucket.First();
            }
            bucket.Remove(item);
            Count--;

            if (bucket.Count == 0) {
                _maxIndex = _buckets.FindLastIndex(b => b.Count > 0); // 更新最小索引
            }

            return item;
        }
        
        public bool Contains(T item) {
            if (item == null) {
                return false;
            }
            int index = item.PriorityValue / _bucketSize;
            if (index < 0 || index >= _buckets.Count) {
                return false;
            }
            return _buckets[index].Contains(item);
        }
        
        public void Clear() {
            foreach (var bucket in _buckets) {
                bucket.Clear();
            }
            _minIndex = -1;
            _maxIndex = -1;
            Count = 0;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() {
            foreach (var bucket in _buckets) {
                foreach (var item in bucket) {
                    yield return item;
                }
            }
        }
        
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The target array is too small.");

            foreach (var bucket in _buckets) {
                foreach (var item in bucket) {
                    array[arrayIndex++] = item;
                }
            }
        }
    }
}