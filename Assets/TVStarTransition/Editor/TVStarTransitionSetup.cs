#if UNITY_EDITOR
using System.IO;
using IWantGoHome.ScreenEffects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace IWantGoHome.ScreenEffects.Editor
{
    public static class TVStarTransitionSetup
    {
        private const string RootFolder = "Assets/TVStarTransition";
        private const string MaterialFolder = RootFolder + "/Runtime/Materials";
        private const string MaterialPath = MaterialFolder + "/M_TVStarGlitchV10.mat";
        private const string ShaderName = "Hidden/IWantGoHome/TVStarGlitchV10";

        [MenuItem("Tools/I Want Go Home/TV Star Transition/Create Fullscreen Glitch V10 Setup")]
        public static void CreateFullscreenGlitchSetup()
        {
            Material material = GetOrCreateMaterial();
            if (material == null)
            {
                Debug.LogError("TV Star Transition V10 setup failed: material could not be created.");
                return;
            }

            GameObject root = new GameObject("TVStarTransitionController");
            Undo.RegisterCreatedObjectUndo(root, "Create TV Star Transition Controller");
            TVStarTransitionController controller = root.AddComponent<TVStarTransitionController>();

            GameObject canvasObject = new GameObject("TVStarTransitionUI");
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create TV Star Transition UI");
            canvasObject.transform.SetParent(root.transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32000;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject resultRoot = new GameObject("ResultUIGroup_Optional");
            Undo.RegisterCreatedObjectUndo(resultRoot, "Create Result UI Group");
            resultRoot.transform.SetParent(canvasObject.transform, false);

            RectTransform rect = resultRoot.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            CanvasGroup group = resultRoot.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            CreateGuideText(resultRoot.transform);

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("rootCanvas").objectReferenceValue = canvas;
            so.FindProperty("transitionMaterial").objectReferenceValue = material;
            so.FindProperty("resultUIGroup").objectReferenceValue = group;
            so.FindProperty("makeMaterialInstance").boolValue = false;
            so.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeObject = root;
            EditorGUIUtility.PingObject(material);
            Debug.Log("TV Star Transition V10 setup created. Add a URP Full Screen Pass Renderer Feature and assign material: " + MaterialPath);
        }

        [MenuItem("Tools/I Want Go Home/TV Star Transition/Create V10 Material Only")]
        public static void CreateMaterialOnly()
        {
            Material material = GetOrCreateMaterial();
            if (material != null)
            {
                Selection.activeObject = material;
                EditorGUIUtility.PingObject(material);
            }
        }

        private static void CreateGuideText(Transform parent)
        {
            GameObject textObject = new GameObject("PlaceholderText_Optional_ReplaceMe");
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(960f, 180f);

            Text text = textObject.AddComponent<Text>();
            text.text = "OPTIONAL RESULT UI\nF9 holds dark glitch screen / F10 releases effect / F11 hides";
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 38;
            text.color = Color.white;
            text.raycastTarget = false;
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont != null) text.font = defaultFont;
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

            Material material = new Material(shader) { name = "M_TVStarGlitchV10" };
            ApplyDefaultMaterialValues(material);
            AssetDatabase.CreateAsset(material, MaterialPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return material;
        }

        private static void ApplyDefaultMaterialValues(Material material)
        {
            material.SetFloat("_Mode", 0f);
            material.SetFloat("_Progress", 0f);
            material.SetFloat("_SceneGlitchIntensity", 1.28f);
            material.SetFloat("_RGBSplit", 0.038f);
            material.SetFloat("_HorizontalTear", 0.145f);
            material.SetFloat("_WaveDistortion", 0.020f);
            material.SetFloat("_FineNoise", 0.007f);
            material.SetFloat("_HoldBurstIntensity", 1.00f);
            material.SetFloat("_HoldLineDensity", 2.60f);
            material.SetFloat("_HoldVerticalSpikeIntensity", 0.04f);
            material.SetFloat("_StarEdge", 0.0030f);
            material.SetFloat("_StarSharpness", 22f);
            material.SetFloat("_StarHorizontalReach", 2.85f);
            material.SetFloat("_StarVerticalReach", 0.52f);
            material.SetFloat("_StarHorizontalThickness", 0.090f);
            material.SetFloat("_StarVerticalThickness", 0.030f);
            material.SetFloat("_StarTipWidth", 0.0010f);
            material.SetFloat("_StarIntensity", 2.25f);
            material.SetFloat("_StarGlowIntensity", 0.16f);
            material.SetFloat("_FlashIntensity", 3.65f);
            material.SetFloat("_AfterimageIntensity", 1.60f);
            material.SetFloat("_AfterimageOffset", 0.082f);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(folderName)) return;
            EnsureFolder(parent);
            if (!AssetDatabase.IsValidFolder(folderPath)) AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
