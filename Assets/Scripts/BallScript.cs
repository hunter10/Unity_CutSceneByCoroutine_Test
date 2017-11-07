using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BallScript : MonoBehaviour {

    public static List<BallScript> allBalls = new List<BallScript>();

    void Awake()
    {
        allBalls.Add(this);
    }

    void OnDestroy()
    {
        allBalls.Remove(this);
    }

    void Update()
    {
        if (transform.position.y < -5)
            Destroy(gameObject);
    }
}

