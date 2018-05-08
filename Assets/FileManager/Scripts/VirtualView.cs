using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ScrollRect))]
public class VirtualView : MonoBehaviour
{
    public delegate void GameObjectIDCallback(GameObject t, int i);

    #region Variables

    [Tooltip("The transform that will be instantiated for each element of the list.")]
    public RectTransform _tTemplate;
    [Tooltip("Height of the elements.")]
    public float _fHeight;
    [Tooltip("The space between each elements.")]
    public float _fSpacing;

    private GameObjectIDCallback _tCallback;
    public ScrollRect _tScrollRect; // The attached scrollrect
    private List<RectTransform> _tView = new List<RectTransform>(); // The list of elements displayed
    private int _iTotalLength; // The total lenght of the list provided
    private int _iStartSiblingIndex;

    #endregion

    #region UnityCallbacks

    void Start()
    {
        _tScrollRect.onValueChanged.AddListener(UpdateView);
    }

    #endregion

    #region Functions

    public void ClearView()
    {
        for (int i = 0; i < _tView.Count; i++)
            DestroyImmediate(_tView[i].gameObject);

        _tView.Clear();

        _iStartSiblingIndex = 0;
        for ( int i = 0; i < _tScrollRect.content.transform.childCount; i++ )
        {
            if (_tScrollRect.content.transform.GetChild(i).gameObject.activeSelf)
                _iStartSiblingIndex++;
        }

        _tScrollRect.verticalScrollbar.value = 1.0f;
    }

    public void CreateView(int iTotalListLength, GameObjectIDCallback tCallback) // To call when wanting to refresh the list with new elements
    {
        _iTotalLength = iTotalListLength;
        _tCallback = tCallback;

        // Clear current list

        ClearView();

        // Calculate the minimal number of elements to create to fit in the view

        int iDisplayNumber = (int)((_tScrollRect.GetComponent<RectTransform>().rect.height + _fHeight * 2) / (_fSpacing + _fHeight)) + 1;

        for (int i = 0; i < iDisplayNumber && i < _iTotalLength; i++)
        {
            // Instantiate template

            GameObject tNewGO = Instantiate(_tTemplate.gameObject);
            tNewGO.SetActive(true);

            // Invoke callback for people to customize each element

            _tCallback(tNewGO, i);

            // Place element and add it to the list

            tNewGO.name = i.ToString();
            RectTransform tNewRect = tNewGO.GetComponent<RectTransform>();
            tNewRect.SetParent(_tTemplate.parent, false);
            tNewRect.anchoredPosition = new Vector2(tNewRect.anchoredPosition.x, -(_fSpacing + (_fSpacing + _fHeight) * (i+ _iStartSiblingIndex)));
            _tView.Add(tNewRect);
        }

        _tScrollRect.content.sizeDelta = new Vector2(_tScrollRect.content.sizeDelta.x, _fSpacing + (_fSpacing + _fHeight) * (_iTotalLength+_iStartSiblingIndex));
    }

    public void UpdateLook()
    {
        // Calculate the minimal number of elements to create to fit in the view

        int iDisplayNumber = (int)((_tScrollRect.GetComponent<RectTransform>().rect.height + _fHeight * 2) / (_fSpacing + _fHeight)) + 1;
        
        if (_tView.Count > iDisplayNumber) // Remove unecessary entries
        {
            for (int i = iDisplayNumber; i < _tView.Count; i++)
            {
                Destroy(_tView[i].gameObject);
            }

            _tView.RemoveRange(iDisplayNumber, _tView.Count - iDisplayNumber);
        } 
        else if (_tView.Count < iDisplayNumber && _tView.Count != _iTotalLength) // Add missing entries
        {
            int iToCreate = iDisplayNumber - _tView.Count;
            int iStartID = int.Parse(_tView[_tView.Count-1].gameObject.name);

            for ( int i = 0; i < iToCreate; i++ )
            {
                // Instantiate template

                GameObject tNewGO = Instantiate(_tTemplate.gameObject);
                tNewGO.SetActive(true);

                // Invoke callback for people to customize each element

                _tCallback(tNewGO, iStartID+i);

                // Place element and add it to the list

                tNewGO.name = (iStartID+i).ToString();
                RectTransform tNewRect = tNewGO.GetComponent<RectTransform>();
                tNewRect.SetParent(_tTemplate.parent, false);
                tNewRect.anchoredPosition = new Vector2(tNewRect.anchoredPosition.x, -(_fSpacing + (_fSpacing + _fHeight) * (iStartID+i)));
                _tView.Add(tNewRect);
            }
        }

        // Update template

        _tTemplate.sizeDelta = new Vector2(_tTemplate.sizeDelta.x, _fHeight);

        // Update content height to fit everything

        _tScrollRect.content.sizeDelta = new Vector2(_tScrollRect.content.sizeDelta.x, _fSpacing + (_fSpacing + _fHeight) * (_iTotalLength + _iStartSiblingIndex));

        int iCurrentID = 0;
        for( int i = 0; i < _tScrollRect.content.transform.childCount; i++ )
        {
            // Update the size of all the elements in the list
            RectTransform tRect = _tScrollRect.content.transform.GetChild(i).GetComponent<RectTransform>();
            tRect.sizeDelta = new Vector2(tRect.sizeDelta.x, _fHeight);
            
            if( _tView.Contains(tRect) )
            {
                int iID = int.Parse(tRect.gameObject.name);
                tRect.anchoredPosition = new Vector2(tRect.anchoredPosition.x, -(_fSpacing + (_fSpacing + _fHeight) * (iID + _iStartSiblingIndex)));
            }
            else if(tRect.gameObject.activeSelf)
            {
                tRect.anchoredPosition = new Vector2(tRect.anchoredPosition.x, -(_fSpacing + (_fSpacing + _fHeight) * iCurrentID));
                iCurrentID++;
            }
        }
    }

    public void UpdateView(Vector2 tScollRectValue)
    {
        if (_tView.Count == 0)
            return;

        while (_tView[_tView.Count - 1].anchoredPosition.y + _tScrollRect.content.anchoredPosition.y < - _tScrollRect.viewport.rect.height - _fHeight)
        {
            // Retrieve id. If negative, element does not exists, so cancel

            int iID = int.Parse(_tView[0].gameObject.name) - 1;

            if (iID < 0)
                break;

            // Retrieve element out of bounds

            RectTransform tRect = _tView[_tView.Count - 1];

            // Invoke callback for people to customize the element

            _tCallback(tRect.gameObject, iID);

            // Place element and add it to the list

            tRect.gameObject.name = iID.ToString();
            tRect.anchoredPosition = new Vector2(tRect.anchoredPosition.x, _tView[0].anchoredPosition.y + _fHeight + _fSpacing);
            tRect.SetSiblingIndex(_iStartSiblingIndex);
            _tView.RemoveAt(_tView.Count - 1);
            _tView.Insert(0, tRect);
        }

        while (_tView[0].anchoredPosition.y + _tScrollRect.content.anchoredPosition.y > _fHeight)
        {
            // Retrieve id. If too high, element does not exists, so cancel

            int iID = int.Parse(_tView[_tView.Count - 1].gameObject.name) + 1;

            if (iID >= _iTotalLength)
                break;

            // Retrieve element out of bounds

            RectTransform tRect = _tView[0];

            // Invoke callback for people to customize the element

            _tCallback(tRect.gameObject, iID);

            // Place element and add it to the list

            tRect.gameObject.name = iID.ToString();
            tRect.anchoredPosition = new Vector2(tRect.anchoredPosition.x, _tView[_tView.Count - 1].anchoredPosition.y - _fHeight - _fSpacing);
            tRect.SetAsLastSibling();
            _tView.RemoveAt(0);
            _tView.Add(tRect);
        }
    }

    #endregion
}
