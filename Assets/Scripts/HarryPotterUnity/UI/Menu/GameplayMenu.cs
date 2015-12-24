﻿using System.Linq;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.UI.Menu
{
    public class GameplayMenu : BaseMenu
    {
        private Player _localPlayer;
        private Player _remotePlayer;
        
        public Player LocalPlayer
        {
            private get { return _localPlayer; }
            set
            {
                _localPlayer = value;
                _localPlayer.OnTurnStart += () => _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", true);
                _localPlayer.OnTurnEnd   += () => _actionsLeftLabelLocal.GetComponent<Animator>().SetBool("IsOpen", false);
            }
        }

        public Player RemotePlayer
        {
            private get { return _remotePlayer; }
            set
            {
                _remotePlayer = value;
                _remotePlayer.OnTurnStart += () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", true);
                _remotePlayer.OnTurnEnd   += () => _actionsLeftLabelRemote.GetComponent<Animator>().SetBool("IsOpen", false);

            }
        }

        private Text _actionsLeftLabelLocal;
        private Text _actionsLeftLabelRemote;

        private Text _cardsLeftLabelLocal;
        private Text _cardsLeftLabelRemote;

        private Text _mainMenuTitle;
        private Image _mainMenuBackground;

        protected override void Awake()
        {
            base.Awake();

            var allText = FindObjectsOfType<Text>();

            _mainMenuBackground = FindObjectsOfType<Image>().FirstOrDefault(i => i.name.Contains("MainMenuBackground"));
            _mainMenuTitle = allText.FirstOrDefault(i => i.name.Contains("MainMenuTitle"));

            _actionsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Local"));
            _actionsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("ActionsLeftLabel_Remote"));

            _cardsLeftLabelLocal = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Local"));
            _cardsLeftLabelRemote = allText.FirstOrDefault(t => t.name.Contains("CardsLeftLabel_Remote"));
            
            if (_actionsLeftLabelLocal  == null || 
                _actionsLeftLabelRemote == null || 
                _cardsLeftLabelLocal    == null || 
                _cardsLeftLabelRemote   == null || 
                _mainMenuTitle          == null || 
                _mainMenuBackground     == null)
            {
                Log.Error("Could not find all needed HUD elements in gameplay menu, report this error!");
            }
        }

        protected override void Update()
        {
            base.Update();

            if (LocalPlayer != null)
            {
                //Update Local Player Properties
                _actionsLeftLabelLocal.text = string.Format("Actions Left: {0}", LocalPlayer.ActionsAvailable);
                _cardsLeftLabelLocal.text = string.Format("Cards Left: {0}", LocalPlayer.Deck.Cards.Count);
            }

            if (RemotePlayer != null)
            {
                //Update Remote Plater Properties
                _actionsLeftLabelRemote.text = string.Format("Actions Left: {0}", RemotePlayer.ActionsAvailable);
                _cardsLeftLabelRemote.text = string.Format("Cards Left: {0}", RemotePlayer.Deck.Cards.Count);
            }
        }

        [UsedImplicitly]
        public void SkipAction()
        {
            GameManager.Network.RPC("ExecuteSkipAction", PhotonTargets.All);
        }

        public override void OnShowMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", false);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", false);
        }

        public override void OnHideMenu()
        {
            _mainMenuBackground.GetComponent<Animator>().SetBool("IsVisible", true);
            _mainMenuTitle.GetComponent<Animator>().SetBool("IsVisible", true);
        }
    }
}