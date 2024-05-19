using System;
using System.Collections.Generic;
using Managers;
using Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoManager<InputManager>
    {
        public event Action<Tile> SelectedTile;
        
        private bool mouseOverUI;

        private void Update()
        {
            mouseOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        public void OnSetDestinationEvent(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            var tile = GetClickedTile();
            if (tile != null) SelectedTile?.Invoke(tile);
        }
        
        public void OnSetBlockEvent(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            GetClickedTile()?.ChangeBlockedState();
        }

        private Tile GetClickedTile()
        {
            if (mouseOverUI) return null;
            var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            var vector = new Vector2Int(Mathf.FloorToInt(hit.point.x + 0.5f), Mathf.FloorToInt(hit.point.z + 0.5f));
            return MapManager.Current.GetTile(vector);
        }
    }
}