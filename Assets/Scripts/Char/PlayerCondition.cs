using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    private PlayerController playerController;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue == 0.0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0.0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void JumpCount(float amount, float duration)
    {
        StartCoroutine(JumpUpCoroutine(amount, duration));
    }

    public IEnumerator JumpUpCoroutine(float amount, float duration)
    {
        playerController.maxJumpCount += (int)amount;
        yield return new WaitForSeconds(duration);
        playerController.maxJumpCount -= (int)amount;
    }

    //float value;
    //public void MoveSpeed(float amount)
    //{
    //    playerController.moveSpeed += amount;
    //    value = amount;
    //    Invoke("AA", 5);
    //}

    //public void AA()
    //{
    //    playerController.moveSpeed -= value;
    //}

    public void MoveSpeed(float amount, float duration)
    {
        StartCoroutine(SpeedUpCoroutine(amount, duration));
    }
    IEnumerator SpeedUpCoroutine(float amount, float duration)
    {
        playerController.moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        playerController.moveSpeed -= amount;
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}