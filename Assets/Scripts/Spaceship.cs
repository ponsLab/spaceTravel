using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;



public class Spaceship : MonoBehaviour
{
    // Start is called before the first frame update
    private const float JUMP_AMOUNT = 20f;
    private float scaleFactor;

    //other
    public int cntMode = 1; // 1: Using Pyhton communication, 2: Using keyboard arrows
    //public float mapGain = 2.0f;
    public static float rewardearned = 0;
    public static float reward_flag = 0;
    public static float Moving_reward_flag = 0;
    public static float Speed_down_flag = 0;
    public static float enemy_flag = 0;
    public float CurrentTime;

    public float mappingGain;
    public float M1MaxROM;
    public float M1MinROM;

    public static Spaceship instance;


    public static Spaceship GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D Spaceshiprigidbody2D;
    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }
    private void Awake()
    {
        instance = this;
        Spaceshiprigidbody2D = GetComponent<Rigidbody2D>();
        Spaceshiprigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }

    private void Update()
    {
        CurrentTime = Time.time;
        mappingGain = GameOverWindow.mapGain;
        M1MaxROM = GameOverWindow.MaxROM;
        M1MinROM = GameOverWindow.MinROM;
        scaleFactor = 50 / ((M1MaxROM - M1MinROM) / 2);

        //print(CurrentTime);

        switch (state)
        {  
            default:
            case State.WaitingToStart:
                if (CurrentTime == 0)
                {
                    if (OnDied != null) OnDied(this, EventArgs.Empty);
                }
                    
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    Spaceshiprigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
 
                }
                break;
            case State.Playing:
                if (cntMode == 1)
                {
                    transform.position = (new Vector3(0, Communication.receivedPos.y, 0) * scaleFactor * mappingGain);
                    //print(Communication.receivedPos.y * scaleFactor * mappingGain);

                    GetComponent<Rigidbody2D>().MovePosition(transform.position);

                    if (Mathf.Sign(Communication.receivedPos.y) > 0.1f)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 7);
                    }
                    else if (Mathf.Sign(Communication.receivedPos.y) < -0.1f)
                    {
                        transform.eulerAngles = new Vector3(0, 0, -7);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                }
                else if (cntMode == 2)
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        Up();
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        Down();
                    }

                    transform.eulerAngles = new Vector3(0, 0, Spaceshiprigidbody2D.velocity.y * 0.3f);
                }

                break;
            case State.Dead:
                Communication.running = false;
                
                break;
        }
    }

    private void Up()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.SpaceshipJump);
    }

    private void Down()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.SpaceshipJump);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "rock")
        {
            Spaceshiprigidbody2D.bodyType = RigidbodyType2D.Static;
            SoundManager.PlaySound(SoundManager.Sound.Lose);
            state = State.Dead;
            rewardearned = 0;
            reward_flag = 0;
            enemy_flag = 0;
            if (OnDied != null) OnDied(this, EventArgs.Empty);
        }
        else if (collider.gameObject.tag == "reward")
        {
            rewardearned += 10;
            SoundManager.PlaySound(SoundManager.Sound.Reward);
            reward_flag = 1;
            //Moving_reward_flag = 0;
            enemy_flag = 0;
            //print("Hit Yellow Target!");
        }
        else if (collider.gameObject.tag == "big_reward")
        {
            rewardearned += 50;
            SoundManager.PlaySound(SoundManager.Sound.Reward3);
            reward_flag = 1;
            //Moving_reward_flag = 0;
            enemy_flag = 0;
            //print("Hit Red Target!");
        }
        else if (collider.gameObject.tag == "enemy")
        {
            SoundManager.PlaySound(SoundManager.Sound.Reward2);
            Spaceshiprigidbody2D.bodyType = RigidbodyType2D.Static;
            enemy_flag = 1;
            reward_flag = 0;
            state = State.Dead;
            rewardearned = 0;
            if (OnDied != null) OnDied(this, EventArgs.Empty);
            //reward_flag = 1;
        }
        else if (collider.gameObject.tag == "MovingEnemy")
        {
            SoundManager.PlaySound(SoundManager.Sound.Reward2);
            Spaceshiprigidbody2D.bodyType = RigidbodyType2D.Static;
            enemy_flag = 1;
            reward_flag = 0;
            state = State.Dead;
            rewardearned = 0;
            if (OnDied != null) OnDied(this, EventArgs.Empty);
        }
        else if (collider.gameObject.tag == "max_reward")
        {
            rewardearned += 1000;
            SoundManager.PlaySound(SoundManager.Sound.Reward3);
            //reward_flag = 0;
            enemy_flag = 0;
            Moving_reward_flag = 1;

            //print("Hit moving Target!");
        }
        else if (collider.gameObject.tag == "SpeedDown")
        {
            rewardearned += 100;
            SoundManager.PlaySound(SoundManager.Sound.Reward3);
            //reward_flag = 0;
            enemy_flag = 0;
            Speed_down_flag = 1;

            //print("Hit moving Target!");
        }


    }

}


