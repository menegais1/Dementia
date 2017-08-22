using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Physics
{


    public static Vector2 movementByForce(float force, float constant,
        float maxVelocity, float direction, Rigidbody2D rigidBody)
    {
        Vector2 forceApplied = new Vector2((force * constant), 0) * direction;

        if (rigidBody.velocity.x > (maxVelocity * constant) || rigidBody.velocity.x < -(maxVelocity * constant))
        {
            rigidBody.AddRelativeForce(forceApplied * -1);
        }

        rigidBody.AddRelativeForce(forceApplied);

        return forceApplied;

    }

    public static Vector2 addContraryForce(Vector2 forceApplied, Rigidbody2D rigidBody, int movementState)
    {

        if (movementState == (int)Movement.MovementStateENUM.IDLE && rigidBody.velocity.x != 0f)
        {
            if (Mathf.Abs(rigidBody.velocity.x) < 0.1f)
            {
                rigidBody.velocity = Vector2.zero;
            }

            rigidBody.AddRelativeForce(forceApplied.magnitude * -rigidBody.velocity.normalized);
        }

        return forceApplied;

    }

    public static Vector2 addImpulseForce(float force, Rigidbody2D rigidBody, bool facingRight)
    {

        Vector2 forceApplied = new Vector2(force, 0);

        if (facingRight)
        {
            rigidBody.AddForce(forceApplied, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody.AddForce(-forceApplied, ForceMode2D.Impulse);
        }

        return forceApplied;
    }


    public static void jump(float force, Rigidbody2D rigidBody)
    {
        rigidBody.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }

}
