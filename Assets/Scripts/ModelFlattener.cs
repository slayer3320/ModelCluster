using UnityEngine;
using System.Collections.Generic;

public class ModelFlattener : MonoBehaviour
{
    private static ModelFlattener _instance;
    public static ModelFlattener Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ModelFlattener>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ModelFlattener");
                    _instance = obj.AddComponent<ModelFlattener>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    public void FlattenHierarchy(GameObject root)
    {
        if (root == null) return;

        Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
        List<Transform> childrenToMove = new List<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child != root.transform)
            {
                childrenToMove.Add(child);
            }
        }

        // 移动所有子对象到root的直接子级
        foreach (Transform child in childrenToMove)
        {
            child.SetParent(root.transform, true);
        }
    }
}
