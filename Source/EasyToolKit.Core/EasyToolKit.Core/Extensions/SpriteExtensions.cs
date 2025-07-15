using UnityEngine;

namespace EasyToolKit.Core
{
    public static class SpriteExtensions
    {
        public static Sprite[] SliceByCount(this Sprite sprite, int rows, int columns)
        {
            // 获取原始 Sprite 的纹理
            Texture2D texture = sprite.texture;
            Rect textureRect = sprite.textureRect;

            // 计算每个小 Sprite 的宽度和高度
            float spriteWidth = textureRect.width / columns;
            float spriteHeight = textureRect.height / rows;

            // 存储分割出来的 Sprite 数组
            Sprite[] subSprites = new Sprite[rows * columns];

            // 循环分割
            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // 计算每个子 Sprite 的区域
                    float x = textureRect.x + column * spriteWidth;
                    float y = textureRect.y + row * spriteHeight;

                    Rect subSpriteRect = new Rect(x, y, spriteWidth, spriteHeight);

                    // 创建每个小 Sprite
                    subSprites[index] = Sprite.Create(texture, subSpriteRect, new Vector2(0.5f, 0.5f));
                    subSprites[index].name = $"{sprite.name}_{index}";

                    index++;
                }
            }

            return subSprites;
        }
    }
}
