using UnityEngine;

public class SwipeController : MonoBehaviour
{
     [SerializeField] int maxPage;
     int currentPage;
     Vector3 targetPos;
     [SerializeField] Vector3 pageStep;
     [SerializeField] RectTransform levelPagesRect;

     [SerializeField] private float tweenTime;

     [SerializeField] LeanTweenType tweenType;

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        } else if (currentPage == maxPage)
        {
            currentPage = 0;
            targetPos -= maxPage*pageStep;
            Next();
        }
        PlayerPrefs.SetInt("Song", currentPage);
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
        else if (currentPage == 1) {
                    currentPage = maxPage + 1;
                    targetPos += maxPage*pageStep;
                    Previous();
        }
        PlayerPrefs.SetInt("Song", currentPage);
    }

    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPos,tweenTime).setEase(tweenType);
    }
}
