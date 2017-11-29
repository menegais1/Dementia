using UnityEngine;

public class CollectibleNote : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;

    [SerializeField] private string noteName;
    [SerializeField] private TextAsset noteContent;

    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public PlayerStatusVariables PlayerStatusVariables
    {
        get { return playerStatusVariables; }
        set { playerStatusVariables = value; }
    }

    public string NoteName
    {
        get { return noteName; }
        set { noteName = value; }
    }

    public TextAsset NoteContent
    {
        get { return noteContent; }
        set { noteContent = value; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeNote = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        playerStatusVariables.canTakeNote = false;
    }

    public void DestroyNote()
    {
        Destroy(this.gameObject);
    }
}