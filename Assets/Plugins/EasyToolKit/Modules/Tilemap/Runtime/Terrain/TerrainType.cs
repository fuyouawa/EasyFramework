namespace EasyToolKit.Tilemap
{
    public enum TerrainType
    {
        /// <summary>
        /// 左下部分的内转角地形
        /// </summary>
        BottomLeftInteriorCorner,

        /// <summary>
        /// 右下部分的内转角地形
        /// </summary>
        BottomRightInteriorCorner,

        /// <summary>
        /// 左上部分的内转角地形
        /// </summary>
        TopRightInteriorCorner,

        /// <summary>
        /// 右上部分的内转角地形
        /// </summary>
        TopLeftInteriorCorner,

        /// <summary>
        /// 左下部分的外转角地形
        /// </summary>
        BottomRightExteriorCorner,

        /// <summary>
        /// 右下部分的外转角地形
        /// </summary>
        BottomLeftExteriorCorner,

        /// <summary>
        /// 右上部分的外转角地形
        /// </summary>
        TopRightExteriorCorner,

        /// <summary>
        /// 左上部分的外转角地形
        /// </summary>
        TopLeftExteriorCorner,

        /// <summary>
        /// 右边缘地形
        /// </summary>
        RightEdge,

        /// <summary>
        /// 下边缘地形
        /// </summary>
        BottomEdge,

        /// <summary>
        /// 左边缘地形
        /// </summary>
        LeftEdge,

        /// <summary>
        /// 上边缘地形
        /// </summary>
        TopEdge,

        /// <summary>
        /// 填充地形
        /// </summary>
        Fill
    }
}
