using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    

    private Data data = new Data();

    [SerializeField] GameEvents events = null;

    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    private List<int> FinishedQuestions = new List<int>();
    private int currentQuestion = 0;

    private IEnumerator IE_WaitUntilNextRound = null;
    

    [Header("Timer")]
    [SerializeField] Animator timerAnimator;
    [SerializeField] TextMeshProUGUI timerText;
    private Color timerDefaultColor = Color.white;
    [SerializeField] Color timerHalfWayColor = Color.yellow;
    [SerializeField] Color timerAlmostDone = Color.red;
    private IEnumerator IE_startTimer;
    private int timerStateParameter = 0;

    private bool IsFinished //si ya se acabaron las preguntas retorna true
    {
        get
        {
            return (FinishedQuestions.Count < data.Questions.Length ? false : true);
        }
    }
    #endregion

    // UNITY FUNCTIONS
    private void OnEnable()
    {
        events.UpdateQuestionAnswer += UpdateAnswers;
    }
    private void OnDisable()
    {

        events.UpdateQuestionAnswer -= UpdateAnswers;
    }
    private void Start()
    {
        events.StartupHighScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        timerDefaultColor = timerText.color;
        timerStateParameter = Animator.StringToHash("timerState");
        loadData();
        events.CurrentFinalScore = 0;

        var seed = UnityEngine.Random.Range(int.MinValue,int.MaxValue);
        UnityEngine.Random.InitState(seed);
        

        Display();
    }


    //MY FUNCTIONS

    public void UpdateAnswers(AnswerData newAnswer)
    {
        if (data.Questions[currentQuestion].Type == AnswerType.Single)
        {
            foreach(var answer in PickedAnswers)
            {
                if (answer != newAnswer)
                {
                    answer.Reset();
                }
            }
            PickedAnswers.Clear();
            PickedAnswers.Add(newAnswer);
        }
        else
        {
            bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
            if (alreadyPicked)
            {
                PickedAnswers.Remove(newAnswer);
            }
            else
            {
                PickedAnswers.Add(newAnswer);
            }
        }
    }

    //muestra una pregunta aleatoria en el UI y borra las respuestas anteriores.
    public void EraseAnswers()
    {
        PickedAnswers = new List<AnswerData>();
    }
    void Display()
    {
        EraseAnswers();
        var question = getRandomQuestion();
        if(events.UpdateQuestionUI!= null)
        {
            events.UpdateQuestionUI(question);
        }
        else
        {
            Debug.LogWarning("Error when trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null in GameManager.Display() method.");
        }
        if (question.UseTimer)
        {
            UpdateTimer(question.UseTimer);
        }
    }

    //revisa si se respondió correctamente
    public void Accept()
    {
        UpdateTimer(false);
        bool IsCorrect = CheckAnswers();
        FinishedQuestions.Add(currentQuestion);

        UpdateScore((IsCorrect) ? data.Questions[currentQuestion].AddScore : -data.Questions[currentQuestion].AddScore);
        if (IsFinished)
        {
            SetHighScore();
        }

        var type = (IsFinished) ? UIManager.ResolutionScreenType.Finish : (IsCorrect) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;

        if(events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, data.Questions[currentQuestion].AddScore);
        }

        if(IE_WaitUntilNextRound != null)
        {
            StopCoroutine(IE_WaitUntilNextRound);
        }
        IE_WaitUntilNextRound = WaitForNextRound();
        StartCoroutine(IE_WaitUntilNextRound);

    }
    IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }
    bool CheckAnswers()
    {
        if (!CompareAnswers())
        {
            return false;
        }
        return true;
    }
    bool CompareAnswers()
    {
        if (PickedAnswers.Count > 0)
        {
            List<int> CorrectAnswers = data.Questions[currentQuestion].GetCorrectAnswers();
            List<int> picked = PickedAnswers.Select(x => x.AnswerIndex).ToList();

            var f = CorrectAnswers.Except(picked).ToList();
            var s = picked.Except(CorrectAnswers).ToList();

            return !f.Any() && !s.Any(); //retorna true si f y s contienen elementos.
        }
        return false;
    }

    //para mostrar preguntas aleatorias
    Question getRandomQuestion()
    {
        var random = getRandomQuestionIndex();
        currentQuestion = random;
        return data.Questions[currentQuestion];

    }    
    int getRandomQuestionIndex()
    {
        var random = 0;
        if(FinishedQuestions.Count < data.Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, data.Questions.Length);
            } while (FinishedQuestions.Contains(random) || random==currentQuestion);
        }
        return random;

    }

    //Carga los datos desde el archivo xml
    void loadData()
    {
        data = Data.Cargar();
    }

    // Suma al highscore y actualiza el puntaje 
    private void UpdateScore(int add)
    {

        events.CurrentFinalScore += add;
        events.ScoreUpdated?.Invoke();

    }

    //para el temporizador
    void UpdateTimer(bool state)
    {
        switch (state)
        {
            case true:
                IE_startTimer = StartTimer();
                StartCoroutine(IE_startTimer);

                timerAnimator.SetInteger(timerStateParameter, 2); //play popup animation
                break;
            case false:
                if (IE_startTimer != null)
                {
                    StopCoroutine(IE_startTimer);
                }

                timerAnimator.SetInteger(timerStateParameter, 1);
                break;
                    
        }
    }
    IEnumerator StartTimer()
    {
        var totalTime = data.Questions[currentQuestion].Timer;
        var timeLeft = totalTime;

        timerText.color = timerDefaultColor;
        while (timeLeft > 0)
        {
            timeLeft--;
            if (timeLeft== totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = timerHalfWayColor;
            }
            if(timeLeft == totalTime / 4)
            {
                timerText.color = timerAlmostDone;
            }
            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }

    //carga el mayor puntaje entre jugadas
    private void SetHighScore()
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if(highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
        }
    }


    
    



}
