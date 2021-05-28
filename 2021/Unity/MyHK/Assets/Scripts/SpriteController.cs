using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite Idle;
    [SerializeField] Sprite[] MovingAnimation;
    [SerializeField] Sprite Jump;

    int movingIndex;
    [SerializeField] float movingLoopLength = 0.5f;
    float animationTimer;

    CharacterInfo info;

    // Start is called before the first frame update
    void Start()
    {
        info = GetComponent<CharacterInfo>();
        animationTimer = movingLoopLength / MovingAnimation.Length;
    }

    // Update is called once per frame
    void Update()
    {
        Sprite sprite = default;
        if (!info.IsGrounded)
            sprite = Jump;
        else if (info.IsMoving)
        {
            sprite = MovingAnimation[movingIndex];
        }
        else
            sprite = Idle;

        animationTimer -= Time.deltaTime;
        if (animationTimer < 0)
        {
            animationTimer += movingLoopLength / MovingAnimation.Length;
            movingIndex = (movingIndex + 1) % MovingAnimation.Length;
        }

        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = info.IsFacingLeft;
    }
}
