using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private enum HorizontalDirection
    {
        Left,
        Center,
        Right
    }

    private enum RaycastDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private float horizontalSpeed;
    private float verticalSpeed;
    CharacterController controller;
    CharacterInfo info;

    private float maxFallingSpeed = 15f;
    private float minJumpTime = 0.15f;
    private float jumpQueueTime = 0.2f;
    private float jumpQueueTimer = 0;
    public float ungroundedTime;
    private bool isJumping;
    public int jumpCharges = 0;
    private bool hitCeiling;
    HorizontalDirection direction;
    HorizontalDirection facingDirection = HorizontalDirection.Right;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        info = GetComponent<CharacterInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        QualitySettings.vSyncCount = 0;

        var maxJumpCharges = info.MaxJumpCharges;
        float verticalMovement = 0;
        float horizontalMovement = 0;
        var isGrounded = ClosestWallInDirection(RaycastDirection.Down, info.ColliderHeight / 2 * 1.01f) != -1;
        var maxSpeed = info.MaxSpeed;
        var maxJumpTime = info.MaxJumpTime;
        var jumpVelocity = info.JumpVelocity;
        var gravity = info.Gravity;

        #region Horizontal Movement
        HorizontalDirection newDirection = HorizontalDirection.Center;
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            newDirection = HorizontalDirection.Center;
        else if (Input.GetKey(KeyCode.D))
            newDirection = HorizontalDirection.Right;
        else if (Input.GetKey(KeyCode.A))
            newDirection = HorizontalDirection.Left;

        if (newDirection == HorizontalDirection.Center || newDirection != direction)
            horizontalSpeed = 0;
        if (newDirection == HorizontalDirection.Left)
        {
            facingDirection = HorizontalDirection.Left;
            horizontalSpeed -= info.HorizontalAcceleration * Time.deltaTime;
        }
        if (newDirection == HorizontalDirection.Right)
        {
            facingDirection = HorizontalDirection.Right;
            horizontalSpeed += info.HorizontalAcceleration * Time.deltaTime;
        }

        direction = newDirection;

        if (horizontalSpeed > info.MaxSpeed)
            horizontalSpeed = info.MaxSpeed;
        if (horizontalSpeed < -info.MaxSpeed)
            horizontalSpeed = -info.MaxSpeed;
        horizontalMovement = horizontalSpeed;
        #endregion

        #region Vertical Movement
        if (isGrounded)
        {
            ungroundedTime = 0;
            isJumping = false;
            hitCeiling = false;
            verticalSpeed = 0;
            jumpCharges = maxJumpCharges;
        }
        else
        {
            ungroundedTime += Time.deltaTime;
            if (!isJumping)
            {
                verticalSpeed -= gravity * Time.deltaTime;
                if (verticalSpeed < - maxFallingSpeed)
                    verticalSpeed = - maxFallingSpeed;
                verticalMovement = verticalSpeed;
            }
        }

        if (jumpCharges == maxJumpCharges && ungroundedTime > 0.1f)
            jumpCharges--;
        if (Input.GetKeyDown(KeyCode.Space))
            jumpQueueTimer = jumpQueueTime;
        if (jumpQueueTimer > 0 && jumpCharges > 0)
        {
            jumpQueueTimer = 0;
            ungroundedTime = 0;
            isJumping = true;
            jumpCharges--;
        }
        if ((!Input.GetKey(KeyCode.Space) || ungroundedTime > maxJumpTime) && ungroundedTime > minJumpTime)
            isJumping = false;
        if (isJumping)
        {
            verticalSpeed = 0;
            verticalMovement = jumpVelocity;
            var breakingPoint = maxJumpTime * 0.6f;
            var secondBreakingPoint = maxJumpTime * 0.9f;
            var maxSpeedReduction = 0.2f;
            var smth = (ungroundedTime - breakingPoint) / (secondBreakingPoint - breakingPoint);
            if (ungroundedTime > breakingPoint)
                verticalMovement *= 1 - (smth * smth) * (1 - maxSpeedReduction);
            if (ungroundedTime > secondBreakingPoint)
                verticalMovement *= maxSpeedReduction;
        }

        if (jumpQueueTimer > 0)
            jumpQueueTimer -= Time.deltaTime;
        #endregion

        #region Wall collision avoidance
        var verticalDirection = verticalMovement < 0 ? -1 : 1;
        var raycastDirection = verticalMovement < 0 ? RaycastDirection.Down : RaycastDirection.Up;
        var wallBelow = ClosestWallInDirection(raycastDirection, info.ColliderHeight / 2 + verticalDirection * verticalMovement * Time.deltaTime);
        if (wallBelow != -1 && verticalDirection * verticalMovement * Time.deltaTime > wallBelow - info.ColliderHeight / 2)
        {
            isJumping = false;
            verticalMovement = verticalDirection * (wallBelow - info.ColliderHeight / 2) / Time.deltaTime;
        }
        var horizontalDirection = horizontalMovement < 0 ? -1 : 1;
        raycastDirection = horizontalMovement < 0 ? RaycastDirection.Left : RaycastDirection.Right;
        var wallToTheSide = ClosestWallInDirection(raycastDirection, info.ColliderWidth / 2 + horizontalDirection * horizontalMovement * Time.deltaTime);
        if (wallToTheSide != -1 && horizontalDirection * horizontalMovement * Time.deltaTime > wallToTheSide - info.ColliderWidth / 2)
        {
            horizontalMovement = -(wallToTheSide - info.ColliderWidth / 2) / Time.deltaTime;
            horizontalSpeed = 0;
        }
        #endregion

        //Assuring Z value doesn't change
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.01f);

        transform.position += new Vector3(horizontalMovement, verticalMovement, 0) * Time.deltaTime;

        info.IsGrounded = isGrounded;
        info.IsMoving = horizontalMovement != 0;
        info.IsFacingLeft = facingDirection == HorizontalDirection.Left;
    }

    private float ClosestWallInDirection(RaycastDirection direction, float maxDistance)
    {
        float result = -1;

        Vector3 directionVector = default;
        switch(direction)
        {
            case RaycastDirection.Down:
                directionVector = Vector3.down;
                break;
            case RaycastDirection.Up:
                directionVector = Vector3.up;
                break;
            case RaycastDirection.Left:
                directionVector = Vector3.left;
                break;
            case RaycastDirection.Right:
                directionVector = Vector3.right;
                break;
        }

        var colliderHeight = info.ColliderHeight * 0.99f;
        var colliderWidth = info.ColliderWidth * 0.99f;
        bool indexZero = true;
        RaycastHit hit;
        for (int i = 0; i < info.RaycastsPrecision; i++)
        {
            var rayDistance = maxDistance;
            Vector3 startShift = default;
            if (direction == RaycastDirection.Left || direction == RaycastDirection.Right)
            {
                var yShift = colliderHeight / 2 - i * colliderHeight / (info.RaycastsPrecision - 1);
                startShift = new Vector3(0, yShift, 0);
            }
            if (direction == RaycastDirection.Up || direction == RaycastDirection.Down)
            {
                var xShift = colliderWidth / 2 - i * colliderWidth / (info.RaycastsPrecision - 1);
                startShift = new Vector3(xShift, 0, 0);
            }
            if (Physics.Raycast(transform.position + startShift,
                directionVector, out hit, rayDistance))
            {
                if (indexZero || hit.distance < result)
                {
                    result = hit.distance;
                    indexZero = false;
                }
            }
        }
        return result;
    }
}
