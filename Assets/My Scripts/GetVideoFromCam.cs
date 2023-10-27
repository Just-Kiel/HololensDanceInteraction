using UnityEngine;

public class GetVideoFromCam : MonoBehaviour
{
    static WebCamTexture webcamTexture;
    // Define a material to display the webcam feed
    public Material displayMaterial;
    public int indexCam;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice device = WebCamTexture.devices[indexCam];
        if (webcamTexture == null)
            // Create a new WebCamTexture with the first available webcam
            webcamTexture = new WebCamTexture(device.name);

        //GetComponent<Renderer>().material.mainTexture = webcamTexture;
        // Assign the webcam texture to the material
        displayMaterial.mainTexture = webcamTexture;

        if (!webcamTexture.isPlaying)
            webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any additional logic you want to perform on each frame
    }
}
