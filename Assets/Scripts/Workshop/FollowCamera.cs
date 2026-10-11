using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 2.2f;
    public float mouseSensitivity = 3f;
    public float smoothing = 6f;

    float _yaw;
    float _pitch = 20f;

    void Awake()
    {
        GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (Input.GetMouseButton(1))
        {
            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch - Input.GetAxis("Mouse Y") * mouseSensitivity, 5f, 70f);
        }
        distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * 0.5f, 2f, 10f);

        Quaternion orbit = Quaternion.Euler(_pitch, target.eulerAngles.y + _yaw, 0f);
        Vector3 desired = target.position + orbit * new Vector3(0f, 0f, -distance) + Vector3.up * (height - 1f);
        transform.position = Vector3.Lerp(transform.position, desired, smoothing * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}
