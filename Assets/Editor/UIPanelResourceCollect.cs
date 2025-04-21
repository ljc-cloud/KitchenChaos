using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class UIPanelResourceCollect
    {
        [MenuItem("Tools/UIPanel Resource Collect")]
        public static void CollectUIPanel()
        {
            string path = $"{Application.dataPath}/Resources/UIPanel";
            string[] paths = Directory.GetFiles(path, "*.prefab")
                .Select(item => item.Substring(item.IndexOf("UIPanel", StringComparison.Ordinal))
                    .Replace("\\", "/")).ToArray();
            string soPath = $"{Application.dataPath}/So/UIPanelSo";
            if (!Directory.Exists(soPath))
            {
                Directory.CreateDirectory(soPath);
            }

            string[] soPaths = Directory.GetFiles(soPath).Select(item
                => item.Substring(item.IndexOf("Assets", StringComparison.Ordinal))).ToArray();
            foreach (string sp in soPaths)
            {
                AssetDatabase.DeleteAsset(sp);
            }
            
            List<UIPanelSo> uiPanelSoList = new List<UIPanelSo>();
            string soRelativePath = soPath.Substring(soPath.IndexOf("Assets", StringComparison.Ordinal));
            foreach (var p in paths)
            {
                UIPanelSo uiPanelSo = ScriptableObject.CreateInstance<UIPanelSo>();
                string name = p.Substring(p.LastIndexOf("/", StringComparison.Ordinal) + 1).Replace(".prefab","");
                if (Enum.TryParse(name, out UIPanelType uiPanelType))
                {
                    uiPanelSo.uIPanelType = uiPanelType;
                }
                // uiPanelSo.uIPanelType = Enum.Parse<UIPanelType>(name);
                uiPanelSo.name = name;
                uiPanelSo.path = p.Replace(".prefab", "");
                AssetDatabase.CreateAsset(uiPanelSo, soRelativePath + "/" + name + ".asset");
                uiPanelSoList.Add(uiPanelSo);
            }

            UIPanelSoListSo uiPanelSoListSo = ScriptableObject.CreateInstance<UIPanelSoListSo>();
            string listSoName = "_UIPanelSoListSo";
            uiPanelSoListSo.name = listSoName;
            uiPanelSoListSo.uIPanelSoList = uiPanelSoList;
            Debug.Log(soRelativePath);
            AssetDatabase.CreateAsset(uiPanelSoListSo, soRelativePath + "/"+ listSoName + ".asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}