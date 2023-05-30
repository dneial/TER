using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 5f;    // Speed at which the camera zooms in and out
    public float minZoomDistance = 5f;    // Minimum allowed zoom distance
    public float maxZoomDistance = 20f;   // Maximum allowed zoom distance


    public Transform target; 


    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z - 10f);
    }

    void Update()
    {
        // Get the scroll wheel input value
        float scrollValue = -Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new zoom distance based on the scroll value
        float newZoomDistance = transform.position.z - scrollValue * zoomSpeed;

        // Set the new camera position
        transform.position = new Vector3(transform.position.x, transform.position.y, newZoomDistance);



        if(Input.GetKeyDown(KeyCode.Z)) {
            transform.position = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            transform.position = new Vector3(transform.position.x, transform.position.y-0.5f, transform.position.z);
        }
        if(Input.GetKeyDown(KeyCode.Q)) {
            transform.position = new Vector3(transform.position.x-0.5f, transform.position.y, transform.position.z);
        }
        if(Input.GetKeyDown(KeyCode.D)) {
            transform.position = new Vector3(transform.position.x+0.5f, transform.position.y, transform.position.z);
        }
    }
}
