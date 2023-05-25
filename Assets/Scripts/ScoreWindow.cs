using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    // Start is called before the first frame update
    private Text highscoreText;
    private Text scoreText;
    public static float timePast;
    public static float playerscore;


    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
    }

    private void Start()
    {
        highscoreText.text = "HIGHSCORE: " + string.Format("{0:0.00}", Score.GetHighScore());
        Spaceship.GetInstance().OnDied += ScoreWindow_OnDied;
        Spaceship.GetInstance().OnStartedPlaying += ScoreWindow_OnstartedPlaying;
        Hide();
    }

    private void ScoreWindow_OnstartedPlaying(object sender, System.EventArgs e)
    {
        Show();
    }

    private void ScoreWindow_OnDied(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void FixedUpdate()
    {
        timePast += Time.fixedDeltaTime;
        playerscore = timePast + Spaceship.rewardearned;
        scoreText.text = "SCORE: " + string.Format("{0:0.00}", playerscore);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
