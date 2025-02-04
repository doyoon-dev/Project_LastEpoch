using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
  public void StartGame()
    {
        SoundManager.Inst.StopBGMSound();
        SceneManager.LoadScene("Modular Dungeon");
    }
}
