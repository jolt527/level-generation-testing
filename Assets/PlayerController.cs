using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed = 1f;
    public float turnSpeed = 180f;

    void Update() {
        int moveLeft = Input.GetKey(KeyCode.A) ? 1 : 0;
        int moveRight = Input.GetKey(KeyCode.D) ? 1 : 0;
        int moveUp = Input.GetKey(KeyCode.W) ? 1 : 0;
        int moveDown = Input.GetKey(KeyCode.S) ? 1 : 0;
        Vector2 movementDirection = new Vector2(moveRight - moveLeft, moveUp - moveDown).normalized;
        transform.position += (transform.right * movementDirection.x + transform.forward * movementDirection.y) * moveSpeed * Time.deltaTime;

        int turnLeft = Input.GetKey(KeyCode.J) ? 1 : 0;
        int turnRight = Input.GetKey(KeyCode.L) ? 1 : 0;
        int turnAmount = turnRight - turnLeft;
        transform.Rotate(0f, turnAmount * turnSpeed * Time.deltaTime, 0f);
    }
}
