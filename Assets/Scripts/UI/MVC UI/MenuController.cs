using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField]
    Page _initPage;
    [SerializeField]
    GameObject _firstFocusItem;

    Canvas _rootCanvas;
    Stack<Page> _pageStack = new Stack<Page> ();

    private void Awake()
    {
        _rootCanvas = GetComponent<Canvas> ();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_firstFocusItem);
        if (_initPage != null)
        {
            PushPage( _initPage);
        }
    }

    public void PushPage(Page page)
    {
        page.Enter(true);
        if (_pageStack.Count > 0)
        {
            Page currentPage = _pageStack.Peek(); //see the next page on the stack without popping
            if (currentPage.ExitOnNewPage)
                currentPage.Exit();
        }

        _pageStack.Push(page);
    }

    public void PopPage(Page page)
    {
        if(_pageStack.Count > 1)
        {

        }
    }
}
