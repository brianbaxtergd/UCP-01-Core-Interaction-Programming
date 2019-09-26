// These were used to test a case where some Asteroids were getting lost off screen.
// #define DEBUG_Asteroid_TestOOBVel 
// #define DEBUG_Asteroid_ShotOffscreenDebugLines

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if DEBUG_Asteroid_TestOOBVel
using UnityEditor;
#endif

public class Asteroid : MonoBehaviour {
    public const int    INITIAL_SIZE = 3;

    static Text         SCORE_GT;
    static int[]        SCORE_VALUES_BY_SIZE = { 0, 400, 200, 100};

    //public AsteroidsScriptableObject AsteroidsScriptableObject.S;

    public int          size = 3;

    Rigidbody           rigid; // protected
    OffScreenWrapper    offScreenWrapper;
    

#if DEBUG_Asteroid_ShotOffscreenDebugLines
	bool                trackOffscreen;
	Vector3             trackOffscreenOrigin;
#endif

	private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        offScreenWrapper = GetComponent<OffScreenWrapper>();
	}

	// Use this for initialization
	void Start () {
        AsteraX.AddAsteroid(this);

		transform.localScale = Vector3.one * size * AsteroidsScriptableObject.S.asteroidScale;
        if (parentIsAsteroid) {
            InitAsteroidChild();
        } else {
            InitAsteroidParent();
        }

        // Spawn child Asteroids
        if (size > 1) {
            Asteroid ast;
            for (int i=0; i<AsteroidsScriptableObject.S.numSmallerAsteroidsToSpawn; i++) {
                ast = SpawnAsteroid();
                ast.size = size-1;
                ast.transform.SetParent(transform);
                Vector3 relPos = Random.onUnitSphere / 2;
                ast.transform.rotation = Random.rotation;
                ast.transform.localPosition = relPos;

                ast.gameObject.name = gameObject.name + "_" + i.ToString("00");
            }
        }
	}

    private void OnDestroy()
    {
        AsteraX.RemoveAsteroid(this);
    }

    public void InitAsteroidParent() {
#if DEBUG_Asteroid_ShotOffscreenDebugLines
		Debug.LogWarning(gameObject.name+" InitAsteroidParent() "+Time.time);
#endif

        offScreenWrapper.enabled = true;
        rigid.isKinematic = false;
        // Snap this GameObject to the z=0 plane
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        // Initialize the velocity for this Asteroid
        InitVelocity();
    }

    public void InitAsteroidChild() {
        offScreenWrapper.enabled = false;
        rigid.isKinematic = true;
        transform.localScale = transform.localScale.ComponentDivide(transform.parent.lossyScale);
    }

    public void InitVelocity() {
        Vector3 vel;

        // The initial velocity depends on whether the Asteroid is currently off screen or not
        if (ScreenBounds.OOB( transform.position ))
        {
            // If the Asteroid is out of bounds, just point it toward a point near the center of the sceen
            vel = ((Vector3) Random.insideUnitCircle*4) - transform.position;
            vel.Normalize();
#if DEBUG_Asteroid_TestOOBVel
            Debug.LogWarning("Asteroid:InitVelocity() - " + gameObject.name + " is OOB. Vel is: " + vel);
            EditorApplication.isPaused = true;
#endif

#if DEBUG_Asteroid_ShotOffscreenDebugLines
			Debug.DrawLine(transform.position, transform.position+vel, Color.red, 60);
			Debug.DrawLine(transform.position+Vector3.down, transform.position+Vector3.up, Color.cyan, 60);
            Debug.DrawLine(transform.position+Vector3.left, transform.position+Vector3.right, Color.cyan, 60);
			trackOffscreen = true;
			trackOffscreenOrigin = transform.position;
#endif
        }
        else
        {
            // If in bounds, choose a random direction, and make sure that when you Normalize it, it doesn't
            //  have a length of 0 (which might happen if Random.insideUnitCircle returned [0,0,0].
            do {
                vel = Random.insideUnitCircle;
                vel.Normalize();
            } while ( Mathf.Approximately(vel.magnitude, 0f) );
        }

        // Multiply the unit length of vel by the correct speed (randomized) for this size of Asteroid
        vel = vel * Random.Range(AsteroidsScriptableObject.S.minVel, AsteroidsScriptableObject.S.maxVel) / (float) size;
        rigid.velocity = vel;

        rigid.angularVelocity = Random.insideUnitSphere * AsteroidsScriptableObject.S.maxAngularVel;
    }



#if DEBUG_Asteroid_ShotOffscreenDebugLines
	private void FixedUpdate()
	{
		if (trackOffscreen) {
			Debug.DrawLine(trackOffscreenOrigin, transform.position, Color.yellow, 0.1f);
		}
	}
#endif

	//Vector3 ForceVelToKeepAsteroidOnScreen(Vector3 vel)
	//{
	//    // Make sure that if this Asteroid is inited out of bounds (which sometimes happens with child
	//    //  Asteroids if the parent is shot very near the edge of the screen), vel will move it on screen.
	//    int oob;
	//    oob = ScreenBounds.OOB_X(transform.position);
	//    if (oob != 0)
	//    {
	//        vel.x = Mathf.Abs(vel.x) * -oob;
	//    }

	//    oob = ScreenBounds.OOB_Y(transform.position);
	//    if (oob != 0)
	//    {
	//        vel.y = Mathf.Abs(vel.y) * -oob;
	//    }

	//    oob = ScreenBounds.OOB_Z(transform.position);
	//    if (oob != 0)
	//    {
	//        vel.z = Mathf.Abs(vel.z) * -oob;
	//    }

	//    return vel;
	//}

	// NOTE: Allowing parentIsAsteroid and parentAsteroid to call GetComponent<> every
	//  time is inefficient, however, this only happens when a bullet hits an Asteroid
	//  which is rarely enough that it isn't a performance hit.
	bool parentIsAsteroid {
        get {
            return (parentAsteroid != null);
        }
    }

    Asteroid parentAsteroid {
        get {
            if (transform.parent != null) {
                return transform.parent.GetComponent<Asteroid>();
            }
            return null;
        }
    }

    public void OnCollisionEnter(Collision coll) {
        // If this is the child of another Asteroid, pass this collision up the chain
        if (parentIsAsteroid) {
            parentAsteroid.OnCollisionEnter(coll);
            return;
        }

        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "Bullet") {
            Destroy(otherGO);
            if (size > 1) {
                //// Spawn smaller Asteroids
                //for (int i=0; i<numSmallerAsteroidsToSpawn; i++) {
                //    GameObject go = Instantiate<GameObject>(gameObject);
                //    Asteroid a = go.GetComponent<Asteroid>();
                //    a.size = size-1;
                //}
                // Detach the children
                Asteroid[] children = GetComponentsInChildren<Asteroid>();
                for (int i=0; i<children.Length; i++) {
                    if (children[i] == this || children[i].transform.parent != transform) {
                        continue;
                    }
                    children[i].transform.SetParent(null, true);
                    children[i].InitAsteroidParent();
                }
            }
            // Add some scoring here
            if (SCORE_GT == null) {
                GameObject go = GameObject.Find("ScoreGT");
                if (go != null) {
                    SCORE_GT = go.GetComponent<Text>();
                } else {
                    Debug.LogError("Asteroid:OnCollisionEnter() - Could not find a GameObject named ScoreGT!!!!");
                    return;
                }
            }
            string s = SCORE_GT.text.Replace(",","");
            int score = int.Parse( s );
            score += SCORE_VALUES_BY_SIZE[size];
            SCORE_GT.text = score.ToString("N0");

            Destroy(gameObject);
        }
    }
    
    static public Asteroid SpawnAsteroid()
    {
        GameObject aGO = Instantiate<GameObject>(AsteroidsScriptableObject.S.GetAsteroidPrefab());
        Asteroid ast = aGO.GetComponent<Asteroid>();
        return ast;
    }
}
