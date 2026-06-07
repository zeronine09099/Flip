
using UnityEngine;
using System.Collections;
using System.Diagnostics.Tracing;

public class TurnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform buildingParent;
    [SerializeField] private GameObject ProduceBuilding;
    [SerializeField] private GameObject CombatBuilding;


    [Header("Flip Animation")]
    [SerializeField] private float flipDuration = 0.6f;
    private bool isCombatTurn;
    private bool isFlipping;
    private Quaternion baseLocalRotation;
    private Vector3 currentFlipEuler;
    private bool isFlipHalfProgressed;

    private void Awake()
    {
        if (buildingParent != null)
        {
            baseLocalRotation = buildingParent.localRotation;
        }

        currentFlipEuler = Vector3.zero;
    }

    private void OnEnable()
    {
        Events.Fliped += FlipActivate;
    }

    private void OnDisable()
    {
        Events.Fliped -= FlipActivate;
    }
    void Start()
    {
        isCombatTurn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FliptoCombat()
    {
        isCombatTurn = true;
        //x축, z축 180도 회전
        if (buildingParent == null)
        {
            Debug.LogWarning("buildingParent가 지정되지 않았습니다.");
            return;
        }

        // x축, z축 180도 회전
        StartCoroutine(FlipRoutine(new Vector3(180f, 0f, 0f)));
        //TODO : 현재 이 코드는 Y축만 돌아감
        // 돌아가는 애니메이션이 보여야 하므로 coroutine을 이용해 돌아가는 모습이 보이게 하기
    }

    private void FliptoProduce()
    {
        isCombatTurn = false;
        //x축, z축 -180도 회전
        if (buildingParent == null)
        {
            Debug.LogWarning("buildingParent가 지정되지 않았습니다.");
            return;
        }

        // x축, z축 -180도 회전
        StartCoroutine(FlipRoutine(Vector3.zero));
    }

    private void FlipActivate()
    {
        if (isCombatTurn & !isFlipping)
        {
            FliptoProduce();
        }
        else if (!isCombatTurn & !isFlipping)
        {
            FliptoCombat();
        }
    }

    private IEnumerator FlipRoutine(Vector3 targetFlipEuler)
    {
        isFlipping = true;
        isFlipHalfProgressed = false;

        Vector3 startFlipEuler = currentFlipEuler;

        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / flipDuration;
            t = Mathf.Clamp01(t);

            if(t >= 0.5f & !isFlipHalfProgressed)
            {
                isFlipHalfProgressed = true;
                ChangeAppearBuilding();
            }

            // 부드러운 가감속
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            currentFlipEuler = new Vector3(
                Mathf.LerpAngle(startFlipEuler.x, targetFlipEuler.x, smoothT),
                Mathf.LerpAngle(startFlipEuler.y, targetFlipEuler.y, smoothT),
                Mathf.LerpAngle(startFlipEuler.z, targetFlipEuler.z, smoothT)
            );

            buildingParent.localRotation =
                baseLocalRotation * Quaternion.Euler(currentFlipEuler);

            yield return null;
        }

        currentFlipEuler = targetFlipEuler;

        buildingParent.localRotation =
            baseLocalRotation * Quaternion.Euler(currentFlipEuler);

        isFlipping = false;
    }

    private void ChangeAppearBuilding()
    {
        if(isCombatTurn)
        {
            CombatBuilding.SetActive(true);
            ProduceBuilding.SetActive(false);
        }
        else
        {
            CombatBuilding.SetActive(false);
            ProduceBuilding.SetActive(true);
        }
    }
}
