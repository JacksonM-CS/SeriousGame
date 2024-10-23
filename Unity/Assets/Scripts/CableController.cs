using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

// This script manages the connection of two cable ends to different sockets and provides feedback (visual and audio) 
// when the connections are made or broken. It also triggers events when both cables are connected.
public class CableController : MonoBehaviour
{
    // Public variables that can be set from the Unity Editor.
    // These represent the main parts of the cable system (the cable ends) and the socket components (where the cables are plugged in).
    [Header("Cable Components")]  // Group for cable parts
    public GameObject CableHead;  // The first end of the cable
    public GameObject CableEnd;   // The second end of the cable

    [Header("Socket Components")]  // Group for socket-related variables
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor StationSocketInteractor;  // The socket where the CableHead will be connected
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor CarSocketInteractor;      // The socket where the CableEnd will be connected

    [Header("Connection Feedback")]  // Group for feedback settings (visual/audio)
    public Material ConnectedMaterial;    // Material used when the cable is connected
    public Material DisconnectedMaterial; // Material used when the cable is disconnected
    public AudioSource ConnectionSound;   // Sound to play when a cable is connected
    public AudioSource DisconnectionSound;// Sound to play when a cable is disconnected

    // Private variables for internal tracking of the cable connection status
    private bool IsCableHeadConnected = false;  // Track if the head of the cable is connected
    private bool IsCableEndConnected = false;   // Track if the end of the cable is connected

    // Variables to store the grab interactables for each cable end (for XR interactions)
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable HeadGrabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable EndGrabInteractable;

    // Action that triggers when both cables are connected, used by other scripts
    private System.Action BothCablesConnected;  
    public System.Action<bool> CarPluggedUnplugged;  // Action to trigger when the Car plug is connected/disconnected

    // Called when the script is first initialized
    private void Start()
    {
        Debug.Log("CableController Start() called");

        // Get the XR grab interactable components attached to the cable ends
        HeadGrabInteractable = CableHead.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        EndGrabInteractable = CableEnd.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Register events for the station and car socket interactions (connect/disconnect)
        StationSocketInteractor.selectEntered.AddListener(OnStationSocketConnected);
        StationSocketInteractor.selectExited.AddListener(StationUnplugged);
        CarSocketInteractor.selectEntered.AddListener(OnCarSocketConnected);
        CarSocketInteractor.selectExited.AddListener(CarUnplugged);

        // Update the materials for the cable ends to reflect their current connection status
        UpdateCableMaterials();
    }

    // This function is called when the CableHead is connected to the station socket
    private void OnStationSocketConnected(SelectEnterEventArgs args)
    {
        // Check if the object being connected is the CableHead
        if (args.interactableObject.transform.gameObject == CableHead)
        {
            IsCableHeadConnected = true;  // Mark the cable head as connected
            UpdateCableMaterials();  // Update visual feedback
            ConnectionSound?.Play();  // Play the connection sound
            CheckBothConnected();  // Check if both ends of the cable are connected
        }
    }

    // This function is called when the CableHead is disconnected from the station socket
    private void StationUnplugged(SelectExitEventArgs args)
    {
        // Check if the object being disconnected is the CableHead
        if (args.interactableObject.transform.gameObject == CableHead)
        {
            IsCableHeadConnected = false;  // Mark the cable head as disconnected
            UpdateCableMaterials();  // Update visual feedback
            DisconnectionSound?.Play();  // Play the disconnection sound
        }
    }

    // This function is called when the CableEnd is connected to the car socket
    private void OnCarSocketConnected(SelectEnterEventArgs args)
    {
        // Check if the object being connected is the CableEnd
        if (args.interactableObject.transform.gameObject == CableEnd)
        {
            IsCableEndConnected = true;  // Mark the cable end as connected
            UpdateCableMaterials();  // Update visual feedback
            ConnectionSound?.Play();  // Play the connection sound
            CarPluggedUnplugged?.Invoke(true);  // Notify that the car plug has been connected
            CheckBothConnected();  // Check if both ends of the cable are connected
        }
    }

    // This function is called when the CableEnd is disconnected from the car socket
    private void CarUnplugged(SelectExitEventArgs args)
    {
        // Check if the object being disconnected is the CableEnd
        if (args.interactableObject.transform.gameObject == CableEnd)
        {
            IsCableEndConnected = false;  // Mark the cable end as disconnected
            UpdateCableMaterials();  // Update visual feedback
            DisconnectionSound?.Play();  // Play the disconnection sound
            CarPluggedUnplugged?.Invoke(false);  // Notify that the car plug has been disconnected
        }
    }

    // Update the materials of the cable ends to reflect their connection status
    private void UpdateCableMaterials()
    {
        // Set the material based on the connection status of the CableHead
        UpdateCableEndMaterial(CableHead, IsCableHeadConnected);

        // Set the material based on the connection status of the CableEnd
        UpdateCableEndMaterial(CableEnd, IsCableEndConnected);
    }

    // Helper function to update the material of a specific cable end
    private void UpdateCableEndMaterial(GameObject CableEnd, bool Connected)
    {
        // Choose the appropriate material based on whether the cable end is connected
        Material newMaterial = Connected ? ConnectedMaterial : DisconnectedMaterial;

        // Get the Renderer component to change the material
        var material = CableEnd.GetComponent<Renderer>();
        if (material != null)
        {
            material.material = newMaterial;  // Apply the new material
        }
    }

    // Check if both cable ends are connected, and if so, trigger the event to start the scenario
    private void CheckBothConnected()
    {
        // If both ends are connected and a callback is set, trigger the event
        if (IsCableHeadConnected && IsCableEndConnected && BothCablesConnected != null)
        {
            Debug.Log("Both cables connected, starting scenario");
            BothCablesConnected.Invoke();  // Call the event (triggers the scenario)
            BothCablesConnected = null;  // Ensure the event is only triggered once
        }
    }

    // Function to check if both cable ends are connected (can be called from other scripts)
    public bool BothConnected()
    {
        return IsCableHeadConnected && IsCableEndConnected;
    }

    // Function to set the callback to be triggered when both cables are connected
    public void SetOnBothCablesConnected(System.Action callback)
    {
        BothCablesConnected = callback;
    }
}
