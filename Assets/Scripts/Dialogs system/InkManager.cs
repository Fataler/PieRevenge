using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.Runtime;
using System.Collections;
using TMPro;

public class InkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvas;

	[SerializeField]
	public Story _story;
    private TextAsset _newStory;

	[SerializeField]
	private TextMeshProUGUI _textField = null;
    [SerializeField]
    private VerticalLayoutGroup _choiceButtonContainer;
    [SerializeField]
    private Button _choiceButtonPrefab;

    public bool _blockInteractions = false;

    [SerializeField]
    public TextMeshProUGUI _nameField = null;

    [SerializeField]
    private TextWriterEffect _textWriter;

    //���������� ����� �� NPC, ������ �������
    public void NewStory(TextAsset text)
    {
        _newStory = text;
        StartStory(_newStory);
    }

    // ������ �������, ����������� ������ �������
    private void StartStory(TextAsset text)
    {
        _story = new Story(text.text);
        DisplayNextLine();
    }

    // ����������� ��������� ������
    public void DisplayNextLine()
    {
        if (_story.canContinue)
        {
            string text = _story.Continue();
            // �������� ������ ��������, ���� ����
            text = text?.Trim();
            _textWriter.AddWriter(_textField, text, .1f);
        }

        if (_story.currentChoices.Count > 0)
        {
            Invoke("DisplayChoices", _textWriter._textToWrite.Length / 10f);
        }

        else
        {
            _textField.text = null;
            // ���� ����� �������, �� ������� ������
            gameObject.SetActive(false);
            // ������������� ����������� ���������� ��������� ������ ������ ������ �������
            _blockInteractions = false;
        }
    }

    // �������� ����� �������
    public void DisplayChoices()
    {
        // ������� ���� ��������� ������
        for (int i = 0; i < _story.currentChoices.Count; i++)
        {
            Choice choice = _story.currentChoices[i];
            // �������� ������ � �������
            Button button = CreateChoiceButton(choice.text.Trim());
            // ������� ������, ��� ������, ���� �� ��� ������
            button.onClick.AddListener(() => OnClickChoiceButton(choice));
        }
    }

    // �������� ������, ������������ ������� ������
    Button CreateChoiceButton(string text)
    {
        //�������� ������ ������ �� �������
        Button choiceButton = Instantiate(_choiceButtonPrefab);
        choiceButton.transform.SetParent(_choiceButtonContainer.transform, false);

        //��������� ������ �� ������ �� �������
        TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;

        return choiceButton;
    }

    // ����� �������� �� ������ ������, ������� ����, ��� ��������� �������
    private void OnClickChoiceButton(Choice choice)
    {
        _story.ChooseChoiceIndex(choice.index);
        // ������� �������� ������ � ������
        RefreshChoiceView();
        _story.Continue();
        DisplayNextLine();
    }

    // ������� �������� ������ � ������
    private void RefreshChoiceView()
    {
        if (_choiceButtonContainer != null)
        {
            foreach (Button button in _choiceButtonContainer.GetComponentsInChildren<Button>())
            {
                Destroy(button.gameObject);
            }
        }
    }
}
