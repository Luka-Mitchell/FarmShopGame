using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    //public float smoothSpeed = 0.125f;  // Adjust for smoothness
    public Vector3 offset;  // Offset from the player's position

    private void LateUpdate()
    {
        if (player != null)
        {
            // Desired position with offset
            Vector3 desiredPosition = player.position + offset;

            // Smoothly interpolate to the desired position
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update camera position
            transform.position = desiredPosition;
        }
    }
}
