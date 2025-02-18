using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework
{
    public static class CollectionExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        public static void AddRange<T>(this IList<T> source, IEnumerable<T> enumerable)
        {
            foreach (var e in enumerable)
            {
                source.Add(e);
            }
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
    }
}
