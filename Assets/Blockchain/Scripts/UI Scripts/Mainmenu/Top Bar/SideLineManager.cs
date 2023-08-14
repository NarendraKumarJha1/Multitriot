using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SideLineManager : MonoBehaviour
{
    public List<Button> sideBtns = new List<Button>();

    int btnIndex = 0;

    private void OnEnable()
    {
        btnIndex = 0;

        sideBtns.ForEach((x) =>
        {
            x.gameObject.SetActive(false);
            x.transform.localPosition = new Vector2(50f, 60f);
        });

        this.GetComponent<VerticalLayoutGroup>().enabled = false;
        this.GetComponent<ContentSizeFitter>().enabled = false;

        OpenSlideBtns();
    }

    void OpenSlideBtns()
    {
        if (btnIndex < sideBtns.Count)
        {
            sideBtns[btnIndex].gameObject.SetActive(true);
            sideBtns[btnIndex].transform.DOLocalMoveY((-50f - (105f * btnIndex)), 0.5f).OnComplete(() =>
            {
                btnIndex++;
                OpenSlideBtns();
            });
        }
        else
        {
            this.GetComponent<VerticalLayoutGroup>().enabled = true;
            this.GetComponent<ContentSizeFitter>().enabled = true;
        }
    }
}