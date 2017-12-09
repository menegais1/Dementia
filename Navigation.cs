using System.Collections.Generic;
using UnityEngine;

public struct NavigationInfo
{
    public Transform transform;
    public BoxCollider2D boxCollider2D;
    public Vector2 topMostColliderPosition;
    public NavigationNodeType type;

    public NavigationInfo(Transform transform, BoxCollider2D boxCollider2D, NavigationNodeType type)
    {
        this.transform = transform;
        this.boxCollider2D = boxCollider2D;
        this.type = type;

        var center = boxCollider2D.offset;
        this.topMostColliderPosition =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x,
                center.y + boxCollider2D.size.y / 2));
    }
}

public class Navigation : MonoBehaviour
{
    private List<NavigationInfo> verticalNavigationInfoList;
    private MonoBehaviour monoBehaviour;
    private GameObject gameDataHolder;


    public List<NavigationInfo> VerticalNavigationInfoList
    {
        get { return verticalNavigationInfoList; }
        set { verticalNavigationInfoList = value; }
    }

    public Navigation()
    {
        gameDataHolder = GameObject.FindGameObjectWithTag("Game Data Holder");
        if (gameDataHolder == null) return;
        VerticalNavigationInfoList = new List<NavigationInfo>();

        var obstacles = gameDataHolder.GetComponentsInChildren<ObstacleController>();
        var stairsList = gameDataHolder.GetComponentsInChildren<StairsController>();
        var ladders = gameDataHolder.GetComponentsInChildren<LadderController>();

        foreach (var obstacle in obstacles)
        {
            var obstacleTransform = obstacle.transform.parent.transform;
            var obstacleCollider = obstacle.Collider2D;
            if (!obstacleTransform.CompareTag("Obstacle")) continue;

            if (!VerticalNavigationInfoList.Exists(
                lambdaExpression => lambdaExpression.transform == obstacleTransform))
            {
                VerticalNavigationInfoList.Add(new NavigationInfo(obstacleTransform, obstacleCollider,
                    NavigationNodeType.Obstacle));
            }
        }

        foreach (var stairs in stairsList)
        {
            var stairsTransform = stairs.transform.parent.transform;
            var stairsCollider = stairs.transform.GetComponentInChildren<BoxCollider2D>();
            if (!stairsTransform.CompareTag("Stairs")) continue;

            if (!VerticalNavigationInfoList.Exists(lambdaExpression => lambdaExpression.transform == stairsTransform))
            {
                VerticalNavigationInfoList.Add(new NavigationInfo(stairsTransform, stairsCollider,
                    NavigationNodeType.Stairs));
            }
        }

        foreach (var ladder in ladders)
        {
            if (ladder.ladder != LadderType.Ladder || !ladder.CompareTag("Ladder")) continue;

            var ladderTransform = ladder.transform;
            var ladderCollider = ladder.ladderCollider;

            if (!VerticalNavigationInfoList.Exists(lambdaExpression => lambdaExpression.transform == ladderTransform))
            {
                VerticalNavigationInfoList.Add(new NavigationInfo(ladderTransform, ladderCollider,
                    NavigationNodeType.Ladder));
            }
        }
    }
}