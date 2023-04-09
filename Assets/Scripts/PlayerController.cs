using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotateSpeed = 0.15f;

    private PlayerInput _playerInput;

    private Vector2 _moveVector;

    private void Update()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(_moveVector.x, 0f, _moveVector.y);

        if(!_moveVector.Equals(Vector3.zero))
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotateSpeed);

        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
    }
}
