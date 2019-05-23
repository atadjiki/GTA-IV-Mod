using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace Mod
{
    public class Streetwar : Script
    {
        private bool scripton = false;
        private bool revent = false;

        private string[] CopSkins = { "M_Y_COP", "M_Y_SWAT", "M_M_FBI", "M_M_FATCOP_01" };
        private string[] CriminalSkins = { "M_Y_GOON_01", "M_Y_THIEF", "M_Y_GRU2_LO_01", "M_M_GRU2_LO_02", "M_M_GRU2_HI_02", "M_M_GRU2_HI_01",
                                           "M_Y_GRUS_HI_02", "M_Y_GRUS_LO_02", "M_Y_GRUS_LO_01", "M_O_GRUS_HI_01", "M_Y_GALB_LO_01", "M_Y_GALB_LO_02",
                                           "M_Y_GALB_LO_03", "M_Y_GALB_LO_04", "M_Y_GMAF_HI_01", "M_Y_GMAF_HI_02", "M_M_FATMOB_01" };

        private const int maxTotalPeds = 32;
        private const int maxSpawnDuringTick = 5;
        private int spawnRadius = 50;
        private int fightRadius = 300;
        public List<Ped> peds;

        public Streetwar()
        {
            //set interval
            Interval = Settings.GetValueInteger("INTERVAL", "SETTINGS", 10000);
            BindKey(Settings.GetValueKey("Toggle Script", "SETTINGS", Keys.K), new KeyPressDelegate(ScriptOn));
            BindKey(Settings.GetValueKey("Toggle Pedestrian Events", "SETTINGS", Keys.L), new KeyPressDelegate(RandomEventsPedestrians));

            //bind tick event
            this.Tick += new EventHandler(RandomPedEvents_Tick);

            peds = new List<Ped>();

        }
        private void SpawnPedestrian(bool cop)
        {

            if (peds.Count >= maxTotalPeds)
            {
                foreach (Ped ped in peds)
                {
                    if (ped.isAlive == false)
                    {
                        peds.Remove(ped);
                    }
                }

                return;
            }

            int mod = 1;

            if (!cop) { mod *= -1; }

            for (int i = 0; i < RandomNumber(0, maxSpawnDuringTick); i++)
            {
                Ped ped = World.CreatePed(RandomModel(cop), Player.Character.GetOffsetPosition(new Vector3(mod * spawnRadius, spawnRadius, 0.0f)).ToGround());
                if (Exists(ped) && ped != null)
                {
                    ped.BecomeMissionCharacter();

                    if (cop)
                    {
                        ped.RelationshipGroup = RelationshipGroup.Cop;
                        ped.ChangeRelationship(RelationshipGroup.Criminal, Relationship.Hate);
                    }
                    else
                    {
                        ped.RelationshipGroup = RelationshipGroup.Criminal;
                        ped.ChangeRelationship(RelationshipGroup.Cop, Relationship.Hate);
                    }

                    ped.Task.FightAgainstHatedTargets(300);
                    ped.MaxHealth = 100;
                    ped.Health = 100;
                    ped.Armor = 50;
                    RandomWeapons(ped, cop);

                    peds.Add(ped);

                    Wait(10);
                }
            }
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public void RandomEventsPedestrians()
        {
            if (scripton)
            {
                revent = !revent;
                if (revent)
                {
                    Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Starting...", 4000, 1);
                }
                else
                {
                    Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Over", 4000, 1);
                }
            }
        }
        public void RandomPedEvents_Tick(object sender, EventArgs e)
        {
            if (scripton)
            {
                if (revent)
                {
                    if (Player.Character.isInVehicle() == false)
                    {
                        int faction = RandomNumber(0, 10);

                        if (faction <= 5)
                        {
                            SpawnPedestrian(true);
                        }
                        else if (faction > 5)
                        {
                            SpawnPedestrian(false);
                        }
                    }
                }
            }
        }
        public void RandomWeapons(Ped ped, bool cop)
        {
            if (cop)
            {
                int weapon = RandomNumber(0, 4);

                if (weapon == 0)
                {
                    ped.Weapons.AssaultRifle_M4.Ammo = 999999;
                }
                else if (weapon == 1)
                {
                    ped.Weapons.MP5.Ammo = 999999;
                }
                else if (weapon == 2)
                {
                    ped.Weapons.DesertEagle.Ammo = 999999;
                }
                else if (weapon == 3)
                {
                    ped.Weapons.BarettaShotgun.Ammo = 999999;
                }
            }
            else
            {
                int weapon = RandomNumber(0, 6);

                if (weapon == 0)
                {
                    ped.Weapons.AssaultRifle_AK47.Ammo = 999999;
                }
                else if (weapon == 1)
                {
                    ped.Weapons.Uzi.Ammo = 999999;
                }
                else if (weapon == 2)
                {
                    ped.Weapons.Glock.Ammo = 999999;
                }
                else if (weapon == 3)
                {
                    ped.Weapons.BasicShotgun.Ammo = 999999;
                }
                else if (weapon == 4)
                {
                    ped.Weapons.MolotovCocktails.Ammo = 10;
                }
                else if (weapon == 5)
                {
                    ped.Weapons.RocketLauncher.Ammo = 10;
                }
            }

        }
        private Model RandomModel(bool cop)
        {
            if (cop) { return CopSkins[RandomNumber(0, CopSkins.Length)]; }
            else { return CriminalSkins[RandomNumber(0, CriminalSkins.Length)]; }
        }
        public void ScriptOn() { scripton = !scripton; }
    }
}
