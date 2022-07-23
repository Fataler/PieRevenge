using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISystem : MonoBehaviour
{
    [SerializeField] private HealthBehavior playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        // �������� � 10 �������� � ���. ������������ ������ ������ � ����������� �� ������������� ���������� �������� ������
        totalHealthBar.fillAmount = playerHealth.StartingHealthPlayer / 10;
    }
    private void Update()
    {
        // �������� � 10 �������� � ���. ������������ ������ ������ � ����������� �� �������� ���������� �������� ������
        currentHealthBar.fillAmount = playerHealth.CurrentHealthPlayer / 10;
    }
}
