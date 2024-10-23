using UnityEngine;
using TMPro;
using System.Collections;


// This script handles the interaction of a button in a VR environment, triggering a payment scenario when the button is grabbed.
public class ButtonPush : MonoBehaviour
{
    // Reference to the ScenarioHandler, which manages game scenarios like payments
    private ScenarioHandler ScenarioHandler;

    // Called when the script is initialized
    private void Start()
    {
        // Find the ScenarioHandler in the scene (which manages scenarios like payment processing)
        ScenarioHandler = FindObjectOfType<ScenarioHandler>();

        // Get the XR Grab Interactable component attached to this button
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // If the button has a grab interactable component, add an event listener for when the button is grabbed
        if (interactable != null)
        {
            // Link the grab event to the ScenarioHandler's OnPaymentButtonGrabbed method
            interactable.selectEntered.AddListener(ScenarioHandler.OnPaymentButtonGrabbed);
        }
    }
}
