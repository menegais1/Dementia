using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class PhysicsHelpers
{
    public static void ClimbLadder(float climbLadderVelocity, float climbLadderMovement, Rigidbody2D rigidbody2D)
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, climbLadderVelocity * climbLadderMovement);
    }

    public static Vector2 HorizontalMovementByForce(float acceleration, float constant,
        float maxSpeed, float direction, Rigidbody2D rigidBody, float surfaceAngle, Vector2 surfaceNormal)
    {
        var velocity = (maxSpeed / 3.6f) * constant;
        var force = ForceCalcByAcceleration(acceleration, rigidBody.mass) +
                    FrictionForceCalc(0.4f, Physics2D.gravity.y, rigidBody.mass) +
                    SlopeForceCalc(surfaceAngle, surfaceNormal, direction, rigidBody.mass, Physics2D.gravity.y);


        var forceApplied = new Vector2(force * constant, 0) * direction;

        if (!MathHelpers.Approximately(surfaceAngle, 0, float.Epsilon))
        {
            forceApplied = new Vector2(
                               force * MathHelpers.AbsCos(surfaceAngle),
                               force * (!SlopeInclinationRight(surfaceNormal)
                                   ? -MathHelpers.AbsSin(surfaceAngle)
                                   : MathHelpers.AbsSin(surfaceAngle))) * direction * constant;
        }

        if (rigidBody.velocity.magnitude > velocity)
        {
            rigidBody.AddRelativeForce(-forceApplied);
        }

        rigidBody.AddRelativeForce(forceApplied);

        return forceApplied;
    }

    public static Vector2 PreventSlide(Vector2 forceApplied, float slopeAngle, Rigidbody2D rigidBody)
    {
        if (!MathHelpers.Approximately(rigidBody.velocity.x, 0, float.Epsilon))
        {
            if (!MathHelpers.Approximately(slopeAngle, 0, float.Epsilon))
            {
                if (MathHelpers.Approximately(rigidBody.velocity.magnitude, 0, 0.2f))
                {
                    rigidBody.velocity = Vector2.zero;
                }
            }
            else if (MathHelpers.Approximately(rigidBody.velocity.magnitude, 0, 0.1f))
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }

            rigidBody.AddRelativeForce(forceApplied.magnitude * -rigidBody.velocity.normalized);
        }

        return forceApplied;
    }

    public static void PreventSlideOnSlopes(float slopeAngle, Vector2 surfaceNormal, bool isIdle,
        Rigidbody2D rigidbody2D)
    {
        var forceMagnitude = SlopeForceCalc(slopeAngle, rigidbody2D.mass, Physics2D.gravity.y);
        var force = new Vector2(
            SlopeInclinationRight(surfaceNormal)
                ? forceMagnitude * MathHelpers.AbsCos(slopeAngle)
                : -forceMagnitude * MathHelpers.AbsCos(slopeAngle),
            forceMagnitude * MathHelpers.AbsSin(slopeAngle));

        if (MathHelpers.Approximately(rigidbody2D.velocity.magnitude, 0, 0.1f) && isIdle)
        {
            rigidbody2D.velocity = Vector2.zero;
        }

        if (rigidbody2D.velocity.y > 0) return;
        rigidbody2D.AddRelativeForce(force);
    }


    public static Vector2 AddImpulseForce(float force, float surfaceAngle, Vector2 surfaceNormal, Rigidbody2D rigidBody,
        FacingDirection facingDirection)
    {
        force += SlopeForceCalc(surfaceAngle, surfaceNormal, facingDirection == FacingDirection.Right ? 1 : -1,
                     rigidBody.mass, Physics2D.gravity.y) * 0.385f;

        var forceApplied = new Vector2(force * MathHelpers.AbsCos(surfaceAngle),
            force * (!SlopeInclinationRight(surfaceNormal)
                ? -MathHelpers.AbsSin(surfaceAngle)
                : MathHelpers.AbsSin(surfaceAngle)));

        if (facingDirection == FacingDirection.Right)
        {
            rigidBody.AddForce(forceApplied, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody.AddForce(-forceApplied, ForceMode2D.Impulse);
        }
        return forceApplied;
    }

    public static Vector2 AddImpulseForce(float force, bool toRightDirection, Rigidbody2D rigidBody)
    {
        var forceApplied = new Vector2(force, 0);

        rigidBody.AddForce(toRightDirection ? forceApplied : -forceApplied, ForceMode2D.Impulse);

        return forceApplied;
    }

    public static Vector2 AddImpulseForce(float force, Rigidbody2D rigidBody)
    {
        var forceApplied = new Vector2(force, 0);

        rigidBody.AddRelativeForce(forceApplied, ForceMode2D.Impulse);

        return forceApplied;
    }

    public static void Jump(float force, Rigidbody2D rigidBody)
    {
        rigidBody.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }

    public static float FrictionForceCalc(float frictionCoefficient, float gravity, float mass)
    {
        return frictionCoefficient * (-gravity) * mass;
    }

    public static float ForceCalcByAcceleration(float acceleration, float mass)
    {
        return acceleration * mass;
    }

    public static float SlopeForceCalc(float angle, Vector2 surfaceNormal, float direction, float mass, float gravity)
    {
        if (direction > 0 && SlopeInclinationRight(surfaceNormal) ||
            direction < 0 && !SlopeInclinationRight(surfaceNormal))
        {
            return Mathf.Abs(MathHelpers.Sin(angle) * mass * gravity);
        }
        return 0;
    }

    public static float SlopeForceCalc(float angle, float mass, float gravity)
    {
        return Mathf.Abs(MathHelpers.Sin(angle) * mass * gravity);
    }

    public static bool SlopeInclinationRight(Vector2 surfaceNormal)
    {
        return surfaceNormal.x < 0;
    }

    public static void ResetVelocityX(Rigidbody2D rigidbody2D)
    {
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
    }

    public static void ResetVelocityY(Rigidbody2D rigidbody2D)
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
    }


    public static void IgnoreCollision(Collider2D collider2D, Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(collider2D, other, ignore);
    }

    public static void IgnoreLayerCollision(LayerMask layerMask, LayerMask layerMaskToIgnore, bool ignore)
    {
        Physics2D.IgnoreLayerCollision(layerMask.value, layerMaskToIgnore.value, ignore);
    }


    public static void SwitchGravity(Rigidbody2D rigidbody2D, bool on, float gravityScaleToChange)
    {
        rigidbody2D.gravityScale = on ? gravityScaleToChange : 0;
    }
}