using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    static private PlayerShip _S;
    static public PlayerShip S
    {
        get
        {
            return _S;
        }
        private set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to set PlayerShip singleton _S.");
            }
            _S = value;
        }
    }

    [Header("Set in Inspector")]
    public float shipSpeed = 10f;
    public GameObject bulletPrefab;

    Rigidbody rigid;


    private void Start()
    {
        S = this;
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float aX = CrossPlatformInputManager.GetAxis("Horizontal");
        float aY = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 vel = new Vector3(aX, aY);
        if (vel.magnitude > 1)
        {
            vel.Normalize();
        }

        rigid.velocity = vel * shipSpeed;

        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    void Fire()
    {

    }

    static public float MAX_SPEED
    {
        get
        {
            return _S.shipSpeed;
        }
        set
        {
            // NOTE: This is a bad estimation of the purpose for a MAX_SPEED property.
            _S.shipSpeed = value;
        }
    }
}
