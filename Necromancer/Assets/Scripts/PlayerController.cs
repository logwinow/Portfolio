using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float normalSpeed = 10f;
    [SerializeField]
    float runSpeed = 10f;
    [SerializeField]
    private float rotationSensetive = 4f;
    [SerializeField]
    private float rotMin = -90f;
    [SerializeField]
    private float rotMax = 90f;
    [SerializeField]
    Transform m_camera;
    [SerializeField]
    float digDistance;
    [SerializeField]
    float cameraCheckDistance = 5f;
    [SerializeField]
    Transform groundPoint;
    [SerializeField]
    GameObject shovel;
    [SerializeField]
    AudioClip shovelDigSound;
    [SerializeField]
    AudioClip shovelAirSound;
    [SerializeField]
    AudioClip[] walkingAudio;
    [SerializeField]
    AudioClip[] runningAudio;
    Animator shovelAnim;
    Rigidbody rb;
    Vector3 move;

    AudioSource audioSource;
    Interactable currentInteractableObject;

    public static Vector3 currentPosition;

    private float yRot;

    private void Awake()
    {
        if (m_camera == null)
            m_camera = Camera.main.transform;

        shovelAnim = shovel.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        currentPosition = transform.position;
        if (!GameManager.Instance.IsPaused)
        {
            Move();

            Rotation();

            if (CheckInteract())
            {
                if (Input.GetMouseButtonDown(0))
                    currentInteractableObject?.Interact();
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !shovelAnim.GetBool("isDig") && GameManager.Instance.IsCurrentShovel)
                {
                    shovelAnim.SetBool("isDig", true);
                    Dig();
                }

                if (currentInteractableObject != null)
                {
                    GameManager.Instance.SetCursor(CursorType.Default);
                    currentInteractableObject = null;
                }
            }
        }
        else
        {
            move = Vector3.zero;
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.CloseActivePopup();
            }
        }
    }

    private bool CheckInteract()
    {
        Ray rayCam = new Ray(m_camera.transform.position, m_camera.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(rayCam, out hit, cameraCheckDistance))
        {
            Interactable inter = hit.transform.gameObject.GetComponent<Interactable>();

            if (inter != null)
            {
                currentInteractableObject = inter;

                if (currentInteractableObject is UnderGrave && !GameManager.Instance.IsCurrentShovel)
                    GameManager.Instance.SetCursor(CursorType.TakeItAlive);
                else if (currentInteractableObject is ExitDoors)
                {
                    if (((ExitDoors)currentInteractableObject).CanExit)
                    {
                        GameManager.Instance.SetCursor(CursorType.Take);
                    }
                }
                else
                    GameManager.Instance.SetCursor(CursorType.See);

                return true;
            }
        }

        return false;
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float cspeed = normalSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            cspeed = runSpeed;

        float rotTmp = transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        move = Vector3.ClampMagnitude(transform.TransformDirection(new Vector3(x, 0, z)), 1f) * cspeed;

        if (move != Vector3.zero && !audioSource.isPlaying )
        {
            if (cspeed == runSpeed)
            {
                audioSource.PlayOneShot(runningAudio[Random.Range(0, runningAudio.Length)]);
            }
            else
                audioSource.PlayOneShot(walkingAudio[Random.Range(0, walkingAudio.Length)]);
        }

        transform.rotation = Quaternion.Euler(rotTmp, 0, 0) * transform.rotation;
    }

    private void Rotation()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

        yRot = Mathf.Clamp(yRot - y * rotationSensetive, rotMin, rotMax);

        transform.rotation *= Quaternion.Euler(0, x * rotationSensetive, 0);
        m_camera.localRotation = Quaternion.Euler(yRot, 0, 0);
    }

    private void Dig()
    {
        Ray rayCam = new Ray(m_camera.transform.position, m_camera.transform.forward);
        RaycastHit hitCam;

        if (Physics.Raycast(rayCam, out hitCam, digDistance))
        {
            GraveDirtController gdc = hitCam.transform.gameObject.GetComponentInParent<GraveDirtController>();
            if (gdc != null && gdc?.dirtWhole != null)
            {
                audioSource.PlayOneShot(shovelDigSound, .5f);
                gdc.OnDig();
            }
            else
            {
                audioSource.PlayOneShot(shovelAirSound, .3f);
            }
        }
        else
            audioSource.PlayOneShot(shovelAirSound, .3f);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(move.x, Physics.gravity.y/2f, move.z);
    }
}
