using ECM.Components;
using ECM.Controllers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace ECM.EditorTools
{
    public static class ECMMenu
    {
        [MenuItem("GameObject/ECM/Character", false, 0)]
        public static void CreateCharacter()
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("ECM_Character", typeof(Rigidbody), typeof(CapsuleCollider),
                typeof(GroundDetection), typeof(CharacterMovement), typeof(BaseCharacterController));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material =
                AssetDatabase.LoadAssetAtPath<PhysicMaterial>(
                    "Assets/Easy Character Movement/Physic Materials/Frictionless.physicMaterial");

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = new PhysicMaterial("Frictionless")
                {
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    bounciness = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Multiply,
                    bounceCombine = PhysicMaterialCombine.Multiply
                };

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/ECM/Agent", false, 0)]
        public static void CreateAgent()
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("ECM_Agent", typeof(NavMeshAgent), typeof(Rigidbody),
                typeof(CapsuleCollider), typeof(GroundDetection), typeof(CharacterMovement),
                typeof(BaseAgentController));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material =
                AssetDatabase.LoadAssetAtPath<PhysicMaterial>(
                    "Assets/Easy Character Movement/Physic Materials/Frictionless.physicMaterial");

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = new PhysicMaterial("Frictionless")
                {
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    bounciness = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Multiply,
                    bounceCombine = PhysicMaterialCombine.Multiply
                };

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem("GameObject/ECM/First Person", false, 0)]
        public static void CreateFirstPerson()
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("ECM_FirstPerson", typeof(Rigidbody), typeof(CapsuleCollider),
                typeof(GroundDetection), typeof(CharacterMovement), typeof(BaseFirstPersonController), typeof(MouseLook));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material =
                AssetDatabase.LoadAssetAtPath<PhysicMaterial>(
                    "Assets/Easy Character Movement/Physic Materials/Frictionless.physicMaterial");

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = new PhysicMaterial("Frictionless")
                {
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    bounciness = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Multiply,
                    bounceCombine = PhysicMaterialCombine.Multiply
                };

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Add child camera

            var o = new GameObject("Camera", typeof(Camera), typeof(AudioListener), typeof(FlareLayer));

            o.transform.SetParent(gameObject.transform);
            o.transform.localPosition = new Vector3(0f, 1.6f, 0.0f);

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }
}