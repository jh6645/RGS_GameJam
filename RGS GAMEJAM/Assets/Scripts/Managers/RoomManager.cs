using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviour
{
    [Header("Ready")]
    [SerializeField] private Button hostStartBtn;
    [SerializeField] private TMP_Text clientReadyTxt;

    [Header("CharacterSelect")]
    [SerializeField] private GameObject charSelPanel;

    [Header("Roomsetting")]
    [SerializeField] private GameObject roomSetPanel;
    [SerializeField] private Toggle isPrivateToggle;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Slider playerAmountSlider;

    private void Start()
    {
        if (NetworkServer.active)
        {
            hostStartBtn.gameObject.SetActive(true);
            clientReadyTxt.gameObject.SetActive(false);
            
        }
        else
        {
            hostStartBtn.gameObject.SetActive(false);
            clientReadyTxt.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (((NetworkRoomManager)NetworkManager.singleton).allPlayersReady)
        {
            hostStartBtn.interactable = true;
        }
        else
        {
            hostStartBtn.interactable = false;
        }
    }

    public void OnPressedStartButton()
    {
        ((CustomNetworkRoomManager)NetworkManager.singleton).StartGame();
    }
    public void OnChangeReadyState(bool readyState)
    {
        if (readyState)
        {
            clientReadyTxt.text = "READY";
            clientReadyTxt.color = Color.blue;
        }
        else
        {
            clientReadyTxt.text = "Press SPACE to Ready";
            clientReadyTxt.color = Color.white;
        }
    }

}
