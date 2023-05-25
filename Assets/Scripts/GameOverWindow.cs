using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;


public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;
    public static int GM = 2;
    public static int Amp = 15;
    public static float Freq = 0.1f;
    public static float HT = 10f;
    public static float VT = 10f;

    public static float MaxROM = 50f;
    public static float MinROM = -50f;
    public static float mapGain = 1f;

    private Button_UI retryBtn;
    
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();

        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.GameScene); };
        retryBtn = transform.Find("retryBtn").GetComponent<Button_UI>();
        transform.Find("retryBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("mainMenuBtn").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.MainMenu); };
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().AddButtonSounds();


        //transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }


    public void GetInput_GameMode(string gameMode)
    {
        GM = Convert.ToInt32(gameMode);
    }
    public void GetInput_Amp(string Amplitude)
    {
        Amp = Convert.ToInt32(Amplitude);
    }
    public void GetInput_Freq(string Frequency)
    {
        Freq = float.Parse(Frequency);
    }
    public void GetInput_VerticalTrans(string VerT)
    {
        VT = float.Parse(VerT);
    }
    public void GetInput_HorizontalTrans(string HorT)
    {
        HT = float.Parse(HorT);
    }
    public void GetInput_MaxROM(string maxROM)
    {
        MaxROM = float.Parse(maxROM);
    }
    public void GetInput_MinROM(string minROM)
    {
        MinROM = float.Parse(minROM);
    }
    public void GetInput_mapGain(string mapG)
    {
        mapGain = float.Parse(mapG);
    }


    private void Start()
    {
        Spaceship.GetInstance().OnDied += Spaceship_OnDied;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Retry
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    private void Spaceship_OnDied(object sender, System.EventArgs e)
    {
        scoreText.text = string.Format("{0:0.00}", ScoreWindow.playerscore);

        if (ScoreWindow.playerscore >= Score.GetHighScore())
        {
            //New Highscore!
            highscoreText.text = "NEW HIGHSCORE!!";
        }else
        {
            highscoreText.text = "HIGHSCORE: " + Math.Round(Score.GetHighScore()*100)/100;
        }
        Show();
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
