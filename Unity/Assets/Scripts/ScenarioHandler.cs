using UnityEngine;
using TMPro;  // TextMeshPro is used for handling text in Unity
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;  // Toolkit for VR interactions

// This class handles different gameplay scenarios, like payment processing and hacking, within the game.
public class ScenarioHandler : MonoBehaviour
{
    // Public variables that can be set from the Unity Editor.
    // These are references to various objects and components that the script will manipulate.
    [Header("General Settings")]  // Grouping general settings
    public CableController cableController;  // Reference to a script handling cable connections
    public AudioSource errorSound;  // Audio to play when an error occurs

    [Header("Payment Scenario")]  // Grouping payment-related settings
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable paymentButtonInteractable;  // XR interaction for VR, triggered when the player grabs the payment button
    public GameObject PaymentButton;  // Reference to the button used in the payment scenario
    public Material GreenMaterial;  // Green color for button (success)
    public Material RedMaterial;  // Red color for button (error)
    public TextMeshProUGUI errorMessageText;  // Text field to display error messages

    [Header("Hacker Scenario")]  // Grouping hacker-related settings
    public GameObject car;  // The car object to be stolen in the hacker scenario
    public Transform carStartLocation;  // Where the car starts
    public Transform carEndLocation;  // Where the car is moved to when stolen
    public float CarSpeed = 5f;  // Speed at which the car moves
    public float TimeBeforeCarStolen = 10f;  // Delay before the car gets stolen

    // Private variables that are used internally in the script.
    private Renderer buttonRenderer;  // Renderer component of the PaymentButton
    private bool scenarioStarted = false;  // To track if a scenario has started
    private bool isHackerScenario = false;  // To track if the hacker scenario is active
    private bool bothCablesConnected = false;  // To track if both cables are connected in the game
    private bool carPlugDisconnected = false;  // To track if the car plug has been disconnected
    private Coroutine carTheftCoroutine;  // To hold the ongoing coroutine for the car theft process

    // Called once when the script is initialized
    private void Start()
    {
        // Debug message to help with troubleshooting
        Debug.Log("ScenarioHandler Start() called");

        // Disable the error message text at the start
        errorMessageText.gameObject.SetActive(false);

        // Get the Renderer component of the payment button to change its color
        buttonRenderer = PaymentButton.GetComponent<Renderer>();

        // Register the event listener for when the payment button is grabbed
        paymentButtonInteractable.selectEntered.AddListener(OnPaymentButtonGrabbed);

        // Initially, disable the payment button until the correct scenario is chosen
        paymentButtonInteractable.enabled = false;

        // Set up cable connection events and car plug connection change event
        cableController.SetOnBothCablesConnected(OnBothCablesConnected);
        cableController.CarPluggedUnplugged += CarPluggedUnplugged;

        // Randomly select a scenario to start
        PickRandomScenario();
    }

    // This function is called when both cables are connected in the game.
    private void OnBothCablesConnected()
    {
        bothCablesConnected = true;

        // If the hacker scenario is active, the game waits for the car plug to be disconnected
        if (isHackerScenario)
        {
            Debug.Log("Both cables connected in hacker scenario. Waiting for car plug disconnection.");
        }
    }

    // Called when the car plug connection status changes (connected/disconnected)
    private void CarPluggedUnplugged(bool isConnected)
    {
        // If both cables are connected, the car plug is disconnected, and it's the hacker scenario, the car can be stolen
        if (bothCablesConnected && !isConnected && isHackerScenario)
        {
            carPlugDisconnected = true;
            StartCarTheftCountdown();
        }
    }

    // Start the countdown before the car is stolen
    private void StartCarTheftCountdown()
    {
        // If there's an ongoing car theft coroutine, stop it before starting a new one
        if (carTheftCoroutine != null)
        {
            StopCoroutine(carTheftCoroutine);
        }
        carTheftCoroutine = StartCoroutine(CarStolenAfterDelay());
    }

    // Randomly pick between the payment scenario and the hacker scenario
    private void PickRandomScenario()
    {
        isHackerScenario = Random.value > 0.5f;  // 50% chance for either scenario

        if (!isHackerScenario)
        {
            Debug.Log("Selected Scenario: Payment Scenario");
            RunPaymentScenario();
        }
        else
        {
            Debug.Log("Selected Scenario: Hacker Scenario");
            RunHackerScenario();
        }
    }

    // Start the payment scenario by enabling the payment button for interaction
    private void RunPaymentScenario()
    {
        paymentButtonInteractable.enabled = true;
        Debug.Log("Payment scenario running");
    }

    // Start the hacker scenario (no immediate action needed)
    private void RunHackerScenario()
    {
        Debug.Log("Hacker scenario running");
    }

    // Called when the player grabs the payment button in VR
    public void OnPaymentButtonGrabbed(SelectEnterEventArgs args)
    {
        AcknowledgePayment();
    }

    // Acknowledge the payment by changing the button color
    private void AcknowledgePayment()
    {
        StartCoroutine(ChangeButtonColor(GreenMaterial, 3));  // Turn green for 3 seconds
    }

    // Change the button color and then reset it after a delay
    private IEnumerator ChangeButtonColor(Material color, float delay)
    {
        buttonRenderer.material = color;  // Change to green
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        buttonRenderer.material = RedMaterial;  // Change back to red
        DisplayPaymentError("Payment Error: Charging failed.");
    }

    // Show an error message and play an error sound
    private void DisplayPaymentError(string message)
    {
        errorMessageText.gameObject.SetActive(true);  // Display the error message
        errorMessageText.text = message;  // Set the text of the error message
        errorSound.Play();  // Play the error sound
    }

    // Wait for a delay and then steal the car if the car plug is disconnected
    private IEnumerator CarStolenAfterDelay()
    {
        Debug.Log("Car plug disconnected. Waiting for " + TimeBeforeCarStolen + " seconds before car is stolen");
        yield return new WaitForSeconds(TimeBeforeCarStolen);

        if (carPlugDisconnected)
        {
            StartCoroutine(MoveCar());  // Move the car if conditions are met
        }
    }

    // Move the car from the start to the end location over time
    private IEnumerator MoveCar()
    {
        float startTime = Time.time;  // Record when the movement starts
        float journeyLength = Vector3.Distance(carStartLocation.position, carEndLocation.position);  // Calculate the distance to move

        // Reset car position and rotation
        car.transform.position = carStartLocation.position;
        car.transform.rotation = carStartLocation.rotation;

        Debug.Log("Car is being stolen and moving from " + carStartLocation.position + " to " + carEndLocation.position);

        // Move the car over time based on its speed
        while (Time.time - startTime < CarSpeed)
        {
            float distCovered = (Time.time - startTime) * (journeyLength / CarSpeed);  // Calculate distance covered
            float fractionOfJourney = distCovered / journeyLength;  // Get the fraction of the journey completed

            car.transform.position = Vector3.Lerp(carStartLocation.position, carEndLocation.position, fractionOfJourney);  // Move position
            car.transform.rotation = Quaternion.Lerp(carStartLocation.rotation, carEndLocation.rotation, fractionOfJourney);  // Rotate accordingly
            yield return null;
        }

        // Set final position and rotation once the car reaches the end
        car.transform.position = carEndLocation.position;
        car.transform.rotation = carEndLocation.rotation;
        Debug.Log("Car reached the stolen endpoint");
    }
}
