
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static bool buildActived;
    public static bool buildEnable;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject buildAvatarPrefab;
    [SerializeField] private MouseScript mouse;
    [SerializeField] private Transform buildingParent;

    [Header("Avatar Color")]
    [SerializeField] private Color enableColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color disableColor = new Color(1f, 0f, 0f, 0.6f);

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
            buildAvatarInstance.transform.position, 
            Quaternion.identity,
            buildingParent
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


    private void MoveBuildAvatar()
    {
        float avatarDepthOffset = 0.05f;

        if (buildAvatarInstance == null)
        {
            return;
        }

        Vector3 targetPosition;

        if (!buildEnable)
        {
            targetPosition = currentBuildWorldPosition;
        }
        else
        {
            if (!mouse.TryGetMouseTileCenterWorldPosition(out targetPosition, out currentCellPosition))
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


}
