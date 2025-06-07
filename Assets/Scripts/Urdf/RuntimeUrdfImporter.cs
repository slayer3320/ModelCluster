using UnityEngine;
using RosSharp.Urdf;
using System.IO;

// public class RuntimeUrdfImporter : MonoBehaviour
// {
//     public string urdfFilePath;
//     public GameObject importedRobot;
//
//     public void ImportRobot()
//     {
//         string urdfContent = System.IO.File.ReadAllText(urdfFilePath);
//         UrdfRobot robot = UrdfRobotExtensions.Create(urdfContent);
//         
//         importedRobot = new GameObject(robot.name);
//         robot.CreateVisuals(importedRobot.transform);
//         robot.CreateColliders(importedRobot.transform);
//         robot.CreateRigidbodies(importedRobot.transform);
//     }
// }
