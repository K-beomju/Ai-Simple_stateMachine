using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemys : LivingObjs
{
    public enum State // Enemy FSM
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public State state = State.Patrol;

    public LayerMask whatIsTarget; // 추적 레이어
    private LivingObjs targetEntity; // 추적 대상

    private NavMeshAgent pathFinder; // 경로 계산 AI
    private Animator enemyAnimator; // 애니메이터

    private float attackRange = 3f; // 공격범위
    public float damage = 20f; // 데미지
    public float attackDelay = 1f; // 공격 딜레이
    private float lastAttackTime; // 마지막 공격시간

    public float patrolSpeed = 3f; // 정찰 스피드

    public float chasingSpeed = 5f; // 추적 스피드

    public Transform tr;

    // 추적 대상 존재여부 and 추적 대상 죽음여부
    private bool hasTarget => targetEntity != null && !targetEntity.dead;

    private bool isChaing; // is추적
    private bool isAttack; // is공격
    private bool isIdle;

    // Patrol
    public Transform[] points; // 정찰 포인트
    public int current; // 포인트 갯수
    private float curDist; // 거리
    float timer; // 타이머
    int waitingTime; // 타임

    private float dist; // 추적대상 거리


    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable(); //체력 상속
        tr = GetComponent<Transform>();
    }

    public void Setup(float health, float damage, float chasingSpeed, float patrolSpeed)
    {
        // 체력 설정
        this.startingHealth = health; // 초기 시작 체력
        this.health = health;  // 체력

        // 내비메쉬 에이전트의 이동 속도 설정
        this.chasingSpeed = chasingSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;  // 공격력


        pathFinder.speed = patrolSpeed; // 위에서 변경된 patrolSpeed로 다시 적용
    }

    public float Radisuo = 4f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //  Gizmos.DrawRay(transform.position, transform.forward.normalized * 4.0f);
        Gizmos.DrawCube(transform.position, new Vector3(Radisuo, 0.1f, Radisuo));
    }

    void Start()
    {
        current = 0;
        timer = 0.0f;
        waitingTime = 2;

    }
    void Update()
    {
        switch (state)
        {
            case State.Patrol: { patrol();  break;}

                //  case FSMState.Attack: UpdateAttackState(); break;
                // case FSMState.Dead: UpdateDeadState(); break;
        }

       //  enemyAnimator.SetBool("isChaing", isChaing);
        // enemyAnimator.SetBool("isAttack", isAttack);

       // enemyAnimator.SetBool("isIdle", isIdle);
        if (hasTarget)
        dist = Vector3.Distance(tr.position, targetEntity.transform.position);

    }

    protected void patrol()
    {
          timer -= Time.deltaTime;
          curDist = Vector3.Distance(transform.position, points[current].position);
        if (transform.position != points[current].position && waitingTime > timer)
        {
            state = State.Patrol;
            pathFinder.isStopped = false;
            isIdle = false;
            pathFinder.speed = 2f;
            pathFinder.SetDestination(points[current].position);
        }
        if (curDist < 1f)
        {
            timer = 5f;
           enemyAnimator.SetTrigger("New Trigger");
            pathFinder.isStopped = true;
            current = (current + 1) % points.Length;
        }

    }
    protected void FindPoint()
    {


    }



}
