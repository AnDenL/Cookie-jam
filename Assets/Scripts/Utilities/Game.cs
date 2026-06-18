using UnityEngine;
using Creatures;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class Game
{
    public static Camera mainCamera;
    
    public static float TimeSpeed = 1;
    public static bool IsPaused;
    public static PrefabsList GlobalObjects
    {
        get
        {
            if (objectsCache != null)
                return objectsCache;
            else
            {
                objectsCache = Resources.Load<PrefabsList>("GlobalObjects");
                return objectsCache;
            }
        }
    }

    private static PrefabsList objectsCache;
    private static readonly Collider2D[] results = new Collider2D[32];
    
    public static Creature FindNearestToMouse(float radius = 1f)
    {
        Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos.z = 0f;

        int count = Physics2D.OverlapCircleNonAlloc(cursorWorldPos, radius, results);

        Creature nearestCreature = null;
        float nearestSqrDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (!col) continue;

            if (!col.TryGetComponent(out Creature creature))
                continue;

            if (creature.Alignment != Alignment.Enemy) continue;

            float sqrDist = ((Vector2)col.transform.position - (Vector2)cursorWorldPos).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearestCreature = creature;
            }
        }

        return nearestCreature;
    }

    public static bool HoverUI()
    {
        Vector2 position = Input.mousePosition;
        PointerEventData pointer = new(EventSystem.current)
        {
            position = position
        };
        List<RaycastResult> raycastResults = new();

        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.distance == 0 && result.isValid)
                {
                    return true;
                }
            }
        }

        return false;
    }
}