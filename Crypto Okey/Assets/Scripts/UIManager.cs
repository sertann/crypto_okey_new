using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField] TextMeshProUGUI emailText;
    public GameObject emailSentPage;

    private void Awake()
    {
        if (instance == null) instance = this;
    }


    public void EmailSentNotification(string email)
    {
        emailText.text = email;
        emailSentPage.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
