using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

    float rightMax = 1.5f;
    float leftMax = -1.5f;
    float currentPos;
    float direction = 3.0f;

    private void Start() {

        Invoke("Update", 2f);
        currentPos = transform.position.x;
        
    }
    private void Update() {
        
        currentPos += direction * Time.deltaTime;

        if(currentPos >= rightMax) {

            direction *= -1;
            currentPos = rightMax;
        }

        else if(currentPos <= leftMax) {

            direction *= -1;
            currentPos = leftMax;
        }

        transform.position = new Vector3(currentPos, 3.8f, 0);
    }
}
