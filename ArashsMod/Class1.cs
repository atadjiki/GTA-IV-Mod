﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace Mod
{
    public class Mod : Script
    {
        private bool scripton = false;
        private bool revent = false;
        private string[] CopSkins = {"M_Y_COP", "M_Y_SWAT", "M_M_FBI", "M_M_FATCOP_01" };
        private string[] CriminalSkins = { "M_Y_GOON_01", "M_Y_THIEF", "M_Y_GRU2_LO_01", "M_M_GRU2_LO_02", "M_M_GRU2_HI_02", "M_M_GRU2_HI_01", "M_Y_GRUS_HI_02", "M_Y_GRUS_LO_02", "M_Y_GRUS_LO_01", "M_O_GRUS_HI_01" };

        public Mod()
        {
            //set interval
            Interval = Settings.GetValueInteger("INTERVAL", "SETTINGS", 10000);
            BindKey(Settings.GetValueKey("Toggle Script", "SETTINGS", Keys.K), new KeyPressDelegate(ScriptOn));
            BindKey(Settings.GetValueKey("Toggle Pedestrian Events", "SETTINGS", Keys.L), new KeyPressDelegate(ScriptOn));

            //bind tick event
            this.Tick += new EventHandler(RandomPedEvents_Tick);

        }

        //tick method, ran every 20 secs
        public void RandomPedEvents_Tick(object sender, EventArgs e)
        {
            if (scripton)
            {
                if (revent)
                {
                    if(Player.Character.isInVehicle() == false)
                    {
                        int faction = RandomNumber(0, 25);

                        if (faction == 5)
                        {
                            RandomEventCops();
                        }else if(faction == 15)
                        {
                            RandomEventCriminals();
                        }
                    }
                }
            }
        }

        private void RandomEventCops()
        {

            for(int i = 0; i < 5; i++)
            {
                Ped ped = World.CreatePed(RandomModel(true), Player.Character.GetOffsetPosition(new Vector3(30, 30, 0.0f)).ToGround());
                if (Exists(ped) && ped != null)
                {
                    ped.BecomeMissionCharacter();
                    ped.RelationshipGroup = RelationshipGroup.Cop;
                    ped.ChangeRelationship(RelationshipGroup.Dealer, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Criminal, Relationship.Hate);
                    ped.Task.FightAgainstHatedTargets(1000);
                    ped.MaxHealth = 100;
                    ped.Health = 100;
                    ped.Armor = 50;
                    RandomWeapons(ped, true);
                    Wait(10);
                }
            }

           
        }

        private void RandomEventCriminals()
        {

            for (int i = 0; i < 5; i++)
            {

                Ped ped = World.CreatePed(RandomModel(false), Player.Character.GetOffsetPosition(new Vector3(-30, 30, 0.0f)).ToGround());
                if (Exists(ped) && ped != null)
                {
                    ped.BecomeMissionCharacter();
                    ped.RelationshipGroup = RelationshipGroup.Criminal;
                    ped.ChangeRelationship(RelationshipGroup.Dealer, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Cop, Relationship.Hate);
                    ped.Task.FightAgainstHatedTargets(1000);
                    ped.MaxHealth = 100;
                    ped.Health = 100;
                    ped.Armor = 50;
                    RandomWeapons(ped, false);
                    Wait(10);
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
                }else if(weapon == 1)
                {
                    ped.Weapons.MP5.Ammo = 999999;
                }else if(weapon == 2)
                {
                    ped.Weapons.DesertEagle.Ammo = 999999;
                }else if(weapon == 3)
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
                }else if(weapon == 4)
                {
                    ped.Weapons.MolotovCocktails.Ammo = 10;
                }
                else if (weapon == 5)
                {
                    ped.Weapons.RocketLauncher.Ammo = 10;
                }
            }

        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private Model RandomModel(bool cop)
        {
            if (cop)
            {
                int index = RandomNumber(0, CopSkins.Length);
                return CopSkins[index];
            }
            else
            {
                int index = RandomNumber(0, CriminalSkins.Length);
                return CriminalSkins[index];
            }
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

        public void ScriptOn()
        {
            scripton = !scripton;
        }

        
    }
} 
