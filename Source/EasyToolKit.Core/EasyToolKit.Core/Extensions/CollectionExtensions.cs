using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    public static class CollectionExtensions
    {
        public static bool AllSame<T>(this IEnumerable<T> enumerator)
        {
            return enumerator.AllSame((a, b) => EqualityComparer<T>.Default.Equals(a, b));
        }

        public static bool AllSame<T>(this IEnumerable<T> enumerator, Func<T, T, bool> comparison)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            T fst = default;
            bool hasFst = false;
            foreach (var value in enumerator)
            {
                if (!hasFst)
                {
                    fst = value;
                    hasFst = true;
                }
                else
                {
                    if (!comparison(fst, value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> callback)
        {
            foreach (var elem in enumerator)
            {
                callback(elem);
            }
        }

        public static bool HasDuplicate<T, E>(this IEnumerable<T> enumerator, Func<T, E> selector)
        {
            var set = new HashSet<E>();

            foreach (var e in enumerator)
            {
                if (!set.Add(selector(e)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasDuplicate<T>(this IEnumerable<T> enumerator)
        {
            return enumerator.HasDuplicate(e => e);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        public static T[,] To2dArray<T>(this IList<T> source, int rows, int columns)
        {
            if (source.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException("Source length does not match the specified rows and columns.");
            }

            var result = new T[rows, columns];

            // 填充二维数组
            for (int i = 0; i < source.Count; i++)
            {
                int row = i / columns; // 计算当前行
                int col = i % columns; // 计算当前列
                result[row, col] = source[i]; // 将元素放入二维数组
            }

            return result;
        }

        public static T[][] ToJaggedArray<T>(this IList<T> source, int rows, int columns)
        {
            if (source.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException("Source length does not match the specified rows and columns.");
            }

            var result = new T[rows][];

            // 为每一行创建一个长度为 columns 的数组
            for (int row = 0; row < rows; row++)
            {
                result[row] = new T[columns];
            }

            // 填充 jagged array
            for (int i = 0; i < source.Count; i++)
            {
                int row = i / columns; // 计算当前行
                int col = i % columns; // 计算当前列
                result[row][col] = source[i]; // 将元素放入 jagged array
            }

            return result;
        }

        public static T[] Kick<T>(this T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            // 创建新的数组
            T[] newArray = new T[array.Length - 1];

            // 复制原数组中的元素到新数组
            Array.Copy(array, 0, newArray, 0, index);  // 复制索引之前的元素
            Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);

            return newArray;
        }
    }
}
