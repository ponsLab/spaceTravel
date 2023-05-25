using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToStartWindow : MonoBehaviour
{
    private void Start()
    {
        Spaceship.GetInstance().OnStartedPlaying += WaitingToStartWindow_OnStartPlaying;
        
    }

    private void WaitingToStartWindow_OnStartPlaying(object sender, System.EventArgs e)
    {
        Hide();
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
