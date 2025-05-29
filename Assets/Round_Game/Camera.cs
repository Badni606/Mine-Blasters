using System.Collections.Generic;
using UnityEngine;

public class RoundCamera : MonoBehaviour
{
    public List<Transform> players;  // Assign player transforms
    public Transform[] playerOrder;
    public float smoothTime = 0.5f;  // Smooth movement
    public float minZoom = 5f;       // Minimum zoom level
    public float maxZoom = 15f;      // Maximum zoom level
    public float zoomLimiter = 10f;  // Controls zoom sensitivity

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        playerOrder = new Transform[GameManager.instance.playerCount];
    }

    void LateUpdate()
    {
        if (players.Count == 0) return;

        MoveCamera();
        AdjustZoom();
    }

    void MoveCamera()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void AdjustZoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    public Vector3 GetCenterPoint()
    {
        if (players.Count == 1) return players[0].position;

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);
        foreach (Transform player in players)
        {
            bounds.Encapsulate(player.position);
        }
        return bounds.center;
    }

    float GetGreatestDistance()
    {
        Bounds bounds = new Bounds(players[0].position, Vector3.zero);
        foreach (Transform player in players)
        {
            bounds.Encapsulate(player.position);
        }
        return bounds.size.x;
    }

    public void AddObjectToFollow(Transform objectTransform)
    {
        players.Add(objectTransform);
        playerOrder[players.Count - 1] = objectTransform;
    }

    public void RemoveObject(int playerIndex)
    {
        players.Remove(playerOrder[playerIndex]);
    }
}


