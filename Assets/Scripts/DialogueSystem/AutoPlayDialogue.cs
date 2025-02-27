using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayDialogue : MonoBehaviour
{
    void Start()
    {
        DialogueManager.Instance.PlayStream(GetComponent<DialogueStream>());
    }
}
