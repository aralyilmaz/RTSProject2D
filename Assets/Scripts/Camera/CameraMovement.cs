using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = +1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = +1f;

        Vector3 moveVector = new Vector3(moveX, moveY).normalized;
        if(TryGetComponent<IMoveVelocity>(out IMoveVelocity motor))
        {
            motor.SetVelocity(moveVector);
        }
    }
}
