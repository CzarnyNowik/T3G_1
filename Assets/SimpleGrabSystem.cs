using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region NEW_INPUT
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#endregion

/// <summary>
/// Simple example of Grabbing system.
/// </summary>
public class SimpleGrabSystem : MonoBehaviour
{
    // Reference to the character camera
    [SerializeField] private Camera characterCamera;

    // Reference to the slot for holding picked item
    [SerializeField] private Transform slot;

    // Reference to the currently held item
    private PickableItem pickedItem;

    [Header("Throw")]
    // Velocity which which object will be thrown.
    [SerializeField]
    private Vector3 throwVelocity = new Vector3(0, 0, 5);

    /// <summary>
    /// Event class which will be displayed in the inspector.
    /// </summary>
    [System.Serializable]
    public class LocationChanged : UnityEvent<Vector3, Vector3> { }

    [Space]

    // Event for location change. Used to update ballistic trajectory.
    public LocationChanged OnLocationChanged;

    /// <summary>
    /// Method called very frame.
    /// </summary>
    void Update()
    {
        // Execute logic only on button pressed

#region NEW_INPUT
        Mouse mouse = InputSystem.GetDevice<Mouse>();
        if(mouse.leftButton.wasPressedThisFrame)
#endregion

#region OLD_INPUT
        //if (Input.GetButtonDown("Fire1"))
#endregion

        {
            // Check if player picked some item already
            if (pickedItem)
            {
                // If yes, drop picked item
                DropItem(pickedItem);
            }
            else
            {
                // If no, try to pick item in front of the player
                // Create rat from center of the screen
                var ray = characterCamera.ViewportPointToRay(Vector3.one * 0.5f);
                RaycastHit hit;
                // Shot ray to find object to pick
                if (Physics.Raycast(ray, out hit, 1.5f))
                {
                    // Check if object is pickable
                    var pickable = hit.transform.GetComponent<PickableItem>();

                    // If object has PickableItem class
                    if (pickable)
                    {
                        // Pick it
                        PickItem(pickable);
                    }
                }
            }
        }
        // Broadcast location change
        OnLocationChanged?.Invoke(slot.position, slot.rotation * throwVelocity);
    }

    /// <summary>
    /// Method for picking up item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void PickItem(PickableItem item)
    {
        // Assign reference
        pickedItem = item;

        // Disable rigidbody and reser velocities
        item.Rb.isKinematic = true;
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;

        // Set Slot as parent
        item.transform.SetParent(slot);

        // Reset position and rotation
        item.transform.localPosition = Vector3.zero;
        item.transform.localEulerAngles = Vector3.zero;
    }


    /// <summary>
    /// Method for dropping item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void DropItem(PickableItem item)
    {
        // Remove reference
        pickedItem = null;

        // Remove parent
        item.transform.SetParent(null);

        // Enable rigidbody
        item.Rb.isKinematic = false;

        // Add force to throw item a little bit
        //item.Rb.AddForce(item.transform.forward * 2, ForceMode.VelocityChange);

        // Add velocity to throw the item
        item.Rb.velocity = slot.rotation * throwVelocity;
    }
}
