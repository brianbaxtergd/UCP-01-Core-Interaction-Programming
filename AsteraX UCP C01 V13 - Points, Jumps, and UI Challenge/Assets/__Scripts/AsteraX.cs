//#define DEBUG_AsteraX_LogMethods

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsteraX : MonoBehaviour
{
    // Private Singleton-style instance. Accessed by static property S later in script
    static private AsteraX _S;

    static List<Asteroid>           ASTEROIDS;
    static List<Bullet>             BULLETS;
    
    const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;

    ScoreText scoreText;
    int level;

    // System.Flags changes how eGameStates are viewed in the Inspector and lets multiple 
    //  values be selected simultaneously (similar to how Physics Layers are selected).
    // It's only valid for the game to ever be in one state, but I've added System.Flags
    //  here to demonstrate it and to make the ActiveOnlyDuringSomeGameStates script easier
    //  to view and modify in the Inspector.
    // When you use System.Flags, you still need to set each enum value so that it aligns 
    //  with a power of 2. You can also define enums that combine two or more values,
    //  for example the all value below that combines all other possible values.
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        mainMenu = 1,   // 00000001
        preLevel = 2,   // 00000010
        level = 4,      // 00000100
        postLevel = 8,  // 00001000
        gameOver = 16,  // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }

    eGameState gameState = eGameState.level;


    [Header("Set in Inspector")]
    [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
    public AsteroidsScriptableObject asteroidsSO;
    [Tooltip("How many seconds to wait at Game Over screen before automatically restarting the game.")]
    public float timeBeforeRestart;
    public GameOverPanelScript gameOverPanel;



    private void Awake()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Awake()");
#endif

        S = this;

        scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();

        level = 1;
    }


    void Start()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Start()");
#endif

        ASTEROIDS = new List<Asteroid>();
        
        // Spawn the parent Asteroids, child Asteroids are taken care of by them
        for (int i = 0; i < 3; i++)
        {
            SpawnParentAsteroid(i);
        }
    }


    void SpawnParentAsteroid(int i)
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:SpawnParentAsteroid("+i+")");
#endif

        Asteroid ast = Asteroid.SpawnAsteroid();
        ast.gameObject.name = "Asteroid_" + i.ToString("00");
        // Find a good location for the Asteroid to spawn
        Vector3 pos;
        do
        {
            pos = ScreenBounds.RANDOM_ON_SCREEN_LOC;
        } while ((pos - PlayerShip.POSITION).magnitude < MIN_ASTEROID_DIST_FROM_PLAYER_SHIP);

        ast.transform.position = pos;
        ast.size = asteroidsSO.initialSize;
    }



    // ---------------- Static Section ---------------- //

    /// <summary>
    /// <para>This static public property provides some protection for the Singleton _S.</para>
    /// <para>get {} does return null, but throws an error first.</para>
    /// <para>set {} allows overwrite of _S by a 2nd instance, but throws an error first.</para>
    /// <para>Another advantage of using a property here is that it allows you to place
    /// a breakpoint in the set clause and then look at the call stack if you fear that 
    /// something random is setting your _S value.</para>
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

    static public void AddScore(int asteroidSize)
    {
        if (asteroidSize > 3 || asteroidSize < 1)
            Debug.Log("AsteraX:AddScore() arg is OOR.");
        else
        {
            S.scoreText.AddScore(AsteroidsSO.pointsForAsteroidSize[asteroidSize]);
        }
    }

    static public Vector3 FindSafeJump()
    {
        float dist = 1000f;
        Vector3 safeJumpPos;
        int loopLimit = 1000;
        do
        {
            loopLimit -= 1;
            // Get a random position within the game screen.
            safeJumpPos = ScreenBounds.RANDOM_ON_SCREEN_LOC_FOR_SAFE_JUMPS;
            // Compare position to all Asteroid instances.
            for (int i = 0; i < ASTEROIDS.Count; i++)
            {
                float curDist = Vector3.Distance(safeJumpPos, ASTEROIDS[i].transform.position);
                // If the current distance check is less than the previous distance check, update dist value.
                if (curDist < dist)
                    dist = curDist;
            }
        }
        while (dist < 3 && loopLimit > 0); // Continue looping until random position is at least 3 units away from all Asteroid instances.

        return safeJumpPos;
    }

    static public void GameOver()
    {
        S.gameState = eGameState.gameOver;
        S.gameOverPanel.gameObject.SetActive(true);
        S.Invoke("ReloadScene", S.timeBeforeRestart);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    static public int LEVEL
    {
        get
        {
            return S.level;
        }
        set
        {
            S.level = value;
        }
    }
}
