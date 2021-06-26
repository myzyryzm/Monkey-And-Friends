﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.UI.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;
using TMPro;
using Ryzm.Blockchain;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.UI
{
    public class MainMenu : RyzmMenu
    {
        public TextMeshProUGUI loginText;

        List<MenuType> loginMenus = new List<MenuType>
        {
            MenuType.Login,
            MenuType.Header
        };

        List<MenuType> mainMenus = new List<MenuType>
        {
            MenuType.Main,
            MenuType.Header
        };

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        Message.AddListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.Send(new LoginRequest());
                        // Message.Send(new DisableHeaderBackButton());
                        Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        Message.Send(new MenuSetRequest(MenuSet.LoginMenu));
                    }
                    else
                    {
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        
        public void OnClickStart()
        {
            Message.Send(new MakeWorld());
        }

        public void OnClickLogin()
        {
            Debug.Log("on click login");
            if(IsActive)
            {
                Message.Send(new ActivateMenu(activatedTypes: loginMenus));
                // Message.Send(new DeactivateMenu(activatedTypes: loginMenus));
                Message.Send(new EnableHeaderBackButton(mainMenus));
            }
        }

        void OnLoginResponse(LoginResponse response)
        {
            switch(response.status)
            {
                case LoginStatus.FetchingKeys:
                    loginText.text = "Loading...";
                    break;
                case LoginStatus.LoggedIn:
                    loginText.text = response.accountName;
                    break;
                default:
                    loginText.text = "Login";
                    break;
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
            else if(response.set == MenuSet.LoginMenu)
            {
                loginMenus = response.menus;
            }
        }
    }
}
