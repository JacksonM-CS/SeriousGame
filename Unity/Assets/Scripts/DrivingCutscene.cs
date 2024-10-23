using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

// This script controls a driving cutscene where the player is moved from a start point to an end point in a car, 
// with a fade-in effect at the beginning and then transitioning the player to a spawn location at the end of the cutscene.
public class DrivingCutscene : MonoBehaviour
{
    // Public variables that can be assigned in the Unity Editor to customize the cutscene behavior.
    public Transform StartPoint;       // Starting position of the car
    public Transform EndPoint;         // Ending position of the car
    public float TravelTime = 5f;      // Time it takes for the car to travel from the start to the end
    public CanvasGroup ScreenFade;     // UI element for screen fading in and out
    public GameObject Car;             // The car game object that will move during the cutscene
    public GameObject xrRig;           // XR Rig (the player in VR), which will be parented to the car during the cutscene
    public Transform SpawnLocation;    // Location where the player (xrRig) will be placed at the end of the cutscene

    // Private variables for internal state tracking
    private float StartTime;           // Timestamp when the cutscene starts
    private bool CutsceneIsPlaying = false; // Boolean to track if the cutscene is currently playing

    // Called when the script is first initialized
    void Start()
    {
        StartCoroutine(ScreenFadeIn());  // Begin the screen fade-in effect and start the cutscene
    }

    // Coroutine that fades in the screen by gradually reducing the alpha value of the ScreenFade CanvasGroup
    IEnumerator ScreenFadeIn()
    {
        ScreenFade.alpha = 1;  // Start with a fully opaque screen
        while (ScreenFade.alpha > 0)  // Continue fading in until the screen is fully transparent
        {
            ScreenFade.alpha -= Time.deltaTime;  // Reduce alpha over time to create the fade effect
            yield return null;  // Wait for the next frame before continuing
        }

        // Once the screen has fully faded in, start the driving cutscene
        StartDriveCutscene();
    }

    // Function that sets up and starts the driving cutscene
    void StartDriveCutscene()
    {
        // If any of the necessary objects (start, end points, car, or player rig) are missing, stop the cutscene setup
        if (StartPoint == null || EndPoint == null || Car == null || xrRig == null)
        {
            return;
        }

        // Attach the player (xrRig) to the car so that the player moves with the car during the cutscene
        xrRig.transform.SetParent(Car.transform);

        // Position the player inside the car (relative to the car's transform)
        xrRig.transform.localPosition = Vector3.zero;

        // Record the time when the cutscene starts
        StartTime = Time.time;

        // Set the cutscene flag to true
        CutsceneIsPlaying = true;

        // Start the coroutine that moves the car from the start to the end point
        StartCoroutine(DriveToCS());
    }

    // Coroutine that moves the car from StartPoint to EndPoint over the specified TravelTime
    IEnumerator DriveToCS()
    {
        // Calculate the total distance of the journey between the start and end points
        float journeyLength = Vector3.Distance(StartPoint.position, EndPoint.position);

        // Continue moving the car until the travel time has elapsed
        while (Time.time - StartTime < TravelTime)
        {
            // Calculate how far the car has traveled based on the time elapsed and the total journey length
            float distCovered = (Time.time - StartTime) * (journeyLength / TravelTime);
            float fractionOfJourney = distCovered / journeyLength;  // Normalize to a fraction between 0 and 1

            // Move the car's position smoothly along the path from StartPoint to EndPoint
            Car.transform.position = Vector3.Lerp(StartPoint.position, EndPoint.position, fractionOfJourney);

            yield return null;  // Wait for the next frame before continuing
        }

        // Once the car reaches the destination, call the function to handle the end of the cutscene
        LocationReached();
    }

    // Function called when the car reaches the destination (EndPoint)
    void LocationReached()
    {
        // Set the flag to indicate the cutscene is no longer playing
        CutsceneIsPlaying = false;

        // Detach the player (xrRig) from the car, allowing them to be independent again
        xrRig.transform.SetParent(null);

        // Move the player (xrRig) to the specified spawn location after the cutscene
        xrRig.transform.position = SpawnLocation.position;
        xrRig.transform.rotation = SpawnLocation.rotation;
    }
}
