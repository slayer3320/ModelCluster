using UnityEngine;
using System.Xml;
using System.IO;

public class RuntimeUrdfExporter
{
    public static RuntimeUrdfExporter Instance
    {
        get 
        {
            if (instance == null) instance = new RuntimeUrdfExporter();
            return instance;
        }
        private set => instance = value;
    }

    public static RuntimeUrdfExporter instance;
    
    
    public void ExportURDF(GameObject root, string filePath)
    {
        XmlDocument doc = new XmlDocument();

        XmlElement robotElement = doc.CreateElement("robot");
        robotElement.SetAttribute("name", root.name);
        doc.AppendChild(robotElement);

        TraverseHierarchy(doc, robotElement, root, null);

        doc.Save(filePath);
    }

    void TraverseHierarchy(XmlDocument doc, XmlElement robot, GameObject obj, GameObject parent)
    {
        // Create link element
        XmlElement link = doc.CreateElement("link");
        link.SetAttribute("name", obj.name);
        robot.AppendChild(link);

        // Optionally add geometry and material info here...

        if (parent != null)
        {
            // Create joint element
            XmlElement joint = doc.CreateElement("joint");
            joint.SetAttribute("name", $"{parent.name}_to_{obj.name}");
            joint.SetAttribute("type", "fixed");

            XmlElement parentElem = doc.CreateElement("parent");
            parentElem.SetAttribute("link", parent.name);
            joint.AppendChild(parentElem);

            XmlElement childElem = doc.CreateElement("child");
            childElem.SetAttribute("link", obj.name);
            joint.AppendChild(childElem);

            robot.AppendChild(joint);
        }

        // Recurse into children
        foreach (Transform child in obj.transform)
        {
            TraverseHierarchy(doc, robot, child.gameObject, obj);
        }
    }
}
