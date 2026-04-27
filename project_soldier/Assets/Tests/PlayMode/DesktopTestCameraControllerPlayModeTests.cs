using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ProjectSoldier.Tests.PlayMode
{
    public class DesktopTestCameraControllerPlayModeTests
    {
        [UnityTest]
        public IEnumerator PressingWMovesCameraForward()
        {
            Func<Vector2> moveForward = () => new Vector2(0f, 1f);
            Func<Vector2> noLook = () => Vector2.zero;
            Func<bool> alwaysFalse = () => false;

            GameObject cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.AddComponent<Camera>();

            Type controllerType = Type.GetType("ProjectSoldier.DesktopTestCameraController, Assembly-CSharp");
            Assert.IsNotNull(controllerType, "Could not resolve DesktopTestCameraController type from Assembly-CSharp.");
            Component controller = cameraGo.AddComponent(controllerType);
            Assert.IsNotNull(controller, "DesktopTestCameraController could not be attached.");

            controllerType.GetField("MoveInputOverride")?.SetValue(null, moveForward);
            controllerType.GetField("LookInputOverride")?.SetValue(null, noLook);
            controllerType.GetField("SprintHeldOverride")?.SetValue(null, alwaysFalse);
            controllerType.GetField("KeyQOverride")?.SetValue(null, alwaysFalse);
            controllerType.GetField("KeyEOverride")?.SetValue(null, alwaysFalse);
            controllerType.GetField("EscapeDownOverride")?.SetValue(null, alwaysFalse);
            controllerType.GetField("RightMouseDownOverride")?.SetValue(null, alwaysFalse);

            yield return null;
            yield return null;

            Camera mainCamera = Camera.main;
            Assert.IsNotNull(mainCamera, "Main Camera was not found.");

            Vector3 before = mainCamera.transform.position;

            yield return null;
            yield return null;
            yield return null;

            Vector3 after = mainCamera.transform.position;
            float movedDistance = Vector3.Distance(before, after);

            Assert.Greater(movedDistance, 0.001f, $"Camera did not move. Distance={movedDistance}");

            controllerType.GetField("MoveInputOverride")?.SetValue(null, null);
            controllerType.GetField("LookInputOverride")?.SetValue(null, null);
            controllerType.GetField("SprintHeldOverride")?.SetValue(null, null);
            controllerType.GetField("KeyQOverride")?.SetValue(null, null);
            controllerType.GetField("KeyEOverride")?.SetValue(null, null);
            controllerType.GetField("EscapeDownOverride")?.SetValue(null, null);
            controllerType.GetField("RightMouseDownOverride")?.SetValue(null, null);
        }
    }
}
