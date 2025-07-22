using UnityEngine;

namespace EasyToolKit.Core
{
    public static class SpriteExtensions
    {
        public static Sprite[,] SliceByCount(this Sprite sprite, int rows, int columns)
        {
            // 存储分割出来的 Sprite 数组
            Sprite[,] subSprites = new Sprite[rows, columns];

            // 循环分割
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    subSprites[row, column] = CreateSlice(sprite, column, row, columns, rows);
                }
            }

            return subSprites;
        }
        
        private static Sprite CreateSlice(Sprite sourceSprite, int columnIndex, int rowIndex, int totalColumns, int totalRows)
        {
            Texture2D texture = sourceSprite.texture;
            Rect textureRect = sourceSprite.textureRect;
            
            float spriteWidth = textureRect.width / totalColumns;
            float spriteHeight = textureRect.height / totalRows;
            
            // 计算每个子 Sprite 的区域
            float x = textureRect.x + columnIndex * spriteWidth;
            float y = textureRect.y + (totalRows - rowIndex - 1) * spriteHeight; // 修正Y坐标以匹配Unity的坐标系
            
            Rect subSpriteRect = new Rect(x, y, spriteWidth, spriteHeight);
            
            // 创建每个小 Sprite
            Sprite slice = Sprite.Create(texture, subSpriteRect, new Vector2(0.5f, 0.5f));
            slice.name = $"{sourceSprite.name}_{rowIndex}_{columnIndex}";
            
            return slice;
        }
    }
}
