using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class NoteContent : MonoBehaviour
{
    [SerializeField] private Text textContent;
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button lastPageButton;
    [SerializeField] private NoteSlot note;

    private int pageCount;
    private string[] pages;
    private int currentPage;
    private string noteText;

    public NoteSlot Note
    {
        get { return note; }
        set { note = value; }
    }

    private void Start()
    {
        backButton.onClick.AddListener(BackButton);
        nextPageButton.onClick.AddListener(NextPageButton);
        lastPageButton.onClick.AddListener(LastPageButton);
    }

    private void BackButton()
    {
        note.Selected = false;
        note = null;
        textContent.text = "";
        gameObject.SetActive(false);
    }

    private void NextPageButton()
    {
        if (pageCount > 1)
        {
            currentPage++;
            if (currentPage == pageCount - 1)
            {
                nextPageButton.gameObject.SetActive(false);
            }
            lastPageButton.gameObject.SetActive(true);

            textContent.text = pages[currentPage];
        }
    }

    private void LastPageButton()
    {
        if (currentPage > 0)
        {
            currentPage--;
            if (currentPage == 0)
            {
                lastPageButton.gameObject.SetActive(false);
            }
            nextPageButton.gameObject.SetActive(true);
            textContent.text = pages[currentPage];

        }
    }

    public void FillContent(NoteSlot note)
    {
        this.note = note;
        noteText = note.NoteContent;
        pages = noteText.Split('|');
        pageCount = pages.Length;
        currentPage = 0;

        textContent.text = pages[currentPage];


        lastPageButton.gameObject.SetActive(false);

        if (pageCount == 1)
        {
            nextPageButton.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        this.note = null;
        pages = new string[0];
        noteText = "";
        pageCount = 1;
        currentPage = 0;
        textContent.text = "";
        lastPageButton.gameObject.SetActive(true);
        nextPageButton.gameObject.SetActive(true);
    }
}