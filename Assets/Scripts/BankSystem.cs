using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankSystem : MonoBehaviour
{
    private int peachCounter = 0;

    private void Start()
    {
        EventManager.OnPeachUp += peachUp;
    }

    private void peachUp()
    {
        peachCounter++;
        Debug.Log("+1 peach");
        GetComponent<Text>().text = peachCounter.ToString();
    }
}