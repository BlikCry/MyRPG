using UnityEngine;
using UnityEngine.SceneManagement;

internal class Door: MonoBehaviour
{
    [SerializeField]
    private string leadingScene;
    [SerializeField]
    private Vector3 leadingPoint;

    public static Vector3? CurrentPoint;

    public void Go()
    {
        CurrentPoint = leadingPoint;
        SaveController.Instance.Save();
        SceneManager.LoadScene(leadingScene);
    }
}