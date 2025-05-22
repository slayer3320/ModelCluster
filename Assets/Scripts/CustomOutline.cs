using UnityEngine;
using System.Collections;


[System.Serializable]
public class CustomOutline : MonoBehaviour 
{
    public Color OutlineColor = Color.yellow;
    public float OutlineWidth = 5f;
    public Mode OutlineMode = Mode.OutlineVisible;

    public enum Mode 
    {
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette
    }

    private Material outlineMaterial;
    private Renderer renderer;

    void Start() 
    {
        renderer = GetComponent<Renderer>();
        CreateOutlineMaterial();
        UpdateOutline();
    }

    void CreateOutlineMaterial() 
    {
        outlineMaterial = new Material(Shader.Find("Standard"));
        outlineMaterial.SetColor("_Color", OutlineColor);
        outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
    }

    void UpdateOutline() 
    {
        if (renderer != null && outlineMaterial != null) 
        {
            var materials = renderer.sharedMaterials;
            if (OutlineMode == Mode.OutlineVisible) 
            {
                // 添加轮廓材质作为额外材质
                var newMaterials = new Material[materials.Length + 1];
                materials.CopyTo(newMaterials, 0);
                newMaterials[materials.Length] = outlineMaterial;
                renderer.sharedMaterials = newMaterials;
            }
            else 
            {
                renderer.sharedMaterials = materials;
            }
        }
    }

    void OnValidate() 
    {
        if (outlineMaterial != null) 
        {
            outlineMaterial.SetColor("_Color", OutlineColor);
            outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
            UpdateOutline();
        }
    }
}

