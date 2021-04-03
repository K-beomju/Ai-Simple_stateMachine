using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;




public class LivingObjs : MonoBehaviour, IDamageAble
{

    public float startingHealth = 100f; //시작 체력
    public float health { get; protected set; } // 체력
    public bool dead { get; protected set; } //사망
    public event Action onDeath; //사망 시 발동



    #region   //생명체가 활성화될 떄 상태를 리셋


    protected virtual void OnEnable()
    {
        //dead = false;
        health = startingHealth;
    }
    #endregion


    #region   //피해를 받는 기능

    public void OnDamage(float damage)
    {
         if (!dead)
        health -= damage; // health = health - damage;


        if (health <= 0 /*&!dead*/)
        {
            Die();
        }
    }
    #endregion


    #region //사망 처리

    public virtual void Die()
    {
        //onDeath 이벤트에 등록된 메서드가 있다면 실행
        if (onDeath != null)
         {
            onDeath();
         }
            dead = true;
    }

}
#endregion