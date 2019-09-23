//#define DEBUG_TurretPointAtMouse_DrawMousePoint

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPointAtMouse : MonoBehaviour
{

#if DEBUG_TurretPointAtMouse_DrawMousePoint
    public bool DrawMousePoint = false;
#endif

    private Vector3 mousePoint3D;


    void Update()
    {
        PointAtMouse();
    }

    void PointAtMouse()
    {
        mousePoint3D = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);

        transform.LookAt(mousePoint3D, Vector3.back);
    }

#if DEBUG_TurretPointAtMouse_DrawMousePoint
    private void OnDrawGizmos()
    {
        if (DrawMousePoint && Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(mousePoint3D, 0.2f);
            Gizmos.DrawLine(transform.position, mousePoint3D);
        }
    }
#endif
}
