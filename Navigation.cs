using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct FloorInfo
{
    public Vector2 startPointX, endPointX;
    public Vector2 startPointY, endPointY;
}

public struct Floor
{
    public int number;
    public Transform transform;
    public List<Floor> consecutiveFloors;
    public List<TransitionFloor> transitionFloors;
    public FloorInfo FloorInfo;
    public Collider2D collider2D;

    public Floor(Transform transform, Collider2D collider2D, int number)
    {
        this.number = number;
        this.transform = transform;
        FloorInfo = new FloorInfo();
        transitionFloors = new List<TransitionFloor>();
        consecutiveFloors = new List<Floor>();
        this.collider2D = collider2D;
        UpdateFloorInfo(collider2D);
    }

    private void UpdateFloorInfo(Collider2D collider2D)
    {
        var center = collider2D.transform.TransformPoint(collider2D.offset);

        FloorInfo.startPointX =
            collider2D.bounds.min;

        FloorInfo.startPointY =
            new Vector2(center.x,
                collider2D.bounds.min.y);

        if (transform.rotation.z > 0)
        {
            FloorInfo.endPointY = new Vector2(center.x, center.y + 10);
            FloorInfo.endPointX = new Vector2(collider2D.bounds.max.x, collider2D.bounds.max.y);
        }
        else if (transform.rotation.z < 0)
        {
            FloorInfo.startPointX = new Vector3(collider2D.bounds.min.x, collider2D.bounds.max.y);
            FloorInfo.endPointY = new Vector2(center.x, center.y + 10);
            FloorInfo.endPointX = new Vector2(collider2D.bounds.max.x, collider2D.bounds.min.y);
        }
        else
        {
            FloorInfo.endPointX =
                new Vector2(collider2D.bounds.max.x, collider2D.bounds.min.y);
            var raycastHits = Physics2D.RaycastAll(FloorInfo.startPointY, Vector2.up, 100f,
                LayerMask.GetMask("Ground", "Ignore Ground"));
            foreach (var raycastHit2D in raycastHits)
            {
                if (raycastHit2D.collider != collider2D)
                {
                    FloorInfo.endPointY = raycastHit2D.point;
                    break;
                }
                FloorInfo.endPointY = new Vector2(FloorInfo.startPointY.x, float.MaxValue);
            }

            if (raycastHits.Length == 0)
            {
                FloorInfo.endPointY = new Vector2(FloorInfo.startPointY.x, float.MaxValue);
            }
        }
    }
}

public struct TransitionFloor
{
    public List<Floor> floors;
    public TransitionFloorType type;
    public Transform transform;
    public ColliderBounds colliderBounds;

    public TransitionFloor(Floor firstFloor, Floor secondFloor, Transform transform, ColliderBounds colliderBounds,
        TransitionFloorType type)
    {
        floors = new List<Floor>()
        {
            firstFloor,
            secondFloor
        };

        this.transform = transform;
        this.type = type;
        this.colliderBounds = colliderBounds;
    }
}

public struct NavigationInfo
{
    public Transform transform;
    public BoxCollider2D boxCollider2D;
    public ColliderBounds colliderBounds;
    public TransitionFloorType type;

    public NavigationInfo(Transform transform, BoxCollider2D boxCollider2D, TransitionFloorType type)
    {
        this.transform = transform;
        this.boxCollider2D = boxCollider2D;
        this.type = type;
        colliderBounds = new ColliderBounds();

        UpdateColliderBounds(boxCollider2D);
    }

    private void UpdateColliderBounds(BoxCollider2D boxCollider2D)
    {
        if (boxCollider2D == null) return;
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

        center = boxCollider2D.transform.TransformPoint(boxCollider2D.offset);
        colliderBounds.boundingBoxBottomY =
            new Vector2(center.x,
                boxCollider2D.bounds.min.y);

        if (!MathHelpers.Approximately(boxCollider2D.transform.rotation.z, 0, float.Epsilon))
        {
            colliderBounds.boundingBoxTopY =
                new Vector2(center.x, center.y + boxCollider2D.bounds.size.y / 2 + 1f);
        }
        else
        {
            colliderBounds.boundingBoxTopY =
                new Vector2(center.x, center.y + boxCollider2D.bounds.size.y / 2);
        }
    }
}

public class Navigation : MonoBehaviour
{
    private MonoBehaviour monoBehaviour;
    private GameObject gameDataHolder;

    public List<Floor> floorList;
    public List<NavigationInfo> WorldNavigationInfo { get; set; }
    public List<TransitionFloor> transitionFloorList { get; set; }

    private ContactFilter2D contactFilter2D;

    public void Awake()
    {
        gameDataHolder = GameObject.FindGameObjectWithTag("Game Data Holder");
        if (gameDataHolder == null) return;
        WorldNavigationInfo = new List<NavigationInfo>();
        this.floorList = new List<Floor>();
        this.transitionFloorList = new List<TransitionFloor>();
        var obstacles = gameDataHolder.GetComponentsInChildren<ObstacleController>();
        var stairsList = gameDataHolder.GetComponentsInChildren<StairsController>();
        var ladders = gameDataHolder.GetComponentsInChildren<LadderController>();
        this.contactFilter2D = new ContactFilter2D()
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Player", "Enemy"),
        };
        foreach (var obstacle in obstacles)
        {
            var obstacleTransform = obstacle.transform.parent.transform;
            var obstacleCollider = obstacle.Collider2D;
            if (!obstacleTransform.CompareTag("Obstacle")) continue;

            if (!WorldNavigationInfo.Exists(
                lambdaExpression => lambdaExpression.transform == obstacleTransform))
            {
                WorldNavigationInfo.Add(new NavigationInfo(obstacleTransform, obstacleCollider,
                    TransitionFloorType.Obstacle));
            }
        }

        foreach (var stairs in stairsList)
        {
            var stairsTransform = stairs.transform.parent.transform;
            var stairsColliders = stairsTransform.GetComponentsInChildren<BoxCollider2D>();

            if (!stairsTransform.CompareTag("Stairs")) continue;

            BoxCollider2D stairsCollider = new BoxCollider2D();
            foreach (var stairsColliderIterator in stairsColliders)
            {
                if (stairsColliderIterator.transform.gameObject.layer !=
                    LayerMask.NameToLayer("Stairs Ground")) continue;
                stairsCollider = stairsColliderIterator;
                break;
            }

            if (!WorldNavigationInfo.Exists(lambdaExpression => lambdaExpression.transform == stairsTransform))
            {
                WorldNavigationInfo.Add(new NavigationInfo(stairsTransform, stairsCollider,
                    TransitionFloorType.Stairs));
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
                    TransitionFloorType.Ladder));
            }
        }

        List<GameObject> floorList = new List<GameObject>();
        floorList.AddRange(GameObject.FindGameObjectsWithTag("Scenery"));
        floorList.AddRange(GameObject.FindGameObjectsWithTag("Obstacle"));

        floorList =
            floorList.Where(floor =>
                    floor.layer == LayerMask.NameToLayer("Ground") ||
                    floor.layer == LayerMask.NameToLayer("Ground Ignore"))
                .ToList();

        floorList = floorList.OrderBy(lambdaExpression => lambdaExpression.transform.position.y)
            .ToList();
        var count = 1;
        foreach (var floor in floorList)
        {
            var boxColliders2D = floor.transform.GetComponents<BoxCollider2D>();
            if (boxColliders2D.Length > 0)
            {
                foreach (var collider2D in boxColliders2D)
                {
                    this.floorList.Add(new Floor(floor.transform, collider2D, count));
                    count++;
                }
            }
            else
            {
                var edgeColliders2D = floor.transform.GetComponents<EdgeCollider2D>();
                foreach (var collider2D in edgeColliders2D)
                {
                    this.floorList.Add(new Floor(floor.transform, collider2D, count));
                    count++;
                }
            }
        }


        foreach (var navigationInfo in WorldNavigationInfo)
        {
            Floor floorNumber1, floorNumber2;
            switch (navigationInfo.type)
            {
                case TransitionFloorType.Ladder:

                    floorNumber1 = this.floorList.Find(lambdaExpression =>
                        lambdaExpression.FloorInfo.startPointX.x <= navigationInfo.colliderBounds.topMid.x &&
                        lambdaExpression.FloorInfo.endPointX.x >= navigationInfo.colliderBounds.topMid.x &&
                        lambdaExpression.FloorInfo.startPointY.y <= navigationInfo.colliderBounds.topMid.y &&
                        lambdaExpression.FloorInfo.endPointY.y >= navigationInfo.colliderBounds.topMid.y);
                    floorNumber2 = this.floorList.Find(lambdaExpression =>
                        lambdaExpression.FloorInfo.startPointX.x <= navigationInfo.colliderBounds.bottomMid.x &&
                        lambdaExpression.FloorInfo.endPointX.x >= navigationInfo.colliderBounds.bottomMid.x &&
                        lambdaExpression.FloorInfo.startPointY.y <= navigationInfo.colliderBounds.bottomMid.y &&
                        lambdaExpression.FloorInfo.endPointY.y >= navigationInfo.colliderBounds.bottomMid.y);

                    if (floorNumber1.number < floorNumber2.number)
                    {
                        floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Ladder));
                        floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Ladder));
                    }
                    else
                    {
                        floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Ladder));
                        floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Ladder));
                    }
                    break;
                case TransitionFloorType.Obstacle:
                    var obstacleColliders = navigationInfo.transform.GetComponentsInChildren<BoxCollider2D>();
                    foreach (var boxCollider2D in obstacleColliders)
                    {
                        if (boxCollider2D.isTrigger == false)
                        {
                            continue;
                        }

                        var bounds = boxCollider2D.bounds;

                        var floor = this.floorList.Find(lambdaExpression =>
                            lambdaExpression.FloorInfo.startPointX.x <= bounds.min.x &&
                            lambdaExpression.FloorInfo.endPointX.x >= bounds.max.x &&
                            lambdaExpression.FloorInfo.startPointY.y <= bounds.min.y &&
                            lambdaExpression.FloorInfo.endPointY.y >= bounds.max.y);
                        if (floor.number != 0)
                        {
                            floorNumber1 = floor;
                            floorNumber2 = this.floorList.Find(lambdaExpression =>
                                lambdaExpression.transform == navigationInfo.transform);

                            if (floorNumber1.number < floorNumber2.number)
                            {
                                floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                                    boxCollider2D.transform, navigationInfo.colliderBounds,
                                    TransitionFloorType.Obstacle));
                                floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                                    boxCollider2D.transform, navigationInfo.colliderBounds,
                                    TransitionFloorType.Obstacle));
                            }
                            else
                            {
                                floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                                    boxCollider2D.transform, navigationInfo.colliderBounds,
                                    TransitionFloorType.Obstacle));
                                floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                                    boxCollider2D.transform, navigationInfo.colliderBounds,
                                    TransitionFloorType.Obstacle));
                            }
                        }
                    }
                    break;
                case TransitionFloorType.Stairs:
                    var tempStairsColliders = navigationInfo.transform.GetComponentsInChildren<BoxCollider2D>();
                    var stairsColliders = new List<BoxCollider2D>();
                    foreach (var boxCollider2D in tempStairsColliders)
                    {
                        if (boxCollider2D != navigationInfo.boxCollider2D)
                        {
                            stairsColliders.Add(boxCollider2D);
                        }
                    }
                    var bounds1 = stairsColliders[0].bounds;
                    var bounds2 = stairsColliders[1].bounds;

                    floorNumber1 = this.floorList.Find(lambdaExpression =>
                        lambdaExpression.FloorInfo.startPointX.x <= bounds1.min.x &&
                        lambdaExpression.FloorInfo.endPointX.x >= bounds1.max.x &&
                        lambdaExpression.FloorInfo.startPointY.y <= bounds1.min.y &&
                        lambdaExpression.FloorInfo.endPointY.y >= bounds1.max.y);
                    floorNumber2 = this.floorList.Find(lambdaExpression =>
                        lambdaExpression.FloorInfo.startPointX.x <= bounds2.min.x &&
                        lambdaExpression.FloorInfo.endPointX.x >= bounds2.max.x &&
                        lambdaExpression.FloorInfo.startPointY.y <= bounds2.min.y &&
                        lambdaExpression.FloorInfo.endPointY.y >= bounds2.max.y);

                    if (floorNumber1.number == 0 || floorNumber2.number == 0) return;
                    if (floorNumber1.number < floorNumber2.number)
                    {
                        floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Stairs));
                        floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber1, floorNumber2,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Stairs));
                    }
                    else
                    {
                        floorNumber1.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Stairs));
                        floorNumber2.transitionFloors.Add(new TransitionFloor(floorNumber2, floorNumber1,
                            navigationInfo.transform, navigationInfo.colliderBounds, TransitionFloorType.Stairs));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        foreach (var outerFloor in this.floorList)
        {
            foreach (var outerFloorTransitionFloor in outerFloor.transitionFloors)
            {
                if (!transitionFloorList.Exists(lambdaExpression =>
                    lambdaExpression.transform == outerFloorTransitionFloor.transform))
                    transitionFloorList.Add(outerFloorTransitionFloor);
            }
            foreach (var innerFloor in this.floorList)
            {
                if (!MathHelpers.Approximately(outerFloor.transform.rotation.z, 0, float.Epsilon))
                {
                    if (MathHelpers.Approximately(outerFloor.FloorInfo.endPointX.y,
                            innerFloor.FloorInfo.startPointX.y,
                            2f) && MathHelpers.Approximately(
                            outerFloor.FloorInfo.endPointX.x,
                            innerFloor.FloorInfo.startPointX.x,
                            0.7f) && outerFloor.number != innerFloor.number)
                    {
                        outerFloor.consecutiveFloors.Add(innerFloor);
                        innerFloor.consecutiveFloors.Add(outerFloor);
                        continue;
                    }
                }
                else
                {
                    if (MathHelpers.Approximately(outerFloor.FloorInfo.endPointX.y,
                            innerFloor.FloorInfo.startPointX.y,
                            0.6f) && MathHelpers.Approximately(
                            outerFloor.FloorInfo.endPointX.x,
                            innerFloor.FloorInfo.startPointX.x,
                            0.7f) && outerFloor.number != innerFloor.number)
                    {
                        outerFloor.consecutiveFloors.Add(innerFloor);
                        innerFloor.consecutiveFloors.Add(outerFloor);
                        continue;
                    }
                }

                if (outerFloor.transform.position.x >
                    innerFloor.FloorInfo.startPointX.x
                    && outerFloor.transform.position.x <
                    innerFloor.FloorInfo.endPointX.x
                    && outerFloor.transform.position.y >
                    innerFloor.FloorInfo.startPointY.y
                    && outerFloor.transform.position.y <
                    innerFloor.FloorInfo.endPointY.y && outerFloor.number != innerFloor.number)
                {
                    outerFloor.consecutiveFloors.Add(innerFloor);
                    innerFloor.consecutiveFloors.Add(outerFloor);
                }
            }
        }

//        foreach (var floor in this.floorList)
//        {
//            // Debug.Log(floor.transform.name + " [" + floor.number + "]" + " : " + floor.FloorInfo.endPointY);
//            foreach (var transitionFloor in floor.transitionFloors)
//            {
//                Debug.Log(" [" + floor.number + "]" + " : " +
//                          " floor1 : " + transitionFloor.floors[0].number + "|| floor2 : " +
//                          transitionFloor.floors[1].number);
//            }
//        }

        foreach (var transitionFloor in transitionFloorList)
        {
            if (transitionFloor.type == TransitionFloorType.Stairs)
            {
                var teste = GameObject.CreatePrimitive(PrimitiveType.Cube);
                teste.transform.position = transitionFloor.colliderBounds.boundingBoxBottomY;
                var teste1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                teste1.transform.position = transitionFloor.colliderBounds.boundingBoxTopY;
            }
        }
    }

    public void CheckForCurrentFloor(Transform transform, CapsuleCollider2D collider2D, ref Floor currentFloor,
        ref TransitionFloor currentTransitionFloor)
    {
        //Altura como safe check
        /*  currentTransitionFloor = new TransitionFloor();
          var temporaryFloors = this.floorList.FindAll(lambdaExpression =>
          lambdaExpression.FloorInfo.startPointX.x <= transform.position.x &&
          lambdaExpression.FloorInfo.endPointX.x >= transform.position.x &&
          lambdaExpression.FloorInfo.startPointY.y <= transform.position.y &&
          lambdaExpression.FloorInfo.endPointY.y >= transform.position.y);*/
        currentTransitionFloor = new TransitionFloor();
        var temporaryFloors = this.floorList.FindAll(lambdaExpression =>
            lambdaExpression.FloorInfo.startPointX.x <= transform.position.x &&
            lambdaExpression.FloorInfo.endPointX.x >= transform.position.x &&
            lambdaExpression.FloorInfo.startPointY.y <= transform.position.y &&
            lambdaExpression.FloorInfo.endPointY.y >= transform.position.y);
        Floor currentFloorForTest = new Floor();
        Collider2D[] colliders2D = new Collider2D[5];
        foreach (var temporaryFloor in temporaryFloors)
        {
            temporaryFloor.collider2D.GetContacts(this.contactFilter2D, colliders2D);
            if (colliders2D.Where(contactCollider2D => contactCollider2D != null)
                .Any(contactCollider2D => contactCollider2D == collider2D &&
                                          contactCollider2D.gameObject.layer == transform.gameObject.layer))
            {
                currentFloorForTest = temporaryFloor;
            }
            if (currentFloorForTest.number != 0)
                break;
        }
        if (currentFloorForTest.number == 0) return;
        if (!MathHelpers.Approximately(currentFloorForTest.transform.rotation.z, 0, float.Epsilon))
        {
            var raycastHit2D = Physics2D
                .Raycast(transform.position, Vector2.up, 100f, LayerMask.GetMask("Ground", "Obstacle"));
            if (raycastHit2D.collider != null &&
                raycastHit2D.collider.transform == currentFloorForTest.transform) return;
        }
        currentFloor = currentFloorForTest;
    }

    public void CheckForCurrentTransitionFloor(Transform transform, CapsuleCollider2D collider2D,
        ref Floor currentFloor,
        ref TransitionFloor currentTransitionFloor, TransitionFloorType type)
    {
        currentFloor = new Floor();
        if (type == TransitionFloorType.Obstacle)
        {
            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                MathHelpers.Approximately(lambdaExpression.transform.position.x, transform.position.x, 0.5f) &&
                MathHelpers.Approximately(lambdaExpression.transform.position.y, transform.position.y, 3f));
        }
        else if (type == TransitionFloorType.Ladder)
        {
            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                MathHelpers.Approximately(lambdaExpression.transform.position.x, transform.position.x, 0.5f) &&
                transform.position.y >= lambdaExpression.colliderBounds.bottomMid.y &&
                transform.position.y <= lambdaExpression.colliderBounds.topMid.y + collider2D.size.y / 2);
        }
        else if (type == TransitionFloorType.Stairs)
        {
            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                lambdaExpression.colliderBounds.bottomLeft.x <= transform.position.x &&
                lambdaExpression.colliderBounds.bottomRight.x >= transform.position.x &&
                lambdaExpression.colliderBounds.boundingBoxBottomY.y <= transform.position.y &&
                lambdaExpression.colliderBounds.boundingBoxTopY.y >= transform.position.y);
        }
    }
}