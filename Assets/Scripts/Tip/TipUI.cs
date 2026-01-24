using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipUI : MonoBehaviour
{
    [SerializeField]
    private GameObject contentPanel;
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private RectTransform highlightCircle;

    [Header("Animation Settings")]
    [SerializeField]
    private float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;
    private Dictionary<int, float> pauseEvents = new Dictionary<int, float>();
    private string fullText;
    public bool IsAnimating { get; private set; }

    public void Show(TipData data)
    {
        if (contentPanel != null)
            contentPanel.SetActive(true);

        if (highlightCircle != null)
        {
            highlightCircle.gameObject.SetActive(data.showHighlight);
            if (data.showHighlight)
            {
                highlightCircle.anchoredPosition = data.highlightPosition;
            }
        }

        if (messageText != null)
        {
            PreprocessMessage(data.message);
            messageText.text = fullText;
            messageText.maxVisibleCharacters = 0;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText());
        }
    }

    private void PreprocessMessage(string rawMessage)
    {
        pauseEvents.Clear();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int visibleCount = 0;
        int i = 0;

        while (i < rawMessage.Length)
        {
            if (rawMessage[i] == '§')
            {
                if (i + 1 < rawMessage.Length)
                {
                    char code = rawMessage[i + 1];
                    // Gestion des couleurs
                    if (code == 'b')
                    {
                        sb.Append("<color=#40a6ff>");
                        i += 2;
                    }
                    else if (code == 'R')
                    {
                        sb.Append("<color=#ff4d4d>");
                        i += 2;
                    }
                    else if (code == 'j')
                    {
                        sb.Append("<color=#ffda44>");
                        i += 2;
                    }
                    else if (code == 'v')
                    {
                        sb.Append("<color=#4cd137>");
                        i += 2;
                    }
                    else if (code == 'V')
                    {
                        sb.Append("<color=#9c88ff>");
                        i += 2;
                    }
                    else if (code == 'i')
                    {
                        sb.Append("<i>");
                        i += 2;
                    }
                    else if (code == 's')
                    {
                        sb.Append("<u>");
                        i += 2;
                    }
                    else if (code == 'g')
                    {
                        sb.Append("<b>");
                        i += 2;
                    }
                    else if (code == 'I')
                    {
                        sb.Append("</i>");
                        i += 2;
                    }
                    else if (code == 'S')
                    {
                        sb.Append("</u>");
                        i += 2;
                    }
                    else if (code == 'G')
                    {
                        sb.Append("</b>");
                        i += 2;
                    }
                    else if (code == 'r')
                    {
                        sb.Append("</color></i></u></b>");
                        i += 2;
                    }
                    else if (code == 'w')
                    {
                        i += 2;
                        int startNum = i;
                        while (i < rawMessage.Length && (char.IsDigit(rawMessage[i]) || rawMessage[i] == '.'))
                        {
                            i++;
                        }
                        string numStr = rawMessage.Substring(startNum, i - startNum);
                        if (float.TryParse(numStr,
                                           System.Globalization.NumberStyles.Any,
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           out float duration))
                        {
                            if (!pauseEvents.ContainsKey(visibleCount))
                                pauseEvents.Add(visibleCount, duration);
                            else
                                pauseEvents[visibleCount] += duration;
                        }
                    }
                    else
                    {
                        sb.Append(rawMessage[i]);
                        visibleCount++;
                        i++;
                    }
                }
                else
                {
                    sb.Append(rawMessage[i]);
                    visibleCount++;
                    i++;
                }
            }
            else if (rawMessage[i] == '<')
            {
                int closeIndex = rawMessage.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    sb.Append(rawMessage.Substring(i, closeIndex - i + 1));
                    i = closeIndex + 1;
                }
                else
                {
                    sb.Append(rawMessage[i]);
                    visibleCount++;
                    i++;
                }
            }
            else
            {
                sb.Append(rawMessage[i]);
                visibleCount++;
                i++;
            }
        }
        fullText = sb.ToString();
    }

    private IEnumerator TypeText()
    {
        IsAnimating = true;

        messageText.ForceMeshUpdate();
        int totalVisibleCharacters = messageText.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            messageText.maxVisibleCharacters = i;

            if (pauseEvents.ContainsKey(i))
            {
                yield return new WaitForSecondsRealtime(pauseEvents[i]);
            }

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        messageText.maxVisibleCharacters = 99999;
        IsAnimating = false;
    }

    public void SkipAnimation()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        messageText.maxVisibleCharacters = 99999;
        IsAnimating = false;
    }

    public void Hide()
    {
        if (contentPanel != null)
            contentPanel.SetActive(false);
        IsAnimating = false;
    }
}
