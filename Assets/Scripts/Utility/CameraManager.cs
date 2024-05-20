using System;
using Managers;
using Map;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class CameraManager : MonoManager<CameraManager>
    {
        [SerializeField, Range(0, 0.5f)] private float movementSensitivity;
        [SerializeField, Range(1, 10f)] private float movementRunup;
        [SerializeField, Range(1, 10)] private float movementSpeed;
        [SerializeField] private float height;

        private float movementFactor;

        private Vector3 lastMovementVector;
        
        private void Start()
        {
            transform.position = new Vector3(0, height, -2);
        }

        private void Update()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            var mousePosition = Mouse.current.position.value;
            var movementVector = CalculateMovementVector(mousePosition, out var shouldMove);
            if (!shouldMove || IsMouseOutOfBounds())
            {
                movementFactor = Mathf.Clamp01(movementFactor - (Time.deltaTime * movementRunup));
            }
            else
            {
                movementFactor = Mathf.Clamp01(movementFactor + (Time.deltaTime * movementRunup));
                lastMovementVector = new Vector3(movementVector.x, 0, movementVector.y) * (Time.deltaTime * movementSpeed);
            }
            transform.position += lastMovementVector * movementFactor;
        }

        private Vector2 CalculateMovementVector(Vector2 mousePosition, out bool shouldMove)
        {
            var normalizedMousePosition =
                new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            var movementVector = Vector2.zero;
            if (normalizedMousePosition.x > 1 - movementSensitivity) {movementVector += Vector2.right;}
            if (normalizedMousePosition.x < movementSensitivity) movementVector += Vector2.left;
            if (normalizedMousePosition.y > 1 - movementSensitivity) movementVector += Vector2.up;
            if (normalizedMousePosition.y < movementSensitivity) movementVector += Vector2.down;
            shouldMove = movementVector.magnitude > 0;
            return movementVector;
        }

        private static bool IsMouseOutOfBounds()
        {
            var mousePosition = Mouse.current.position.value;
            return mousePosition.x > Screen.width || mousePosition.x < 0 || mousePosition.y > Screen.height ||
                   mousePosition.y < 0;
        }
    }
}