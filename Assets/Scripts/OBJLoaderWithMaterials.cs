using UnityEngine;
using System.IO;

namespace Dummiesman
{
    public class OBJLoaderWithMaterials
    {
        public GameObject Load(string objPath)
        {
            string objDirectory = Path.GetDirectoryName(objPath);
            string mtlPath = null;

            // 读取 .obj 文件，提取 mtllib 信息
            foreach (var line in File.ReadLines(objPath))
            {
                if (line.StartsWith("mtllib "))
                {
                    string mtlFileName = line.Substring(7).Trim();
                    mtlPath = Path.Combine(objDirectory, mtlFileName);
                    break;
                }
            }

            // 创建加载器并加载
            var loader = new OBJLoader();
            return loader.Load(objPath, mtlPath); // ✅ 使用绝对路径加载
        }
    }
}
