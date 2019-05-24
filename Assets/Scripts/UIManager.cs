using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Answer Options")]
    [SerializeField] float margins;
    public float Margins { get { return margins; } }

    [Header("Resolution Screen Options")]
    [SerializeField] Color correctBGColor;
    public Color CorrectBGColor { get { return correctBGColor; } }

    [SerializeField] Color incorrectBGColor;
    public Color IncorrectBGColor { get { return incorrectBGColor; } }

    [SerializeField] Color finalBGColor;
    public Color FinalBGColor { get { return finalBGColor; } }
}

[Serializable()]
public struct UIElements
{
    [SerializeField] RectTransform answersContentArea;
    public RectTransform AnswerContentArea { get { return answersContentArea; } }

    [SerializeField] TextMeshProUGUI questionInfoTextObject;
    public TextMeshProUGUI QuestionInfoTextOject { get { return questionInfoTextObject; } }

    [SerializeField] TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText { get { return scoreText; } }

    [Space]
    [SerializeField] Image resolutionBackground;
    public Image ResolutionBackground { get { return resolutionBackground; } }

    [SerializeField] TextMeshProUGUI resolutionStateInfoText;
    public TextMeshProUGUI ResolutionStateInfoText { get { return resolutionStateInfoText; } }
    
    [SerializeField] TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText { get { return resolutionScoreText; } }
    

    [Space]
    [SerializeField] TextMeshProUGUI highscoreText;
    public TextMeshProUGUI HighscoreText { get { return highscoreText; } }

    [SerializeField] CanvasGroup mainCanvas;
    public CanvasGroup MainCanvas { get { return mainCanvas; } }

    [SerializeField] RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements; } }

    [Space]
    [SerializeField] Animator resScreenAnimator;
    public Animator ResScreenAnimator { get { return resScreenAnimator; } }

}

public class UIManager : MonoBehaviour
{
    public enum ResolutionScreenType { Correct, Incorrect, Finish }

    [Header("References")]
    [SerializeField] GameEvents events;

    [Header("UI Elements (prefabs)")]
    [SerializeField] AnswerData answerPrefab;

    [SerializeField] UIElements uIElements;

    [SerializeField] UIManagerParameters parameters;

    List<AnswerData> currentAnswer = new List<AnswerData>();

    private int resStateParameter = 0;
    private IEnumerator IE_DisplayTimeResolution;

    private void Start()
    {
        resStateParameter = Animator.StringToHash("ScreenState");
    }

    private void OnEnable()
    {
        events.UpdateQuestionUI += updateQuestionUI;
        events.DisplayResolutionScreen += DisplayResolution;
    }
    private void OnDisable()
    {
        events.UpdateQuestionUI -= updateQuestionUI;
        events.DisplayResolutionScreen -= DisplayResolution;
    }

    void updateQuestionUI(Question question)
    {
        uIElements.QuestionInfoTextOject.text = question.Info;
        createAnswers(question);
    }

    void createAnswers(Question question)
    {
        eraseAnswers();
        float offset = 0 - parameters.Margins;
        for (int i = 0; i < question.Answers.Length; i++)
        {
            AnswerData newAnswer = (AnswerData)Instantiate(answerPrefab, uIElements.AnswerContentArea);
            newAnswer.UpdateData(question.Answers[i].Info, i);

            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uIElements.AnswerContentArea.sizeDelta = new Vector2(uIElements.AnswerContentArea.sizeDelta.x, offset * -1);

            currentAnswer.Add(newAnswer);
        }
    }

    void eraseAnswers()
    {
        foreach (var answer in currentAnswer)
        {
            Destroy(answer.gameObject);
        }
        currentAnswer.Clear();
    }

    void DisplayResolution(ResolutionScreenType type, int score)
    {
        UpdateResUI(type, score);
        uIElements.ResScreenAnimator.SetInteger(resStateParameter, 2); //display popup animation
        uIElements.MainCanvas.blocksRaycasts = false; //recieve no imputs while animation is playing

        if(type!= ResolutionScreenType.Finish)
        {
            if(IE_DisplayTimeResolution != null)
            {
                StopCoroutine(IE_DisplayTimeResolution);
            }
            IE_DisplayTimeResolution = DisplayTimeRes();
            StartCoroutine(IE_DisplayTimeResolution);
        }
    }
    IEnumerator DisplayTimeRes()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        uIElements.ResScreenAnimator.SetInteger(resStateParameter, 1); // fade out
        uIElements.MainCanvas.blocksRaycasts = true; //recieve inputs when res screen fades out

    }

    void UpdateResUI(ResolutionScreenType type, int score)
    {
        var highScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        switch (type)
        {
            case ResolutionScreenType.Correct:
                uIElements.ResolutionBackground.color = parameters.CorrectBGColor;
                uIElements.ResolutionStateInfoText.text = "CORRECT!";
                uIElements.ResolutionScoreText.text = "+" + score;
                break;
            case ResolutionScreenType.Incorrect:
                uIElements.ResolutionBackground.color = parameters.IncorrectBGColor;
                uIElements.ResolutionStateInfoText.text = "INCORRECT!";
                uIElements.ResolutionScoreText.text = "-" + score;
                break;
            case ResolutionScreenType.Finish:
                uIElements.ResolutionBackground.color = parameters.FinalBGColor;
                uIElements.ResolutionStateInfoText.text = "Final Score";

                StartCoroutine(CalculateScore());
                uIElements.FinishUIElements.gameObject.SetActive(true);
                uIElements.HighscoreText.gameObject.SetActive(true);
                uIElements.HighscoreText.text = ((highScore) > events.StartupHighScore ? "<color=yellow>New Highscore: </color>" + highScore : string.Empty + "Highscore: " + highScore);
                break;
        }

    }
    IEnumerator CalculateScore()
    {
        var scoreValue = 0;
        while(scoreValue < events.CurrentFinalScore)
        {
            scoreValue++;
            uIElements.ResolutionScoreText.text = scoreValue.ToString();

            yield return null;
        }
    }

}
