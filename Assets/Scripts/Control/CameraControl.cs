using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : Singleton<CameraControl>
{
    public GameObject player;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
    public float distanceFromPlayer;
    public float maxDistance = 3;
    public float maxOffset = 1;

    private float correctedMaxDistance {
        get {
            if (isInSpecialAtk)
                return 0;
            return maxDistance;
        }
    }
    private float correctedMaxOffset
    {
        get
        {
            if (isInSpecialAtk)
                return 0;
            return maxOffset;
        }
    }
    Vector3 toPos;

    void Start()
    {
        toPos = transform.position;

    }
    public bool isLeft;
    public bool isInSpecialAtk;
    public Vector3 targetPosition {
        get {
            return player.transform.position + 
                (new Vector3(player.GetComponent<CapsuleCollider2D>().offset.x, player.GetComponent<CapsuleCollider2D>().offset.y, 0) * player.transform.localScale.x);
        }
    }
    void Update()
    {
        if (player == null)
        {
            return;
        }

        distanceFromPlayer = Mathf.Abs(transform.position.x - targetPosition.x);
        isLeft = transform.position.x < targetPosition.x;

        if (distanceFromPlayer > correctedMaxDistance)
        {
            toPos.x = targetPosition.x + (isLeft ? -correctedMaxDistance : correctedMaxDistance);
        }
        Vector3 newPos = transform.position;
        newPos = Vector3.SmoothDamp(transform.position, toPos, ref velocity, smoothTime);
        if (distanceFromPlayer > correctedMaxDistance + correctedMaxOffset)
        {
            if (isLeft)
                newPos.x = targetPosition.x - (correctedMaxDistance + correctedMaxOffset);
            else
                newPos.x = targetPosition.x + (correctedMaxDistance + correctedMaxOffset);
        }
        transform.position = newPos;

    }

    public void onSpecialAtkFor(float duration)
    {
        StartCoroutine(_onSpecialAtkFor(duration));
    }
    IEnumerator _onSpecialAtkFor(float duration)
    {
        isInSpecialAtk = true;
        yield return new WaitForSeconds(duration);
        isInSpecialAtk = false;
    }
}
