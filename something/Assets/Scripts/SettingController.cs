using UnityEngine;

public class PopupController : MonoBehaviour
{
    public GameObject menu;
    private bool isMenuActive = false;

    void Start()
    {
        menu.SetActive(isMenuActive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        menu.SetActive(isMenuActive);
    }

    public void SetMenuActive(bool isActive)
    {
        isMenuActive = isActive;
        menu.SetActive(isMenuActive);
    }
}
