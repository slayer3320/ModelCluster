using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class Triangle : Graphic
{
    [Tooltip("1: 等边倒三角形（顶点向下）; 2: 等边向右三角形（顶点向右）")]
    public int type = 1;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float w = rectTransform.rect.width;
        float h = rectTransform.rect.height;

        // 计算等边三角形高度
        float equilateralHeight = Mathf.Sqrt(3) / 2 * w;

        if (type == 1)
        {
            // 等边倒三角形，顶点向下，pivot在中心
            // 三角形中心垂直居中
            // 顶点
            Vector2 bottom = new Vector2(0, -equilateralHeight / 3);
            // 底边左右两点
            Vector2 left = new Vector2(-w / 2, equilateralHeight * 2 / 3);
            Vector2 right = new Vector2(w / 2, equilateralHeight * 2 / 3);

            vh.AddVert(left, color, new Vector2(0, 1));
            vh.AddVert(right, color, new Vector2(1, 1));
            vh.AddVert(bottom, color, new Vector2(0.5f, 0));

            vh.AddTriangle(0, 1, 2);
        }
        else if (type == 2)
        {
            // 等边向右三角形，顶点向右，pivot在中心
            // 右顶点
            Vector2 right = new Vector2(equilateralHeight * 2 / 3, 0);
            // 左上，左下两点
            Vector2 leftTop = new Vector2(-equilateralHeight / 3, w / 2);
            Vector2 leftBottom = new Vector2(-equilateralHeight / 3, -w / 2);

            vh.AddVert(leftTop, color, new Vector2(0, 1));
            vh.AddVert(leftBottom, color, new Vector2(0, 0));
            vh.AddVert(right, color, new Vector2(1, 0.5f));

            vh.AddTriangle(0, 1, 2);
        }
        else
        {
            // 默认倒三角
            Vector2 bottom = new Vector2(0, -equilateralHeight / 3);
            Vector2 left = new Vector2(-w / 2, equilateralHeight * 2 / 3);
            Vector2 right = new Vector2(w / 2, equilateralHeight * 2 / 3);

            vh.AddVert(left, color, new Vector2(0, 1));
            vh.AddVert(right, color, new Vector2(1, 1));
            vh.AddVert(bottom, color, new Vector2(0.5f, 0));

            vh.AddTriangle(0, 1, 2);
        }
    }
}
