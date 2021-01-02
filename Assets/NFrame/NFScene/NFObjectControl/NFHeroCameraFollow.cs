using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFHeroCameraFollow : MonoBehaviour 
{
    [Header("Camera")]
    [Tooltip("Reference to the target GameObject.")]
    public Transform target;
    private float sensitivity = 120f;
    [Tooltip("Current relative offset to the target.")]
    private Vector3 offset = new Vector3(0, 1, -10);
    [Tooltip("Minimum relative offset to the target GameObject.")]
    private Vector3 minOffset = new Vector3(0, 0, -17);
	[Tooltip("Maximum relative offset to the target GameObject.")]
    private Vector3 maxOffset = new Vector3(0, 0, -4);


    private Transform cameraTransform;
    private Vector2 cameraRotation;

    void Awake()
    {
        cameraRotation.x = -45;
        cameraRotation.y = 45;

        DontDestroyOnLoad(this.gameObject);
    }


    // Use this for initialization
    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning(gameObject.name + ": No target found!");
        }

        cameraTransform = transform;
    }

    public void SetPlayer(Transform transform)
    {
        target = transform;
    }

        // Update is called once per frame
    private void Update()
    {

    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {

        if (target && Input.GetMouseButton(1))
        {
            cameraRotation.x += Input.GetAxis("Mouse X") * sensitivity * 4 * Time.deltaTime;
            cameraRotation.y -= Input.GetAxis("Mouse Y") * sensitivity * 4 * Time.deltaTime;

            if (cameraRotation.y < 15)
            {
                cameraRotation.y = 15;
            }
            if (cameraRotation.y > 75)
            {
                cameraRotation.y = 75;
            }
        }
        if (target)
        {
            offset.z += Input.GetAxis("Mouse ScrollWheel") * sensitivity * Time.deltaTime;
            offset.z = Mathf.Clamp(offset.z, minOffset.z, maxOffset.z);

            Quaternion rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
            Vector3 position = rotation * new Vector3(offset.x, offset.y, offset.z) + target.position;

            cameraTransform.rotation = rotation;
            cameraTransform.position = position;
        }

    }
}
