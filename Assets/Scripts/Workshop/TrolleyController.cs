using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrolleyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float turnSpeed = 120f;

    Rigidbody _body;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float forward = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 velocity = transform.forward * forward * moveSpeed;
        velocity.y = _body.linearVelocity.y;
        _body.linearVelocity = velocity;
        _body.MoveRotation(_body.rotation * Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f));
    }
}
