using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public List<Floor> transitionFloors;
    public FloorInfo FloorInfo;
    public Collider2D collider2D;

    public Floor(Transform transform, Collider2D collider2D, int number)
    {
        this.number = number;
        this.transform = transform;
        FloorInfo = new FloorInfo();
        transitionFloors = new List<Floor>();
        this.collider2D = collider2D;
        UpdateFloorInfo(collider2D);
    }

    public Floor(int number)
    {
        this.number = -1;
        this.transform = null;
        FloorInfo = new FloorInfo();
        transitionFloors = new List<Floor>();
        this.collider2D = null;
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

    public TransitionFloor(Floor firstFloor, Floor secondFloor, Transform transform,
        ColliderBounds colliderBounds,
        TransitionFloorType type)
    {
        floors = new List<Floor>()
        {
            firstFloor,
            secondFloor
        };
        floors.Sort((floor, floor1) => floor.number - floor1.number);
        this.transform = transform;
        this.type = type;
        this.colliderBounds = colliderBounds;
    }

    public TransitionFloor(Floor firstFloor, Floor secondFloor,
        TransitionFloorType type)
    {
        floors = new List<Floor>()
        {
            firstFloor,
            secondFloor
        };
        floors.Sort((floor, floor1) => floor.number - floor1.number);

        this.transform = null;
        this.type = type;
        this.colliderBounds = new ColliderBounds();
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

struct Graph
{
    public LinkedList<Floor>[] floors;
    public int vertex;
    public int arcs;

    public Graph(List<Floor> floorsList, int arcs)
    {
        vertex = floorsList.Count;
        floorsList = floorsList.OrderBy(LambdaExpression => LambdaExpression.number).ToList();
        floors = new LinkedList<Floor>[vertex];

        for (var i = 0; i < vertex; i++)
        {
            floors[i] = new LinkedList<Floor>();
            foreach (var floor in floorsList[i].transitionFloors)
            {
                floors[i].AddLast(floor);
            }
        }

        this.arcs = arcs;
    }
}

public class Navigation : MonoBehaviour
{
    private MonoBehaviour monoBehaviour;
    private GameObject gameDataHolder;

    public List<Floor> floorList;
    public List<NavigationInfo> WorldNavigationInfo { get; set; }
    public List<TransitionFloor> transitionFloorList { get; set; }
    private Graph floorsGraph;
    private ContactFilter2D contactFilter2DForFloorCheck;

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
        this.contactFilter2DForFloorCheck = new ContactFilter2D()
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
        var count = 0;
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

                    if (floorNumber1.number == -1 || floorNumber2.number == -1) continue;
                    transitionFloorList.Add(new TransitionFloor(floorNumber1, floorNumber2,
                        navigationInfo.transform, navigationInfo.colliderBounds,
                        TransitionFloorType.Ladder));
                    floorNumber1.transitionFloors.Add(floorNumber2);
                    floorNumber2.transitionFloors.Add(floorNumber1);


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

                        floorNumber1 = this.floorList.Find(lambdaExpression =>
                            lambdaExpression.FloorInfo.startPointX.x <= bounds.min.x &&
                            lambdaExpression.FloorInfo.endPointX.x >= bounds.max.x &&
                            lambdaExpression.FloorInfo.startPointY.y <= bounds.min.y &&
                            lambdaExpression.FloorInfo.endPointY.y >= bounds.max.y);

                        floorNumber2 = this.floorList.Find(lambdaExpression =>
                            lambdaExpression.transform == navigationInfo.transform);

                        if (floorNumber1.number == -1 || floorNumber2.number == -1) continue;

                        transitionFloorList.Add(new TransitionFloor(floorNumber1, floorNumber2,
                            boxCollider2D.transform,
                            navigationInfo.colliderBounds,
                            TransitionFloorType.Obstacle));
                        floorNumber1.transitionFloors.Add(floorNumber2);
                        floorNumber2.transitionFloors.Add(floorNumber1);
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

                    if (floorNumber1.number == -1 || floorNumber2.number == -1) continue;

                    transitionFloorList.Add(new TransitionFloor(floorNumber1, floorNumber2,
                        navigationInfo.transform, navigationInfo.colliderBounds,
                        TransitionFloorType.Stairs));
                    floorNumber1.transitionFloors.Add(floorNumber2);
                    floorNumber2.transitionFloors.Add(floorNumber1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var outerFloor in this.floorList)
        {
            foreach (var innerFloor in this.floorList)
            {
                float tresholdX = 0.7f;
                float tresholdY = 0.6f;
                var center = outerFloor.collider2D.transform.TransformPoint(outerFloor.collider2D.offset);
                if (!MathHelpers.Approximately(outerFloor.transform.rotation.z, 0, float.Epsilon) ||
                    !MathHelpers.Approximately(innerFloor.transform.rotation.z, 0, float.Epsilon))
                {
                    tresholdX = 0.7f;
                    tresholdY = 2f;
                }

                if (MathHelpers.Approximately(outerFloor.FloorInfo.endPointX.y,
                        innerFloor.FloorInfo.startPointX.y,
                        tresholdY) && MathHelpers.Approximately(
                        outerFloor.FloorInfo.endPointX.x,
                        innerFloor.FloorInfo.startPointX.x,
                        tresholdX) && outerFloor.number != innerFloor.number)
                {
                    transitionFloorList.Add(new TransitionFloor(innerFloor, outerFloor,
                        TransitionFloorType.Consecutive));
                    innerFloor.transitionFloors.Add(outerFloor);
                    outerFloor.transitionFloors.Add(innerFloor);
                }
            }
        }

//        transitionFloorList =
//            transitionFloorList.OrderBy(LambdaExpression => LambdaExpression.floors[0].number).ToList();
//        foreach (var transitionFloor in transitionFloorList)
//        {
//            Debug.Log("[ " + transitionFloor.floors[0].number + " , " + transitionFloor.floors[1].number + " ]");
//            Debug.Log("[ " + transitionFloor.floors[0].transform.name + " , " +
//                      transitionFloor.floors[1].transform.name + " ]");
//        }

        this.floorsGraph = new Graph(this.floorList, this.transitionFloorList.Count);
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
        var temporaryFloors = this.floorList.FindAll(lambdaExpression =>
            lambdaExpression.FloorInfo.startPointX.x <= transform.position.x &&
            lambdaExpression.FloorInfo.endPointX.x >= transform.position.x &&
            lambdaExpression.FloorInfo.startPointY.y <= transform.position.y &&
            lambdaExpression.FloorInfo.endPointY.y >= transform.position.y);
        Floor currentFloorForTest = new Floor(-1);
        Collider2D[] colliders2D = new Collider2D[5];
        foreach (var temporaryFloor in temporaryFloors)
        {
            temporaryFloor.collider2D.GetContacts(this.contactFilter2DForFloorCheck, colliders2D);
            if (colliders2D.Where(contactCollider2D => contactCollider2D != null)
                .Any(contactCollider2D => contactCollider2D == collider2D &&
                                          contactCollider2D.gameObject.layer == transform.gameObject.layer))
            {
                currentFloorForTest = temporaryFloor;
            }
            if (currentFloorForTest.number != 0)
                break;
        }
        if (currentFloorForTest.number == -1) return;
        if (!MathHelpers.Approximately(currentFloorForTest.transform.rotation.z, 0, float.Epsilon))
        {
            var raycastHit2D = Physics2D
                .Raycast(transform.position, Vector2.up, 100f, LayerMask.GetMask("Ground", "Obstacle"));
            if (raycastHit2D.collider != null &&
                raycastHit2D.collider.transform == currentFloorForTest.transform) return;
        }
        currentTransitionFloor = new TransitionFloor();
        currentFloor = currentFloorForTest;
    }

    public void CheckForCurrentFloor(Transform transform, Collider2D floorCollider2D, ref Floor currentFloor,
        ref TransitionFloor currentTransitionFloor)
    {
        var temporaryFloor = this.floorList.Find(lambdaExpression =>
            lambdaExpression.FloorInfo.startPointX.x <= transform.position.x &&
            lambdaExpression.FloorInfo.endPointX.x >= transform.position.x &&
            lambdaExpression.FloorInfo.startPointY.y <= transform.position.y &&
            lambdaExpression.FloorInfo.endPointY.y >= transform.position.y &&
            lambdaExpression.collider2D == floorCollider2D);
        if (temporaryFloor.number == -1) return;

        currentTransitionFloor = new TransitionFloor();
        currentFloor = temporaryFloor;
    }

    public void CheckForCurrentTransitionFloor(Transform transform,
        ref Floor currentFloor,
        ref TransitionFloor currentTransitionFloor, TransitionFloorType type)
    {
        if (type == TransitionFloorType.Ladder)
        {
            currentFloor = new Floor(-1);

            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                lambdaExpression.transform != null &&
                MathHelpers.Approximately(lambdaExpression.transform.position.x, transform.position.x, 0.5f) &&
                transform.position.y >= lambdaExpression.colliderBounds.bottomMid.y &&
                transform.position.y <= lambdaExpression.colliderBounds.topMid.y);
        }
        else if (type == TransitionFloorType.Stairs)
        {
            currentFloor = new Floor(-1);
            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                !lambdaExpression.colliderBounds.Equals(null) &&
                lambdaExpression.colliderBounds.bottomLeft.x <= transform.position.x &&
                lambdaExpression.colliderBounds.bottomRight.x >= transform.position.x &&
                lambdaExpression.colliderBounds.boundingBoxBottomY.y <= transform.position.y &&
                lambdaExpression.colliderBounds.boundingBoxTopY.y >= transform.position.y);
        }
    }

    public void CheckForCurrentTransitionFloor(Transform transform, CapsuleCollider2D collider2D,
        ref Floor currentFloor,
        ref TransitionFloor currentTransitionFloor, TransitionFloorType type)
    {
        if (type == TransitionFloorType.Obstacle)
        {
            currentFloor = new Floor(-1);

            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                lambdaExpression.transform != null &&
                MathHelpers.Approximately(lambdaExpression.transform.position.x, transform.position.x, 0.5f) &&
                MathHelpers.Approximately(lambdaExpression.transform.position.y, transform.position.y, 3f));
        }
        else if (type == TransitionFloorType.Ladder)
        {
            currentFloor = new Floor(-1);

            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                lambdaExpression.transform != null &&
                MathHelpers.Approximately(lambdaExpression.transform.position.x, transform.position.x, 0.5f) &&
                transform.position.y >= lambdaExpression.colliderBounds.bottomMid.y &&
                transform.position.y <= lambdaExpression.colliderBounds.topMid.y + collider2D.size.y / 2);
        }
        else if (type == TransitionFloorType.Stairs)
        {
            currentFloor = new Floor(-1);

            currentTransitionFloor = this.transitionFloorList.Find(lambdaExpression =>
                !lambdaExpression.colliderBounds.Equals(null) &&
                lambdaExpression.colliderBounds.bottomLeft.x <= transform.position.x &&
                lambdaExpression.colliderBounds.bottomRight.x >= transform.position.x &&
                lambdaExpression.colliderBounds.boundingBoxBottomY.y <= transform.position.y &&
                lambdaExpression.colliderBounds.boundingBoxTopY.y >= transform.position.y);
        }
    }


    public List<TransitionFloor> CalculatePath(Floor currentFloor, Floor aimFloor, List<Floor> transitionFloors)
    {
        if (currentFloor.number == -1 || aimFloor.number == -1) return null;
        if (currentFloor.number == aimFloor.number) return new List<TransitionFloor>();
        Queue<int> queue = new Queue<int>(floorsGraph.vertex);
        Floor[] parent = new Floor[floorsGraph.vertex];
        int[] visitedFloorsList = new int[floorsGraph.vertex];
        int[] dist = new int[floorsGraph.vertex];

        int i;
        for (i = 0; i < floorsGraph.vertex; i++)
        {
            visitedFloorsList[i] = 0;
        }

        dist[currentFloor.number] = 0;
        parent[currentFloor.number] = currentFloor;
        queue.Enqueue(currentFloor.number);
        while (queue.ToArray().Length != 0)
        {
            LinkedListNode<Floor> link;
            i = queue.Dequeue();
            for (link = floorsGraph.floors[i].First; link != null; link = link.Next)
            {
                int floorNumber = link.Value.number;
                if (visitedFloorsList[floorNumber] == 0)
                {
                    parent[floorNumber] = link.Value;
                    visitedFloorsList[floorNumber] = i;
                    dist[floorNumber] = dist[i] + 1;
                    if (parent[floorNumber].number == aimFloor.number)
                    {
                        return CalculatePath(currentFloor, aimFloor, dist[aimFloor.number]);
                    }
                    queue.Enqueue(floorNumber);
                }
            }
        }


        return null;
    }

    private List<TransitionFloor> CalculatePath(Floor currentFloor, Floor aimFloor, int dist)
    {
        var visitedTransitions = new List<TransitionFloor>();
        var transitionlist = new List<TransitionFloor>();
        var currentAim = new Floor(-1);
        return null;
    }

    public void PrintFloors()
    {
        foreach (var floor in this.floorList)
        {
            foreach (var transitionFloor in floor.transitionFloors)
            {
                Debug.Log(floor.transform.name + "[" + floor.number + "]" + " : " + transitionFloor.transform.name);
            }
        }
    }
}