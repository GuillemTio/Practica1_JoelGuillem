using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }

    TState m_State;
    TState m_LastState;
    NavMeshAgent m_NavMeshAgent;
    public float m_MinDistanceToAttack;
    public float m_HearsPlayerRadius;
    public List<Transform> m_PatrolPositions;
    public List<GameObject> m_Boxes;
    int m_CurrentPatrolPositionId = 0;
    public float m_MaxDistanceToSeePlayer;
    public float m_ConeVisionAngle;
    public float m_AlertRotationVelocity;
    public LayerMask m_SeesPlayerLayerMask;
    public GameObject m_ImpactParticles;

    int m_Life;
    public int m_MaxLife = 100;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    Quaternion m_LastRotationPose;
    float m_RotationOffset = 2f;
    bool m_CanStopRotating;

    public Material m_EnemyMaterial;
    public float m_TimeToFade;
    float m_FadeTimer = 0;

    [Header("LifeBar")]
    public Transform m_LifeBarAnchor;
    public RectTransform m_LifeBarBackgroundRectTransform;
    public Image m_LifeBarImage;
    public float m_LifeBarOffset;

    [Header("Attack")]
    public float m_ShootDamage;
    public float m_TimeToShoot;
    float m_ShootTimer = 0;
    public float m_MaxDistanceToShoot;
    float m_DistanceOffset = 0.0001f;


    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    void Start()
    {
        m_Life = m_MaxLife;
        GameController.GetGameController().AddEnemy(this);
        SetIdleState();
    }

    private void OnDestroy()
    {
        GameController l_GameController = GameController.GetGameController();
        if (l_GameController != null)
        {
            l_GameController.RemoveEnemy(this);
        }
    }

    private void Update()
    {
        switch (m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;
            default:
                break;
        }
        //que se vea segun distancia y raycast(cuando el jugador vea al dron)
        UpdateLifeBarPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_HearsPlayerRadius);
        Gizmos.DrawWireSphere(transform.position, m_MaxDistanceToSeePlayer);
        Gizmos.DrawWireSphere(transform.position, m_MinDistanceToAttack);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MaxDistanceToShoot);
    }

    private void UpdateLifeBarPosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_DistanceToPlayer = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);


        Vector3 l_EnemyToPlayer = l_PlayerPosition - l_EnemyPosition;

        l_EnemyToPlayer.y = 0.0f;
        l_EnemyToPlayer.Normalize();

        Ray l_Ray = new Ray(l_EnemyPosition + Vector3.up * 1.8f, l_EnemyToPlayer);
        Debug.DrawRay(l_EnemyPosition, l_EnemyToPlayer * l_DistanceToPlayer, Color.yellow);
        if (!Physics.Raycast(l_Ray, l_DistanceToPlayer, m_SeesPlayerLayerMask.value))
        {
            if (!m_LifeBarBackgroundRectTransform.gameObject.activeSelf)
            {
                m_LifeBarBackgroundRectTransform.gameObject.SetActive(true);
            }
            Vector3 l_ViewportPosition = GameController.GetGameController().m_Player.m_Camera.WorldToViewportPoint(m_LifeBarAnchor.position+(Vector3.up*m_LifeBarOffset));
            m_LifeBarBackgroundRectTransform.anchoredPosition = new Vector3(l_ViewportPosition.x * Screen.width, -(Screen.height - l_ViewportPosition.y * Screen.height));
            m_LifeBarBackgroundRectTransform.gameObject.SetActive(l_ViewportPosition.z >= 0f);
            ShowLifeBar();
        }
        else if (m_LifeBarBackgroundRectTransform.gameObject.activeSelf)
        {
            m_LifeBarBackgroundRectTransform.gameObject.SetActive(false);
        }
    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_NavMeshAgent.isStopped = false;
        m_CanStopRotating = false;
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_LastRotationPose = transform.rotation;
        m_NavMeshAgent.isStopped = true;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_CanStopRotating = false;
        m_NavMeshAgent.isStopped = false;
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }
    void SetHitState()
    {
        m_LastState = m_State;
        m_State = TState.HIT;

    }
    void SetDieState()
    {
        m_State = TState.DIE;

    }

    void SetLastState()
    {
        m_State = m_LastState;
        Debug.Log(m_State);
    }

    void UpdateIdleState()
    {
        SetPatrolState();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            NextPatrolPosition();
        if (HearsPlayer())
            SetAlertState();

    }
    void UpdateAlertState()
    {
        transform.Rotate(new Vector3(0, m_AlertRotationVelocity * Time.deltaTime, 0));
        if ((int)transform.rotation.eulerAngles.y > ((int)m_LastRotationPose.eulerAngles.y + m_RotationOffset) && (!m_CanStopRotating))
            m_CanStopRotating = true;

        if (((int)transform.rotation.eulerAngles.y > ((int)m_LastRotationPose.eulerAngles.y - m_RotationOffset)) &&
            ((int)transform.rotation.eulerAngles.y < ((int)m_LastRotationPose.eulerAngles.y + m_RotationOffset)) && m_CanStopRotating)
        {
            transform.rotation = m_LastRotationPose;
            SetPatrolState();
        }
        else if (SeesPlayer())
            SetChaseState();


    }
    void UpdateChaseState()
    {
        SetNextChasePosition();
        if (!SeesPlayer())
            SetPatrolState();

        if (CanStartAttacking())
            SetAttackState();
    }


    void UpdateAttackState()
    {
        if (DistanceToPlayer() > m_MaxDistanceToShoot)
            SetChaseState();
        if (CanShoot())
        {
            m_ShootTimer = 0;
            Shoot();
        }

    }

    void UpdateHitState()
    {
        if (m_LastState == TState.PATROL || m_LastState == TState.IDLE)
            SetAlertState();
        else
            SetLastState();
    }

    void UpdateDieState()
    {
        SetMaterialModeTransparent(m_EnemyMaterial);
        m_EnemyMaterial.color = Color.Lerp(m_EnemyMaterial.color, new Color(m_EnemyMaterial.color.r, m_EnemyMaterial.color.g, m_EnemyMaterial.color.b, 0), m_FadeTimer / m_TimeToFade);

        m_FadeTimer += Time.deltaTime;
        if (m_FadeTimer > m_TimeToFade)
        {
            m_FadeTimer = 0;
            gameObject.SetActive(false);
            SetMaterialModeOpaque(m_EnemyMaterial);
            m_EnemyMaterial.color = new Color(m_EnemyMaterial.color.r, m_EnemyMaterial.color.g, m_EnemyMaterial.color.b, 1);
            DropRandomItem();
        }
    }

    private void DropRandomItem()
    {
        float l_RandomNum = UnityEngine.Random.Range(0, m_Boxes.Count-1);
        Instantiate(m_Boxes[(int)l_RandomNum], transform.position, transform.rotation);
    }

    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        Vector3 l_DirectionToEnemy = l_EnemyPosition - l_PlayerPosition;
        l_DirectionToEnemy.Normalize();
        Vector3 l_DesiredPosition = l_PlayerPosition + l_DirectionToEnemy * (m_MinDistanceToAttack - m_DistanceOffset);
        m_NavMeshAgent.SetDestination(l_DesiredPosition);
    }

    void NextPatrolPosition()
    {
        ++m_CurrentPatrolPositionId;
        if (m_CurrentPatrolPositionId >= m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;
        MoveToNextPatrolPosition();
    }

    void MoveToNextPatrolPosition()
    {
        m_NavMeshAgent.SetDestination(m_PatrolPositions[m_CurrentPatrolPositionId].position);
    }

    bool HearsPlayer()
    {
        return DistanceToPlayer() < m_HearsPlayerRadius;
    }

    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_DistanceToPlayer = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);

        if (DistanceToPlayer() < m_MaxDistanceToSeePlayer)
        {
            Vector3 l_EnemyForward = transform.forward;
            l_EnemyForward.y = 0.0f;
            l_EnemyForward.Normalize();

            Vector3 l_EnemyToPlayer = l_PlayerPosition - l_EnemyPosition;
            l_EnemyToPlayer.y = 0.0f;
            l_EnemyToPlayer.Normalize();

            float l_DotAngle = Vector3.Dot(l_EnemyForward, l_EnemyToPlayer);

            if (l_DotAngle >= Mathf.Cos(Mathf.Deg2Rad * m_ConeVisionAngle / 2f))
            {
                Ray l_Ray = new Ray(l_EnemyPosition + Vector3.up * 1.8f, l_EnemyToPlayer);
                if (!Physics.Raycast(l_Ray, l_DistanceToPlayer, m_SeesPlayerLayerMask.value))

                    return true;
            }
        }

        return false;
    }
    public void Hit(int LifePoints, Vector3 HitPosition)
    {
        m_Life -= LifePoints;

        if (LifePoints > 0)
            Instantiate(m_ImpactParticles,HitPosition,Quaternion.identity);

        if (m_Life <= 0)
            SetDieState();

        else
            SetHitState();
    }
    private float DistanceToPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        return Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
    }
    private bool CanStartAttacking()
    {
        return DistanceToPlayer() <= m_MinDistanceToAttack;
    }
    private bool CanShoot()
    {
        m_ShootTimer += Time.deltaTime;
        return (DistanceToPlayer() < m_MaxDistanceToShoot) && (m_ShootTimer > m_TimeToShoot);
    }
    private void Shoot()
    {
        GameController.GetGameController().m_Player.TakeDamage(m_ShootDamage);
    }
    public void RestartLevel()
    {
        gameObject.SetActive(true);
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_NavMeshAgent.enabled = true;
        m_Life = m_MaxLife;
        SetIdleState();
    }

    void ShowLifeBar()
    {
        m_LifeBarImage.fillAmount = m_Life / (float)m_MaxLife;
    }

    private void SetMaterialModeOpaque(Material m)
    {
        m.SetFloat("_Mode", 0);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        m.SetInt("_ZWrite", 1);
        m.DisableKeyword("_ALPHATEST_ON");
        m.DisableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        m.renderQueue = -1;
    }

    private void SetMaterialModeTransparent(Material m)
    {
        m.SetFloat("_Mode", 2);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        m.renderQueue = 3000;
    }

}
