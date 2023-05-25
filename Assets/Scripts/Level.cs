using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using System.Security.Cryptography;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float ROCK_WIDTH = 0.5f;
    private const float ROCK_DESTROY_X_POSITION = -100f;    // camera dependent value
    private const float ROCK_SPAWN_X_POSITION = +100f;
    private const float GROUND_DESTROY_X_POSITION = -200f;    // camera dependent value

    private const float PLANET_DESTROY_X_POSITION = -160f;    // camera dependent value
    private const float PLANET_SPAWN_X_POSITION = +160f;
    private const float PLANET_SPAWN_Y_POSITION = 0f;

    private const float REWARD_DESTROY_X_POSITION = -160f;    // camera dependent value
    private const float REWARD_SPAWN_X_POSITION = +130f;
    private const float REWARD_SPAWN_Y_POSITION = 0f;

    private const float ENEMY_DESTROY_X_POSITION = -160f;    // camera dependent value
    private const float ENEMY_SPAWN_X_POSITION = +100f;
    private const float ENEMY_SPAWN_Y_POSITION = 10f;

    private const float SPACESHIP_X_POSITION = 0f;
    private int Trajectory_Count = 1;
    public float internalClock = 0f;
    public int LevelID = 0;
    public static int randomSineOffset = 0;

    public static float ROCK_MOVE_SPEED = 50f;  // rock right to left speed
    public static float Level_SPEED = 50f;

    public int GameMode;
    public int Amplitude;
    public float Frequency;
    public float RewardSpawnTimerMax;
    public float RewardSpawnTimerMin;
    public float EnemySpawnTimerMax;
    public float EnemySpawnTimerMin;
    public float MovingEnemySpawnTimerMin;
    public float MovingEnemySpawnTimerMax;
    public int difficulty_flag = 0;

    private static Level instance;
    private int rocksPassedCount;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Transform> groundList;
    private List<Transform> planetList;
    private List<Transform> rewardList;
    private List<Transform> MovingrewardList;
    private List<Transform> enemyList;
    private List<Rock> rockList;

    private float planetSpawnTimer;
    private float rewardSpawnTimer;
    private float enemySpawnTimer;

    public int rockSpawned;
    private int rewardSpawned;
    private int enemySpawned;

    private float rockSpawnTimer;
    private float rockSpawnTimerMax;
    private float gapSize;
    private float M1ROM;
    private State state;
    private float heightvar;
    private int level_interval = 50;

    private float Speed_timer;

    public enum Difficulty
    {
        Alevel,
        Blevel,
        Clevel,
        Dlevel,
        Elevel,
        Flevel,
        Glevel,
        Hlevel,
        Ilevel,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        SpaceshipDead,
    }


    private void Awake()
    {
        instance = this;
        SpawnInitialPlanets();
        SpawnInitialRewards();
        SpawnInitialEnemy();

        rockList = new List<Rock>();
        rockSpawnTimerMax = 1f;   // how fast the rocks are spawned
        SetDifficulty(Difficulty.Alevel);
        state = State.WaitingToStart;
        heightvar = 50f;
    }

    private void Start()
    {
        Spaceship.GetInstance().OnDied += Spaceship_OnDied;
        Spaceship.GetInstance().OnStartedPlaying += Spaceship_OnStartPlaying;
    }

    private void Spaceship_OnStartPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }


    private void Spaceship_OnDied(object sender, System.EventArgs e)
    {
        state = State.SpaceshipDead;
    }
    

    private void FixedUpdate()
    {
        GameMode = GameOverWindow.GM;
        Amplitude = GameOverWindow.Amp;
        Frequency = GameOverWindow.Freq;
        //HorTrans = GameOverWindow.HT;
        //VerTrans = GameOverWindow.VT;
        internalClock += Time.fixedDeltaTime;

        Speed_timer -= Time.fixedDeltaTime;

        if (Speed_timer < 0)
        {
            ROCK_MOVE_SPEED = Level_SPEED;
        }


        //print(Speed_timer);


        if (state == State.Playing)
        {
            HandleRockMovement();
            HandleRockSpawining();
            HandlePlanets();
            HandleRewards();
            HandleDroppingReward();

            if (difficulty_flag == 1 || enemyList != null)
            {
                if (LevelID == 0)
                {
                    HandleEnemy();
                    //HandleMovingEnemy();
                }
                else if (LevelID == 1)
                {
                    HandleMovingEnemy();
                }
                else
                {
                    HandleEnemy();
                    HandleMovingEnemy();
                }
                
            }

        }
        else 
        {
            ROCK_MOVE_SPEED = 50;
            Level_SPEED = ROCK_MOVE_SPEED;
            LevelID = 0;
        }
    }

    private void SpawnInitialPlanets()
    {
        planetList = new List<Transform>();
        Transform planetTransform;
        float PLANET_Y_POSITION = UnityEngine.Random.Range(30f, -30f);
        planetTransform = Instantiate(GetPlanetPrefabTransform(), new Vector3(0, PLANET_Y_POSITION, 0), Quaternion.identity);
        planetList.Add(planetTransform);
    }

    private Transform GetPlanetPrefabTransform()
    {
        switch (Random.Range(0, 8))
        {
            default:
            case 0: return GameAssets.GetInstance().pfPlanet_1;
            case 1: return GameAssets.GetInstance().pfPlanet_2;
            case 2: return GameAssets.GetInstance().pfPlanet_3;
            case 3: return GameAssets.GetInstance().pfPlanet_4;
            case 4: return GameAssets.GetInstance().pfPlanet_5;
            case 5: return GameAssets.GetInstance().pfPlanet_6;
            case 6: return GameAssets.GetInstance().pfPlanet_7;
            case 7: return GameAssets.GetInstance().pfPlanet_8;
        }
    }

    private void HandlePlanets()
    {
        planetSpawnTimer -= Time.fixedDeltaTime;
        if (planetSpawnTimer < 0)
        {
            float planetSpawnTimerMax = 4f;
            planetSpawnTimer = planetSpawnTimerMax;

            float PLANET_Y_POSITION = UnityEngine.Random.Range(30f, -30f);

            Transform planetTransform = Instantiate(GetPlanetPrefabTransform(), new Vector3(PLANET_SPAWN_X_POSITION, PLANET_Y_POSITION, 0), Quaternion.identity);
            planetList.Add(planetTransform);
        }

        for (int i = 0; i<planetList.Count; i++)
        {
            Transform planetTransform = planetList[i];
            {
                float PLANET_VEL = UnityEngine.Random.Range(0.3f, 0.6f);
                planetTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.deltaTime * PLANET_VEL; 
                if (planetTransform.position.x < PLANET_DESTROY_X_POSITION)
                {
                    Destroy(planetTransform.gameObject);
                    planetList.RemoveAt(i);
                    i--;
                }
            }
        }
    }


    private void SpawnInitialRewards()
    {
        rewardList = new List<Transform>();
        MovingrewardList = new List<Transform>();

        Transform rewardTransform;
        Transform MovingrewardTransform;

        float REWARD_Y_POSITION = UnityEngine.Random.Range(40f, -40f);
        float REWARD_X_POSITION = UnityEngine.Random.Range(30f, -30f);

        rewardTransform = Instantiate(GetRewardPrefabTransform(), new Vector3(REWARD_X_POSITION, REWARD_Y_POSITION, 0), Quaternion.identity);
        MovingrewardTransform = Instantiate(GetRewardPrefabTransform(), new Vector3(REWARD_X_POSITION, REWARD_Y_POSITION, 0), Quaternion.identity);

        rewardList.Add(rewardTransform);
        MovingrewardList.Add(MovingrewardTransform);

    }

    private Transform GetRewardPrefabTransform()
    {
        switch (Random.Range(0, 2))
        {
            default:
            case 0: return GameAssets.GetInstance().pfReward_1;
            case 1: return GameAssets.GetInstance().pfReward_2;
            // case 2: return GameAssets.GetInstance().pfReward_3;
        }

        //return GameAssets.GetInstance().pfReward_1;
    }

    private void HandleRewards()
    {
        rewardSpawnTimer -= Time.fixedDeltaTime/0.1f;
        //rewardSpawnTimer -= 0.05f;
        if (rewardSpawnTimer < 0)
        {
            randomSineOffset = UnityEngine.Random.Range(0, 10);
            //float rewardSpawnTimerMax = UnityEngine.Random.Range(RewardSpawnTimerMin, RewardSpawnTimerMax);
            float rewardSpawnTimerMax = 5;
            rewardSpawnTimer = rewardSpawnTimerMax;

            //float REWARD_Y_POSITION = UnityEngine.Random.Range(20f, -20f);
            //float REWARD_Y_POSITION = UnityEngine.Random.Range(40/2, -40/2);
            M1ROM = GameOverWindow.MaxROM - GameOverWindow.MinROM;
            float REWARD_Y_POSITION = Communication.receivedPos.z*gapSize/M1ROM*0.8f;


            Transform rewardTransform = Instantiate(GetRewardPrefabTransform(), new Vector3(REWARD_SPAWN_X_POSITION, REWARD_Y_POSITION, 0), Quaternion.identity);
            rewardList.Add(rewardTransform);
            
            
        }

        for (int i = 0; i < rewardList.Count; i++)
        {
            Transform rewardTransform = rewardList[i];
            {
                float REWARD_VEL = UnityEngine.Random.Range(0.5f, 1f);
                rewardTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.deltaTime * REWARD_VEL;

                if (rewardTransform.position.x < REWARD_DESTROY_X_POSITION || rewardTransform.position.y < -70f)
                {
                    Destroy(rewardTransform.gameObject);
                    rewardList.RemoveAt(i);
                    i--;
                }
                
                if (rewardList[i].position.x <= 5 &&  rewardList[i].position.x > -1 && Spaceship.reward_flag == 1 )
                {
                    Destroy(rewardTransform.gameObject);
                    Spaceship.reward_flag = 0;
                    rewardList.RemoveAt(i);
                }
            }
            
        }
    }

    private void SpawnInitialEnemy()
    {
        enemyList = new List<Transform>();
        Transform enemyTransform;
        float ENEMY_Y_POSITION = UnityEngine.Random.Range(50f, 20f);
        float ENEMY_X_POSITION = UnityEngine.Random.Range(40, 10);
        //enemyTransform = Instantiate(GetEnemyPrefabTransform(), new Vector3(ENEMY_X_POSITION, ENEMY_Y_POSITION, 0), Quaternion.identity);
        //enemyList.Add(enemyTransform);
    }

    private Transform GetEnemyPrefabTransform()
    {
        switch (Random.Range(0, 1))
        {
            default:
            case 0: return GameAssets.GetInstance().pfEnemy_1;
        }
    }

    private void HandleEnemy()
    {
        enemySpawnTimer -= Time.fixedDeltaTime / 0.1f;

        //print(enemySpawnTimer);
        if (enemySpawnTimer < 0 && difficulty_flag == 1)
        {
            float enemySpawnTimerMax = UnityEngine.Random.Range(EnemySpawnTimerMin, EnemySpawnTimerMax);

            enemySpawnTimer = enemySpawnTimerMax;

            float ENEMY_Y_POSITION = UnityEngine.Random.Range(50, 0);

            Transform enemyTransform = Instantiate(GetEnemyPrefabTransform(), new Vector3(ENEMY_SPAWN_X_POSITION, ENEMY_Y_POSITION, 0), Quaternion.identity);
            enemyList.Add(enemyTransform);
        }


        for (int i = 0; i < enemyList.Count; i++)
        {
            Transform enemyTransform = enemyList[i];
            {
                float ENEMY_VEL = UnityEngine.Random.Range(0.5f, 1f);
                enemyTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.deltaTime * ENEMY_VEL;

                if (enemyTransform.position.x < ENEMY_DESTROY_X_POSITION || enemyTransform.position.y < -70f)
                {
                    Destroy(enemyTransform.gameObject);
                    enemyList.RemoveAt(i);
                    i--;
                }

                if (enemyList[i].position.x <= 1 && enemyList[i].position.x > -3 && Spaceship.enemy_flag == 1)
                {
                    Destroy(enemyTransform.gameObject);
                    Spaceship.enemy_flag = 0;
                    enemyList.RemoveAt(i);
                }
            }
        }
  
    }



    private Transform GetMovingEnemyPrefabTransform()
    {
        switch (Random.Range(0, 1))
        {
            default:
            case 0: return GameAssets.GetInstance().MovingEnemy;
        }
    }

    private void HandleMovingEnemy()
    {
        enemySpawnTimer -= Time.fixedDeltaTime / 0.1f;

        // print(enemySpawnTimer);
        if (enemySpawnTimer < 0 && difficulty_flag == 1)
        {
            float enemySpawnTimerMax = UnityEngine.Random.Range(MovingEnemySpawnTimerMin, MovingEnemySpawnTimerMax);

            print(EnemySpawnTimerMin);
            enemySpawnTimer = enemySpawnTimerMax;

            float ENEMY_Y_POSITION = UnityEngine.Random.Range(50, -30);

            Transform enemyTransform = Instantiate(GetMovingEnemyPrefabTransform(), new Vector3(ENEMY_SPAWN_X_POSITION, ENEMY_Y_POSITION, 0), Quaternion.identity);
            enemyList.Add(enemyTransform);
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            Transform enemyTransform = enemyList[i];
            {
                float ENEMY_VEL = UnityEngine.Random.Range(0.5f, 1f);
                enemyTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.deltaTime * ENEMY_VEL;

                if (enemyTransform.position.x < ENEMY_DESTROY_X_POSITION || enemyTransform.position.y < -70f)
                {
                    Destroy(enemyTransform.gameObject);
                    enemyList.RemoveAt(i);
                    i--;
                }

                if (enemyList[i].position.x <= 1 && enemyList[i].position.x > -1 && Spaceship.enemy_flag == 1)
                {
                    Destroy(enemyTransform.gameObject);
                    Spaceship.enemy_flag = 0;
                    enemyList.RemoveAt(i);
                }
            }

        }
    }


    private Transform GetDroppingRewardPrefabTransform()
    {
        switch (Random.Range(0, 2))
        {
            default:
            case 0: return GameAssets.GetInstance().pfReward_3;
            //case 1: return GameAssets.GetInstance().SpeedDown;
        }
    }

    private void HandleDroppingReward()
    {
        enemySpawnTimer -= Time.fixedDeltaTime / 0.1f;

        // print(enemySpawnTimer);
        if (enemySpawnTimer < 0)
        {
            float enemySpawnTimerMax = UnityEngine.Random.Range(40, 70);

            enemySpawnTimer = enemySpawnTimerMax;

            float ENEMY_Y_POSITION = UnityEngine.Random.Range(50, -20);

            Transform MovingrewardTransform = Instantiate(GetDroppingRewardPrefabTransform(), new Vector3(ENEMY_SPAWN_X_POSITION, ENEMY_Y_POSITION, 0), Quaternion.identity);
            MovingrewardList.Add(MovingrewardTransform);
        }

        for (int i = 0; i < MovingrewardList.Count; i++)
        {
            Transform MovingrewardTransform = MovingrewardList[i];
            {
                float REWARD_VEL = UnityEngine.Random.Range(0.6f, 1.1f);
                MovingrewardTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.deltaTime * REWARD_VEL;

                if (MovingrewardList[i].position.x < REWARD_DESTROY_X_POSITION || MovingrewardList[i].position.y < -70f)
                {
                    Destroy(MovingrewardTransform.gameObject);
                    MovingrewardList.RemoveAt(i);
                    i--;
                }

                if (MovingrewardList[i].position.x <= 5 && MovingrewardList[i].position.x > 0 && Spaceship.Moving_reward_flag == 1)
                {
                    Destroy(MovingrewardTransform.gameObject);
                    Spaceship.Moving_reward_flag = 0;
                    MovingrewardList.RemoveAt(i);
                    Speed_timer = 3f;
                    ROCK_MOVE_SPEED += 40;



                }
                else if (MovingrewardList[i].position.x <= 5 && MovingrewardList[i].position.x > 0 && Spaceship.Speed_down_flag == 1)
                {
                    Destroy(MovingrewardTransform.gameObject);
                    Spaceship.Speed_down_flag = 0;
                    MovingrewardList.RemoveAt(i);
                    //ROCK_MOVE_SPEED -= 20;
                    //Speed_timer = 5;
                }
            }

        }

    }
    






    private void HandleRockSpawining()
    {
        rockSpawnTimer -= Time.deltaTime*ROCK_MOVE_SPEED / 70;

        if (rockSpawnTimer < 0)
        {
            rockSpawnTimer += rockSpawnTimerMax;

            float heightEdgeLimit = 2f;
            float minHeight = gapSize * 0.7f + heightEdgeLimit;   // minimum gap size
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.7f - heightEdgeLimit;

            
            //float height = UnityEngine.Random.Range(minHeight, maxHeight);  // Change to have specific waveforms

            if(GameMode == 1) 
            {
                heightvar = 50f;
                gapSize = 90f;   
            }
            else if (GameMode == 2)   // practice mode with random rewards
            {
                heightvar = 50f;           
            }
            else if (GameMode == 3)   // sine waveform
            {
                if (heightvar < maxHeight && heightvar > minHeight)
                {
                    heightvar = +50f + Communication.receivedPos.z;
                }
                else
                {
                    heightvar = heightvar;
                }
            }
            else if (GameMode == 4)  // saw waveform
            {
                if (heightvar < maxHeight && heightvar > minHeight)
                {
                    //heightvar = +50f + Amplitude * Mathf.Asin(Mathf.Sin(((2 * Mathf.PI) / (Frequency)) * Time.time));
                    heightvar = +50f + Amplitude * Mathf.Asin(Mathf.Sin(((2 * Mathf.PI) * (Frequency)) * ScoreWindow.timePast));
                }
                else
                {
                    heightvar = heightvar;
                }               
            }

            CreateGapRock(heightvar, gapSize, ROCK_SPAWN_X_POSITION); // CreateGapRock(float gapY, float gapSize, float xPosition)
        }
    }


    private void HandleRockMovement()
    {
        for (int i = 0; i < rockList.Count; i++)
        {
            Rock rock = rockList[i];
            bool isToTheRightOfShip = rock.GetXPosition() > SPACESHIP_X_POSITION;
            rock.Move();
            if(isToTheRightOfShip && rock.GetXPosition() <= SPACESHIP_X_POSITION && rock.IsBottom())
            {
                // Rock passed the rocket
                rocksPassedCount++;
            }

            if (rock.GetXPosition() < ROCK_DESTROY_X_POSITION)
            {
                rock.DestroySelf();
                rockList.Remove(rock);
                i--;
            }

        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {


        switch (difficulty)
        {
            case Difficulty.Alevel:
                gapSize = 50f;
                rockSpawnTimerMax = 0.1f;
                difficulty_flag = 0;
                break;
            case Difficulty.Blevel:
                gapSize = 60f;
                rockSpawnTimerMax = 0.1f;
                break;
            case Difficulty.Clevel:
                gapSize = 70f;
                rockSpawnTimerMax = 0.1f;
                break;
            case Difficulty.Dlevel:
                gapSize = 80;
                rockSpawnTimerMax = 0.1f;
                break;
            case Difficulty.Elevel:
                gapSize = 90;
                rockSpawnTimerMax = 0.1f;
                break;
            case Difficulty.Flevel:
                gapSize = 90;
                rockSpawnTimerMax = 0.1f;
                EnemySpawnTimerMax = 15.0f;
                EnemySpawnTimerMin = 11.0f;
                MovingEnemySpawnTimerMax = 16.0f;
                MovingEnemySpawnTimerMin = 11.0f;
                difficulty_flag = 1;
                break;
            case Difficulty.Glevel:
                gapSize = 90;
                rockSpawnTimerMax = 0.1f;
                EnemySpawnTimerMax = 13.0f;
                EnemySpawnTimerMin = 8.0f;
                MovingEnemySpawnTimerMax = 11.0f;
                MovingEnemySpawnTimerMin = 9.0f;
                difficulty_flag = 1;
                break;
            case Difficulty.Hlevel:
                gapSize = 90;
                rockSpawnTimerMax = 0.1f;
                EnemySpawnTimerMax = 11.0f;
                EnemySpawnTimerMin = 8.0f;
                MovingEnemySpawnTimerMax = 11.0f;
                MovingEnemySpawnTimerMin = 9.0f;
                difficulty_flag = 1;
                break;
            case Difficulty.Ilevel:
                gapSize = 90;
                rockSpawnTimerMax = 0.1f;
                EnemySpawnTimerMax = 11.0f;
                EnemySpawnTimerMin = 7.0f;
                MovingEnemySpawnTimerMax = 11.0f;
                MovingEnemySpawnTimerMin = 9.0f;
                difficulty_flag = 0;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (rockSpawned >= level_interval*7+50) return Difficulty.Ilevel;
        if (rockSpawned >= level_interval*7) return Difficulty.Hlevel;
        if (rockSpawned >= level_interval*6) return Difficulty.Glevel;
        if (rockSpawned >= level_interval*5) return Difficulty.Flevel;
        if (rockSpawned >= level_interval*4) return Difficulty.Elevel;
        if (rockSpawned >= level_interval*3) return Difficulty.Dlevel;
        if (rockSpawned >= level_interval*2) return Difficulty.Clevel;
        if (rockSpawned >= level_interval) return Difficulty.Blevel;
        return Difficulty.Alevel;
    }


    private void CreateGapRock(float gapY, float gapSize, float xPosition)
    {
        CreateRock(gapY - gapSize * 0.5f, xPosition, true);
        CreateRock(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        rockSpawned++;
        SetDifficulty(GetDifficulty());

        if (rockSpawned == level_interval * 7 + 50) 
        {
            rockSpawned = 0;
            LevelID += 1;
            if (LevelID % 2 == 0 && LevelID != 0)
            {
                ROCK_MOVE_SPEED += 20;
                Level_SPEED = 50 + (LevelID%2)*20;
            }
            
        }

    }




    private void CreateRock(float height, float xPosition, bool isBottom)
    {
        Transform rockBody = Instantiate(GameAssets.GetInstance().pfRockBody);

        float rockBodyYPosition;

        if (isBottom)
        {
            rockBodyYPosition = -CAMERA_ORTHO_SIZE;

        }
        else
        {
            rockBodyYPosition = +CAMERA_ORTHO_SIZE;
            rockBody.localScale = new Vector3(20, -1, 1);
        }

        rockBody.position = new Vector3(xPosition, rockBodyYPosition);
        SpriteRenderer rockBodySpriteRenderer = rockBody.GetComponent<SpriteRenderer>();
        rockBodySpriteRenderer.size = new Vector2(ROCK_WIDTH, height);

        BoxCollider2D rockBodyBoxCollider = rockBody.GetComponent<BoxCollider2D>();
        rockBodyBoxCollider.size = new Vector2(ROCK_WIDTH, height);
        rockBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);



        Rock rock = new Rock(rockBody, isBottom);
        rockList.Add(rock);
    }


    public int GetRockSpawned()
    {
        return rockSpawned;
    }

    public int GetRocksPassedCount()
    {
        return rocksPassedCount; 
    }

    /*
     * Represents a single rock
     * /*/
    private class Rock
    {
        private Transform rockBodyTransform; 
        private bool isBottom;

        public Rock(Transform rockBodyTransform, bool isBottom)
        {
            this.rockBodyTransform = rockBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            rockBodyTransform.position += new Vector3(-1, 0, 0) * ROCK_MOVE_SPEED * Time.fixedDeltaTime; 
        }

        public float GetXPosition()
        {
            return rockBodyTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void DestroySelf()
        {
            Destroy(rockBodyTransform.gameObject);
        }
    }
}
