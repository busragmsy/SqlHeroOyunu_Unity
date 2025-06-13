using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public int questionNo;
    public string type;
    public string question;
    public List<string> options;
    public List<string> orderWords;
    public List<string> answer;
    public string answerString;
}

[System.Serializable]
public class Level
{
    public int levelId;
    public List<Question> questions;
}

[System.Serializable]
public class QuestionData
{
    public List<Level> levels;
}
