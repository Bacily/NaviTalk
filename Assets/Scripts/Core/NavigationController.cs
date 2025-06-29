using UnityEngine;
using UnityEngine.AI;

public class NavigationController : MonoBehaviour {

    public Vector3 TargetPosition { get; set; } = Vector3.zero;

    public NavMeshPath CalculatedPath { get; private set; }

    [SerializeField]
    public GameObject UseStairsMessage;

    [SerializeField]
    public GameObject InfoMessage;

    [SerializeField]
    public GameObject QrPanel;

    [SerializeField]
    public GameObject lineOptionPanel;

    [SerializeField]
    public GameObject Map;

    [SerializeField]
    public GameObject exitButton;



    NavMeshHit hit;

    private int walkableAreaMask;

    private int jumpAreaMask;


    private void Start() {
        jumpAreaMask = 1 << (NavMesh.GetAreaFromName("Jump"));
        walkableAreaMask = 1 << (NavMesh.GetAreaFromName("Walkable"));
        CalculatedPath = new NavMeshPath();
        //QrPanel.SetActive(true);
        //lineOptionPanel.SetActive(false);
        //Map.SetActive(false);
        //exitButton.SetActive(false);
    }

    private void Update() {
        if (TargetPosition != Vector3.zero) {
            NavMesh.CalculatePath(transform.position, TargetPosition, walkableAreaMask, CalculatedPath);
        }
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, jumpAreaMask) && !QrPanel.activeSelf && !InfoMessage.activeSelf)
        {
            UseStairsMessage.SetActive(true);
        }
        else {
            UseStairsMessage.SetActive(false);
        }
    }

    public void ShowInfo() {
        InfoMessage.SetActive(true);
        UseStairsMessage.SetActive(false);
    }


    public void HideInfo()
    {
        InfoMessage.SetActive(false);

    }
}
