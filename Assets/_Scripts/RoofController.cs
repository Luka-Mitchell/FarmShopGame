using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public GameObject roof;          // The roof (top) wall of the room
    public float roofFadeDuration = 0.2f;  // Duration of the roof fade (in seconds)
    public GameObject mainCamera;
    public float zoomedCameraSize = 10f;

    private float cameraSize;
    private bool isExiting = false;

    private void Start()
    {
        cameraSize = mainCamera.GetComponent<Camera>().orthographicSize;
    }

    private void OnDisable()
    {
        // Stop all coroutines if this object is being disabled
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        // Set the flag when the application is exiting
        isExiting = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isExiting)  // Check if the player entered the room
        {
            // Start the roof fade-out over time
            StartCoroutine(FadeRoofOut());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isExiting)  // Check if the player exited the room
        {
            // Start the roof fade-in over time
            StartCoroutine(FadeRoofIn());
        }
    }

    private IEnumerator FadeRoofOut()
    {
        SpriteRenderer roofRenderer = roof.GetComponent<SpriteRenderer>();
        float startAlpha = roofRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < roofFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / roofFadeDuration);
            roofRenderer.color = new Color(roofRenderer.color.r, roofRenderer.color.g, roofRenderer.color.b, alpha);
            float size = Mathf.Lerp(cameraSize, zoomedCameraSize, elapsedTime / roofFadeDuration);
            mainCamera.GetComponent<Camera>().orthographicSize = size;
            yield return null;
        }

        roofRenderer.color = new Color(roofRenderer.color.r, roofRenderer.color.g, roofRenderer.color.b, 0f);
    }

    private IEnumerator FadeRoofIn()
    {
        SpriteRenderer roofRenderer = roof.GetComponent<SpriteRenderer>();
        float startAlpha = roofRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < roofFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / roofFadeDuration);
            roofRenderer.color = new Color(roofRenderer.color.r, roofRenderer.color.g, roofRenderer.color.b, alpha);
            float size = Mathf.Lerp(zoomedCameraSize, cameraSize, elapsedTime / roofFadeDuration);
            mainCamera.GetComponent<Camera>().orthographicSize = size;
            yield return null;
        }

        roofRenderer.color = new Color(roofRenderer.color.r, roofRenderer.color.g, roofRenderer.color.b, 1f);
    }
}
