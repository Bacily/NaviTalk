using UnityEngine;




public class SceneStartup : MonoBehaviour
{

    [SerializeField]
    public QrCodeRecenter QRscript;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Scene-specific logic running");
        QRscript.ToggleScanning();
        QRscript.exitButton.SetActive(false);
        QRscript.backButton.SetActive(true);
    }

}
