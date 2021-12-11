using System.Collections;
using UnityEngine;

public class TableVP1L2 : TablePhasesManager
{
    [SerializeField]
    GameObject scoreCanvas;

    private void Start()
    {
       // scoreCanvas.SetActive(false);
    }
    void Update()
    {
        if (UIManager.isSession)
        {
            timer += Time.deltaTime;

            StartCoroutine(Target());
        }
        else
        {
            timer = 0;

            StopCoroutine(Target());
        }
    }

    private IEnumerator Target()
    {
        switch ((int)timer)
        {
            case 1:
                MoveTargetUp(target_50R);
                yield return new WaitForSecondsRealtime(3.0f);
                MoveTargetDown(target_50R);
                break;
            case 9:
                MoveTargetUp(target_100);
                yield return new WaitForSecondsRealtime(3.0f);
                MoveTargetDown(target_100);
                break;
            case 14:
                MoveTargetUp(target_150);
                yield return new WaitForSecondsRealtime(3.0f);
                MoveTargetDown(target_150);
                break;
            case 19:
                MoveTargetUp(target_50L);
                MoveTargetUp(target_150);
                MoveTargetUp(target_200);
                yield return new WaitForSecondsRealtime(12.0f);
                MoveTargetDown(target_50L);
                MoveTargetDown(target_150);
                MoveTargetDown(target_200);
                break;
            case 33:
                MoveTargetUp(target_150);
                MoveTargetUp(target_200);
                MoveTargetUp(target_250);
                MoveTargetUp(target_300);
                yield return new WaitForSecondsRealtime(16.0f);
                MoveTargetDown(target_150);
                MoveTargetDown(target_200);
                MoveTargetDown(target_250);
                MoveTargetDown(target_300);
                break;
            case 57:
                DisplayL2();
            //    scoreCanvas.SetActive(true);
                break;
        }
    }
}
