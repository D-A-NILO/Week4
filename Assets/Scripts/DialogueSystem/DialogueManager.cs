using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueContainer;
    [SerializeField]
    private TMP_Text dialogueOut;
    [SerializeField]
    private TMP_Text nameOut;
    [SerializeField]
    private SpriteRenderer iconOut;

    [Tooltip("The prefab used to create option buttons, should contain button component on highest level and a text componenet in child")]
    public GameObject optionButtonPrefab;
    private GameObject[] activeButtons;
    [Tooltip("bounding box for buttons to be packed in")]
    public RectTransform buttonBounds;

    [Tooltip("Input Action used to continue to next dialogue")]
    public InputAction continueAction;

    public bool clickToContinue = true;
    private bool canContinue;

    public static DialogueManager Instance;

    private DialogueStream activeStream;
    private Coroutine activeRoutine;

    #region Enable/Disable continueAction

    private void OnEnable()
    {
        continueAction.Enable();
        continueAction.performed += (ContinueAction);
    }

    private void OnDisable()
    {
        continueAction.Disable();
    }

    private void ContinueAction(InputAction.CallbackContext context)
    {
        if (clickToContinue)
        {
            Continue();
        }
    }

    public void Continue()
    {
        canContinue = true;
    }

    #endregion

    private void Awake()
    {
        if (Instance != null) // get singletoned
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    public void showUI()
    {
        dialogueContainer.SetActive(true);
    }

    public void hideUI()
    {
        dialogueContainer.SetActive(false);
    }

   

    /// <summary>
    /// Plays a dialogue Stream, deos not require WaitUntilDialogueContinue to be called
    /// </summary>
    /// <param name="dialogueStream">The dialogue stream to be played</param>
    public void PlayStream(DialogueStream dialogueStream)
    {
        if(dialogueStream == null)
        {
            Debug.LogError("Cannot Play Unassigned Dialogue Stream");
            return;
        }
        if(activeRoutine != null)
        {
            StopActiveStream();
        }
        activeStream = dialogueStream;
        activeRoutine = StartCoroutine(RunStream());
        showUI();
    }

    /// <summary>
    /// Plays only a single piece of dialogue, WaitUntilDialogueContinue should be used after this to function as intended
    /// </summary>
    /// <param name="dialogue">The dialogue to be played</param>
    public void PlaySingle(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogError("Cannot Play Unassigned Dialogue");
            return;
        }
        if (activeRoutine != null)
        {
            StopActiveStream();
        }
        packDialogue(dialogue);
        showUI();
    }


    public void StopActiveStream()
    {
        StopCoroutine(activeRoutine);
        activeRoutine = null;
        activeStream = null;
        hideUI();
    }

    private IEnumerator RunStream()
    {
        foreach (Dialogue dialogue in activeStream.dialogue)
        {
            packDialogue(dialogue);
            yield return WaitUntilDialogueContinue();
            dialogue.OnDialogueComplete.Invoke();
        }

        if(activeStream == null)
            yield break;
        
        DialogueStream nextStream;
        if(activeStream.nextStream == null)
            nextStream = null;
        else
            nextStream = activeStream.nextStream;


        activeStream = null;
        activeRoutine = null;

        if (nextStream != null)
        {
            PlayStream(nextStream);
        }else
        {
            hideUI();
        }
    }

    /// <summary>
    /// Packs a dialogue's content to the applicable components, WaitUntilDialogueContinue should be used after this to function as intended
    /// </summary>
    /// <param name="dialogue">The dialogue to be packed</param>
    private void packDialogue(Dialogue dialogue)
    {
        if (nameOut != null && dialogue.name != null) nameOut.text = dialogue.name;

        if (dialogueOut != null && dialogue.text != null) dialogueOut.text = dialogue.text;

        if (iconOut != null && dialogue.icon != null) iconOut.sprite = dialogue.icon;

        //destroy old buttons
        destroyButtons();

        clickToContinue = dialogue.clickToContinue;

        if (dialogue.options.Length > 0)
        {
            // sets up button gameobjects and components
            packOptionButtons(dialogue.options, dialogue.layout);
        }
    }


    private void destroyButtons()
    {
        if (activeButtons != null)
        {
            foreach (GameObject buttonObj in activeButtons)
            {
                Destroy(buttonObj);
            }

            activeButtons = null;
        }
    }

    private void packOptionButtons(Dialogue.DialogueOption[] options, Dialogue.Layout layout = Dialogue.Layout.Horizontal)
    {
        int count = options.Length;
        activeButtons = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            if(buttonBounds == null)
                Debug.LogError("button bounds not assigned");

            activeButtons[i] = Instantiate(optionButtonPrefab, buttonBounds);

            switch (layout)
            {
                case Dialogue.Layout.Horizontal:

                    //streatch button to fit bounds
                    RectTransform buttonTrans = activeButtons[i].GetComponent<RectTransform>();
                    buttonTrans.anchorMin = new Vector2((float)i /count, buttonTrans.anchorMin.y);
                    buttonTrans.anchorMax = new Vector2((float)(i+1)/count, buttonTrans.anchorMax.y);
                    break;
                default:
                    Debug.LogError(layout.ToString() + " layout not implemented");
                    break;
            }

            //change button text
            activeButtons[i].GetComponentInChildren<TMP_Text>().text = options[i].optionText;

            //add listeners to button
            Button button = activeButtons[i].GetComponent<Button>();
            button.onClick.AddListener(options[i].Invoke);

            //change stream only if one is available, otherwise continue after choice made
            if (options[i].optionStream == null)
            {
                button.onClick.AddListener(() => canContinue = true);
            }
        }
    }

    /// <summary>
    /// modifys the dialogue text without changing any other dialgue settings
    /// </summary>
    /// <param name="text">changes the current text to this string</param>
    public void ModifyText(string text)
    {
        dialogueOut.text = text;
    }

    public IEnumerator WaitUntilDialogueContinue()
    {
        yield return new WaitUntil(() => canContinue);
        canContinue = false;
    }


}
