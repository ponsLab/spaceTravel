using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite RockFillSprite;
    public Transform pfRockBody;
    public Transform pfGround;
    public Transform pfPlanet_1;
    public Transform pfPlanet_2;
    public Transform pfPlanet_3;
    public Transform pfPlanet_4;
    public Transform pfPlanet_5;
    public Transform pfPlanet_6;
    public Transform pfPlanet_7;
    public Transform pfPlanet_8;
    public Transform pfEnemy_1;
    public Transform MovingEnemy;

    public Transform pfReward_1;
    public Transform pfReward_2;
    public Transform pfReward_3;
    public Transform SpeedDown;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
