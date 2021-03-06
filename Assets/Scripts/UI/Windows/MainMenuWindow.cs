﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MonoBehaviour
{
    public Button closeButton;
    public CanvasGroup uiCanvasGroup;

    public event WindowEvent onWindowClose;

    public bool IsOpened
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        DarkestDungeonManager.GamePaused = true;
        uiCanvasGroup.blocksRaycasts = false;
    }

    public void WindowClosed()
    {
        DarkestDungeonManager.GamePaused = false;
        gameObject.SetActive(false);

        if (onWindowClose != null)
            onWindowClose();
        uiCanvasGroup.blocksRaycasts = true;
    }

    public void ReturnToCampaignSelection()
    {
        if(SceneManager.GetActiveScene().name == "DungeonMultiplayer")
        {
            WindowClosed();
            RaidSceneManager.Instanse.AbandonButtonClicked();
            return;
        }
        else if(SceneManager.GetActiveScene().name == "EstateManagement")
        {
            EstateSceneManager.Instanse.OnSceneLeave();
            DarkestDungeonManager.SaveData.UpdateFromEstate();
            DarkestDungeonManager.Instanse.SaveGame();
        }
        else if (SceneManager.GetActiveScene().name == "Dungeon")
        {
            if (!RaidSceneManager.HasAnyEvents)
            {
                DarkestDungeonManager.SaveData.UpdateFromRaid();
                DarkestDungeonManager.Instanse.SaveGame();
            }
            RaidSceneManager.Instanse.OnSceneLeave();
        }
        DarkestSoundManager.SilenceNarrator();
        SceneManager.LoadScene("CampaignSelection");
        WindowClosed();
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name == "DungeonMultiplayer")
        {
            RaidSceneManager.Instanse.OnSceneLeave();
            PhotonGameManager.Instanse.LeaveRoom();
            WindowClosed();
            return;
        }
        else if (SceneManager.GetActiveScene().name == "EstateManagement")
        {
            EstateSceneManager.Instanse.OnSceneLeave();
            DarkestDungeonManager.SaveData.UpdateFromEstate();
            DarkestDungeonManager.Instanse.SaveGame();
        }
        else if (SceneManager.GetActiveScene().name == "Dungeon")
        {
            if(!RaidSceneManager.HasAnyEvents)
            {
                DarkestDungeonManager.SaveData.UpdateFromRaid();
                DarkestDungeonManager.Instanse.SaveGame();
            }
            RaidSceneManager.Instanse.OnSceneLeave();
        }
        DarkestSoundManager.SilenceNarrator();
        Application.Quit();
    }
}