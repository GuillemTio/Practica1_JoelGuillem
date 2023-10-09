using System;
using UnityEngine;
public class FPSController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    float m_VerticalSpeed;
    public Transform m_PitchController;

    internal bool CanPickAmmo()
    {
        return true;
    }

    public float m_YawSpeed;

    internal void AddAmmo(int m_AmmoCount)
    {
        //not implemented
    }

    public float m_PitchSpeed;
    public bool m_YawInverted;
    public bool m_PitchInverted;
    public float m_MinPitch;
    public float m_MaxPitch;
    public float m_Speed;
    public float m_SprintSpeed;
    public float m_JumpSpeed;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Animation")]
    public Animation m_WeaponAnimation;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ReloadAnimationClip;

    public Camera m_Camera;

    CharacterController m_CharacterController;

    [Header("Shoot")]
    public float m_MaxShootDistance;
    public LayerMask m_LayerMask;
    public GameObject m_HitParticlesPrefab;


    [Header("Input")]

    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public int m_ShootMouseButton = 0;

    [Header("DebugInput")]
    bool m_AngleLocked = false;
    bool m_AimLocked = true;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    private void Awake()
    {

        m_CharacterController = GetComponent<CharacterController>();
        if(GameController.GetGameController().m_Player == null)
        {
            GameController.GetGameController().m_Player = this;
            GameObject.DontDestroyOnLoad(gameObject);
            m_StartPosition = transform.position;
            m_StartRotation = transform.rotation;

            m_Yaw = transform.rotation.eulerAngles.y;
        }
        else
        {
            GameController.GetGameController().m_Player.SetStartPosition(transform);
            GameObject.Destroy(this.gameObject);
        }

    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SetIdleWeaponAnimation();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
#endif


        float l_HorizontalMovement = Input.GetAxis("Mouse X");
        float l_VerticalMovement = Input.GetAxis("Mouse Y");

        if (m_AngleLocked)
        {
            l_HorizontalMovement = 0f;
            l_VerticalMovement = 0f;
        }



        float l_Speed = m_Speed;

        if(Input.GetKeyDown(m_JumpKeyCode)&& m_VerticalSpeed == 0)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }

        if (Input.GetKey(m_SprintKeyCode))
        {
            l_Speed = m_SprintSpeed;
        }
        /*
        float l_YawInverted = 1f;
        float l_PitchInverted = 1f;

        if (m_YawInverted)
        {
            l_YawInverted = -1.0f;
        }
        if (m_PitchInverted)
        {
            l_PitchInverted = -1.0f;
        }*/

        float l_YawInverted = m_YawInverted ? -1f : 1f;
        float l_PitchInverted = m_PitchInverted ? -1f : 1f;

        m_Yaw = m_Yaw + m_YawSpeed * l_HorizontalMovement * Time.deltaTime * l_YawInverted;
        m_Pitch = m_Pitch + m_PitchSpeed * l_VerticalMovement * Time.deltaTime * l_PitchInverted;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90) * Mathf.Deg2Rad;

        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0, Mathf.Cos(l_Yaw90InRadians));

        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
        }
        else if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
        }

        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement -= l_Forward;
        }

        l_Movement.Normalize();

        l_Movement *= l_Speed * Time.deltaTime;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow)!= 0)
            m_VerticalSpeed = 0f;
        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0 && m_VerticalSpeed>0f)
            m_VerticalSpeed = 0f;

        //m_CharacterController.Move(l_Movement);

        if (Input.GetMouseButtonDown(m_ShootMouseButton) && CanShoot())
        {
            Shoot();
        }
        if (Input.GetKeyDown(m_ReloadKeyCode)&& CanReload())
        {
            Reload();
        }
    }

    private bool CanReload()
    {
        return true;
    }

    private void Reload()
    {
        SetReloadWeaponAnimation();
    }

    bool CanShoot()
    {
        return true;
    }

    private void Shoot()
    {
        SetShootWeaponAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxShootDistance, m_LayerMask.value))
        {
            CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);
        }
    }

    private void CreateShootHitParticles(Vector3 point, Vector3 normal)
    {
        GameObject l_HitParticles = GameObject.Instantiate(m_HitParticlesPrefab,GameController.GetGameController().m_DestroyObjects.transform);
        l_HitParticles.transform.position = point;
        l_HitParticles.transform.rotation = Quaternion.LookRotation(normal);
    }

    void SetIdleWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_IdleAnimationClip.name);
    }

    void SetShootWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ShootAnimationClip.name,0.1f);
        m_WeaponAnimation.CrossFadeQueued(m_IdleAnimationClip.name,0.1f);
    }

    void SetReloadWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ReloadAnimationClip.name, 0.1f);
        m_WeaponAnimation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
    }
    public void RestartLevel()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = 0.0f;
        m_CharacterController.enabled = true;
    }
    void SetStartPosition(Transform startTransform)
    {
        m_StartPosition = startTransform.position;
        m_StartRotation = startTransform.rotation;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = 0.0f;
        m_CharacterController.enabled = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item l_Item = other.GetComponent<Item>();
            if (l_Item.CanPick()) l_Item.Pick();
        }
    }
}