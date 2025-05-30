/*
Author: DA LAB
Copy form youtube video: https://youtu.be/dcAfXEVcLMg?si=xEf3k6OORj_PQYJL
Code reop: https://github.com/DA-LAB-Tutorials/YouTube-Unity-Tutorials/blob/main/SaveFile.cs
*/


using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using SFB;
using TMPro;
using UnityEngine.Networking;
using System;
using Dummiesman;
using Unity.VisualScripting; //Load OBJ Model

public class FileManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject model; //Load OBJ Model
    public Transform modelRoot;
    private IEnumerator OutputRoutineOpen(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            textMeshPro.text = www.downloadHandler.text;

            // 保存当前模型的位置、旋转和缩放（如果有的话）
            Vector3 originalPosition = modelRoot.localPosition;
            Quaternion originalRotation = modelRoot.localRotation;
            Vector3 originalScale = modelRoot.localScale;

            // Load OBJ Model
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));

            // 清除modelRoot下的所有子物体
            foreach (Transform child in modelRoot)
            {
                Destroy(child.gameObject);
            }

            // 加载新模型并设置为modelRoot的子物体
            model = new OBJLoader().Load(textStream);
            model.transform.SetParent(modelRoot, false);

            // 恢复原始变换
            modelRoot.localPosition = originalPosition;
            modelRoot.localRotation = originalRotation;
            modelRoot.localScale = originalScale;

            // 对新模型应用特定的缩放（如果需要）
            model.transform.localScale = new Vector3(-1, 1, 1); // set the position of parent model. Reverse X to show properly 
            AssignDefaultMaterial(model);
            FitOnScreen();
            DoublicateFaces();
        }
    }
    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
    public void OnClickSave()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "model", "obj");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, textMeshPro.text);
        }
    }

    private Bounds GetBound(GameObject gameObj)
    {
        Bounds bound = new Bounds(gameObj.transform.position, Vector3.zero);
        var rList = gameObj.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList)
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }

    public void FitOnScreen()
    {
        Bounds bound = GetBound(model);
        Vector3 boundSize = bound.size;
        float diagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z)); //Get box diagonal
        Camera.main.orthographicSize = diagonal / 2.0f;
        Camera.main.transform.position = bound.center;
    }



    // Doublicate the size of mesh components, in which the second half of the tringles winding order and normals are reverse of the first half to enable displaying front and back faces
    //https://answers.unity.com/questions/280741/how-make-visible-the-back-face-of-a-mesh.html
    public void DoublicateFaces()
    {
        for (int i = 0; i < model.GetComponentsInChildren<Renderer>().Length; i++) //Loop through the model children
        {
            // Get oringal mesh components: vertices, normals triangles and texture coordinates 
            Mesh mesh = model.GetComponentsInChildren<MeshFilter>()[i].mesh;
            Vector3[] vertices = mesh.vertices;
            int numOfVertices = vertices.Length;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            int numOfTriangles = triangles.Length;
            Vector2[] textureCoordinates = mesh.uv;
            if (textureCoordinates.Length < numOfTriangles) //Check if mesh doesn't have texture coordinates 
            {
                textureCoordinates = new Vector2[numOfVertices * 2];
            }

            // Create a new mesh component, double the size of the original 
            Vector3[] newVertices = new Vector3[numOfVertices * 2];
            Vector3[] newNormals = new Vector3[numOfVertices * 2];
            int[] newTriangle = new int[numOfTriangles * 2];
            Vector2[] newTextureCoordinates = new Vector2[numOfVertices * 2];

            for (int j = 0; j < numOfVertices; j++)
            {
                newVertices[j] = newVertices[j + numOfVertices] = vertices[j]; //Copy original vertices to make the second half of the mew vertices array
                newTextureCoordinates[j] = newTextureCoordinates[j + numOfVertices] = textureCoordinates[j]; //Copy original texture coordinates to make the second half of the mew texture coordinates array  
                newNormals[j] = normals[j]; //First half of the new normals array is a copy original normals
                newNormals[j + numOfVertices] = -normals[j]; //Second half of the new normals array reverse the original normals
            }

            for (int x = 0; x < numOfTriangles; x += 3)
            {
                // copy the original triangle for the first half of array
                newTriangle[x] = triangles[x];
                newTriangle[x + 1] = triangles[x + 1];
                newTriangle[x + 2] = triangles[x + 2];
                // Reversed triangles for the second half of array
                int j = x + numOfTriangles;
                newTriangle[j] = triangles[x] + numOfVertices;
                newTriangle[j + 2] = triangles[x + 1] + numOfVertices;
                newTriangle[j + 1] = triangles[x + 2] + numOfVertices;
            }
            mesh.vertices = newVertices;
            mesh.uv = newTextureCoordinates;
            mesh.normals = newNormals;
            mesh.triangles = newTriangle;
        }
    }

    public void LoadMeshToRoot(Mesh mesh, Transform modelRoot)
    {
        // 清空旧模型
        foreach (Transform child in modelRoot)
        {
            Destroy(child.gameObject);
        }

        // 新建物体并赋mesh和材质
        GameObject obj = new GameObject("ImportedOBJ", typeof(MeshFilter), typeof(MeshRenderer));
        obj.transform.SetParent(modelRoot, false);
        obj.GetComponent<MeshFilter>().mesh = mesh;

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        Debug.Log($"urpShader: {urpShader}");
        if (urpShader == null)
        {
            Debug.LogError("URP Shader not found!");
            return;
        }

        Material baseMat = new Material(urpShader);
        baseMat.color = Color.white;

        // 直接赋给唯一MeshRenderer
        obj.GetComponent<MeshRenderer>().material = baseMat;

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }


    private void AssignDefaultMaterial(GameObject modelRoot)
    {
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            Debug.LogError("URP Shader not found!");
            return;
        }

        Material defaultMat = new Material(urpShader);
        defaultMat.color = Color.gray; // 可以改为其他颜色

        foreach (var renderer in modelRoot.GetComponentsInChildren<Renderer>())
        {
            renderer.material = defaultMat;  // 强制替换材质
        }

    }

}