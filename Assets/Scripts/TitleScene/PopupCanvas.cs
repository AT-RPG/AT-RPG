using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class PopupCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject root;

        private Stack<Popup> popups
            = new Stack<Popup>();

        public Stack<Popup> Popups
        {
            get
            {
                return popups;
            }
            set
            {
                popups = value;
            }
        }

        public GameObject Root => root;
    }

}