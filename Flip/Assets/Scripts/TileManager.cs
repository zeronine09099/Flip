using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap buildTilemap;

    private readonly HashSet<Vector3Int> constructedCells = new HashSet<Vector3Int>();

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (buildTilemap == null)
        {
            buildTilemap = GetComponent<Tilemap>();
        }
    }

    private void OnEnable()
    {
        Events.Constructed += DisableConstructedTile;
        Events.BuildCanceled += ResetBuildEnable;
    }

    private void OnDisable()
    {
        Events.Constructed -= DisableConstructedTile;
        Events.BuildCanceled -= ResetBuildEnable;
    }

    private void Start()
    {
        BuildManager.buildEnable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!BuildManager.buildActived)
        {
            return;
        }

        Vector3Int cellPosition;
        Vector3 worldPosition;

        if (!TryGetMouseTile(out cellPosition, out worldPosition))
        {
            BuildManager.buildEnable = false;
            Events.CallBuildTargetChanged(Vector3Int.zero, GetMouseWorldPosition(), false);
            return;
        }

        //TODO : buildActived == true이고 커서가 타일 위에 있을 때 
        // tileEnable == false라면 buildEnable = false로 바꾸기
        // tileEnable == true이면 buildEnable = true로 바꾸기
        bool tileEnable = !constructedCells.Contains(cellPosition);
        BuildManager.buildEnable = tileEnable;

        Events.CallBuildTargetChanged(cellPosition, worldPosition, tileEnable);
        
    }

    //TODO : 커서가 타일 위에 있을 때 Constructed 이벤트가 호출되면 tileEnable = false로 설정
    private void DisableConstructedTile(Vector3Int cellPosition)
    {
        if (buildTilemap == null || !buildTilemap.HasTile(cellPosition))
        {
            return;
        }

        constructedCells.Add(cellPosition);
        BuildManager.buildEnable = false;
    }

    private void ResetBuildEnable()
    {
        BuildManager.buildEnable = true;
    }

    private bool TryGetMouseTile(out Vector3Int cellPosition, out Vector3 worldPosition)
    {
        cellPosition = Vector3Int.zero;
        worldPosition = Vector3.zero;

        if (buildTilemap == null)
        {
            return false;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        cellPosition = buildTilemap.WorldToCell(mouseWorldPosition);

        if (!buildTilemap.HasTile(cellPosition))
        {
            return false;
        }

        worldPosition = buildTilemap.GetCellCenterWorld(cellPosition);
        return true;
    }

    private Vector3 GetMouseWorldPosition()
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
