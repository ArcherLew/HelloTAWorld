using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    public float speed = 20.0f;
    public float jumpSpeed = 20.0f;
    public float gravity = 50.0f;
    private Vector3 moveDirection = Vector3.zero;

    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxes m_axes = RotationAxes.MouseXAndY;
    public float m_sensitivityX = 5f;
    public float m_sensitivityY = 5f;
    // 水平方向的 镜头转向
    public float m_minimumX = -360f;
    public float m_maximumX = 360f;
    // 垂直方向的 镜头转向 (这里给个限度 最大仰角为45°)
    public float m_minimumY = -45f;
    public float m_maximumY = 45f;
    float m_rotationY = 0f;

    Rigidbody rigidBody;

    Camera mainCamera, roleCamera;
    Camera currentCamera;
    int cameraState = 1;

    void Start()
    {
        // rigidBody = GetComponent<Rigidbody>();
        // rigidBody.freezeRotation = true;
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        roleCamera = GameObject.Find("/Role/RoleCamera").GetComponent<Camera>();
        currentCamera = mainCamera;
        cameraState = 1;

        crownLayer = LayerMask.NameToLayer("Crown");
        Util.LogR("crownLayer", crownLayer);
    }

    void Update()
    {
        CameraHandler();
        ClickHandler();

        if (cameraState == 2)
        {
            MoveHandler();
            ViewHandler();
        }
    }

    private RaycastHit hit;
    private int crownLayer;

    void ClickHandler()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Util.Log("mouse 0");

            // 镜头中心开火
            // Vector3 pos = currentCamera.transform.position;
            // Vector3 dir = currentCamera.transform.TransformDirection(Vector3.forward);
            // DrawTool.DrawArrow(pos, pos + dir * 1000, Color.red, 1f, "arrow");

            // 鼠标点选开火
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~crownLayer))
            {
                GameObject obj = hit.transform.gameObject;
                obj.GetComponent<MeshRenderer>().material.SetColor("_UpperColor", Color.red);
                Util.Log(obj.name);
            }
        }
    }

    void CameraHandler()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            mainCamera.enabled = true;
            roleCamera.enabled = false;
            currentCamera = mainCamera;
            cameraState = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            mainCamera.enabled = false;
            roleCamera.enabled = true;
            currentCamera = roleCamera;
            cameraState = 2;
        }
    }

    void MoveHandler()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ViewHandler()
    {
        if (m_axes == RotationAxes.MouseXAndY)
        {
            float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
        }
        else if (m_axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * m_sensitivityX, 0);
        }
        else
        {
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(-m_rotationY, transform.localEulerAngles.y, 0);
        }
    }
}