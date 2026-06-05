using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static bool buildActived;
    public static bool buildEnable;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject buildAvatarPrefab;
    [SerializeField] private GameObject buildingParent;

    [Header("Avatar Color")]
    [SerializeField] private Color enableColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color disableColor = new Color(1f, 0f, 0f, 0.6f);
    [SerializeField] private float avatarZPosition = 10f;

    private GameObject selectedBuildingPrefab;
    private GameObject buildAvatarInstance;
    private SpriteRenderer buildAvatarRenderer;

    private Vector3Int currentCellPosition;
    private Vector3 currentBuildWorldPosition;


    private bool hasBuildTarget;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    //TODO : BuildSelected 이벤트 호출 받으면 BuildAvartar 함수 실행


        private void OnEnable()
    {
        Events.BuildSelected += BuildAvartarActivate;
        Events.BuildTargetChanged += UpdateBuildTarget;
    }

    private void OnDisable()
    {
        Events.BuildSelected -= BuildAvartarActivate;
        Events.BuildTargetChanged -= UpdateBuildTarget;
    }

    void Start()
    {
        buildActived = false;
        buildEnable = true;
    }

    // Update is called once per frame
    void Update()   
    {
        if (!buildActived)
        {
            return;
        }

        //TODO : BuildActived == true라면 
        //마우스 커서 위치에 클릭한 건물의 아바타 띄우기
        MoveBuildAvatar();

        //TODO : BuildEnable != true라면 
        //아바타 빨간색으로 변경
        UpdateBuildAvatarColor();

        //TODO : 우클릭 시 BuildCancel 함수 호출
        if (Input.GetMouseButtonDown(1))
        {
            BuildCancel();
            return;
        }

        //TODO : 좌클릭 시 BuildEnable == true라면 ConstructBuilding 함수 호출, constructed 이벤트 호출
        if (Input.GetMouseButtonDown(0) && buildEnable)
        {
            ConstructBuilding();
            Events.CallConstructed(currentCellPosition);
            BuildCancel();
        }
    }

    private void BuildAvartarActivate(GameObject buildingPrefab)
    {   
        if (buildingPrefab == null)
        {
            return;
        }

        selectedBuildingPrefab = buildingPrefab;

        buildActived = true;
        buildEnable = true;
        hasBuildTarget = false;

        CreateBuildAvatar();
        MoveBuildAvatar();
        UpdateBuildAvatarColor();
        //TODO : 
        
        
    }

    private void BuildCancel()
    {
        buildActived = false;
        buildEnable = false;
        hasBuildTarget = false;
        selectedBuildingPrefab = null;

        if (buildAvatarInstance != null)
        {
            Destroy(buildAvatarInstance);
            buildAvatarInstance = null;
            buildAvatarRenderer = null;
        }
    }

    private void ConstructBuilding()
    {
        //TODO : 좌클릭 한 시점의 좌표에 건물 건설
        if (selectedBuildingPrefab == null || !hasBuildTarget)
        {
            if(selectedBuildingPrefab == null)
            {
                Debug.LogWarning("프리팹 없음 설치 불가");
            }

            else if(!hasBuildTarget)
            {
                Debug.LogWarning("타켓 없음");
            }
            return;
        }

        Instantiate(
            selectedBuildingPrefab, 
            currentBuildWorldPosition, 
            Quaternion.identity
            );
    }

    private void CreateBuildAvatar()
    {
        if (buildAvatarInstance != null)
        {
            Destroy(buildAvatarInstance);
        }

        if (buildAvatarPrefab != null)
        {
            buildAvatarInstance = Instantiate(buildAvatarPrefab);
        }

        buildAvatarRenderer = buildAvatarInstance.GetComponentInChildren<SpriteRenderer>();
        CopySelectedBuildingSpriteToAvatar();
    }

    private void CopySelectedBuildingSpriteToAvatar()
    {
        if (buildAvatarRenderer == null || selectedBuildingPrefab == null)
        {
            return;
        }

        SpriteRenderer selectedRenderer = selectedBuildingPrefab.GetComponentInChildren<SpriteRenderer>();
        if (selectedRenderer == null)
        {
            return;
        }

        buildAvatarRenderer.sprite = selectedRenderer.sprite;
        buildAvatarRenderer.sortingLayerID = selectedRenderer.sortingLayerID;
        buildAvatarRenderer.sortingOrder = selectedRenderer.sortingOrder + 1;
    }

    private bool TryGetMouseTileCenterWorldPosition(out Vector3 cellCenterWorldPosition, out Vector3Int cellPosition)
    {
        cellCenterWorldPosition = Vector3.zero;
        cellPosition = Vector3Int.zero;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null || buildTilemap == null)
        {
            return false;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane tilemapPlane = new Plane(
            buildTilemap.transform.forward,
            buildTilemap.transform.position
        );

        if (!tilemapPlane.Raycast(ray, out float distance))
        {
            return false;
        }

        Vector3 hitWorldPosition = ray.GetPoint(distance);

        cellPosition = buildTilemap.WorldToCell(hitWorldPosition);
        cellCenterWorldPosition = buildTilemap.GetCellCenterWorld(cellPosition);

        return true;
    }

    private void MoveBuildAvatar()
    {
        if (buildAvatarInstance == null)
        {
            return;
        }

        Vector3 targetPosition;

        if (hasBuildTarget)
        {
            targetPosition = currentBuildWorldPosition;
        }
        else
        {
            if (!TryGetMouseTileCenterWorldPosition(out targetPosition, out currentCellPosition))
            {
                return;
            }
        }

        Vector3 towardCamera = (mainCamera.transform.position - targetPosition).normalized;
        Vector3 avatarPosition = targetPosition + towardCamera * avatarDepthOffset;

        buildAvatarInstance.transform.position = avatarPosition;
    }

    private void UpdateBuildAvatarColor()
    {
        if (buildAvatarRenderer == null)
        {
            return;
        }

        buildAvatarRenderer.color = buildEnable ? enableColor : disableColor;
    }

    private void UpdateBuildTarget(Vector3Int cellPosition, Vector3 worldPosition, bool canBuild)
    {
        currentCellPosition = cellPosition;
        currentBuildWorldPosition = worldPosition;
        hasBuildTarget = true;
        buildEnable = canBuild;
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
