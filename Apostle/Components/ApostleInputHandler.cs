﻿using System.Collections.Generic;
using UnityEngine;


public class ApostleInputHandler : MonoBehaviour
{
    [SerializeField] private float aggroTime;
    [SerializeField] private Transform startPointTransform;
    [SerializeField] private Transform endPointTransform;

    private float currentAggroTime;

    private BoxCollider2D triggerArea;
    private Transform currentAim;
    private List<NavigationInfo> worldNavigationInfo;

    private BasicCollisionHandler apostleCollisionHandler;

    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;

    private float movementDirectionValue;
    private bool climbObstacleValue;

    public bool ClimbObstacleValue
    {
        get { return climbObstacleValue; }
    }

    public float MovementDirectionValue
    {
        get { return movementDirectionValue; }
    }

    private void Start()
    {
        var apostleManager = GetComponent<ApostleManager>();
        this.apostleCollisionHandler = apostleManager.ApostleCollisionHandler;
        this.apostleStatusVariables = apostleManager.ApostleStatusVariables;
        currentAim = endPointTransform;
        CreateTriggerArea();
    }

    private void Update()
    {
        SetTriggerAreaDirection();
        if (Time.time >= currentAggroTime && apostleStatusVariables.isAggroed && !apostleStatusVariables.inAggroRange)
        {
            currentAim = startPointTransform;
            apostleStatusVariables.isAggroed = false;
        }

        apostleStatusVariables.isPatrolling = !apostleStatusVariables.isAggroed;

        if (apostleStatusVariables.isPatrolling)
        {
            if (MathHelpers.Approximately(transform.position.x, startPointTransform.position.x, 0.3f))
            {
                currentAim = endPointTransform;
            }
            else if (MathHelpers.Approximately(transform.position.x, endPointTransform.position.x, 0.3f))
            {
                currentAim = startPointTransform;
            }
        }

        if (!currentAim.Equals(null))
            movementDirectionValue =
                MovementDirection(currentAim.position);

        //Debug.Log(!currentAimNode.Equals(null) ? currentAimNode.transform.name : "");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPositionIsOnSight(playerManager.transform.position))
        {
            currentAim = playerManager.transform;

            apostleStatusVariables.isAggroed = true;
            apostleStatusVariables.inAggroRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;


        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPositionIsOnSight(playerManager.transform.position))
        {
            currentAim = playerManager.transform;

            apostleStatusVariables.isAggroed = true;
            apostleStatusVariables.inAggroRange = true;
        }
        else if (apostleStatusVariables.inAggroRange)
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!currentAim.Equals(null))
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }

    private bool CheckIfPositionIsOnSight(Vector3 position)
    {
        var startingPoint = position.x > transform.position.x
            ? apostleCollisionHandler.BoxColliderBounds.topRight
            : apostleCollisionHandler.BoxColliderBounds.topLeft;
        var xDirection = position.x - startingPoint.x;
        var yDirection = position.y - startingPoint.y;
        var ray = Physics2D.Raycast(startingPoint, new Vector2(xDirection, yDirection), triggerArea.size.x,
            LayerMask.GetMask("Ground", "Player"));
        return ray.collider != null && ray.collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    private float MovementDirection(Vector3 position)
    {
        return position.x > transform.position.x ? 1 : -1;
    }

    private void CreateTriggerArea()
    {
        this.triggerArea = GetComponent<BoxCollider2D>();
        var mainCamera = Camera.main;
        var height = mainCamera.orthographicSize * 2;
        var width = mainCamera.aspect * height;
        triggerArea.size = new Vector2((width / 2) + (width / 12), height / 2);
        triggerArea.offset = new Vector2(width / 5, 0);
    }

    private void SetTriggerAreaDirection()
    {
        if (apostleStatusVariables.facingDirection == FacingDirection.Right && triggerArea.offset.x < 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
        }
        else if (apostleStatusVariables.facingDirection == FacingDirection.Left && triggerArea.offset.x > 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
        }
    }
}