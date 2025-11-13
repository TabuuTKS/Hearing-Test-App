using UnityEngine;

public class UI : MonoBehaviour
{

    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject CalibrationMenu;

    private void Awake()
    {
        StaticDataAndHelpers.Init();
    }

    public void CalibrateMenu()
    {
        MainMenu.SetActive(false);
        CalibrationMenu.SetActive(true);
    }

    public void CalibrateBackBTN()
    {
        MainMenu.SetActive(true);
        CalibrationMenu.SetActive(false);
    }
}
