using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap buildTilemap;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public bool TryGetMouseTileCenterWorldPosition(out Vector3 cellCenterWorldPosition, out Vector3Int cellPosition)
    {
        cellCenterWorldPosition = Vector3.zero;
        cellPosition = Vector3Int.zero;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null || buildTilemap == null)
        {
            Debug.LogWarning("카메라 혹은 타일맵 없음");
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane tilemapPlane = new Plane(
            buildTilemap.transform.forward,
            buildTilemap.transform.position
        );

        if (!tilemapPlane.Raycast(ray, out float distance))
        {
            Debug.LogWarning("레이캐스팅 안됨");
            return false;
        }

        Vector3 hitWorldPosition = ray.GetPoint(distance);

        cellPosition = buildTilemap.WorldToCell(hitWorldPosition);

        if (!buildTilemap.HasTile(cellPosition))
        {
            return false;
        }

        cellCenterWorldPosition = buildTilemap.GetCellCenterWorld(cellPosition);

        return true;
    }

    public Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }
}
