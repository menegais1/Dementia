using System;
using UnityEngine;
using UnityEngine.UI;

public class NoteSlot : MonoBehaviour
{
    private Button buttonNote;
    [SerializeField] private Text textNoteName;
    private string noteContent;
    private bool selected;

    public Button ButtonNote
    {
        get { return buttonNote; }
        set { buttonNote = value; }
    }

    public Text TextNoteName
    {
        get { return textNoteName; }
        set { textNoteName = value; }
    }

    public String NoteContent
    {
        get { return noteContent; }
        set { noteContent = value; }
    }

    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }

    void Start()
    {
        buttonNote = GetComponent<Button>();
        buttonNote.onClick.AddListener(OnSelectNote);
    }

    private void OnSelectNote()
    {
        selected = true;
    }

    public void FillNote(CollectibleNote note)
    {
        noteContent = note.NoteContent.text;
        textNoteName.text = note.NoteName;
    }
}