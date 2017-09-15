using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class PhysicsHelpers
{
    public static Vector2 movementByForce(float force, float constant,
        float maxVelocity, float direction, Rigidbody2D rigidBody, bool vertical)
    {
        Vector2 forceApplied = new Vector2((force * constant), 0) * direction;

        if (vertical)
        {
            forceApplied = new Vector2(0, (force * constant)) * direction;
        }


        if (rigidBody.velocity.x > (maxVelocity * constant) || rigidBody.velocity.x < -(maxVelocity * constant))
        {
            rigidBody.AddRelativeForce(forceApplied * -1);
        }

        rigidBody.AddRelativeForce(forceApplied);

        return forceApplied;
    }

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
        if (Mathf.Abs(rigidBody.velocity.x) > velocity)
        {
            rigidBody.AddRelativeForce(-forceApplied);
        }

        rigidBody.AddRelativeForce(forceApplied);

        return forceApplied;
    }

    public static Vector2 PreventSlide(Vector2 forceApplied, Rigidbody2D rigidBody)
    {
        if (!MathHelpers.Approximately(rigidBody.velocity.x, 0, 0) &&
            MathHelpers.Approximately(rigidBody.velocity.y, 0, 0))
        {
            if (MathHelpers.Approximately(rigidBody.velocity.x, 0, 0.1f))
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }

            rigidBody.AddRelativeForce(forceApplied.magnitude * -rigidBody.velocity.normalized);
        }

        return forceApplied;
    }

    public static Vector2 AddImpulseForce(float force, Rigidbody2D rigidBody, FacingDirection facingDirection)
    {
        var forceApplied = new Vector2(force, 0);

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

    public static Vector2 addImpulseForce(float force, Rigidbody2D rigidBody)
    {
        Vector2 forceApplied = new Vector2(force, 0);


        rigidBody.AddForce(forceApplied, ForceMode2D.Impulse);


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

    public static bool SlopeInclinationRight(Vector2 surfaceNormal)
    {
        return surfaceNormal.x < 0;
    }
}