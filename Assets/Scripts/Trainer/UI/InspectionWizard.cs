using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionWizard : MonoBehaviour
{
    enum WizardState { Intro, Question, Result }

    [Serializable]
    public class Question
    {
        public string text;
        public string[] answers;
        public int correct;
    }

    const string ResultKey = "SolarTrainer.InspectionResult";
    const int PassScore = 2;

    public TMP_Text stepLabel;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Toggle[] answerToggles;
    public TMP_Text[] answerLabels;
    public TMP_Text feedbackText;
    public Button backButton;
    public Button nextButton;
    public TMP_Text nextLabel;

    public Question[] questions =
    {
        new Question
        {
            text = "Що потрібно зробити перед обслуговуванням мережевого інвертора?",
            answers = new[] { "Розімкнути DC- та AC-роз'єднувачі й перевірити відсутність напруги", "Збільшити навантаження на інвертор", "Нічого, інвертор можна обслуговувати під напругою" },
            correct = 0
        },
        new Question
        {
            text = "За якими ознаками виявляють пошкоджену фотоелектричну панель?",
            answers = new[] { "За кольором алюмінієвої рами", "За тріщинами на склі, потемнінням комірок і слідами перегріву", "Лише за вагою панелі" },
            correct = 1
        },
        new Question
        {
            text = "Що означає червоний індикатор Fault на інверторі?",
            answers = new[] { "Інвертор працює в нормальному режимі", "Іде заряд акумулятора", "Аварія або помилка: потрібна діагностика" },
            correct = 2
        }
    };

    WizardState _state;
    int _index;
    int[] _selected;

    void Awake()
    {
        backButton.onClick.AddListener(Back);
        nextButton.onClick.AddListener(Next);
        for (int i = 0; i < answerToggles.Length; i++)
        {
            answerToggles[i].onValueChanged.AddListener(_ => feedbackText.text = "");
        }
    }

    void OnEnable()
    {
        if (_selected == null)
            Restart();
    }

    public void Restart()
    {
        _selected = new int[questions.Length];
        for (int i = 0; i < _selected.Length; i++)
            _selected[i] = -1;
        _index = 0;
        Enter(WizardState.Intro);
    }

    void Enter(WizardState state)
    {
        _state = state;
        feedbackText.text = "";
        int totalSteps = questions.Length + 2;

        switch (state)
        {
            case WizardState.Intro:
                stepLabel.text = $"Крок 1 з {totalSteps}";
                titleText.text = "Інструктаж монтажника СЕС";
                string last = PlayerPrefs.GetString(ResultKey, "");
                bodyText.text = "Дайте відповіді на запитання з безпеки монтажу та огляду обладнання. " +
                                "Стан панелей можна переглянути, навівши на них промінь." +
                                (last.Length > 0 ? $"\nОстанній результат: {last}" : "");
                SetAnswersVisible(false);
                backButton.gameObject.SetActive(false);
                nextLabel.text = "Почати";
                break;

            case WizardState.Question:
                var q = questions[_index];
                stepLabel.text = $"Крок {_index + 2} з {totalSteps}";
                titleText.text = $"Питання {_index + 1}";
                bodyText.text = q.text;
                SetAnswersVisible(true);
                for (int i = 0; i < answerToggles.Length; i++)
                {
                    answerLabels[i].text = q.answers[i];
                    answerToggles[i].SetIsOnWithoutNotify(_selected[_index] == i);
                }
                backButton.gameObject.SetActive(true);
                nextLabel.text = _index == questions.Length - 1 ? "Завершити" : "Далі";
                break;

            case WizardState.Result:
                ShowResult();
                break;
        }
    }

    void Next()
    {
        switch (_state)
        {
            case WizardState.Intro:
                _index = 0;
                Enter(WizardState.Question);
                break;

            case WizardState.Question:
                int choice = SelectedAnswer();
                if (choice < 0)
                {
                    feedbackText.text = "<color=#FF6B6B>Оберіть варіант відповіді.</color>";
                    return;
                }
                _selected[_index] = choice;
                if (_index < questions.Length - 1)
                {
                    _index++;
                    Enter(WizardState.Question);
                }
                else
                {
                    Enter(WizardState.Result);
                }
                break;

            case WizardState.Result:
                Restart();
                break;
        }
    }

    void Back()
    {
        if (_state != WizardState.Question)
            return;

        int choice = SelectedAnswer();
        if (choice >= 0)
            _selected[_index] = choice;

        if (_index == 0)
        {
            Enter(WizardState.Intro);
        }
        else
        {
            _index--;
            Enter(WizardState.Question);
        }
    }

    void ShowResult()
    {
        int score = 0;
        var details = new StringBuilder();
        for (int i = 0; i < questions.Length; i++)
        {
            bool ok = _selected[i] == questions[i].correct;
            if (ok)
                score++;
            details.Append(ok ? "<color=#7CFC9A>+</color> " : "<color=#FF6B6B>-</color> ")
                   .Append($"Питання {i + 1}")
                   .Append(ok ? "\n" : $": правильно — «{questions[i].answers[questions[i].correct]}»\n");
        }

        bool passed = score >= PassScore;
        string result = $"{score}/{questions.Length} ({(passed ? "зараховано" : "не зараховано")}), {DateTime.Now:dd.MM HH:mm}";
        PlayerPrefs.SetString(ResultKey, result);
        PlayerPrefs.Save();

        stepLabel.text = $"Крок {questions.Length + 2} з {questions.Length + 2}";
        titleText.text = passed ? "Тест пройдено" : "Тест не пройдено";
        bodyText.text = $"Правильних відповідей: {score} з {questions.Length}\n{details}";
        SetAnswersVisible(false);
        backButton.gameObject.SetActive(false);
        nextLabel.text = "Пройти знову";
    }

    int SelectedAnswer()
    {
        for (int i = 0; i < answerToggles.Length; i++)
        {
            if (answerToggles[i].isOn)
                return i;
        }
        return -1;
    }

    void SetAnswersVisible(bool visible)
    {
        foreach (var toggle in answerToggles)
        {
            toggle.SetIsOnWithoutNotify(false);
            toggle.gameObject.SetActive(visible);
        }
    }
}
