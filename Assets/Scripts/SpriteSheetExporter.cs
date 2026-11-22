
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpriteSheetExporter
{
    [MenuItem("Assets/Export Sliced Sprites to PNG")]
    private static void ExportSprites()
    {
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogWarning("Please select a sprite sheet.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);

        Object[] assets =
            AssetDatabase.LoadAllAssetsAtPath(assetPath);

        string folder = EditorUtility.OpenFolderPanel(
            "Export Sprites",
            Application.dataPath,
            ""
        );

        if (string.IsNullOrEmpty(folder))
            return;

        int exported = 0;

        foreach (Object asset in assets)
        {
            if (asset is not Sprite sprite)
                continue;

            Rect rect = sprite.rect;

            int x = Mathf.RoundToInt(rect.x);
            int y = Mathf.RoundToInt(rect.y);
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            Texture2D newTexture = new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

            Color[] pixels = texture.GetPixels(
                x, y, width, height
            );

            newTexture.SetPixels(pixels);
            newTexture.Apply();

            byte[] png = newTexture.EncodeToPNG();

            string path = Path.Combine(
                folder,
                sprite.name + ".png"
            );

            File.WriteAllBytes(path, png);

            Object.DestroyImmediate(newTexture);

            exported++;
        }

        AssetDatabase.Refresh();

        Debug.Log($"Exported {exported} sprites to {folder}");
    }

    [MenuItem("Assets/Export Sliced Sprites to PNG", true)]
    private static bool ValidateExport()
    {
        return Selection.activeObject is Texture2D;
    }
}
