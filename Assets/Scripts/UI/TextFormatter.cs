using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text)), ExecuteInEditMode]
internal class TextFormatter: MonoBehaviour
{
    [SerializeField]
    private string format;

    private TMP_Text fieldText;

    private void Awake()
    {
        fieldText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;
        fieldText.text = format;
    }

    public void Format(params object[] args)
    {
        fieldText.text = string.Format(format, args);
    }
}