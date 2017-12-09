using System.Collections.Generic;
using UnityEngine;

public struct NavigationInfo
{
    public Transform transform;
    public BoxCollider2D boxCollider2D;
    public ColliderBounds colliderBounds;
    public NavigationNodeType type;

    public NavigationInfo(Transform transform, BoxCollider2D boxCollider2D, NavigationNodeType type)
    {
        this.transform = transform;
        this.boxCollider2D = boxCollider2D;
        this.type = type;
        colliderBounds = new ColliderBounds();

        UpdateColliderBounds(boxCollider2D);
    }

    private void UpdateColliderBounds(BoxCollider2D boxCollider2D)
    {
        var center = boxCollider2D.offset;

        colliderBounds.midLeft =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x - boxCollider2D.size.x / 2,
                center.y));
        colliderBounds.midRight =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x + boxCollider2D.size.x / 2,
                center.y));

        colliderBounds.bottomMid = boxCollider2D.transform.TransformPoint(new Vector2(
            center.x,
            center.y - boxCollider2D.size.y / 2));
        colliderBounds.bottomLeft = boxCollider2D.transform.TransformPoint(new Vector2(
            center.x - boxCollider2D.size.x / 2,
            center.y - boxCollider2D.size.y / 2));
        colliderBounds.bottomRight = boxCollider2D.transform.TransformPoint(new Vector2(
            center.x + boxCollider2D.size.x / 2,
            center.y - boxCollider2D.size.y / 2));

        colliderBounds.topLeft =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x - boxCollider2D.size.x / 2,
                center.y + boxCollider2D.size.y / 2));
        colliderBounds.topRight =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x + boxCollider2D.size.x / 2,
                center.y + boxCollider2D.size.y / 2));
        colliderBounds.topMid =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x,
                center.y + boxCollider2D.size.y / 2));
    }
}

public class Navigation : MonoBehaviour
{
    private MonoBehaviour monoBehaviour;
    private GameObject gameDataHolder;


    public List<NavigationInfo> WorldNavigationInfo { get; set; }

    public void Awake()
    {
        gameDataHolder = GameObject.FindGameObjectWithTag("Game Data Holder");
        if (gameDataHolder == null) return;
        WorldNavigationInfo = new List<NavigationInfo>();
        var obstacles = gameDataHolder.GetComponentsInChildren<ObstacleController>();
        var stairsList = gameDataHolder.GetComponentsInChildren<StairsController>();
        var ladders = gameDataHolder.GetComponentsInChildren<LadderController>();

        foreach (var obstacle in obstacles)
        {
            var obstacleTransform = obstacle.transform.parent.transform;
            var obstacleCollider = obstacle.Collider2D;
            if (!obstacleTransform.CompareTag("Obstacle")) continue;

            if (!WorldNavigationInfo.Exists(
                lambdaExpression => lambdaExpression.transform == obstacleTransform))
            {
                WorldNavigationInfo.Add(new NavigationInfo(obstacleTransform, obstacleCollider,
                    NavigationNodeType.Obstacle));
            }
        }

        foreach (var stairs in stairsList)
        {
            var stairsTransform = stairs.transform.parent.transform;
            var stairsCollider = stairs.transform.GetComponentInChildren<BoxCollider2D>();
            if (!stairsTransform.CompareTag("Stairs")) continue;

            if (!WorldNavigationInfo.Exists(lambdaExpression => lambdaExpression.transform == stairsTransform))
            {
                WorldNavigationInfo.Add(new NavigationInfo(stairsTransform, stairsCollider,
                    NavigationNodeType.Stairs));
            }
        }

        foreach (var ladder in ladders)
        {
            if (ladder.ladder != LadderType.Ladder || !ladder.CompareTag("Ladder")) continue;

            var ladderTransform = ladder.transform;
            var ladderCollider = ladder.ladderCollider;

            if (!WorldNavigationInfo.Exists(lambdaExpression => lambdaExpression.transform == ladderTransform))
            {
                WorldNavigationInfo.Add(new NavigationInfo(ladderTransform, ladderCollider,
                    NavigationNodeType.Ladder));
            }
        }
        
    }
}