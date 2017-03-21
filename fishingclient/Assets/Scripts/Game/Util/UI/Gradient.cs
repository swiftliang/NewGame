using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
    [SerializeField]
    [Range(0, 1)]
    private float gradientRelativeY;
    [SerializeField]
    [Range(0, 1)]
    private float gradientRelativeInterval;
    [SerializeField]
    private Color32
        topColor = Color.white;
    [SerializeField]
    private Color32
        bottomColor = Color.black;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }
        int count = vh.currentVertCount;
        UIVertex[] vertexArray = new UIVertex[count];
        if (count > 0)
        {
            vh.PopulateUIVertex(ref vertexArray[0], 0);
            float bottomY = vertexArray[0].position.y;
            float topY = bottomY;
            for (int i = 1; i < count; i++)
            {
                vh.PopulateUIVertex(ref vertexArray[i], i);
                float y = vertexArray[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }
            float uiElementHeight = topY - bottomY;
            float uiGradientY = gradientRelativeY * uiElementHeight;
            float uiGradientInterval = gradientRelativeInterval * uiElementHeight;
            float gradientBottomY = uiGradientY + bottomY - uiGradientInterval / 2;
            if (gradientBottomY < bottomY)
                gradientBottomY = bottomY;
            float gradientTopY = uiGradientY + bottomY + uiGradientInterval / 2;
            if (gradientTopY > topY)
                gradientTopY = topY;
            for (int i = 0; i < count; i++)
            {
                if (vertexArray[i].position.y > gradientTopY)
                    vertexArray[i].color = topColor;
                else if (vertexArray[i].position.y < gradientBottomY)
                    vertexArray[i].color = bottomColor;
                else
                {
                    vertexArray[i].color = Color32.Lerp(bottomColor, topColor, (vertexArray[i].position.y - gradientBottomY) / uiGradientInterval);
                }
                vh.SetUIVertex(vertexArray[i], i);
            }
        }
    }
}
