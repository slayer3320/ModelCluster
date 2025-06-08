using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main current;
    
    void Awake()
    {
        current = this;
        
        //ModelFlattener.Instance.FlattenHierarchy(this.gameObject);
        
        //.GameObject[] objs = GameObject.FindObjectsOfType<GameObject>(true);
        //OBJExporter.ExportGameObjectsToOBJ(GetAllChildrenObjects(this.gameObject), @"C:\Users\andy2\Desktop\exportedObject.obj");
    }

    public GameObject[] GetAllChildrenObjects(GameObject parent)
    {
        List<GameObject> allChildren = new List<GameObject>();
        GetChildrenRecursive(parent.transform, allChildren);
        return allChildren.ToArray();
    }

    void GetChildrenRecursive(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            ProcessGameObject(child.gameObject);
            list.Add(child.gameObject);
            GetChildrenRecursive(child, list);
        }
    }
    
    void ProcessGameObject(GameObject obj)
    {
        obj.name = obj.name.Split('_')[0];
    }
}
