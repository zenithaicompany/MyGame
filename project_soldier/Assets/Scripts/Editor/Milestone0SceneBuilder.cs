using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectSoldier.EditorTools
{
    public static class Milestone0SceneBuilder
    {
        private const string SceneFolder = "Assets/Scenes";
        private const string ScenePath = "Assets/Scenes/Testbox_MVP.unity";
        private const float EnvironmentScale = 2f;
        private const string PreferredIdleFbxPath =
            "Assets/Art/Characters/Animations/PS_Anim_Rifle_Idle_v1.fbx";
        private const string PreferredMoveFbxPath =
            "Assets/Art/Characters/Animations/PS_Anim_Rifle_Run_v1.fbx";
        private const string FallbackIdleFbxPath =
            "Assets/Kevin Iglesias/Human Animations/Animations/Male/Idles/HumanM@Idle01.fbx";
        private const string FallbackMoveFbxPath =
            "Assets/Kevin Iglesias/Human Animations/Animations/Male/Movement/Run/HumanM@Run01_Forward.fbx";
        private const string PlayerAnimatorControllerPath =
            "Assets/Art/Characters/Animations/PlayerVisual_Motion.controller";

        [MenuItem("Project Soldier/Build Milestone0 Test Scene")]
        public static void BuildMilestone0Scene()
        {
            // 선택된 오브젝트가 씬 재생성 중 파괴되면 Inspector가 null 참조를 잡는 경우가 있어 선제 해제.
            Selection.activeObject = null;

            EnsureFolder(SceneFolder);
            EnsureTag("Target");
            EnsurePlayerPrefabConfigured();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Testbox_MVP";

            CreateLighting();
            CreateGround();
            CreateOuterWalls();
            CreateCoverBoxes();
            CreateTargetBoxes();
            CreatePlayerSpawn();
            ProjectSoldier.PlayerM1Controller controller = CreatePlayerFromPrefab();
            CreateUIRoot(controller);

            EditorSceneManager.SaveScene(scene, ScenePath, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Project Soldier", $"Scene created:\n{ScenePath}", "OK");
            }
        }

        private static void CreateLighting()
        {
            GameObject lightGo = new GameObject("Directional Light");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.0f;
            light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3f * EnvironmentScale, 1f, 3f * EnvironmentScale);
        }

        private static void CreateOuterWalls()
        {
            GameObject root = new GameObject("Environment_Root");
            CreateWall("Wall_North", new Vector3(0f, 1.5f, 15f) * EnvironmentScale, new Vector3(30f, 3f, 1f) * EnvironmentScale, root.transform);
            CreateWall("Wall_South", new Vector3(0f, 1.5f, -15f) * EnvironmentScale, new Vector3(30f, 3f, 1f) * EnvironmentScale, root.transform);
            CreateWall("Wall_East", new Vector3(15f, 1.5f, 0f) * EnvironmentScale, new Vector3(1f, 3f, 30f) * EnvironmentScale, root.transform);
            CreateWall("Wall_West", new Vector3(-15f, 1.5f, 0f) * EnvironmentScale, new Vector3(1f, 3f, 30f) * EnvironmentScale, root.transform);
        }

        private static void CreateWall(string name, Vector3 position, Vector3 scale, Transform parent)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = scale;
        }

        private static void CreateCoverBoxes()
        {
            GameObject root = new GameObject("Cover_Root");
            Vector3[] positions =
            {
                new Vector3(-6f, 0.75f, -5f),
                new Vector3(0f, 0.75f, -5f),
                new Vector3(6f, 0.75f, -5f),
                new Vector3(-8f, 0.75f, 2f),
                new Vector3(-2f, 0.75f, 2f),
                new Vector3(4f, 0.75f, 2f),
                new Vector3(10f, 0.75f, 2f),
                new Vector3(-4f, 0.75f, 9f),
                new Vector3(2f, 0.75f, 9f),
                new Vector3(8f, 0.75f, 9f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.name = $"CoverBox_{i + 1:D2}";
                box.transform.SetParent(root.transform);
                box.transform.position = positions[i] * EnvironmentScale;
                box.transform.localScale = new Vector3(2f, 1.5f, 1.5f) * EnvironmentScale;
            }
        }

        private static void CreateTargetBoxes()
        {
            GameObject root = new GameObject("Target_Root");
            Vector3[] positions =
            {
                new Vector3(-8f, 0.75f, 5f),
                new Vector3(-4f, 0.75f, 7f),
                new Vector3(0f, 0.75f, 9f),
                new Vector3(4f, 0.75f, 11f),
                new Vector3(8f, 0.75f, 13f),
                new Vector3(-10f, 0.75f, 12f),
                new Vector3(-2f, 0.75f, 14f),
                new Vector3(6f, 0.75f, 15f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
                target.name = $"TargetBox_{i + 1:D2}";
                target.transform.SetParent(root.transform);
                target.transform.position = positions[i] * EnvironmentScale;
                target.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f) * EnvironmentScale;
                target.tag = "Target";
                target.AddComponent<ProjectSoldier.TargetHitReaction>();
            }
        }

        private static void CreatePlayerSpawn()
        {
            GameObject spawn = new GameObject("PlayerSpawn");
            spawn.transform.position = new Vector3(0f, 1f, -10f * EnvironmentScale);
            spawn.transform.rotation = Quaternion.identity;
        }

        private static ProjectSoldier.PlayerM1Controller CreatePlayerFromPrefab()
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Characters/Player.prefab");
            if (playerPrefab == null)
            {
                return null;
            }

            GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            if (player == null)
            {
                return null;
            }

            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, -10f * EnvironmentScale);
            player.transform.rotation = Quaternion.identity;
            return player.GetComponent<ProjectSoldier.PlayerM1Controller>();
        }

        private static AnimationClip FindClipAtPath(string assetPath, string nameContains)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            AnimationClip firstValidClip = null;
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is AnimationClip clip)
                {
                    // __preview__ clips are editor-only preview artifacts.
                    if (clip.name.Contains("__preview__"))
                    {
                        continue;
                    }

                    if (firstValidClip == null)
                    {
                        firstValidClip = clip;
                    }

                    if (!string.IsNullOrEmpty(nameContains) && clip.name.Contains(nameContains))
                    {
                        return clip;
                    }
                }
            }

            // Fallback: clip 이름 규칙이 달라져도 첫 유효 클립을 사용해 Missing 방지.
            return firstValidClip;
        }

        private static AnimationClip FindClipFromRepresentations(string assetPath, string nameContains)
        {
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            AnimationClip firstValid = null;
            for (int i = 0; i < subAssets.Length; i++)
            {
                if (subAssets[i] is not AnimationClip clip)
                {
                    continue;
                }

                if (clip.name.Contains("__preview__"))
                {
                    continue;
                }

                if (firstValid == null)
                {
                    firstValid = clip;
                }

                if (!string.IsNullOrEmpty(nameContains) && clip.name.Contains(nameContains))
                {
                    return clip;
                }
            }

            return firstValid;
        }

        private static AnimationClip FindClipFromModelRoot(string modelAssetPath, string nameContains)
        {
            GameObject modelRoot = AssetDatabase.LoadAssetAtPath<GameObject>(modelAssetPath);
            if (modelRoot == null)
            {
                return null;
            }

            AnimationClip[] clips = AnimationUtility.GetAnimationClips(modelRoot);
            AnimationClip firstValid = null;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip == null)
                {
                    continue;
                }

                if (firstValid == null && !clip.name.Contains("__preview__"))
                {
                    firstValid = clip;
                }

                if (!string.IsNullOrEmpty(nameContains) && clip.name.Contains(nameContains))
                {
                    return clip;
                }
            }

            return firstValid;
        }

        private static AnimationClip FindBestClipFromFbx(string fbxPath, string nameContains)
        {
            return FindClipFromRepresentations(fbxPath, nameContains)
                ?? FindClipFromRepresentations(fbxPath, string.Empty)
                ?? FindClipFromModelRoot(fbxPath, nameContains)
                ?? FindClipAtPath(fbxPath, nameContains);
        }

        private static AnimatorController EnsurePlayerAnimatorController(AnimationClip idleClip, AnimationClip moveClip)
        {
            EnsureFolder("Assets/Art");
            EnsureFolder("Assets/Art/Characters");
            EnsureFolder("Assets/Art/Characters/Animations");

            AnimatorController controller =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(PlayerAnimatorControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(PlayerAnimatorControllerPath);
            }

            if (controller == null)
            {
                return null;
            }

            bool hasSpeed = false;
            AnimatorControllerParameter[] parameters = controller.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name == "Speed" && parameters[i].type == AnimatorControllerParameterType.Float)
                {
                    hasSpeed = true;
                    break;
                }
            }

            if (!hasSpeed)
            {
                controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            }

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            ChildAnimatorState[] states = stateMachine.states;
            AnimatorState locomotionState = null;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state != null && states[i].state.name == "Locomotion")
                {
                    locomotionState = states[i].state;
                    break;
                }
            }

            if (locomotionState == null)
            {
                locomotionState = stateMachine.AddState("Locomotion", new Vector3(260f, 120f, 0f));
            }

            BlendTree blendTree = locomotionState.motion as BlendTree;
            if (blendTree == null)
            {
                blendTree = new BlendTree
                {
                    name = "LocomotionBlendTree"
                };
                AssetDatabase.AddObjectToAsset(blendTree, controller);
                locomotionState.motion = blendTree;
            }

            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.blendParameter = "Speed";
            blendTree.useAutomaticThresholds = false;
            blendTree.children = new ChildMotion[0];
            if (idleClip != null)
            {
                blendTree.AddChild(idleClip, 0f);
            }

            if (moveClip != null)
            {
                blendTree.AddChild(moveClip, 1f);
            }

            stateMachine.defaultState = locomotionState;
            EditorUtility.SetDirty(controller);
            EditorUtility.SetDirty(stateMachine);
            EditorUtility.SetDirty(blendTree);
            return controller;
        }

        private static AnimationClip EnsureExtractedAnimClipAsset(
            string sourceFbxPath,
            string targetAnimPath,
            string nameContains)
        {
            AnimationClip source = FindClipAtPath(sourceFbxPath, nameContains);
            if (source == null)
            {
                source = FindClipFromModelRoot(sourceFbxPath, nameContains);
            }
            if (source == null)
            {
                Debug.LogWarning($"[Milestone0SceneBuilder] Source clip not found: {sourceFbxPath}");
                return null;
            }

            EnsureFolder(Path.GetDirectoryName(targetAnimPath)?.Replace("\\", "/") ?? "Assets");
            AssetDatabase.DeleteAsset(targetAnimPath);

            AnimationClip extracted = new AnimationClip();
            EditorUtility.CopySerialized(source, extracted);
            extracted.name = Path.GetFileNameWithoutExtension(targetAnimPath);
            AssetDatabase.CreateAsset(extracted, targetAnimPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AnimationClip extractedAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(targetAnimPath);
            if (extractedAsset == null)
            {
                Debug.LogWarning($"[Milestone0SceneBuilder] Failed to create extracted anim: {targetAnimPath}");
            }
            else
            {
                Debug.Log($"[Milestone0SceneBuilder] Extracted anim created: {targetAnimPath}");
            }

            return extractedAsset;
        }

        private static Avatar FindReferenceAvatar()
        {
            string[] candidateModelPaths =
            {
                "Assets/CharacterPack Lowpoly (FREE)/Models/character1.fbx",
                "Assets/CharacterPack Lowpoly (FREE)/Models/character2.fbx"
            };

            for (int i = 0; i < candidateModelPaths.Length; i++)
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(candidateModelPaths[i]);
                for (int j = 0; j < assets.Length; j++)
                {
                    if (assets[j] is Avatar avatar && avatar.isValid)
                    {
                        return avatar;
                    }
                }
            }

            return null;
        }

        private static void EnsureAnimationRetargetSettings(string motionAssetPath, Avatar sourceAvatar)
        {
            ModelImporter importer = AssetImporter.GetAtPath(motionAssetPath) as ModelImporter;
            if (importer == null)
            {
                return;
            }

            bool changed = false;
            if (importer.importAnimation != true)
            {
                importer.importAnimation = true;
                changed = true;
            }

            // 일부 FBX는 clipAnimations가 비어 있으면 클립 서브에셋이 생성되지 않아 Missing이 발생한다.
            if (importer.clipAnimations == null || importer.clipAnimations.Length == 0)
            {
                ModelImporterClipAnimation[] defaults = importer.defaultClipAnimations;
                if (defaults != null && defaults.Length > 0)
                {
                    importer.clipAnimations = defaults;
                    changed = true;
                }
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        private static void EnsurePlayerPrefabConfigured()
        {
            const string playerPrefabPath = "Assets/Art/Characters/Player.prefab";
            GameObject playerPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);
            if (playerPrefabAsset == null)
            {
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(playerPrefabPath);
            try
            {
                ProjectSoldier.PlayerM1Controller controller = root.GetComponent<ProjectSoldier.PlayerM1Controller>();
                if (controller == null)
                {
                    return;
                }

                Transform visual = root.transform.Find("PlayerVisual");
                if (visual == null)
                {
                    return;
                }

                Animator animator = visual.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = visual.gameObject.AddComponent<Animator>();
                }
                animator.applyRootMotion = false;

                ProjectSoldier.SimpleCharacterMotionPlayable legacyPlayable =
                    visual.GetComponent<ProjectSoldier.SimpleCharacterMotionPlayable>();
                AnimationClip legacyIdle = null;
                AnimationClip legacyMove = null;
                if (legacyPlayable != null)
                {
                    SerializedObject legacySo = new SerializedObject(legacyPlayable);
                    legacyIdle = legacySo.FindProperty("idleClip").objectReferenceValue as AnimationClip;
                    legacyMove = legacySo.FindProperty("moveClip").objectReferenceValue as AnimationClip;
                    Object.DestroyImmediate(legacyPlayable);
                }

                AnimationClip idleClip =
                    FindBestClipFromFbx(PreferredIdleFbxPath, "Idle")
                    ?? FindBestClipFromFbx(FallbackIdleFbxPath, "Idle")
                    ?? legacyIdle;
                AnimationClip moveClip =
                    FindBestClipFromFbx(PreferredMoveFbxPath, "Run")
                    ?? FindBestClipFromFbx(FallbackMoveFbxPath, "Run")
                    ?? legacyMove;

                AnimatorController playerControllerAsset = EnsurePlayerAnimatorController(idleClip, moveClip);
                animator.runtimeAnimatorController = playerControllerAsset;

                ProjectSoldier.PlayerAnimatorParameterDriver motionDriver =
                    visual.GetComponent<ProjectSoldier.PlayerAnimatorParameterDriver>();
                if (motionDriver == null)
                {
                    motionDriver = visual.gameObject.AddComponent<ProjectSoldier.PlayerAnimatorParameterDriver>();
                }

                SerializedObject driverSo = new SerializedObject(motionDriver);
                driverSo.FindProperty("animator").objectReferenceValue = animator;
                driverSo.FindProperty("playerController").objectReferenceValue = controller;
                driverSo.ApplyModifiedPropertiesWithoutUndo();

                if (idleClip == null || moveClip == null || playerControllerAsset == null)
                {
                    Debug.LogWarning(
                        $"[Milestone0SceneBuilder] Animator setup incomplete. idle={(idleClip != null ? idleClip.name : "null")} move={(moveClip != null ? moveClip.name : "null")} controller={(playerControllerAsset != null ? playerControllerAsset.name : "null")}");
                }
                else
                {
                    Debug.Log(
                        $"[Milestone0SceneBuilder] Animator controller configured. idle={idleClip.name} move={moveClip.name}");
                }

                GameObject weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/LowPolyWeapons_LITE/Prefabs/Weapon_02.prefab");
                if (weaponPrefab != null)
                {
                    ProjectSoldier.WeaponAttachToHand runtimeAttach =
                        visual.GetComponent<ProjectSoldier.WeaponAttachToHand>();
                    if (runtimeAttach != null)
                    {
                        Object.DestroyImmediate(runtimeAttach);
                    }

                    Transform hand = FindDeepChildByName(visual.transform, "RightHand");
                    if (hand != null)
                    {
                        Transform equippedRoot = hand.Find("EquippedWeapon");
                        if (equippedRoot == null)
                        {
                            GameObject rootObj = new GameObject("EquippedWeapon");
                            equippedRoot = rootObj.transform;
                            equippedRoot.SetParent(hand, false);
                        }

                        for (int i = equippedRoot.childCount - 1; i >= 0; i--)
                        {
                            Object.DestroyImmediate(equippedRoot.GetChild(i).gameObject);
                        }

                        GameObject weaponInstance = PrefabUtility.InstantiatePrefab(weaponPrefab, equippedRoot) as GameObject;
                        if (weaponInstance != null)
                        {
                            weaponInstance.name = "WeaponModel";
                            weaponInstance.transform.localPosition = new Vector3(0.03f, 0.01f, 0.11f);
                            weaponInstance.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                            weaponInstance.transform.localScale = Vector3.one * 1.8f;
                        }
                    }
                }

                PrefabUtility.SaveAsPrefabAsset(root, playerPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static Transform FindDeepChildByName(Transform parent, string childName)
        {
            if (parent.name == childName)
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindDeepChildByName(parent.GetChild(i), childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static void CreateUIRoot(ProjectSoldier.PlayerM1Controller controller)
        {
            GameObject canvasGo = new GameObject("UI_Root");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            ProjectSoldier.SimpleVirtualJoystick joystick = CreateJoystick(canvasRect);
            ProjectSoldier.LookDragInputArea lookArea = CreateLookArea(canvasRect);
            ProjectSoldier.HoldFireButton fireButton = CreateFireButton(canvasRect);
            CreateCrosshair(canvasRect);

            SerializedObject controllerSo = new SerializedObject(controller);
            controllerSo.FindProperty("moveJoystick").objectReferenceValue = joystick;
            controllerSo.FindProperty("lookArea").objectReferenceValue = lookArea;
            controllerSo.FindProperty("fireButton").objectReferenceValue = fireButton;
            controllerSo.ApplyModifiedPropertiesWithoutUndo();

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                eventSystemGo.AddComponent<InputSystemUIInputModule>();
#else
                eventSystemGo.AddComponent<StandaloneInputModule>();
#endif
            }
        }

        private static ProjectSoldier.SimpleVirtualJoystick CreateJoystick(RectTransform canvasRect)
        {
            GameObject root = new GameObject("JoystickRoot", typeof(RectTransform), typeof(Image), typeof(ProjectSoldier.SimpleVirtualJoystick));
            root.transform.SetParent(canvasRect, false);
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0f, 0f);
            rootRect.anchorMax = new Vector2(0f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = new Vector2(140f, 140f);
            rootRect.sizeDelta = new Vector2(180f, 180f);
            Image rootImage = root.GetComponent<Image>();
            rootImage.color = new Color(1f, 1f, 1f, 0.15f);

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(rootRect, false);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0.5f, 0.5f);
            handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(80f, 80f);
            Image handleImage = handle.GetComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.4f);

            ProjectSoldier.SimpleVirtualJoystick joystick = root.GetComponent<ProjectSoldier.SimpleVirtualJoystick>();
            SerializedObject so = new SerializedObject(joystick);
            so.FindProperty("background").objectReferenceValue = rootRect;
            so.FindProperty("handle").objectReferenceValue = handleRect;
            so.ApplyModifiedPropertiesWithoutUndo();
            return joystick;
        }

        private static ProjectSoldier.LookDragInputArea CreateLookArea(RectTransform canvasRect)
        {
            GameObject lookArea = new GameObject("LookArea", typeof(RectTransform), typeof(Image), typeof(ProjectSoldier.LookDragInputArea));
            lookArea.transform.SetParent(canvasRect, false);
            RectTransform lookRect = lookArea.GetComponent<RectTransform>();
            lookRect.anchorMin = new Vector2(0.35f, 0f);
            lookRect.anchorMax = new Vector2(1f, 1f);
            lookRect.offsetMin = Vector2.zero;
            lookRect.offsetMax = Vector2.zero;

            Image image = lookArea.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            return lookArea.GetComponent<ProjectSoldier.LookDragInputArea>();
        }

        private static ProjectSoldier.HoldFireButton CreateFireButton(RectTransform canvasRect)
        {
            GameObject fire = new GameObject("FireButton", typeof(RectTransform), typeof(Image), typeof(ProjectSoldier.HoldFireButton));
            fire.transform.SetParent(canvasRect, false);
            RectTransform fireRect = fire.GetComponent<RectTransform>();
            fireRect.anchorMin = new Vector2(1f, 0f);
            fireRect.anchorMax = new Vector2(1f, 0f);
            fireRect.pivot = new Vector2(0.5f, 0.5f);
            fireRect.anchoredPosition = new Vector2(-130f, 140f);
            fireRect.sizeDelta = new Vector2(130f, 130f);

            Image fireImage = fire.GetComponent<Image>();
            fireImage.color = new Color(1f, 0.4f, 0.3f, 0.55f);
            return fire.GetComponent<ProjectSoldier.HoldFireButton>();
        }

        private static void CreateCrosshair(RectTransform canvasRect)
        {
            GameObject crosshair = new GameObject("Crosshair", typeof(RectTransform), typeof(Image));
            crosshair.transform.SetParent(canvasRect, false);
            RectTransform crossRect = crosshair.GetComponent<RectTransform>();
            crossRect.anchorMin = new Vector2(0.5f, 0.5f);
            crossRect.anchorMax = new Vector2(0.5f, 0.5f);
            crossRect.pivot = new Vector2(0.5f, 0.5f);
            crossRect.anchoredPosition = Vector2.zero;
            crossRect.sizeDelta = new Vector2(10f, 10f);
            Image crossImage = crosshair.GetComponent<Image>();
            crossImage.color = new Color(1f, 1f, 1f, 0.9f);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            string leaf = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(leaf))
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, leaf);
        }

        private static void EnsureTag(string tag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty element = tagsProp.GetArrayElementAtIndex(i);
                if (element.stringValue == tag)
                {
                    return;
                }
            }

            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }
    }
}
