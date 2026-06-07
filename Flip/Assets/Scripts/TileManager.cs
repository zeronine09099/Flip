using System.Collections.Generic;
using System.Runtime.Serialization;

//using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private MouseScript mouse;
    [SerializeField] private Tilemap buildTilemap;

    private readonly HashSet<Vector3Int> constructedCells = new HashSet<Vector3Int>();



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

        Vector3 cellCenterWorldPosition;
        Vector3Int cellPosition;
        Vector3 worldPosition;

        if (!mouse.TryGetMouseTileCenterWorldPosition(out cellCenterWorldPosition, out cellPosition))
        {
            BuildManager.buildEnable = false;
            Events.CallBuildTargetChanged(Vector3Int.zero, mouse.GetMouseWorldPosition(), false);
            return;
        }

        //TODO : buildActived == true이고 커서가 타일 위에 있을 때 
        // tileEnable == false라면 buildEnable = false로 바꾸기
        // tileEnable == true이면 buildEnable = true로 바꾸기
        worldPosition = mouse.GetMouseWorldPosition();
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

    

}
