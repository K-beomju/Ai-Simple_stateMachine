    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingObjs
{
    private enum State
    {
        Idle,
        Patrol,
        Chasing,
        Attack,
        Dead
    }
    private State state = State.Idle;

    public LayerMask whatIsTarget;
    private LivingObjs targetEntity;

    private NavMeshAgent pathFinder;

    private Animator enemyAnimator;

    public float damage = 20f;
    public float attackDelay = 1f;
    private float lastAttackTime;
    private float dist;

    public Transform tr;
    private float attackRange = 3f;

    private bool isChaing;
    private bool isAttack;
    private bool isDie;

    #region // 타겟 탐지
    private bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }
    #endregion


    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();

    }

     #region // 초기 능력치 셋업
    public void Setup(float newHealth, float newDamage, float newSpeed)
    {
        startingHealth = newHealth;
        health = newHealth;
        damage = newDamage;
        pathFinder.speed = newSpeed;
    }
   #endregion


    void Start()
    {
        StartCoroutine(UpdatePath());
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        enemyAnimator.SetBool("isChaing", isChaing);
        enemyAnimator.SetBool("isAttack", isAttack);

        if (hasTarget)
          dist = Vector3.Distance(tr.position, targetEntity.transform.position);
    }

 #region // 추적
    private IEnumerator UpdatePath()
    {
        while (!dead)
        {
            if (hasTarget)
            {
                Attack();
            }
            else
            {
                //추적 대상이 없을 경우, AI 이동 정지
                pathFinder.isStopped = true;
                isAttack = false;
                isChaing = false;


                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);


                for (int i = 0; i < colliders.Length; i++)
                {

                    LivingObjs livingObjs = colliders[i].GetComponent<LivingObjs>();


                    if (livingObjs != null && !livingObjs.dead)
                    {

                        targetEntity = livingObjs;

                        //for문 루프 즉시 정지
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
 #endregion

 #region // 어택
    public virtual void Attack()
    {
        if (!dead && dist < attackRange)
        {
            isChaing = false;

            this.transform.LookAt(targetEntity.transform);

            if (lastAttackTime + attackDelay <= Time.time)
            {
                isAttack = true;
                lastAttackTime = Time.time;
            }
            else
            {
                isAttack = false;
            }
        }
        else
        {
            isChaing = true;
            isAttack = false;
            pathFinder.isStopped = false;
            pathFinder.SetDestination(targetEntity.transform.position);
        }
    }
    #endregion

    #region // 데미지 이벤트
    public void OnDamageEvent()
    {
        IDamageAble attackTarget = targetEntity.GetComponent<IDamageAble>();
        attackTarget.OnDamage(damage); // 공격 처리
        lastAttackTime = Time.time; // 갱신
    }
    #endregion
}
