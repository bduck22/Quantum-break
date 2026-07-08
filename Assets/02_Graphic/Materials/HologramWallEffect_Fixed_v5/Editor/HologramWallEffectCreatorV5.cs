#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HologramWallEffectCreatorV5
{
    private const string ShaderName = "Custom/HologramWall/TransparentDissolve_Soft_URP_V5";
    private const string RootFolder = "Assets/HologramWallEffect_Fixed_v5";
    private const string ShaderPath = RootFolder + "/Shaders/HologramWallDissolve_Soft_URP_V5.shader";
    private const string MaterialFolder = RootFolder + "/Materials";
    private const string MaterialPath = MaterialFolder + "/M_HologramWall_SoftTransparent_V5.mat";

    [MenuItem("Tools/Hologram Wall V5/Fix And Create Material")]
    public static void FixAndCreateMaterial()
    {
        AssetDatabase.Refresh();
        CreateOrUpdateMaterial();
        Debug.Log("Hologram Wall V5 material created/updated: " + MaterialPath);
    }

    [MenuItem("Tools/Hologram Wall V5/Apply Material To Selected")]
    public static void ApplyMaterialToSelected()
    {
        Material material = CreateOrUpdateMaterial();
        if (material == null)
        {
            Debug.LogError("Material creation failed. Check shader compile errors first.");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("Select one or more wall objects first.");
            return;
        }

        int rendererCount = 0;
        foreach (GameObject selectedObject in selectedObjects)
        {
            Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = material;
                rendererCount++;
            }

            HologramWallDisappearV5 effect = selectedObject.GetComponent<HologramWallDisappearV5>();
            if (effect == null)
            {
                selectedObject.AddComponent<HologramWallDisappearV5>();
            }
        }

        Debug.Log($"Applied Hologram Wall V5 material to {rendererCount} renderer(s).");
    }

    [MenuItem("Tools/Hologram Wall V5/Create Test Wall")]
    public static void CreateTestWall()
    {
        Material material = CreateOrUpdateMaterial();
        if (material == null)
        {
            Debug.LogError("Material creation failed. Check shader compile errors first.");
            return;
        }

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Test_HologramWall_V5";
        wall.transform.position = Vector3.zero;
        wall.transform.localScale = new Vector3(4f, 2.4f, 0.18f);

        Renderer renderer = wall.GetComponent<Renderer>();
        renderer.sharedMaterial = material;

        wall.AddComponent<HologramWallDisappearV5>();
        Selection.activeGameObject = wall;

        Debug.Log("Created Test_HologramWall_V5. Use the component context menu 'Play Disappear' or call PlayDisappear().");
    }

    private static Material CreateOrUpdateMaterial()
    {
        Shader shader = FindShaderAsset();
        if (shader == null)
        {
            Debug.LogError("Shader not found: " + ShaderName + "\nExpected path: " + ShaderPath);
            return null;
        }

        EnsureFolder(MaterialFolder);

        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (material == null)
        {
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, MaterialPath);
        }
        else
        {
            material.shader = shader;
        }

        SetDefaults(material);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return material;
    }

    private static Shader FindShaderAsset()
    {
        Shader shader = Shader.Find(ShaderName);
        if (shader != null)
        {
            return shader;
        }

        shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        if (shader != null)
        {
            return shader;
        }

        string[] guids = AssetDatabase.FindAssets("HologramWallDissolve_Soft_URP_V5 t:Shader");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader != null && shader.name == ShaderName)
            {
                return shader;
            }
        }

        return null;
    }

    private static void SetDefaults(Material material)
    {
        material.SetColor("_TintColor", new Color(0.04f, 0.70f, 1.00f, 0.28f));
        material.SetColor("_LineColor", new Color(0.16f, 0.95f, 1.00f, 1.00f));
        material.SetFloat("_FaceAlpha", 0.24f);
        material.SetFloat("_OverallBrightness", 0.72f);
        material.SetFloat("_MaxOutputBrightness", 1.08f);

        material.SetFloat("_LineSpacing", 0.82f);
        material.SetFloat("_LineWidth", 0.018f);
        material.SetFloat("_LineBrightness", 0.75f);
        material.SetFloat("_LineScrollSpeed", -0.16f);

        material.SetFloat("_GridSpacing", 0.75f);
        material.SetFloat("_GridWidth", 0.008f);
        material.SetFloat("_GridBrightness", 0.025f);

        material.SetFloat("_RimPower", 2.5f);
        material.SetFloat("_RimBrightness", 0.85f);

        material.SetFloat("_DissolveAmount", 0f);
        material.SetFloat("_DissolveEdgeWidth", 0.055f);
        material.SetFloat("_DissolveEdgeBrightness", 1.25f);
        material.SetFloat("_NoiseScale", 2.7f);
        material.SetFloat("_VerticalDissolveBias", 0.10f);

        material.SetFloat("_SurfaceNoiseStrength", 0.055f);
        material.SetFloat("_FlickerStrength", 0.035f);
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        string folderName = Path.GetFileName(folderPath);

        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, folderName);
    }
}
#endif
