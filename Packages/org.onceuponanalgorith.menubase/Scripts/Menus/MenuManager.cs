using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static string WorldDef;
    public static string CharacterName;
    public static string CharacterPhysicalDesc;

    private Dictionary<string, Menu> menus;
    private Coroutine menuTransition;
    private Menu previousMenu;
    private Menu currentMenu;
    private Menu currentOverlay;
    private GameObject gameWorldPrefab;

    [HideInInspector] public UnityEvent<string> onChangedState;
    [HideInInspector] public string currentState;

    private float transitionDuration = 0.5f;

    private void Start()
    {
        menus = new Dictionary<string, Menu>();
        for(int i = 0; i < transform.childCount; i++)
        {
            Menu m = transform.GetChild(i).GetComponent<Menu>();
            if(m != null)
            {
                m.manager = this;
                menus.Add(transform.GetChild(i).name, m);
            }
        }
        menus["StartGame"].Show();
        currentMenu = menus["StartGame"];
        Time.timeScale = 0;
    }

    public void ShowMenu(
        string toMenu,
        System.Action callback = null
    )
    {
        if (menuTransition != null)
        {
            //immediate hide previous menu and current menu
            StopCoroutine(menuTransition);
        }
        menuTransition = StartCoroutine(
            DoAnimateMenu(currentMenu, menus[toMenu], callback)
        );
    }

    public void AddOverlay(
        string toMenu,
        System.Action callback = null
    )
    {
        currentState = toMenu;
        onChangedState.Invoke(currentState);
        if(currentOverlay != null)
        {
            currentOverlay.Hide();
        }
        currentOverlay = menus[toMenu];
        currentMenu.Disable();
        currentOverlay.Show();
    }

    public void RemoveOverlay(
        System.Action callback = null
    )
    {
        currentOverlay.Hide();
        currentOverlay = null;
    }

    public void ReEnableBaseMenu()
    {
        currentMenu.Enable();
        currentState = currentMenu.gameObject.name;
        onChangedState.Invoke(currentState);
    }

    IEnumerator DoAnimateMenu(
        Menu fromMenu,
        Menu toMenu,
        System.Action callback
    )
    {
        currentState = "";
        onChangedState.Invoke(currentState);
        if(currentOverlay != null)
        {
            currentOverlay.Hide();
        }
        previousMenu = fromMenu;
        currentMenu = toMenu;
        if(previousMenu != null)
        {
            previousMenu.Hide();
        }
        yield return new WaitForSeconds(transitionDuration);
        toMenu.Show();
        if (callback != null)
        {
            callback();
        }
        currentState = currentMenu.gameObject.name;
        onChangedState.Invoke(currentState);
    }

    public Menu GetMenu(string name)
    {
        return menus[name];
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }
}
