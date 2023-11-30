using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu
{
    private float previousTimescale;

    public void Unpause()
    {
        manager.RemoveOverlay();
        manager.ReEnableBaseMenu();
    }
    public void ReturnToStart()
    {
        manager.Restart();
    }
    public void Settings()
    {
        manager.AddOverlay("Settings");
    }

    public override void Show()
    {
        base.Show();
        previousTimescale = Time.timeScale;
        Time.timeScale = 0;
    }
    public override void Hide()
    {
        base.Hide();
        Time.timeScale = previousTimescale;
    }
}
