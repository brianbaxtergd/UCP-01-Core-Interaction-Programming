using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(BoxCollider))]
public class OffScreenWrapper : MonoBehaviour
{
    void Update()
    {
        // Adding an Update() method shows the "enabled" checkbox in the inspector.
    }

    // This is called whenever this GameObject exits the bound of OnScreenBounds.
    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;

        // Ensure the other is OnScreenBounds.
        ScreenBounds bounds = other.GetComponent<ScreenBounds>();
        if (bounds == null)
            return;

        ScreenWrap(bounds);
    }

    private void ScreenWrap(ScreenBounds bounds)
    {
        // Wrap whichever direction is necessary.
        Vector3 relativeLoc = bounds.transform.InverseTransformPoint(transform.position);
        // relativeLoc is in the local coords of OnScreenBounds, 0.5f is the screen edge.
        if (Mathf.Abs(relativeLoc.x) > 0.5f)
        {
            relativeLoc.x *= -1;
        }
        if (Mathf.Abs(relativeLoc.y) > 0.5f)
        {
            relativeLoc.y *= -1;
        }
        transform.position = bounds.transform.TransformPoint(relativeLoc);
    }
}
