using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float mouseSensitivity = 2.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Скрыть и зафиксировать курсор в центре экрана
    }

    void Update()
    {
        float moveForwardBackward = Input.GetAxis("Vertical") * speed;
        float moveLeftRight = Input.GetAxis("Horizontal") * speed;

        // Получение значений поворота камеры
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity; // Инвертирование для естественного вертикального обзора
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Ограничение вертикального обзора

        // Поворот персонажа и камеры
        transform.localRotation = Quaternion.Euler(0, rotationX, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);

        // Движение персонажа
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(moveLeftRight, 0, moveForwardBackward);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Применение гравитации
        moveDirection.y -= gravity * Time.deltaTime;

        // Перемещение персонажа
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
