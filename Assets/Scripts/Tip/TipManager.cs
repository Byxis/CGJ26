using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField]
    private TipUI tipUI;

    private bool isTipActive = false;
    private float previousTimeScale = 1f;
    private Queue<TipData> tipQueue = new Queue<TipData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (tipUI != null)
            tipUI.Hide();
    }

    public void TriggerTip(TipData data)
    {
        if (data == null)
            return;

        if (!data.isPersistent && PlayerPrefs.GetInt($"Tip_{data.ID}", 0) == 1)
        {
            return;
        }

        AddToQueue(data);
    }

    private void AddToQueue(TipData data)
    {
        tipQueue.Enqueue(data);

        if (!isTipActive)
        {
            ShowNextTip();
        }
    }

    private void ShowNextTip()
    {
        if (tipQueue.Count == 0)
        {
            EndTipSequence();
            return;
        }

        TipData data = tipQueue.Dequeue();

        if (!isTipActive)
        {
            isTipActive = true;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (tipUI != null)
        {
            tipUI.Show(data);
        }

        if (!data.isPersistent)
        {
            PlayerPrefs.SetInt($"Tip_{data.ID}", 1);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (isTipActive)
        {
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                if (tipUI != null && tipUI.IsAnimating)
                {
                    tipUI.SkipAnimation();
                }
                else
                {
                    ShowNextTip();
                }
            }
        }
    }

    private void EndTipSequence()
    {
        isTipActive = false;
        if (tipUI != null)
            tipUI.Hide();

        Time.timeScale = previousTimeScale > 0 ? previousTimeScale : 1f;
    }

    public void ResetAllTips()
    {
        PlayerPrefs.DeleteAll();
    }
}
