using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiDirectionIndicator : MonoBehaviour
{
    private Camera cam;

    [SerializeField] RectTransform self; 
    [SerializeField] RectTransform playerIconRect;
    [SerializeField] RectTransform arrow;
    [SerializeField] float margin;
    [SerializeField] float deathDelayPopUpEnd;

    [SerializeField] Image playerIcon;
    [SerializeField] Image arrowIcon;
    private PlayerCharacterController player;


    Vector3 ogSize;
    Vector3 arrowOgSize;
    float spawnAnimationDelay = 0.2f;

    bool disableArrow = false; 

    private void Update()
    {
       // if (Input.GetKeyDown(KeyCode.L)) Enable(false);
      //  if (Input.GetKeyDown(KeyCode.K)) Disable();
    }
    void OnPlayerDeath(bool oldValue, bool newValue)
    {
        Enable(newValue);
    }

    public void SetData(Sprite PlayerIcon, Sprite ArrowIcon, PlayerCharacterController Player)
    {
        playerIcon.sprite = PlayerIcon;
        arrowIcon.sprite = ArrowIcon;
        player = Player;
        player.isDead.OnValueChanged += OnPlayerDeath;
    }

    private void Start()
    {
        cam = Camera.main;

        ogSize = playerIconRect.localScale;
        arrowOgSize = arrow.transform.localScale;
        Enable(false);
    }

    private void Enable(bool showIcon)
    {
        if(showIcon) StartCoroutine(ScaleOverTime(playerIconRect, ogSize, spawnAnimationDelay));
        else StartCoroutine(ScaleOverTime(playerIconRect, Vector3.zero, spawnAnimationDelay));
    }
    private void FixedUpdate()
    {
        bool temp = disableArrow;
        SnapToPlayerPosition(player.transform);
        if(temp != disableArrow)
        {
            if (!disableArrow) StartCoroutine(ScaleOverTime(arrow, Vector3.zero, spawnAnimationDelay));
            else StartCoroutine(ScaleOverTime(arrow, arrowOgSize, spawnAnimationDelay));
        }
    }


    private void SnapToPlayerPosition(Transform playerPosition)
    {
        if (playerPosition == null) return;

        Vector3 pointOnPlane = cam.transform.position;
        Vector3 point = playerPosition.position;  
        Vector3 normal = Vector3.Cross(cam.transform.right, cam.transform.up);
        Vector3 planeToPoint = point - pointOnPlane;
        float dotProduct = Vector3.Dot(planeToPoint, normal);

        Vector3 screenPosition = cam.WorldToScreenPoint(playerPosition.position);

        if (dotProduct < 0) screenPosition *= -1;
        Vector3 diffCheck = screenPosition; 

        if (screenPosition.x > cam.pixelWidth - margin) screenPosition.x = cam.pixelWidth - margin;
        if (screenPosition.y > cam.pixelHeight - margin) screenPosition.y = cam.pixelHeight - margin;
        if (screenPosition.x < margin) screenPosition.x = margin;
        if (screenPosition.y < margin) screenPosition.y = margin;

        self.anchoredPosition = screenPosition;

        Vector3 dir = cam.WorldToScreenPoint(playerPosition.position) - screenPosition;
        if (diffCheck != screenPosition) disableArrow = true;
        else disableArrow = false;
        if (dotProduct < 0) dir *= -1;
        arrow.up = dir;
    }
    IEnumerator ScaleOverTime(Transform target, Vector3 endScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float time = 0;

        while (time < duration)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale;
    }
}
