using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTextureUtils
{
    /// <summary>
    /// 根据一个代表形状的网格数据，创建高分辨率的遮罩贴图。
    /// </summary>
    public static Texture2D CreateMaskFromShape(int[,] shapeData, int pixelsPerCell)
    {

        int gridWidth = shapeData.GetLength(1);
        int gridHeight = shapeData.GetLength(0);


        int texWidth = gridWidth * pixelsPerCell;
        int texHeight = gridHeight * pixelsPerCell;

        Texture2D texture = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
        // 防止边缘取样穿帮：使用点采样并钳制边界
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color32[] colorMap = new Color32[texWidth * texHeight];
        Color32 colorWhite = Color.white;
        Color32 colorBlack = Color.black;

        for (int p_y = 0; p_y < texHeight; p_y++)
        {
            for (int p_x = 0; p_x < texWidth; p_x++)
            {
                int gridX = p_x / pixelsPerCell;
                int gridY = p_y / pixelsPerCell;

                // 修正垂直翻转问题
                int flippedGridY = (gridHeight - 1) - gridY;

                int gridValue = shapeData[flippedGridY, gridX];
                Color32 color = (gridValue == 1) ? colorWhite : colorBlack;
                colorMap[p_y * texWidth + p_x] = color;
            }
        }

        texture.SetPixels32(colorMap);
        texture.Apply(false, false);

        return texture;
    }
    /// <summary>
    /// 对遮罩进行扩张 (Dilation)，使白色区域“加粗”。
    /// 外部边界向外移动，内部空洞/岛屿会变小。
    /// </summary>
    public static Texture2D DilateMask(Texture2D source, int iterations)
    {
        return Process(source, iterations, true);
    }
    // 统一的处理函数
    private static Texture2D Process(Texture2D source, int iterations, bool isDilate)
    {
        Texture2D result = source;
        for (int i = 0; i < iterations; i++)
        {
            Texture2D temp = result;
            result = ProcessSingle(temp, isDilate);
            Object.Destroy(temp);
        }
        return result;
    }

    // 单次处理的核心逻辑 (这个版本更健壮)
    private static Texture2D ProcessSingle(Texture2D source, bool isDilate)
    {
        int width = source.width;
        int height = source.height;
        Color32[] srcPixels = source.GetPixels32();
        Color32[] dstPixels = new Color32[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                bool isSrcWhite = srcPixels[index].r > 128;

                bool applyChange = false;
                if (isDilate)
                { // 扩张
                    // 如果当前是黑的，且有任何一个邻居是白的，就变白
                    if (!isSrcWhite && IsAnyNeighbor(srcPixels, x, y, width, height, true))
                    {
                        applyChange = true;
                    }
                }
                else
                { // 腐蚀
                    // 如果当前是白的，且有任何一个邻居是黑的，就变黑
                    if (isSrcWhite && IsAnyNeighbor(srcPixels, x, y, width, height, false))
                    {
                        applyChange = true;
                    }
                }

                if (applyChange)
                {
                    dstPixels[index] = isDilate ? Color.white : Color.black;
                }
                else
                {
                    dstPixels[index] = srcPixels[index];
                }
            }
        }

        Texture2D result = new Texture2D(width, height, TextureFormat.RGB24, false);
        result.filterMode = FilterMode.Point;
        result.wrapMode = TextureWrapMode.Clamp;
        result.SetPixels32(dstPixels);
        result.Apply(false, false);
        return result;
    }
    // 检查邻居的统一辅助方法
    private static bool IsAnyNeighbor(Color32[] pixels, int x, int y, int w, int h, bool checkForWhite)
    {
        byte checkValue = (byte)(checkForWhite ? 255 : 0);
        for (int j = -1; j <= 1; j++)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0 && j == 0) continue;
                int nx = x + i;
                int ny = y + j;
                if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                {
                    bool condition = checkForWhite ? (pixels[ny * w + nx].r > 128) : (pixels[ny * w + nx].r < 128);
                    if (condition) return true;
                }
            }
        }
        return false;
    }
}
