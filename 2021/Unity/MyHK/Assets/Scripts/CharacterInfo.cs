using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] public int MaxJumpCharges = 1;
    [SerializeField] public float MaxSpeed = 7f;
    [SerializeField] public float HorizontalAcceleration = 25f;
    [SerializeField] public float JumpVelocity = 9f;
    [SerializeField] public float MaxJumpTime = 0.45f;
    [SerializeField] public float Gravity = 30f;
    [SerializeField] public float Scale = 0.8f;
    [SerializeField] private float colliderWidth = 0.9f;
    [SerializeField] private float colliderHeight = 2f;
    [SerializeField] public int RaycastsPrecision = 15;
    public float ColliderWidth => colliderWidth * Scale;
    public float ColliderHeight => colliderHeight * Scale;
    public bool IsGrounded { get; set; }
    public bool IsMoving { get; set; }
    public bool IsFacingLeft { get; set; }


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Scale, Scale, Scale);
    }
}
