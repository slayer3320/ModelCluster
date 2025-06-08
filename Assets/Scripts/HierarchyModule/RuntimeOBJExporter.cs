using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class RuntimeOBJExporter : MonoBehaviour
{
    public static RuntimeOBJExporter instance;
    void Awake()
    {
        instance = this;
    }

    public bool onlySelectedObjects = true;
    public bool applyPosition = true;
    public bool applyRotation = true;
    public bool applyScale = true;
    public bool generateMaterials = true;
    public bool exportTextures = true;
    public bool splitObjects = true;
    public bool objNameAddIdNum = false;
    private string lastExportFolder;

    public void ExportGameObjectsToOBJ(GameObject[] objects, GameObject mainObj, string exportPath)
    {
        lastExportFolder = Path.GetDirectoryName(exportPath);

        List<MeshFilter> sceneMeshes = new List<MeshFilter>();

        if (onlySelectedObjects)
        {
            foreach (GameObject g in objects)
            {
                MeshFilter f = g.GetComponent<MeshFilter>();
                if (f != null && f.sharedMesh != null)
                {
                    sceneMeshes.Add(f);
                }
            }
        }
        else
        {
            MeshFilter[] allFilters = FindObjectsOfType<MeshFilter>();
            foreach (MeshFilter f in allFilters)
            {
                if (f.sharedMesh != null) sceneMeshes.Add(f);
            }
        }

        StringBuilder sb = new StringBuilder();
        StringBuilder sbMaterials = new StringBuilder();

        if (generateMaterials)
        {
            sb.AppendLine("mtllib " + Path.GetFileNameWithoutExtension(exportPath) + ".mtl");
        }

        Dictionary<string, bool> materialCache = new Dictionary<string, bool>();
        int globalVertexIndex = 1;

        foreach (MeshFilter mf in sceneMeshes)
        {
            if (mf == null || mf.sharedMesh == null) continue;

            if (mf.gameObject.transform.parent != null)
            {
                sb.AppendLine($"# name:{mf.gameObject.name} parent:{mf.gameObject.transform.parent.name}");
            }
            else
            {
                sb.AppendLine($"# name:{mf.gameObject.name} parent:null");
            }



            MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
            if (mr != null && generateMaterials)
            {
                Material[] mats = mr.sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    Material m = mats[j];
                    if (m == null) continue;

                    if (!materialCache.ContainsKey(m.name))
                    {
                        materialCache[m.name] = true;
                        sbMaterials.Append(MaterialToString(m));
                        sbMaterials.AppendLine();
                    }
                }
            }

            Mesh msh = mf.sharedMesh;
            Matrix4x4 matrix = mf.transform.localToWorldMatrix;
            Matrix4x4 normalMatrix = matrix.inverse.transpose;

            // 顶点处理
            foreach (Vector3 vertex in msh.vertices)
            {
                Vector3 v = matrix.MultiplyPoint(vertex);
                v.x *= -1; // 翻转X轴匹配坐标系
                sb.AppendLine($"v {v.x} {v.y} {v.z}");
            }

            // 法线处理 - 修复法线方向问题
            if (msh.normals != null && msh.normals.Length > 0)
            {
                foreach (Vector3 normal in msh.normals)
                {
                    // 使用逆转置矩阵处理法线
                    Vector3 n = normalMatrix.MultiplyVector(normal).normalized;
                    n.x *= -1; // 翻转X轴匹配坐标系
                    sb.AppendLine($"vn {n.x} {n.y} {n.z}");
                }
            }

            // UV处理
            if (msh.uv != null && msh.uv.Length > 0)
            {
                foreach (Vector2 uv in msh.uv)
                {
                    sb.AppendLine($"vt {uv.x} {uv.y}");
                }
            }

            // 子网格处理
            for (int submesh = 0; submesh < msh.subMeshCount; submesh++)
            {
                int[] triangles = msh.GetTriangles(submesh);
                if (triangles.Length == 0) continue;

                // 应用材质组
                if (mr != null && submesh < mr.sharedMaterials.Length && mr.sharedMaterials[submesh] != null)
                {
                    sb.AppendLine("usemtl " + mr.sharedMaterials[submesh].name);
                }

                string meshName = mf.gameObject.name;
                if (splitObjects)
                {
                    string exportName = meshName;
                    if (objNameAddIdNum)
                    {
                        exportName += "_" + mf.gameObject.GetInstanceID();
                    }
                    sb.AppendLine("g " + exportName);
                }

                // 面处理 - 修正顶点顺序
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    // 修正顶点顺序以保持正确的法线方向
                    int idx0 = globalVertexIndex + triangles[i];
                    int idx1 = globalVertexIndex + triangles[i + 2]; // 交换顶点顺序
                    int idx2 = globalVertexIndex + triangles[i + 1];

                    bool hasNormals = msh.normals != null && msh.normals.Length > 0;
                    bool hasUV = msh.uv != null && msh.uv.Length > 0;

                    sb.AppendLine("f " +
                        $"{ConstructOBJString(idx0, hasUV, hasNormals)} " +
                        $"{ConstructOBJString(idx1, hasUV, hasNormals)} " +
                        $"{ConstructOBJString(idx2, hasUV, hasNormals)}");
                }
            }

            globalVertexIndex += msh.vertexCount;
        }

        mainObj.GetComponentsInChildren<Transform>(true).ToList().ForEach(transform =>
        {
            if (transform == mainObj.transform || transform.GetComponent<MeshFilter>()) return;
            sb.AppendLine("g " + transform.gameObject.name);
            sb.AppendLine($"# name:{transform.gameObject.name} parent:{transform.gameObject.transform.parent.name}");
        });

        // 写入文件
        File.WriteAllText(exportPath, sb.ToString());
        if (generateMaterials && sbMaterials.Length > 0)
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(exportPath),
                Path.GetFileNameWithoutExtension(exportPath) + ".mtl"),
                sbMaterials.ToString());
        }
    }

    // 法线变换处理 - 使用逆转置矩阵
    private Vector3 ApplyNormalTransform(Vector3 normal, Matrix4x4 matrix)
    {
        if (!applyRotation && !applyScale)
            return normal;

        // 使用逆转置矩阵正确处理法线
        Matrix4x4 normalMatrix = matrix.inverse.transpose;
        return normalMatrix.MultiplyVector(normal).normalized;
    }

    // 尝试导出纹理
    private string TryExportTexture(string propertyName, Material m)
    {
        if (m.HasProperty(propertyName))
        {
            Texture t = m.GetTexture(propertyName);
            if (t != null && t is Texture2D)
            {
                return ExportTexture((Texture2D)t);
            }
        }
        return null;
    }

    // 导出纹理到PNG
    private string ExportTexture(Texture2D t)
    {
        try
        {
            string exportName = Path.Combine(lastExportFolder, t.name + ".png");
            Texture2D exTexture = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);
            exTexture.SetPixels(t.GetPixels());
            File.WriteAllBytes(exportName, exTexture.EncodeToPNG());
            return t.name + ".png";
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Could not export texture: " + t.name + "\n" + ex.Message);
            return null;
        }
    }

    // 动态构建OBJ字符串
    private string ConstructOBJString(int index, bool hasUV, bool hasNormals)
    {
        if (hasUV && hasNormals)
            return $"{index}/{index}/{index}";
        if (hasUV)
            return $"{index}/{index}";
        if (hasNormals)
            return $"{index}//{index}";
        return $"{index}";
    }

    // 材质转字符串
    private string MaterialToString(Material m)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("newmtl " + m.name);

        // 基础颜色
        if (m.HasProperty("_Color"))
        {
            Color c = m.color;
            sb.AppendLine($"Kd {c.r} {c.g} {c.b}");
            if (c.a < 1.0f)
            {
                sb.AppendLine($"d {c.a}");
            }
        }

        // 纹理处理
        if (exportTextures)
        {
            string diffusePath = TryExportTexture("_MainTex", m);
            if (!string.IsNullOrEmpty(diffusePath))
            {
                sb.AppendLine("map_Kd " + diffusePath);
            }
        }

        sb.AppendLine("illum 2");
        return sb.ToString();
    }
}