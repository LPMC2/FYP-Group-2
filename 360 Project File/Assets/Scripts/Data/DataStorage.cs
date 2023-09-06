using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataSO", menuName = "ScriptableObjects/DataSO")]
public class DataStorage : ScriptableObject
{
    public string data;
    public string fileName = "data.txt";
    
    public int userCount = 0;
    public QuizSO quizSO;
    public string[] questions = new string[0];
    public int[] optionsNum = new int[0];
    public string[] options = new string[0];
    public string[] userInput = new string[0];
    public string[] answers = new string[0];
    public void writeData()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        userCount++;
        data += "User" + userCount + " - Correct: " + quizSO.correctCount + "\n";
        for (int i = 0; i < quizSO.questions.Length; i++)
        {
            //Load Question and store into the string
            string questionStr = "";
            for (int q = 0; q < quizSO.questions[i].question.Length; q++)
            {
                if (quizSO.questions[i].question[q].language == quizSO.language)
                {
                    questionStr = quizSO.questions[i].question[q].text;
                    questions = arrayBehaviour.addArray(questions, quizSO.questions[i].question[q].text);
                }
            }
            //Check if the answer is correct and store into the string
            string boolCorrect = "";
            switch (quizSO.questions[i].answer == quizSO.questions[i].inputAnswer)
            {
                case true:
                    boolCorrect = "Correct";
                    break;

                case false:
                    boolCorrect = "Incorrect";
                    break;
            }
            //Check if the user have input, Load Answers and store into the string
            string optionsStr = "";
            //user input
            string optionStr = "";
            int optionsNumber = 0;
            //To store available options to answer
            for (int j = 0; j < quizSO.questions[i].options.Length; j++)
            {
                if (quizSO.questions[i].options[j].language == quizSO.language)
                {
                    optionsStr += " - " + quizSO.questions[i].options[j].text + "\n";
                    options = arrayBehaviour.addArray(options, quizSO.questions[i].options[j].text);
                    optionsNumber++;
                    if (quizSO.questions[i].options[j].id == quizSO.questions[i].inputAnswer)
                    {
                        optionStr = quizSO.questions[i].options[j].text;
                        userInput = arrayBehaviour.addArray(userInput, quizSO.questions[i].options[j].text);
                    }
                    if (quizSO.questions[i].options[j].id == quizSO.questions[i].answer)
                    {
                        answers = arrayBehaviour.addArray(answers, quizSO.questions[i].options[j].text);
                    }
                }
            }
            optionsNum = arrayBehaviour.addIntArray(optionsNum, optionsNumber);
            if (quizSO.questions[i].inputAnswer != -1)
            {
                //Store into the main data string
                data += "> Question " + (i + 1) + ": " + questionStr + "\nAvailable answers: \n" + optionsStr + "\nUser Answered: " + optionStr + "(" + boolCorrect + ")\n";
            } else
            {
                //Store into the main data string(Without user input)
                data += "> Question " + (i + 1) + "\nAvailable answers: " + optionsStr + "\nUser Answered: Nothing\n";
            }
        }
        data += "------------------------------\n";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(data);
        }

    }
    //Get Question data
    public string getQuestion(int index)
    {

        return questions[index];
    }
    
}
