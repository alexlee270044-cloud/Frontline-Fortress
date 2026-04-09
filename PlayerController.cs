using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    private float applySpeed;

    [Header("점프 관련")]
    [SerializeField] private float jumpForce = 6f;
    private bool isGround = true;

    [Header("앉기 관련")]
    [SerializeField] private float crouchCameraOffset = -0.5f;
    private float originPosY;
    private float applyCrouchPosY;
    private bool isCrouch = false;

    [Header("카메라 관련")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float cameraRotationLimit = 45f;
    private float currentCameraRotationX;
    [SerializeField] private Camera theCamera;

    private bool isRunning = false;

    private Rigidbody myRigid;
    private CapsuleCollider capsuleCollider;
    private Animator animator;

    private float fireCooldown = 0.1f;
    private float lastFireTime = 0f;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;


        if (animator != null)
            animator.applyRootMotion = false;

        // 🔥 카메라 회전값 초기화
        currentCameraRotationX = Mathf.DeltaAngle(0f, theCamera.transform.localEulerAngles.x);

        // 🔹 추가
        myRigid.freezeRotation = true;
    }
    void Update()
    {
        isGround = IsGround();

        TryJump();
        TryRun();
        TryCrouch();
        CameraRotation();
        CharacterRotation();
        TryFire();
        RunShoot();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void RunShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("RunShoot");
        }
    }

    // 🔫 발사
    private void TryFire()
    {
        if (Input.GetMouseButton(0) && Time.time - lastFireTime >= fireCooldown)
        {
            lastFireTime = Time.time;
            animator.ResetTrigger("Fire");
            animator.SetTrigger("Fire");
        }
    }

    // 👇 앉기
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch(true);
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            Crouch(false);
    }

    private void Crouch(bool crouch)
    {
        isCrouch = crouch;
        applyCrouchPosY = originPosY + (crouch ? crouchCameraOffset : 0f);
        animator.SetBool("isCrouch", isCrouch);

        StopCoroutine("CrouchCoroutine");
        StartCoroutine("CrouchCoroutine");
    }

    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (Mathf.Abs(_posY - applyCrouchPosY) > 0.01f)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15) break;
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // 👣 지면 감지
    private bool IsGround()
    {
        float rayLength = capsuleCollider.bounds.extents.y + 0.2f;
        bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, rayLength);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * rayLength, grounded ? Color.green : Color.red);
        return grounded;
    }

    // 🦘 점프
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
            animator.SetTrigger("Jump");
        }
    }

    private void Jump()
    {
        Crouch(false);
        myRigid.velocity = new Vector3(myRigid.velocity.x, 0f, myRigid.velocity.z);
        myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // 🏃 달리기
    private void TryRun()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    // 🏃 이동
    private void Move()
    {
        float _moveDirX = Input.GetAxis("Horizontal");
        float _moveDirZ = Input.GetAxis("Vertical");

        Vector3 _moveDirection = (transform.right * _moveDirX + transform.forward * _moveDirZ).normalized;
        applySpeed = isCrouch ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        // ✅ Y속도는 건드리지 않고, XZ만 제어
        Vector3 desiredVelocity = _moveDirection * applySpeed;
        Vector3 currentVelocity = myRigid.velocity;
        Vector3 velocityChange = new Vector3(
            desiredVelocity.x - currentVelocity.x,
            0,
            desiredVelocity.z - currentVelocity.z
        );

        myRigid.AddForce(velocityChange, ForceMode.VelocityChange);

        bool isMoving = Mathf.Abs(_moveDirX) > 0.1f || Mathf.Abs(_moveDirZ) > 0.1f;
        animator.SetBool("isMoving", isMoving);
    }

    // 🔄 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxis("Mouse X") * lookSensitivity;
        transform.Rotate(0f, _yRotation, 0f);
    }

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxis("Mouse Y") * lookSensitivity;
        currentCameraRotationX -= _xRotation;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
