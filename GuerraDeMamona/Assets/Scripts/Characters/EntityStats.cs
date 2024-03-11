using UnityEngine;

[CreateAssetMenu(fileName = "EntityStats", menuName = "ScriptableObjects/EntityStats", order = 1)]
public class EntityStats : ScriptableObject
{
    [Header("Attributes")]
    [SerializeField] protected float maxHp = 10f;
    [HideInInspector] public float MaxHp => maxHp;


    [SerializeField] protected float attackDmg = 1f;
    [HideInInspector] public float AttackDmg => attackDmg;

    [SerializeField] protected int range = 3;
    [HideInInspector] public int Range => range;


    [SerializeField] protected float moveSpeed = 4f;
    [HideInInspector] public float MoveSpeed => moveSpeed;


    [SerializeField] protected Sprite portraitSprite;
    [HideInInspector] public Sprite PortraitSprite => portraitSprite;


    //TODO: Animator

}
