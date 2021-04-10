using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;



public class EnemyBoss : LivingEntity
{

    public enum State // Enemy FSM
    {
        Chase,
        Idle,
        Attack, // 기본 공격
        TurnAttack, // 부채꼴 휘두르기
        JumpAttack, // 점프 공격
        DualAttack, // 돌진? 공격
        Hit,
        Dead
    }
    public State state = State.Chase;

    private NavMeshAgent pathFinder;
    private Animator enemyBsAnimator;

    private float attackRange = 2f;
    public float damage = 20f;
    public float attackDelay = 2f;
    private float lastAttackTime;

    [SerializeField]
    private float chasingSpeed = 2f;

    private bool isChaing;
    private bool isAttack;
    private bool isIdel;
    private bool isTurnAttack;
    private bool isDualAttack;
    private bool isJumpAttack;

    private float dist;

    Transform player;

    private Vector3 targetPosition;

    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyBsAnimator = GetComponent<Animator>();
    }

    #region 초기 능력치 셋업
    public void Setup(float health, float damage, float chasingSpeed)
    {
        // 체력 설정
        this.startingHealth = health; // 초기 시작 체력
        this.damage = damage;  // 공격력
        pathFinder.speed = chasingSpeed; // 위에서 변경된 patrolSpeed로 다시 적용
    }
    #endregion

    void Start()
    {
        SetStartingHp(500f);
        onHit = TriggerHit;
        onDeath = EnemeyDie;
        this.pathFinder.velocity = Vector3.zero;
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {
        switch (state)
        {
            // case State.Idle: { patrol(); break; }
            case State.Chase: { Chase(); break; }
                // case State.Attack: { Attack(); break; }
                // case State.TurnAttack: { TurnAttack(); break; }
                // case State.DualAttack: { DualAttack(); break; }
                // case State.JumpAttack: { JumpAttack(); break; }
                // case State.Hit: { break; }
                // case State.Dead: { break; }
        }

    }
    #region
    protected void Chase()
    {
        if (GetDistancePlayer() < attackRange)
        {
            state = State.Attack;
        }
        else
        {
            // state = State.Chase;
            Chasing();
        }


    }
    #endregion

    #region 정찰 활성화
    protected void Chasing()
    {
        enemyBsAnimator.SetBool("isChasing", true);
        pathFinder.SetDestination(player.position);
    }
    #endregion



    #region 공격 활성화
    protected void Attacking()
    {
        if (!dead)
        {
            targetPosition = new Vector3(player.position.x, transform.position.y - 0.3f, player.position.z);
            this.transform.LookAt(targetPosition);

            if (lastAttackTime + attackDelay <= Time.time)
            {
                pathFinder.speed = 0f;
                pathFinder.isStopped = true;
                enemyBsAnimator.SetBool("isAttack", true);
                lastAttackTime = Time.time;
            }
            else
            {
                pathFinder.isStopped = false;
            }

        }

    }
    #endregion

    #region // 적과 플레이어의 거리
    float GetDistancePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance;
    }
    #endregion

    public void TriggerHit()
    {
        enemyBsAnimator.SetTrigger("Hit");
    }
    public void EnemeyDie()
    {
        state = State.Dead;
        enemyBsAnimator.SetTrigger("Die");
        enemyBsAnimator.SetBool("isAttack", false);
        enemyBsAnimator.SetBool("isChasing", false);
        enemyBsAnimator.SetBool("isTurnAttack", false);
        enemyBsAnimator.SetBool("isDualAttack", false);
        enemyBsAnimator.SetBool("isJumpAttack", false);
        enemyBsAnimator.SetBool("isIdle", false);
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.enabled = false;
        //pathFinder.isStopped = true;
        pathFinder.enabled = false;
        Debug.Log("TLqkfBossDieㅋ");
    }


}
