using UnityEngine;

public class UnitMotor : MonoBehaviour, IMoveVelocity
{
    [SerializeField]
    private float moveSpeed = 1f;

    private Vector3 velocity;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        PerformMovement();
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        targetPosition.z = 0;
        this.targetPosition = targetPosition;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        moveDirection.z = 0;
        SetVelocity(moveDirection);
    }

    private void PerformMovement()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.02f)
        {
            transform.position += velocity * moveSpeed * Time.deltaTime;
        }
    }
}
