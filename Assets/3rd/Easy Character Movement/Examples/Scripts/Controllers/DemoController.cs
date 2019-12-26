using ECM.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace ECM.Examples
{
    public sealed class DemoController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public FollowCameraController mainCamera;

        public BaseCharacterController[] characterControllers = new BaseCharacterController[6];

        public Text titleText;
        public Text descriptionText;
        public Text rootMotionText;

        #endregion

        #region FIELDS

        private int _currentControllerId;

        private readonly string[] _titles =
        {
            "Base Character Controller",
            "Base Agent Controller",
            "Base First Person Controller",

            "Ethan Character Controller",
            "Ethan Agent Controller",
            "Ethan Platformer Controller"
        };

        private readonly string[] _descriptions = new string[6];

        #endregion

        #region PROPERTIES

        private BaseCharacterController currentController
        {
            get { return characterControllers[currentControllerId]; }
        }

        public int currentControllerId
        {
            get { return _currentControllerId; }
            set
            {
                if (_currentControllerId == value)
                    return;

                // Disable previous controller

                currentController.gameObject.SetActive(false);

                // If new controller is FPS, disable follow camera

                if (value == 2)
                    mainCamera.gameObject.SetActive(false);

                // Activate new controller

                _currentControllerId = value;
                currentController.gameObject.SetActive(true);

                titleText.text = _titles[_currentControllerId];
                descriptionText.text = _descriptions[_currentControllerId];

                // If new controller is not FPS, assign follow camera target to new controller

                if (_currentControllerId == 2)
                    return;

                mainCamera.gameObject.SetActive(true);
                mainCamera.targetTransform = currentController.transform;
            }
        }

        #endregion

        #region METHODS

        public void ActivateBaseCharacterController()
        {
            currentControllerId = 0;
        }

        public void ActivateBaseAgentController()
        {
            currentControllerId = 1;
        }

        public void ActivateBaseFirstPersonController()
        {
            currentControllerId = 2;
        }

        public void ActivateEthanCharacterController()
        {
            currentControllerId = 3;
        }

        public void ActivateEthanAgentController()
        {
            currentControllerId = 4;
        }

        public void ActivateEthanPlatformerController()
        {
            currentControllerId = 5;
        }

        #endregion

        #region MONOBEHAVIOUR

        public void Awake()
        {
            // Initialize descriptions

            _descriptions[0] = "<b>WASD</b> to move\n" +
                               "<b>Space</b> to jump";

            _descriptions[1] = "<b>Mouse Right-Click</b> to move";

            _descriptions[2] = "<b>ESC</b> to unlock cursor\n" +
                               "<b>WASD</b> to move\n" +
                               "<b>Mouse</b> to look\n" +
                               "<b>Left Shift</b> (hold) to run\n" +
                               "<b>Space</b> to jump\n";

            _descriptions[3] = "<b>WASD</b> to move\n" +
                               "<b>Space</b> to jump\n" +
                               "<b>R</b> to toggle root motion";

            _descriptions[4] = "<b>Mouse Right-Click</b> to move\n" +
                               "<b>R</b> to toggle root motion";

            _descriptions[5] = "<b>WASD</b> to move\n" +
                               "<b>Space</b> to double jump\n" +
                               "<b>R</b> to toggle root motion";

            currentControllerId = 0;
        }

        public void Update()
        {
            if (currentControllerId < 3)
                rootMotionText.enabled = false;
            else
            {
                if (Input.GetKeyDown(KeyCode.R))
                    currentController.useRootMotion = !currentController.useRootMotion;

                rootMotionText.enabled = true;
                rootMotionText.text = currentController.useRootMotion
                    ? "<b>Using Root Motion:</b> True"
                    : "<b>Using Root Motion:</b> False";
            }
        }

        #endregion
    }
}