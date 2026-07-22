using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiAreaBinder : MonoBehaviour
{
    [SerializeField] private RectTransform targetArea; // 300x400 크기로 잡아둔 UI 오브젝트
    [SerializeField] private ParticleSystem confettiParticle;

    void Start()
    {
        SyncToArea();
    }

    public void SyncToArea()
    {
        Vector3[] corners = new Vector3[4];
        targetArea.GetWorldCorners(corners);

        float width = Vector3.Distance(corners[0], corners[3]);  // 좌측 하단 ~ 우측 하단
        float height = Vector3.Distance(corners[0], corners[1]); // 좌측 하단 ~ 좌측 상단

        // 파티클 오브젝트를 영역 중심으로 이동
        confettiParticle.transform.position = targetArea.position;

        // Shape 모듈을 Box로 설정하고 크기 맞추기
        var shape = confettiParticle.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(width, height, 0.1f); // Z는 얇게
    }
}