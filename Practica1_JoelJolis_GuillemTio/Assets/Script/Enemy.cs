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
    NavMeshAgent m_NavMeshAgent;
    public float m_MinDistanceToAttack;
    public float m_HearsPlayerRadius;
    public List<Transform> m_PatrolPositions;
    int m_CurrentPatrolPositionId = 0;
    public float m_MaxDistanceToSeePlayer;
    private float m_ConeVisionAngle;
    public LayerMask m_SeesPlayerLayerMask;
    int m_Life;
    public int m_MaxLife = 100;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("LifeBar")]
    public Transform m_LifeBarAnchor;
    public RectTransform m_LifeBarBackgroundRectTransform;
    public Image m_LifeBarImage;

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
        ShowLifeBar();
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
    }

    private void UpdateLifeBarPosition()
    {
        Vector3 l_ViewportPosition = GameController.GetGameController().m_Player.m_Camera.WorldToViewportPoint(m_LifeBarAnchor.position);
        m_LifeBarBackgroundRectTransform.anchoredPosition = new Vector3(l_ViewportPosition.x * Screen.width, -(Screen.height - l_ViewportPosition.y * Screen.height));
        m_LifeBarBackgroundRectTransform.gameObject.SetActive(l_ViewportPosition.z >= 0f);
        ShowLifeBar();
    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
    }
    void SetChaseState()
    {
        m_State = TState.CHASE;
    }
    void SetAttackState()
    {
        m_State = TState.ATTACK;
    }
    void SetHitState()
    {
        m_State = TState.HIT;
    }
    void SetDieState()
    {
        m_State = TState.DIE;
        gameObject.SetActive(false);
        //animacion
    }
    void UpdateIdleState()
    {
        SetPatrolState();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            NextPatrolPosition();

    }
    void UpdateAlertState()
    {

    }
    void UpdateChaseState()
    {

    }
    void UpdateAttackState()
    {

    }


    void UpdateHitState()
    {

    }

    void UpdateDieState()
    {

    }

    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        Vector3 l_DirectionToEnemy = l_EnemyPosition - l_PlayerPosition;
        l_DirectionToEnemy.Normalize();
        Vector3 l_DesiredPosition = l_PlayerPosition + l_DirectionToEnemy * m_MinDistanceToAttack;
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
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_DistanceToPlayer = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        return l_DistanceToPlayer < m_HearsPlayerRadius;
    }

    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        float l_DistanceToPlayer = Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
        if (l_DistanceToPlayer < m_MaxDistanceToSeePlayer)
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
    public void Hit(int LifePoints)
    {
        m_Life -= LifePoints;
        Debug.Log(m_Life);
        if (m_Life <= 0)
        {
            SetDieState();
        }
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

}
