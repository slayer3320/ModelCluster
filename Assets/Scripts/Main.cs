using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        ModelFlattener.Instance.FlattenHierarchy(this.gameObject);
    }

}
