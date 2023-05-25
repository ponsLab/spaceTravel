using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour
{
    public Vector3 speed = new Vector3(10, 20, 10);
    public Vector3 direction = new Vector3(-1, 0, 0);
    private Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(speed.x * direction.x, speed.y * Mathf.Sin(Time.time), 0);
    }
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = movement;
    }
}
