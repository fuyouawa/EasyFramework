using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class TextureExtensions
    {
        public static Sprite CreateSprite(this Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        public static Texture2D[,] SliceByCount([NotNull] this Texture2D texture, int rows, int columns)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            int sliceWidth = texture.width / columns;
            int sliceHeight = texture.height / rows;

            Texture2D[,] slices = new Texture2D[rows, columns];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    slices[y, x] = CreateSlice(texture, x, y, sliceWidth, sliceHeight);
                }
            }

            return slices;
        }

        private static Texture2D CreateSlice(Texture2D sourceTexture, int columnIndex, int rowIndex, int sliceWidth, int sliceHeight)
        {
            Texture2D slice = new Texture2D(sliceWidth, sliceHeight);

            var pixels = sourceTexture.GetPixels(
                columnIndex * sliceWidth,
                sourceTexture.height - (rowIndex + 1) * sliceHeight,
                sliceWidth,
                sliceHeight
            );

            slice.SetPixels(pixels);
            slice.Apply();
            return slice;
        }
    }
}
