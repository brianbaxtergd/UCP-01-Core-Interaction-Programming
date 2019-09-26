#define DEBUG_AsteraX_LogMethods

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteraX : MonoBehaviour
{
    // Private Singleton-style instance. Accessed by static property S later in script.
    static private AsteraX _S;

    static List<Asteroid> ASTEROIDS;
    static List<Bullet> BULLETS;

    const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;

    [Header("Set in Inspector")]
    [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
    public AsteroidsScriptableObject asteroidsSO;


    private void Awake()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Awake()");
#endif
        S = this;
    }

    private void Start()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Start()");
#endif

        ASTEROIDS = new List<Asteroid>();

        // Spawn the parent Asteroids, child Asteroids are taken care of by them.
        for (int i = 0; i < 3; i++)
        {
            SpawnParentAsteroid(i);
        }
    }

    void SpawnParentAsteroid(int i)
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:SpawnParentAsteroid(" + i + ")");
#endif
        Asteroid ast = Asteroid.SpawnAsteroid();
        ast.gameObject.name = "Asteroid_" + i.ToString("00");
        // Find a good location for the Asteroid to spawn.
        Vector3 pos;
        do
        {
            pos = ScreenBounds.RANDOM_ON_SCREEN_LOC;
        }
        while ((pos - PlayerShip.POSITION).magnitude < MIN_ASTEROID_DIST_FROM_PLAYER_SHIP);

        ast.transform.position = pos;
        ast.size = asteroidsSO.initialSize;
    }

    // Statics.

    /// <summary>
    /// <para>This static public property provides some protection for the Singleton _S.</para>
    /// <para>get {} does return null, but throws an error first.</para>
    /// <para>set {} allows overwrite of _S by a 2nd instance, but throws an error first.</para>
    /// <para>Another advantage of using a property here is it allows you to place a breakpoint in the set 
    /// clause and then look at the call stack if you fear something random is setting your _S value.</para>
    /// </summary>
    static private AsteraX S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("AsteraX:S getter - Attempt to get value of S before it has been set.");
                return null;
            }
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("AsteraX:S setter - Attempt to set S when it has already been set.");
            }
            _S = value;
        }
    }

    static public AsteroidsScriptableObject AsteroidsSO
    {
        get
        {
            if (S != null)
            {
                return S.asteroidsSO;
            }
            return null;
        }
    }

    static public void AddAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) == -1)
        {
            ASTEROIDS.Add(asteroid);
        }
    }

    static public void RemoveAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) != -1)
        {
            ASTEROIDS.Remove(asteroid);
        }
    }
}
