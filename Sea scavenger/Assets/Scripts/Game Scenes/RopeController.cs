using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Custom.Patterns;
using SceneManagement = UnityEngine.SceneManagement;

public class RopeController : Singleton<RopeController>
{
    [SerializeField] private LineRenderer lineRend;
    [SerializeField]
    private HingeJoint2D segmentMidPrefab;
    [SerializeField]
    private int segmentsStartCount = 10;
    [SerializeField]
    private Rigidbody2D playerDockRb;
    [SerializeField]
    private float createRopeDeltaDistance = 10f;
    [SerializeField]
    private float removeRopeDeltaDistance = 5f;

    private List<HingeJoint2D> segments;
    private int originSegmentIndex = 0;
    private float length = 0;
    private Rigidbody2D rb;
    private Vector2 currentAnchorPointLocal;
    private Node _currentAnchorNode;
    private AStarPathfinder pathfinder;
    private Pool<HingeJoint2D> pool;
    private Stack<Vector2> anchors;
    private Camera _camera;
    private float segmentLength = 1f;

    private CancellationTokenSource _cancellationTokenSource;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (pathfinder?.Path == null)
            return;
        
        Gizmos.color = Color.green;
        
        for (int i = 0; i < pathfinder.Path.Count - 1; i++)
        {
            Gizmos.DrawLine((Vector2)pathfinder.Path[i].Point, (Vector2)pathfinder.Path[i + 1].Point);
        }
        
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireCube(Camera.main.transform.position, GetCameraRect().size);
    }
#endif

    protected override void Init()
    {
        segments = new List<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();

        pathfinder = new AStarPathfinder(100);
        _currentAnchorNode =  pathfinder.FindNode(transform.TransformPoint(currentAnchorPointLocal));
        _cancellationTokenSource = new CancellationTokenSource();
        
        pool = new Pool<HingeJoint2D>(transform, segmentMidPrefab, createNewFunc: delegate
        {
            var hj = Instantiate(segmentMidPrefab.gameObject, transform).GetComponent<HingeJoint2D>();
            //SceneManagement::SceneManager.MoveGameObjectToScene(hj.gameObject, _ropeScene);
            //hj.transform.SetParent(transform);
            
            return hj;
        });

        anchors = new Stack<Vector2>();
        _camera = Camera.main;

        segmentLength = segmentMidPrefab.GetComponent<BoxCollider2D>().size.y;
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
    }

    private void Start()
    {
        GenerateRope();

        _ = CheckLengthAsync(_cancellationTokenSource.Token);
    }

    private void Update()
    {
        lineRend.positionCount = segments.Count + 1;
        for (int i = 0; i < segments.Count; i++)
        {
            lineRend.SetPosition(i, segments[i].transform.position);
        }
        lineRend.SetPosition(segments.Count, playerDockRb.transform.position);
    }

    private void GenerateRope()
    {
        segments.Add(GetAirLineSegment());
        ConnectHingeJoint2D(segments[0], rb, Vector3.zero);
        length = segmentLength;
        
        for (int i = 1; i < segmentsStartCount; i++)
        {
            AddSegmentToEnd();
        }

        ConnectLastSegmentWithPlayer();
    }

    private void AddSegmentToEnd()
    {
        InsertSegment(segments.Count);
    }

    private void AddSegmentToEndAndConnectWithPlayer()
    {
        RemoveConnectionWithPlayer();
        AddSegmentToEnd();
        ConnectLastSegmentWithPlayer();
    }

    private void AddSegmentToStart()
    {
        InsertSegment(0);
    }

    private void AddSegmentToOrigin()
    {
        InsertSegment(originSegmentIndex);
    }

    private void RemoveSegmentAt(int index)
    {
        if (index == 0)
        {
            ConnectHingeJoint2D(segments[1], rb, currentAnchorPointLocal);
        }
        else if (index < segments.Count - 1)
        {
            ConnectHingeJoint2D(segments[index + 1], segments[index - 1]);
        }
        
        pool.Release(segments[index]);
        segments.RemoveAt(index);
        length -= segmentLength;
    }

    private void ConnectHingeJoint2D(HingeJoint2D hingeJoint2D, 
        Rigidbody2D connectedBody, Vector2 connectedAnchor)
    {
        hingeJoint2D.connectedBody = connectedBody;
        hingeJoint2D.connectedAnchor = connectedAnchor;
    }

    private void ConnectHingeJoint2D(HingeJoint2D hjToConnection, HingeJoint2D hjConnectWith)
    {
        ConnectHingeJoint2D(hjToConnection, hjConnectWith.attachedRigidbody, Vector2.down * segmentLength);
    }

    private void RemoveSegmentFromStart()
    {
        RemoveSegmentAt(0);
    }

    private void RemoveSegmentFromEnd()
    {
        RemoveSegmentAt(segments.Count - 1);
    }

    private void RemoveSegmentFromEndAndConnectWithPlayer()
    {
        RemoveConnectionWithPlayer();
        RemoveSegmentFromEnd();
        ConnectLastSegmentWithPlayer();
    }
    
    private void InsertSegment(int index)
    {
        HingeJoint2D segm = GetAirLineSegment();
        HingeJoint2D prevSegm = null;
        HingeJoint2D nextSegm = null;
        
        if (index == 0)
        {
            nextSegm = segments[0];
            
            segm.transform.localPosition = currentAnchorPointLocal;
            segm.connectedAnchor = currentAnchorPointLocal;
            segm.connectedBody = rb;
            
            nextSegm.connectedAnchor = Vector2.down * segmentLength;
            nextSegm.connectedBody = segm.attachedRigidbody;
        }
        else if (index == segments.Count)
        {
            prevSegm = segments.Last();
            
            segm.transform.localPosition = prevSegm.transform.localPosition - prevSegm.transform.up * segmentLength;
            segm.connectedAnchor = Vector2.down * segmentLength;
            segm.connectedBody = prevSegm.attachedRigidbody;
        }
        else if (index > 0 && index < segments.Count)
        {
            prevSegm = segments[index - 1];
            nextSegm = segments[index];
            
            segm.transform.localPosition = nextSegm.transform.localPosition;
            segm.connectedAnchor = Vector2.down * segmentLength;
            segm.connectedBody = prevSegm.attachedRigidbody;
            
            nextSegm.connectedAnchor = Vector2.down * segmentLength;
            nextSegm.connectedBody = segm.attachedRigidbody;
        }
        else
        {
            throw new IndexOutOfRangeException();
        }

        segments.Insert(index, segm);

        length += segmentLength;
    }

    private void ShiftSegmentsToStartFrom(int indexFrom)
    {
        for (int i = indexFrom; i > 0; i--)
        {
            ShiftSegmentAt(segments[i].transform, segmentLength);
        }
    }

    private void ShiftSegmentAt(Transform segmentTransform, float distance)
    {
        segmentTransform.position += segmentTransform.up * distance;
    }

    private HingeJoint2D GetAirLineSegment()
    {
        HingeJoint2D segm;

        segm = pool.GetAvailable();
        segm.gameObject.layer = 7;

        return segm;
    }    

    private void ConnectLastSegmentWithPlayer()
    {
        HingeJoint2D conJ = segments.Last().gameObject.AddComponent<HingeJoint2D>();
        conJ.autoConfigureConnectedAnchor = false;
        conJ.anchor = Vector2.down * segmentLength;
        conJ.connectedBody = playerDockRb;
        conJ.connectedAnchor = Vector2.zero;
    }

    private void RemoveConnectionWithPlayer()
    {
        Destroy(segments.Last().GetComponents<HingeJoint2D>()[1]);
    }

    private bool SetSegmentAsAnchor(HingeJoint2D newAnchoredhj)
    {
        if (!InitializeCurrentAnchorNode(newAnchoredhj.transform.position))
            return false;
        
        anchors.Push(currentAnchorPointLocal);
        currentAnchorPointLocal = transform.InverseTransformPoint(newAnchoredhj.transform.position);
        
        newAnchoredhj.connectedAnchor = currentAnchorPointLocal;
        newAnchoredhj.connectedBody = rb;

        return true;
    }

    private bool InitializeCurrentAnchorNode(Vector2 position)
    {
        _currentAnchorNode = pathfinder.FindNode(position);
        
        return _currentAnchorNode != null;
    }

    private void AnchorSegment(int index)
    {
        while (index > 0)
        {
            if (SetSegmentAsAnchor(segments[index]))
            {
                break;
            }

            index--;
        }
        
        for (int i = 0; i < index; i++)
        {
            pool.Release(segments[0]);
            segments.RemoveAt(0);
            length -= segmentLength;
        }
    }

    private void RestoreAnchoredSegment(HingeJoint2D newAnchoredhj, Vector2 lastAnchorPosition)
    {
        currentAnchorPointLocal = lastAnchorPosition;
        InitializeCurrentAnchorNode(transform.TransformPoint(currentAnchorPointLocal));

        newAnchoredhj.connectedAnchor = lastAnchorPosition;
    }

    private void RestoreSegmentsAndAnchor()
    {
        var lastAnchor = anchors.Pop();
        float distToLastAnchor = (segments[0].connectedAnchor - lastAnchor).magnitude;

        do
        {
            AddSegmentToStart();
        } while ((distToLastAnchor -= segmentLength) > 0);
        
        RestoreAnchoredSegment(segments[0], lastAnchor);
    }

    private Rect GetCameraRect()
    {
        Vector2 _halfsize = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
        Rect camRect = new Rect((Vector2)_camera.transform.position - _halfsize, _halfsize * 2);
        camRect.size += Vector2.one * 3;

        return camRect;
    }

    private bool InsideCameraRect(Vector3 position, Rect cameraRect)
    {
        return cameraRect.Contains(position);
    }

    private int FirstSegmentIndexOutsideRect(Rect rect)
    {
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (!InsideCameraRect(segments[i].transform.position, rect))
            {
                return i;
            }
        }

        return 0;
    }
    
    private async Task CheckLengthAsync(CancellationToken cancellationToken)
    {
        Rect cameraRect;
        int prevOrigin;
        
        while(true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Cancelled from CheckLength");
            
                return;
            }
            
            try
            {
                await pathfinder.FindPathAsync(_currentAnchorNode, playerDockRb.transform.position, false);

                if (cancellationToken.IsCancellationRequested)
                    continue;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exeption in the pathinder: \"{e.Message}\"");
                continue;
            }
            
            if (pathfinder.Length > length - createRopeDeltaDistance)
            {
                ShiftSegmentsToStartFrom(segments.Count - 1);
                AddSegmentToEndAndConnectWithPlayer();
            }
            else if (pathfinder.Length < length - removeRopeDeltaDistance)
            {
                RemoveSegmentFromStart();
            }

            cameraRect = GetCameraRect();
            
            prevOrigin = originSegmentIndex;
            originSegmentIndex = FirstSegmentIndexOutsideRect(cameraRect);
            
            if (cameraRect.Contains(segments[0].transform.position))
            {
                while (cameraRect.Contains(segments[0].transform.position) && anchors.Count > 0)
                {
                    RestoreSegmentsAndAnchor();
                }
            }
            else if (prevOrigin != originSegmentIndex)
                AnchorSegment(originSegmentIndex);
        }
    }
}
