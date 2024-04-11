using UnityEditor;
using System.IO;
using UnityEngine;

public class PackAtlas : Editor
{
    public static string TransferToRef(string abusolute)
    {
        return abusolute.Replace(Application.dataPath, "Assets");
    }

    public static string TransferToAbusolate(string relavant)
    {
        return relavant.Replace("Assets", Application.dataPath);
    }


    [MenuItem("Assets/PackOne")]
    public static void PackSingle()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        if (arr.Length != 1)
        {
            Debug.LogError("打包图集失败! 只能选择一个对象进行打包");
            return;
        }

        var folder_obj = arr[0];
        var folder_path = AssetDatabase.GetAssetPath(folder_obj);
        var abusolute_folder_path = PackAtlas.TransferToAbusolate(folder_path);
        if (System.IO.Directory.Exists(abusolute_folder_path) == false)
        {
            Debug.LogError("打包图集失败! 改对象必须是文件夹");
        }

        string[] sections = abusolute_folder_path.Split('/');
        string container_folder = "Assets/Resources/UI_Atlas/" + sections[sections.Length - 1];
        string to_path = PackAtlas.TransferToAbusolate(container_folder + "/" + sections[sections.Length - 1]);

        AtlasPacker.GenerateAtlas(folder_path, to_path, true);

        var guids = AssetDatabase.FindAssets("t:Texture", new string[] { container_folder });   //图片
        foreach (string guid in guids)
        {
            var relative_path = AssetDatabase.GUIDToAssetPath(guid);    //当前的相对路径
            TextureImporter textureImporter = AssetImporter.GetAtPath(relative_path) as TextureImporter;

            AssetDatabase.ImportAsset(relative_path);
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("打包完成", "打包完成 " + to_path, "OK");
    }
}
