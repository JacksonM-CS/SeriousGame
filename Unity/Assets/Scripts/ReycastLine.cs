using UnityEngine;
using TMPro;
using System.Collections;


// This script draws a raycast line for the XR ray interactor (used in VR applications) and updates the position of the line
// based on what the ray is pointing at (either an object hit by the ray or the max ray distance).
public class ReycastLine : MonoBehaviour
{
    // Public references to the ray interactor and line renderer components
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor RayInteractor;  // The XR ray interactor (used for pointing at objects)
    public LineRenderer LineRenderer;      // The line renderer used to draw the ray in the scene

    // Called once per frame to update the raycast line's position
    void Update()
    {
        // Check if the ray from the RayInteractor is hitting an object in the scene
        if (RayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // If the ray hits something, set the start point at the ray origin and the end point at the hit location
            LineRenderer.SetPosition(0, RayInteractor.transform.position);  // Start of the ray
            LineRenderer.SetPosition(1, hit.point);  // End of the ray at the hit point
        }
        else
        {
            // If the ray doesn't hit anything, extend it to its maximum distance
            LineRenderer.SetPosition(0, RayInteractor.transform.position);  // Start of the ray
            LineRenderer.SetPosition(1, RayInteractor.transform.position + RayInteractor.transform.forward * RayInteractor.maxRaycastDistance);  // End of the ray at max distance
        }
    }
}
