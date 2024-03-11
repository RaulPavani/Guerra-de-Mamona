using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor.SceneTemplate;
using UnityEngine;
using TMPro;

public class EntityBase : MonoBehaviour
{
    //TODO: Passar atributos para scriptable object - animator do bixo tambem
    [SerializeField] private EntityStats stats;
    protected float currentHp;

    protected bool canSelect = false;
    protected bool selected = false;
    public bool CanSelect => canSelect;

    [Header("Visual Stats")]
    [SerializeField] private GameObject statsObj;
    [SerializeField] private TextMeshPro attackText;
    [SerializeField] private TextMeshPro lifeText;

    //Animations
    private Animator animator;
    private int selectedBoolAnim = Animator.StringToHash("Selected");
    private int attackTriggerAnim = Animator.StringToHash("Attack");
    private int damageTriggerAnim = Animator.StringToHash("Damage");

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        currentHp = stats.MaxHp;
        canSelect = false;

        attackText.text = $"{stats.AttackDmg}";
        lifeText.text = $"{currentHp}";
        statsObj.SetActive(false);

        EntitiesController.Instance.AddEntityToList(this);
    }

    protected void SetCanSelect(bool state)
    {
        canSelect = state;
    }

    public virtual void Highlight()
    {
        //if (canSelect)
        //{
        animator.SetBool(selectedBoolAnim, true);
        statsObj.SetActive(true);
        //}
    }

    public virtual void RemoveHighlight()
    {
        //if (canSelect && !selected)
        //{
        if (!selected)
        {
            animator.SetBool(selectedBoolAnim, false);
            statsObj.SetActive(false);
        }
        //}
    }

    public virtual void SelectEntity()
    {
        if (canSelect)
        {
            animator.SetBool(selectedBoolAnim, true);
            selected = true;
        }
    }

    public virtual void DeselectEntity()
    {
        animator.SetBool(selectedBoolAnim, false);
        statsObj.SetActive(false);
        selected = false;
    }

    //Movement
    public virtual float GetMoveDuration(Vector2 targetPosition)
    {
        return Vector2.Distance(transform.position, targetPosition) / stats.MoveSpeed;
    }

    //Combat
    public virtual void TakeDamage(int damage)
    {
        animator.SetTrigger(damageTriggerAnim);
        currentHp -= damage;
        attackText.text = $"{currentHp}";

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        EntitiesController.Instance.RemoveEntityFromList(this);
        Destroy(gameObject);
    }

    public virtual void Attack()
    {
        animator.SetTrigger(attackTriggerAnim);
    }

    public Sprite GetPortrait()
    {
        return stats.PortraitSprite;
    }

    public int GetRange()
    {
        return stats.Range;
    }

}
