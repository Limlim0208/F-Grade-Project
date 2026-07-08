using UnityEngine;
using System.Collections.Generic;

public class IconSpawner : MonoBehaviour
{
    [Header("프리팹")]
    public GameObject penguinPrefab;
    public GameObject penguinGrayPrefab;
    public GameObject polarBearPrefab;

    [Header("설정")]
    public int iconCount = 14;
    public float fastSpeedMin = 200f;
    public float fastSpeedMax = 300f;
    public float normalSpeedMin = 30f;
    public float normalSpeedMax = 120f;

    public RectTransform spawnArea;
    public float minDistance = 70f;

    private List<GameObject> spawnedIcons = new List<GameObject>();

    public void SpawnIcons()
    {
        ClearIcons();

        float fastSpeed = Random.Range(fastSpeedMin, fastSpeedMax);
        int fastestIndex = Random.Range(0, iconCount);

        List<Vector2> placedPositions = new List<Vector2>();

        for (int i = 0; i < iconCount; i++)
        {
            // 3종류 중 랜덤 선택
            int randType = Random.Range(0, 3);
            GameObject prefab = randType == 0 ? penguinPrefab :
                                randType == 1 ? penguinGrayPrefab :
                                polarBearPrefab;

            GameObject icon = Instantiate(prefab, spawnArea);

            Vector2 pos = FindValidPosition(placedPositions);
            placedPositions.Add(pos);
            icon.GetComponent<RectTransform>().anchoredPosition = pos;

            RotatingIcon ri = icon.GetComponent<RotatingIcon>();
            ri.rotationSpeed = (i == fastestIndex) ? fastSpeed : Random.Range(normalSpeedMin, normalSpeedMax);
            ri.isFastest = (i == fastestIndex);
            ri.orbitSpeed = 50f;
            ri.spawnArea = spawnArea;                  // 추가

            spawnedIcons.Add(icon);
        }
    }

    Vector2 FindValidPosition(List<Vector2> placed)
    {
        Rect rect = spawnArea.rect;
        float padding = 40f;
        int maxTry = 300;

        for (int t = 0; t < maxTry; t++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(rect.xMin + padding, rect.xMax - padding),
                Random.Range(rect.yMin + padding, rect.yMax - padding)
            );

            bool valid = true;
            foreach (var p in placed)
            {
                if (Vector2.Distance(candidate, p) < minDistance)
                {
                    valid = false;
                    break;
                }
            }
            if (valid) return candidate;
        }

        return new Vector2(
            Random.Range(rect.xMin + padding, rect.xMax - padding),
            Random.Range(rect.yMin + padding, rect.yMax - padding)
        );
    }

    public void ClearIcons()
    {
        foreach (var icon in spawnedIcons)
            if (icon != null) Destroy(icon);
        spawnedIcons.Clear();
    }
}