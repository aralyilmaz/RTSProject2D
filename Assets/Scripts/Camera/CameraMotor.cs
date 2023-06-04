using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    private Vector3 velocity;

    private void FixedUpdate()
    {
        PerformMovement();
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    private void PerformMovement()
    {
        transform.position += velocity * moveSpeed * Time.deltaTime;
    }
}
