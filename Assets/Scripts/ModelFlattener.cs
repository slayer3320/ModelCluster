using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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


    List<Transform> childrenToMove = new List<Transform>();
    public void FlattenHierarchyInChildren(Transform parent, GameObject obj)
    {
        if (parent == null) return;


        parent.GetComponentsInChildren<Transform>().Where(t => t != parent).ToList().ForEach(t =>
        {
            if (t.GetComponentsInChildren<Transform>().Where(tt => tt != t).ToList().Count == 0)
            {
                childrenToMove.Add(t);
            }
            else
            {
                FlattenHierarchyInChildren(t, obj);
                childrenToMove.Add(t);
                t.gameObject.layer = LayerMask.NameToLayer("NoShow");
            }
        });

        foreach (var child in childrenToMove)
        {
            child.SetParent(obj.transform);
        }

        childrenToMove.Clear();
    }
    
    public void FlattenHierarchy(GameObject obj)
    {
        if (obj == null) return;

        FlattenHierarchyInChildren(obj.transform, obj);


    }
}
