using System.Collections.Generic;
using UnityEngine;

public class Diary : MonoBehaviour
{
    [SerializeField] private NoteSlot noteSlotObject;
    [SerializeField] private GameObject notesGroup;
    [SerializeField] private NoteContent noteContent;

    private List<NoteSlot> notesSlots;

    void Start()
    {
        noteContent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (notesSlots == null)
        {
            notesSlots = new List<NoteSlot>();
        }

        for (var i = 0; i < notesSlots.Count; i++)
        {
            notesSlots[i].Selected = false;
        }
    }

    private void OnDisable()
    {
        var noteSlot = notesSlots.Find(lambaExpression => lambaExpression.Selected);
        if (noteSlot != null)
            noteSlot.Selected = false;
        noteContent.Reset();
    }

    private void Update()
    {
        var noteSlot = notesSlots.Find(lambaExpression => lambaExpression.Selected);
        if (noteSlot != null && noteContent.Note == null)
        {
            noteContent.gameObject.SetActive(true);
            noteContent.FillContent(noteSlot);

            notesGroup.gameObject.SetActive(false);
        }
        else if (noteSlot == null)
        {
            notesGroup.gameObject.SetActive(true);
        }
    }

    public void TakeNote(CollectibleNote note)
    {
        if (note != null)
        {
            var noteSlotGameObject = Instantiate(noteSlotObject.gameObject, notesGroup.transform);
            var noteSlotRectTransform = noteSlotGameObject.GetComponent<RectTransform>();

            if (notesSlots.Count % 2 == 0)

            {
                noteSlotRectTransform.anchoredPosition = new Vector2(noteSlotRectTransform.anchoredPosition.x,
                    (noteSlotRectTransform.anchoredPosition.y < 0)
                        ? noteSlotRectTransform.anchoredPosition.y +
                          -(noteSlotRectTransform.sizeDelta.y * notesSlots.Count / 2)
                        : noteSlotRectTransform.anchoredPosition.y +
                          (noteSlotRectTransform.sizeDelta.y * notesSlots.Count / 2));
            }
            else
            {
                noteSlotRectTransform.anchoredPosition = new Vector2(
                    noteSlotRectTransform.anchoredPosition.x + noteSlotRectTransform.sizeDelta.x,
                    (noteSlotRectTransform.anchoredPosition.y < 0)
                        ? noteSlotRectTransform.anchoredPosition.y +
                          -(noteSlotRectTransform.sizeDelta.y * (notesSlots.Count - 1) / 2)
                        : noteSlotRectTransform.anchoredPosition.y +
                          (noteSlotRectTransform.sizeDelta.y * (notesSlots.Count - 1) / 2)
                );
            }


            var noteSlot = noteSlotGameObject.GetComponent<NoteSlot>();

            noteSlot.FillNote(note);
            notesSlots.Add(noteSlot);
        }
    }
}