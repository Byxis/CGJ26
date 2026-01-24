using UnityEngine;

public class TipOpener : MonoBehaviour
{
    [SerializeField]
    private TipData[] tipDatas;

    private void Start()
    {
        StartCoroutine(TriggerTipsAfterDelay());
    }

    private System.Collections.IEnumerator TriggerTipsAfterDelay()
    {
        yield return null;

        if (TipManager.Instance == null)
        {
            yield break;
        }

        for (int i = 0; i < tipDatas.Length; i++)
        {
            TipData tipData = tipDatas[i];
            if (tipData != null)
            {
                TipManager.Instance.TriggerTip(tipData);
            }
        }
    }
}