using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(OffScreenWrapper))]
public class Bullet : MonoBehaviour
{
    static private Transform _BULLET_ANCHOR;
    static Transform BULLET_ANCHOR
    {
        get
        {
            if (_BULLET_ANCHOR == null)
            {
                GameObject go = new GameObject("BulletAnchor");
                _BULLET_ANCHOR = go.transform;
            }
            return _BULLET_ANCHOR;
        }
    }

    public float bulletSpeed = 20f;
    public float lifeTime = 2f;


    void Start()
    {
        transform.SetParent(BULLET_ANCHOR, true);

        // Set bullet to self-destruct in lifeTime seconds.
        Invoke("DestroyMe", lifeTime);

        // Set velocity of the Bullet.
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
