using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class CubeScript : MonoBehaviour {

    public Transform ball;

    

	// Use this for initialization
	IEnumerator Start () {
	    while(true)
        {
            var newBall = Instantiate(ball, transform.position, Quaternion.identity) as Transform;
            newBall.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.value * 2000 - 1000, 2000 + UnityEngine.Random.value * 1000, UnityEngine.Random.value * 2000 - 1000));
            yield return new WaitForSeconds(1 + UnityEngine.Random.value * 1);
        }
	}

    bool _runningSequence;

    void OnGUI()
    {
        if (_runningSequence)
            return;

        if(GUILayout.Button("Retrieve the balls"))
        {
            StartCoroutine(CutSequence());
        }
    }

    IEnumerator MoveObject(Transform objectToMove, Vector3 position, float time, EasingFunction.Ease easeType = EasingFunction.Ease.Linear, Func<float, float, float> easingFunction = null)
    {
        //easingFunction = easingFunction ?? EasingFunction.GetEasingFunction(easeType);
        //EasingFunction.Function = 
        
        var t = 0f;
        var originalPosition = objectToMove.position;
        var lastTime = Time.realtimeSinceStartup;
        while (t < 1)
        {
            objectToMove.position = Vector3.Lerp(originalPosition, position, t);
            //t += Time.deltaTime / time;
            t += (Time.realtimeSinceStartup - lastTime) / time;
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }
    }

    IEnumerator ScaleObject(Transform objectToScale, float size, float time)
    {
        var newScale = objectToScale.localScale * size;
        var t = 0f;
        var originalScale = objectToScale.localScale;
        var lastTime = Time.realtimeSinceStartup;
        while (t < 1)
        {
            objectToScale.localScale = Vector3.Lerp(originalScale, newScale, t);
            //t += Time.deltaTime / time;
            t += (Time.realtimeSinceStartup - lastTime) / time;
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }
    }

    IEnumerator CutSequence()
    {
        _runningSequence = true;
        Time.timeScale = 0;

        var originalPosition = Camera.main.transform.position;



        // 공들을 순회하고 최종적으로 박스로 카메라 이동
        //---
        foreach(var ball in BallScript.allBalls.ToArray())
        {
            if (ball != null)
            {
                var targetPosition = ball.transform.position - Vector3.forward;
                yield return StartCoroutine(MoveObject(Camera.main.transform, targetPosition, 2, EasingFunction.Ease.EaseInOutSine));
                yield return WaitForRealSeconds(0.5f);
            }
        }

        yield return StartCoroutine(MoveObject(Camera.main.transform, originalPosition, 2));
        //---



        // 공들을 모아서 삭제하기
        //---
        foreach (var ball in BallScript.allBalls.ToArray())
        {
            if(ball != null)
            {
                StartCoroutine(MoveObject(ball.transform, transform.position, 2f, EasingFunction.Ease.EaseInQuart));
            }
        }

        yield return WaitForRealSeconds(2f);

        foreach(var ball in BallScript.allBalls.ToArray())
        {
            Destroy(ball.gameObject);
        }
        //---


        // 박스 스케일 조정
        yield return StartCoroutine(ScaleObject(transform, 2f, 1f));
        

        Time.timeScale = 1;

        _runningSequence = false;
    }

    Coroutine WaitForRealSeconds(float time)
    {
        return StartCoroutine(Wait(time));
    }

    IEnumerator Wait(float time)
    {
        var current = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup - current < time)
        {
            yield return null;
        }
    }
}

