using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    private static int counter = 0;

    public static void Start()
    {
        if (counter == 0)
        {
            ResetHighscore();
        }
        counter = counter + 1;
        Spaceship.GetInstance().OnDied += Spaceship_OnDied;
    }

    private static void Spaceship_OnDied(object sender, System.EventArgs e)
    {
        
        TrySetNewHighscore(ScoreWindow.playerscore);
    }

    // Start is called before the first frame update
    public static float GetHighScore()
    {
        return PlayerPrefs.GetFloat("highscore");
    }


    public static bool TrySetNewHighscore(float score)
    {
        float currentHighscore = GetHighScore();
        // Debug.Log("Current score:" + string.Format("{0:0.00}", currentHighscore));
        // Debug.Log("New score:" + string.Format("{0:0.00}", score));

        if (score > currentHighscore)
        {
            //New Highscore
            PlayerPrefs.SetFloat("highscore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResetHighscore()
    {
        PlayerPrefs.SetFloat("highscore", 0);
        PlayerPrefs.Save();
    }
}
