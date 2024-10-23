using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

// This script handles basic game initialization, such as positioning the player at the start location when the game begins.
public class GameManager : MonoBehaviour
{
    // Public reference to the start location where the player should be placed at the beginning of the game
    public Transform StartLocation;

    // Called once when the game starts
    void Start()
    {
        // Check if a StartLocation is assigned, then find the player (by tag) and move them to the StartLocation
        if (StartLocation != null)
        {
            // Find the player object by its tag ("Player") and set its position to the StartLocation
            GameObject.FindGameObjectWithTag("Player").transform.position = StartLocation.position;
        }
    }
}
