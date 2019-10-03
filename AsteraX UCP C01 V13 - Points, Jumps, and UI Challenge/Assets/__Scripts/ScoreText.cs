using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    Text T;
    int score = 0;

    void Start()
    {
        T = GetComponent<Text>();
    }

    public void AddScore(int points)
    {
        score += points;
        T.text = string.Format("{0:#,0}", score); // Format to use thousandth place commas.
    }

    public int GetScore()
    {
        return score;
    }
}
