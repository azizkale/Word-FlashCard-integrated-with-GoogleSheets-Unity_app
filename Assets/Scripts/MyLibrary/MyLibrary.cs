﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyLibrary : MonoBehaviour
{
    public GameObject prefabWordsPairCard; 
    public GameObject prefabUpdateWord;
    public GameObject prefabCompleteSuccessfully;
    public GameObject prefabGeneralWarnung;
    public GameObject scrolContainer;
    public GameObject canvas;
    public GameObject libraryTitle;

   
    private Library theLibrary;
    private List<Word> listToManupulateWords = new List<Word>();   

    void Start()
    {
        createFileNamesCards();
    }

    public void createFileNamesCards()
    {
        theLibrary = Read.getLibraryContentWithoutArchive(CommonVariables.libraryName);
        libraryTitle.GetComponent<TextMeshProUGUI>().text = CommonVariables.charachterLimit(theLibrary.name, 14);

        foreach (Word word in theLibrary.words)
        {
           GameObject cloneprefabWordsPairCard = Instantiate(prefabWordsPairCard, scrolContainer.transform.position, Quaternion.identity, scrolContainer.transform) as GameObject;

            //TMP_Question
            TextMeshProUGUI questionOnPrefab = cloneprefabWordsPairCard.transform.Find("Button").transform.Find("TMP_question").GetComponent<TextMeshProUGUI>();
            questionOnPrefab.text = CommonVariables.charachterLimit(word.theWord,30);

            //TMP_Answer
            TextMeshProUGUI answerOnPrefab = cloneprefabWordsPairCard.transform.Find("Button").transform.Find("TMP_answer").GetComponent<TextMeshProUGUI>();
            answerOnPrefab.text = CommonVariables.charachterLimit(word.meaning,30);

            //Toggle on the word-pair ()
            cloneprefabWordsPairCard.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener((val) => {
                listToManupulateWords.Add(word);
            });

            //the word-pair (select BUTTON)
            cloneprefabWordsPairCard.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => {

                //Update Prefab
                GameObject cloneprefabupdateword = Instantiate(prefabUpdateWord, canvas.transform.position, Quaternion.identity, canvas.transform);
                cloneprefabupdateword.transform.localPosition = Vector3.zero;

                //view Count info
                TextMeshProUGUI textViewCount = cloneprefabupdateword.transform.Find("TopInfo").transform.Find("ViewCount").GetComponent<TextMeshProUGUI>();
                textViewCount.text = word.viewCount.ToString();

                //language info
                TextMeshProUGUI textLanguage = cloneprefabupdateword.transform.Find("TopInfo").transform.Find("language").GetComponent<TextMeshProUGUI>();
                textLanguage.text = word.languageFrom + "-" + word.languageTo;

                //question text
                TMP_InputField inputQ = cloneprefabupdateword.transform.Find("TextField").transform.Find("TMPQuestion").GetComponent<TMP_InputField>();
                inputQ.text = word.theWord;

                //answer text
                TMP_InputField inputA = cloneprefabupdateword.transform.Find("TextField").transform.Find("TMPAnswer").GetComponent<TMP_InputField>();
                inputA.text = word.meaning;

                //UPDATE button
                cloneprefabupdateword.transform.Find("BottomMenu").transform.Find("btnUpdate").GetComponent<Button>().onClick.AddListener(()=> {
                    updateTheWord(word, canvas, cloneprefabupdateword, inputA, inputQ, cloneprefabWordsPairCard);
                });

                //CANCEL button
                cloneprefabupdateword.transform.Find("BottomMenu").transform.Find("btnCancel").GetComponent<Button>().onClick.AddListener(() => {
                    DestroyImmediate(cloneprefabupdateword);
                });

            });
        }
    }

    public void backToMyLibraryNames()
    {
        SceneManager.LoadScene("MyLibraryNames");
    }

    public void startLearning()
    {

    }

    private void updateTheWord(Word word, GameObject canvas, GameObject cloneprefabupdateword, TMP_InputField inputA, TMP_InputField inputQ, GameObject cloneprefabWordsPairCard)
    {
        if (!string.IsNullOrEmpty(inputQ.text) && !string.IsNullOrEmpty(inputA.text))
        {
            Update.updateSingleWord(word, theLibrary, inputQ.text, inputA.text); 
            DestroyImmediate(cloneprefabupdateword);
            alertWarning.completedSuccesfully(prefabCompleteSuccessfully, canvas);

            //renew the on the screen
            cloneprefabWordsPairCard.transform.Find("Button").transform.Find("TMP_question").GetComponent<TextMeshProUGUI>().text = inputQ.text;
            
            cloneprefabWordsPairCard.transform.Find("Button").transform.Find("TMP_answer").GetComponent<TextMeshProUGUI>().text = inputA.text;
        }
        else
            alertWarning.generalWarning(prefabGeneralWarnung, canvas, "Question and answer fields cannot be left blank!");
    }

    public void deleteTheSelectedWords()
    {
        foreach (Word word in listToManupulateWords)
        {
            Delete.deleteSingleWord(word, theLibrary);
        }
        //reload the scene after deleting
        SceneManager.LoadScene("LibraryContent");
    }

}
