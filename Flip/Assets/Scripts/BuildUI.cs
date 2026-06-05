using UnityEngine;
using UnityEngine.EventSystems;

public class BuildUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Build Data")]
    [SerializeField] private GameObject buildingPrefab;

    // TODO : 좌클릭 시 클릭한 오브젝트 정보와 함께 BuildSelected 이벤트 호출
    // 추후에 자원 미충족 시 이벤트 호출 막을 것
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (buildingPrefab == null)
        {
            Debug.LogWarning($"{name}에 buildingPrefab이 할당되지 않았습니다.");
            return;
        }
        Debug.Log("건설 모드");
        Events.CallBuildSelected(buildingPrefab);
    }
}