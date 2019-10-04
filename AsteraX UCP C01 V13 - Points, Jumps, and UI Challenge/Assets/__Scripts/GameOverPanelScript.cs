using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelScript : MonoBehaviour
{
    [Header("Set in Inspector")]
    public ScoreText scoreText;
    public Text finalLevelText;
    public Text finalScoreText;


    void Start()
    {
        // Set final level and score texts.
        finalLevelText.text = "Final Level: " + AsteraX.LEVEL;
        finalScoreText.text = "Final Score: " + string.Format("{0:#,0}", scoreText.GetScore());
    }
}
