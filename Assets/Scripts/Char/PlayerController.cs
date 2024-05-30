using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    public int jumpCount = 1;
    public int maxJumpCount = 1;


    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rigidbody;
    public Button button;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;       // 실행 시 마우스 커서 숨기기
    }


    void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded())
            {
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                jumpCount = 1; //초기화
            }
            else if (jumpCount < maxJumpCount)
            {
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                jumpCount++;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        int targetLayer = LayerMask.NameToLayer("wallLayerMask");
        if (collision.gameObject.layer == targetLayer)
        {
            button.gameObject.SetActive(true);
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        int targetLayer = LayerMask.NameToLayer("wallLayerMask");
        if (collision.gameObject.layer == targetLayer)
        {
            button.gameObject.SetActive(false);
        }
    }
    public float rayDistance = 5.0f;
    public float climbDuration = 2.0f;

    public bool isClimbing = false;

    public IEnumerator ClimbWall()
    {
        isClimbing = true;
        Debug.Log("벽에 매달림");

        // 벽에 매달리는 동작
        // 여기서 캐릭터의 애니메이션이나 위치를 조정할 수 있습니다.
        // 예를 들어, 캐릭터를 벽에 붙이는 코드:
        Vector3 climbPosition = transform.position + (transform.forward * 0.5f) + (transform.up * 1.0f);
        transform.position = climbPosition;

        yield return new WaitForSeconds(climbDuration);

        isClimbing = false;
        Debug.Log("벽에서 내려옴");
    }


    public bool IsFrontBlocked()
    {
        Ray[] rays = new Ray[1]
        {
        new Ray(transform.position + (transform.up * 0.01f), transform.forward)
        };

        foreach (Ray ray in rays)
        {
            if (Physics.Raycast(ray, 5.0f, wallLayerMask))
            {
                return true;
            }
        }
        return false;
    }


    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
