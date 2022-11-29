using UnityEngine;
using System.Collections;

public class LineAnimation : MonoBehaviour
{
   [SerializeField] 
   private float animationDuration = 5f;     // 그려지는 시간

   private LineRenderer lineRenderer;
   private Vector3[]    linePoints;          // 포인트 배열
   private int          pointsCount;         // 라인렌더러의 포인트 카운트
   
   private void Start()
   {
      lineRenderer = GetComponent<LineRenderer>();
      
      // 라인레더러의 포인트를 카운트하고, 카운트 만큼의 배열크기를 만들어 linePoints 배열에 저장
      pointsCount = lineRenderer.positionCount;
      linePoints = new Vector3[pointsCount];
      for (int i = 0; i < pointsCount; i++)
      {
         linePoints[i] = lineRenderer.GetPosition(i);
      }

      // 최초실행
      StartCoroutine(AnimateLine());
   }

   private void OnEnable()
   {
      // 그 이후 실행
      StartCoroutine(AnimateLine());
   }

   private IEnumerator AnimateLine()
   {
      float segmentDuration = animationDuration / pointsCount;

      for (int i = 0; i < pointsCount - 1; i++)
      {
         float startTime = Time.unscaledTime;                  // 퍼즈상태이니, un이고, 시작시간을 기록하는 것 이니, unscaledTime사용 (unscaledDeltaTime X)

         Vector3 startPosition = linePoints[i];
         Vector3 endPosition   = linePoints[i+1];

         Vector3 pos = startPosition;
         while (pos != endPosition)
         {
            float t = (Time.unscaledTime - startTime) / segmentDuration;
            pos     = Vector3.Lerp(startPosition, endPosition, t);

            for (int j = i + 1; j < pointsCount; j++)
               lineRenderer.SetPosition(j,pos);

            yield return null;
         }
      }
   }

}
