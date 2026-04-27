using UnityEditor;
using UnityEngine;

namespace ProjectSoldier.EditorTools
{
    public static class ClipDiagnostic
    {
        [MenuItem("Project Soldier/Debug/Print FBX Clips")]
        public static void PrintFbxClips()
        {
            PrintForPath("Assets/Art/Characters/Animations/PS_Anim_Rifle_Idle_v1.fbx");
            PrintForPath("Assets/Art/Characters/Animations/PS_Anim_Rifle_Run_v1.fbx");
            PrintForPath("Assets/Kevin Iglesias/Human Animations/Animations/Male/Idles/HumanM@Idle01.fbx");
            PrintForPath("Assets/Kevin Iglesias/Human Animations/Animations/Male/Movement/Run/HumanM@Run01_Forward.fbx");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void PrintForPath(string path)
        {
            Debug.Log($"[ClipDiagnostic] --- {path} ---");

            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null)
            {
                int customCount = importer.clipAnimations != null ? importer.clipAnimations.Length : 0;
                int defaultCount = importer.defaultClipAnimations != null ? importer.defaultClipAnimations.Length : 0;
                Debug.Log(
                    $"[ClipDiagnostic] importer importAnimation={importer.importAnimation} animationType={importer.animationType} customClipCount={customCount} defaultClipCount={defaultCount}");
            }

            Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);
            Debug.Log($"[ClipDiagnostic] LoadAllAssetsAtPath count={all.Length}");
            for (int i = 0; i < all.Length; i++)
            {
                Object obj = all[i];
                if (obj == null)
                {
                    continue;
                }

                if (obj is AnimationClip clip)
                {
                    Debug.Log($"[ClipDiagnostic] asset clip name={clip.name}");
                }
            }

            Object[] reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            Debug.Log($"[ClipDiagnostic] LoadAllAssetRepresentationsAtPath count={reps.Length}");
            for (int i = 0; i < reps.Length; i++)
            {
                if (reps[i] is AnimationClip repClip)
                {
                    Debug.Log($"[ClipDiagnostic] rep clip name={repClip.name}");
                }
            }

            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (root == null)
            {
                Debug.LogWarning("[ClipDiagnostic] model root is null");
                return;
            }

            AnimationClip[] clips = AnimationUtility.GetAnimationClips(root);
            Debug.Log($"[ClipDiagnostic] AnimationUtility.GetAnimationClips count={clips.Length}");
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip == null)
                {
                    continue;
                }

                Debug.Log($"[ClipDiagnostic] clip name={clip.name}");
            }
        }
    }
}
