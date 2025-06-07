using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TransformUI : MonoBehaviour
{
    public static TransformUI Instance;
    
    public Vector3 sensitivity;
    
    
    public GameObject currentSelectedObject;
    
    private Dictionary<string, PressDetection> pressDetections = new Dictionary<string, PressDetection>();
    private Dictionary<string, Text> texts = new Dictionary<string, Text>();
    
    private bool isPressingX = false;
    private bool isPressingY = false;
    private bool isPressingZ = false;
    private Vector3 initialMousePosition = Vector3.zero;
    
    private void Awake()
    {
        Instance = this;
        
        GetComponentsInChildren<PressDetection>().ToList().ForEach((pressDetection) =>
        {
            pressDetections.Add(pressDetection.name, pressDetection);
        });
        GetComponentsInChildren<Text>().ToList().ForEach((text) =>
        {
            texts.Add(text.name, text);
        });
        

        
        pressDetections["XButton"].OnPressDown += () =>
        {
            if (!currentSelectedObject) return;
            isPressingX = true;
        };
        pressDetections["YButton"].OnPressDown += () =>
        {
            if (!currentSelectedObject) return;
            
            isPressingY = true;
        };
        pressDetections["ZButton"].OnPressDown += () =>
        {
            if (!currentSelectedObject) return;
            
            isPressingZ = true;
        };
        
    }

    private void Update()
    {
        if (!currentSelectedObject) return;
        
        if(isPressingX || isPressingY || isPressingZ)
        {
            CameraControl.instance.isControlling = false;
        }
        else
        {
            CameraControl.instance.isControlling = true;
        }
        
        if (isPressingX)
        {
            Debug.Log("Pressing X");
            
            currentSelectedObject.transform.position += 
                new Vector3((Input.mousePosition.x - initialMousePosition.x) * sensitivity.x * Time.deltaTime, 0, 0);
        }
        if (isPressingY)
        {
            currentSelectedObject.transform.position += 
                new Vector3(0, (Input.mousePosition.y - initialMousePosition.y) * sensitivity.y * Time.deltaTime, 0);
        }
        if (isPressingZ)
        {
            currentSelectedObject.transform.position += 
                new Vector3(0, 0, (Input.mousePosition.z - initialMousePosition.z) * sensitivity.z * Time.deltaTime);
        }
        
        initialMousePosition = Input.mousePosition;
        
        
        if (Input.GetMouseButtonUp(0))
        {
            isPressingX = false;
            isPressingY = false;
            isPressingZ = false;
        }
        
        texts["XText"].text = currentSelectedObject.transform.position.x.ToString("F2");
        texts["YText"].text = currentSelectedObject.transform.position.y.ToString("F2");
        texts["ZText"].text = currentSelectedObject.transform.position.z.ToString("F2");
        
    }


    public void SetCurrentSelectedObject(GameObject obj)
    {
        if (currentSelectedObject != null)
        {
            // Reset previous selection
            currentSelectedObject.GetComponent<Renderer>().material.color = Color.white;
        }
        
        currentSelectedObject = obj;
        
        if (currentSelectedObject != null)
        {
            // Highlight the new selection
            currentSelectedObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
    
    void OnDestroy()
    {
        
    }
}

