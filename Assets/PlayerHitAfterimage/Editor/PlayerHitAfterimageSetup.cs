#if UNITY_EDITOR
using System.IO;
using IWantGoHome.ScreenEffects;
using UnityEditor;
using UnityEngine;

namespace IWantGoHome.ScreenEffects.Editor
{
    public static class PlayerHitAfterimageSetup
    {
        private const string RootFolder = "Assets/PlayerHitAfterimage";
        private const string MaterialFolder = RootFolder + "/Runtime/Materials";
        private const string MaterialPath = MaterialFolder + "/M_PlayerHitAfterimage.mat";
        private const string ShaderName = "Hidden/IWantGoHome/PlayerHitAfterimage";

        [MenuItem("Tools/I Want Go Home/Player Hit Afterimage/Create Setup")]
        public static void CreateSetup()
        {
            Material material = GetOrCreateMaterial();
            if (material == null)
            {
                Debug.LogError("Player Hit Afterimage setup failed: material could not be created.");
                return;
            }

            GameObject root = new GameObject("PlayerHitAfterimageController");
            Undo.RegisterCreatedObjectUndo(root, "Create Player Hit Afterimage Controller");

            PlayerHitAfterimageController controller = root.AddComponent<PlayerHitAfterimageController>();
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("afterimageMaterial").objectReferenceValue = material;
            so.FindProperty("makeMaterialInstance").boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeObject = root;
            EditorGUIUtility.PingObject(material);

            Debug.Log("Player Hit Afterimage setup created. Add a URP Full Screen Pass Renderer Feature and assign material: " + MaterialPath);
        }

        [MenuItem("Tools/I Want Go Home/Player Hit Afterimage/Create Material Only")]
        public static void CreateMaterialOnly()
        {
            Material material = GetOrCreateMaterial();
            if (material != null)
            {
                Selection.activeObject = material;
                EditorGUIUtility.PingObject(material);
            }
        }

        private static Material GetOrCreateMaterial()
        {
            EnsureFolder(RootFolder);
            EnsureFolder(RootFolder + "/Runtime");
            EnsureFolder(MaterialFolder);

            Material existing = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (existing != null)
            {
                ApplyDefaultMaterialValues(existing);
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                return existing;
            }

            Shader shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {ShaderName}");
                return null;
            }

            Material material = new Material(shader) { name = "M_PlayerHitAfterimage" };
            ApplyDefaultMaterialValues(material);
            AssetDatabase.CreateAsset(material, MaterialPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return material;
        }

        private static void ApplyDefaultMaterialValues(Material material)
        {
            material.SetFloat("_Mode", 0f);
            material.SetFloat("_Progress", 1f);
            material.SetFloat("_AfterimageIntensity", 1.35f);
            material.SetFloat("_AfterimageOffset", 0.0012f);
            material.SetFloat("_RGBSplit", 0.0016f);
            material.SetFloat("_ZoomAmount", 0.0022f);
            material.SetFloat("_CenterGlowIntensity", 0.0f);
            material.SetFloat("_VignetteIntensity", 0.02f);
            material.SetFloat("_SnapshotFlipX", 0f);
            material.SetFloat("_SnapshotFlipY", 1f);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(folderName)) return;

            EnsureFolder(parent);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
#endif
