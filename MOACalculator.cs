using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MOACalculator : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    
    public TextMeshProUGUI MoaDisplay;

    public float minuteofAcc, dis1, dis2, dis3, dis4, dis5, dis6;

    public float groupUnity;
    public float groupInches;
    private List<GameObject> shotlist = new List<GameObject>();
    public Vector3 shot1, shot2, shot3, shot4;

    

    private void Update()
    {
        foreach (Transform child in target.transform)
        {
           if (child.childCount > 0)
            {
                foreach (Transform i in child.transform)
                {
                    if (i.CompareTag("BulletMark"))
                    {
                        if(shotlist.Count == 0)
                        {
                            shotlist.Add(i.gameObject);
                        }
                        if(shotlist.Count != 0)
                        {
                            bool isDifferent = false;

                            for (int j = 0; j < shotlist.Count; j++)
                            {
                                if (shotlist[j].GetInstanceID() == i.gameObject.GetInstanceID())
                                {
                                    isDifferent = false;
                                    break;
                                }
                                else
                                {
                                    isDifferent = true;
                                }

                            }

                            if( isDifferent == true)
                            {
                                shotlist.Add(i.gameObject);
                                isDifferent = false;
                            }
                        }
                    }
                }
            }

        }

        if(shotlist.Count >= 4)
        {
            MOACALC();
        }


    }
    public void MOACALC()
    {

        Vector3 shot1 = shotlist[0].transform.position;


        Vector3 shot2 = shotlist[1].transform.position;


        Vector3 shot3 = shotlist[2].transform.position;


        Vector3 shot4 = shotlist[3].transform.position;



        dis1 = Vector3.Distance(shot1, shot2);
        dis2 = Vector3.Distance(shot1, shot3);
        dis3 = Vector3.Distance(shot1, shot4);
        dis4 = Vector3.Distance(shot2, shot3);
        dis5 = Vector3.Distance(shot2, shot4);
        dis6 = Vector3.Distance(shot3, shot4);


        groupUnity = Mathf.Max(dis1, dis2, dis3, dis4, dis5, dis6);

        groupInches = groupUnity * 39.37f;

        minuteofAcc = 25f / 100f * groupInches;

        MoaDisplay.text = minuteofAcc.ToString("#.00");
    }

    
}
