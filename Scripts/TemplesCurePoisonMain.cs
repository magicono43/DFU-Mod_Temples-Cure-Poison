// Project:         TemplesCurePoison mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    5/24/2022, 11:20 PM
// Last Edit:		5/25/2022, 8:30 PM
// Version:			1.00
// Special Thanks:  Hazelnut, Flynsarmy, Ralzar, Jefetienne, Kab the Bird Ranger, Jehuty, ACNAero, and Interkarma
// Modifier:

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace TemplesCurePoison
{
    public class TemplesCurePoisonMain : MonoBehaviour
	{
        static TemplesCurePoisonMain instance;

        public static TemplesCurePoisonMain Instance
        {
            get { return instance ?? (instance = FindObjectOfType<TemplesCurePoisonMain>()); }
        }

        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            instance = new GameObject("TemplesCurePoison").AddComponent<TemplesCurePoisonMain>(); // Add script to the scene.

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Temples Cure Poison");

            UIWindowFactory.RegisterCustomUIWindow(UIWindowType.GuildServiceCureDisease, typeof(TemplesCurePoisonWindow));
            Debug.Log("TemplesCurePoison Registered Its Window");

            Debug.Log("Finished mod init: Temples Cure Poison");
        }
    }
}