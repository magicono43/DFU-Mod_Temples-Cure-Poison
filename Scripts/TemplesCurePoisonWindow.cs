// Project:         TemplesCurePoison mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    5/24/2022, 11:20 PM
// Last Edit:		5/25/2022, 8:30 PM
// Version:			1.00
// Special Thanks:  Hazelnut, Flynsarmy, Ralzar, Jefetienne, Kab the Bird Ranger, Jehuty, ACNAero, and Interkarma
// Modifier:

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class TemplesCurePoisonWindow : DaggerfallGuildServiceCureDisease
    {
        public TemplesCurePoisonWindow(IUserInterfaceManager uiManager, int buildingFactionId, IGuild guild) : base(uiManager, buildingFactionId, guild)
        {
        }

        protected override void CureDiseaseService()
        {
            int numberOfDiseases = GameManager.Instance.PlayerEffectManager.DiseaseCount;
            int numberOfPoisons = GameManager.Instance.PlayerEffectManager.PoisonCount;

            if (playerEntity.TimeToBecomeVampireOrWerebeast != 0)
                numberOfDiseases++;

            // Check holidays for free / cheaper curing
            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int holidayId = FormulaHelper.GetHolidayId(minutes, GameManager.Instance.PlayerGPS.CurrentRegionIndex);

            if ((numberOfDiseases > 0 || numberOfPoisons > 0) &&
                (holidayId == (int)DFLocation.Holidays.South_Winds_Prayer ||
                 holidayId == (int)DFLocation.Holidays.First_Harvest ||
                 holidayId == (int)DFLocation.Holidays.Second_Harvest))
            {
                // Just cure and inform player
                GameManager.Instance.PlayerEffectManager.CureAllDiseases();
                playerEntity.TimeToBecomeVampireOrWerebeast = 0;
                GameManager.Instance.PlayerEffectManager.CureAllPoisons();
                SetText(TextManager.Instance.GetLocalizedText("freeHolidayCuring"), this);
                ClickAnywhereToClose = true;
            }
            else if (numberOfDiseases > 0 || numberOfPoisons > 0)
            {
                // Get base cost
                int baseCost = 250 * (numberOfDiseases + numberOfPoisons);

                // Apply rank-based discount if this is an Arkay temple and member
                baseCost = Guild.ReducedCureCost(baseCost);

                // Apply temple quality and regional price modifiers
                int costBeforeBargaining = FormulaHelper.CalculateCost(baseCost, buildingDiscoveryData.quality);

                // Halve the price on North Winds Prayer holiday
                if (holidayId == (int)DFLocation.Holidays.North_Winds_Festival)
                    costBeforeBargaining /= 2;

                // Apply bargaining to get final price
                curingCost = FormulaHelper.CalculateTradePrice(costBeforeBargaining, buildingDiscoveryData.quality, false);

                // Index correct message
                int msgOffset = 0;
                if (costBeforeBargaining >> 1 <= curingCost)
                {
                    if (costBeforeBargaining - (costBeforeBargaining >> 2) <= curingCost)
                        msgOffset = 2;
                    else
                        msgOffset = 1;
                }

                // Offer curing at the calculated price
                SetTextTokens(GetCureOfferTokens(msgOffset), this);
                AddButton(MessageBoxButtons.Yes);
                AddButton(MessageBoxButtons.No);
                OnButtonClick += ConfirmCuring_OnButtonClick;
            }
            else
            {   // Not diseased or poisoned
                SetTextTokens(NoDisease, this);
                ClickAnywhereToClose = true;
            }
        }

        protected override void ConfirmCuring_OnButtonClick(DaggerfallMessageBox sender, MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == MessageBoxButtons.Yes)
            {
                if (playerEntity.GetGoldAmount() >= curingCost)
                {
                    playerEntity.DeductGoldAmount(curingCost);
                    GameManager.Instance.PlayerEffectManager.CureAllDiseases();
                    playerEntity.TimeToBecomeVampireOrWerebeast = 0;
                    GameManager.Instance.PlayerEffectManager.CureAllPoisons();
                    DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("curedDisease"));
                }
                else
                    DaggerfallUI.MessageBox(DaggerfallTradeWindow.NotEnoughGoldId);
            }
        }
    }
}
