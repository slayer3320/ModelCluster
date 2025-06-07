using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Robotics.UrdfImporter;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject target;
    
    void Start()
    {
        //RuntimeUrdfExporter.Instance.ExportURDF(target, "C:/Users/andy2/Desktop/exported.urdf");

        int idx = 1;
        target.GetComponentsInChildren<Transform>(true).ToList().ForEach(transform =>
        {
            if (transform == target.transform) return;
            transform.name = "Mesh" + idx++;
        });
    }
    
}
