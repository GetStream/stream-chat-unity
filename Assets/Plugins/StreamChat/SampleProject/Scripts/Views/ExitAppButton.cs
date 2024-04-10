using UnityEngine;
using UnityEngine.UI;

public class ExitAppButton : MonoBehaviour
{
    protected void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(Application.Quit);
    }
}
