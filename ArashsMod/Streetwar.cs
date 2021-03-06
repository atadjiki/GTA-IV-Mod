﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace Mod
{
    public class Streetwar : Script //make sure to extend GTA.Script
    {
        private bool debug = false;
        private int debugTime = 4000; //how long the debug messages appear on screen
        private bool scripton = false;
        private bool revent = false;

        private string[] CopSkins = { "M_Y_SWAT" };
        private string[] CriminalSkins = { "M_Y_GRU2_LO_01", "M_M_GRU2_LO_02", "M_M_GRU2_HI_02", "M_M_GRU2_HI_01",
                                           "M_Y_GRUS_HI_02", "M_Y_GRUS_LO_02", "M_Y_GRUS_LO_01", "M_O_GRUS_HI_01", "M_Y_GALB_LO_01", "M_Y_GALB_LO_02",
                                           "M_Y_GALB_LO_03", "M_Y_GALB_LO_04" };

        private const int interval = 10000; //script fires every 10 seconds
        private const int spawnRadius = 25; //NPC's spawn 25 units around player
        private const int fightRadius = 200; //NPC's will fight hated Pedestrians in a 200 unit radius

        private const int cops = 8;
        private const int robbers = 16;

        private const int copHealth = 100;
        private const int robberHealth = 50;

        private const int wavesUntilCops = 3;

        private int currentWave = 0;
        private const int MaxPeds = 64;
        private int totalSpawned = 0;

        PedCollection peds = new PedCollection(); //for holding pedestrians
        Dictionary<Ped, Blip> blips = new Dictionary<Ped, Blip>(); //for matching pedestrians to blips

        public enum WeaponTier { Melee, Pistols, Full }; //specifies what type of weapons we give the criminals
        WeaponTier weaponTier = WeaponTier.Melee;

        public Streetwar()
        {
            //set interval and key binds
            Interval = Settings.GetValueInteger("INTERVAL", "SETTINGS", interval);
            BindKey(Settings.GetValueKey("Toggle Script", "SETTINGS", Keys.K), new KeyPressDelegate(ScriptOn));
            BindKey(Settings.GetValueKey("Toggle Pedestrian Events", "SETTINGS", Keys.L), new KeyPressDelegate(RandomEventsPedestrians));
            BindKey(Settings.GetValueKey("Toggle Debug", "SETTINGS", Keys.End), new KeyPressDelegate(ToggleDebug));
            BindKey(Settings.GetValueKey("Toggle Debug", "SETTINGS", Keys.F9), new KeyPressDelegate(SpawnPoliceWave));
            BindKey(Settings.GetValueKey("Toggle Debug", "SETTINGS", Keys.F10), new KeyPressDelegate(SpawnRobberWave));


            //bind tick event
            this.Tick += new EventHandler(RandomPedEvents_Tick);

        }

        public void ToggleDebug()
        {
            if (debug)
            {
                debug = false;
            }
            else
            {
                debug = true;
            }
        }

        //Creates a pedestrian, assigns behavior and adds a blip if necessary. Creates a cop if cop=true, criminal if false
        private void SpawnPedestrian(bool cop)
        {

            if (peds.Count >= MaxPeds)
            {
                return;
            }

            int mod = 1;

            if (!cop) { mod *= -1; }

            Ped ped = World.CreatePed(RandomModel(cop), Player.Character.GetOffsetPosition(new Vector3(mod * RandomNumber(spawnRadius / 2, spawnRadius), mod * RandomNumber(spawnRadius / 2, spawnRadius), 0.0f)).ToGround());
            if (Exists(ped) && ped != null)
            {
                ped.BecomeMissionCharacter(); //makes sure the pedestrian persists while player alive

                if (cop)
                {
                    ped.RelationshipGroup = RelationshipGroup.Cop;
                    ped.ChangeRelationship(RelationshipGroup.Criminal, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Dealer, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Player, Relationship.Respect);

                    ped.MaxHealth = copHealth;
                    ped.Health = copHealth;
                    ped.Armor = 50;

                }
                else
                {
                    ped.RelationshipGroup = RelationshipGroup.Criminal;
                    ped.ChangeRelationship(RelationshipGroup.Cop, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Civillian_Male, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Civillian_Female, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Fireman, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Bum, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Medic, Relationship.Hate);
                    ped.ChangeRelationship(RelationshipGroup.Player, Relationship.Neutral);

                    ped.MaxHealth = robberHealth;
                    ped.Health = robberHealth;
                    ped.Armor = 0;
                    ped.WantedByPolice = true;
                    ped.StartKillingSpree(true);
                    Blip blip = ped.AttachBlip();
                    blips.Add(ped, blip);
                }

                ped.Task.FightAgainstHatedTargets(fightRadius); //give the pedestrian their task, will repeat

                RandomWeapons(ped, cop);
                peds.Add(ped);
                totalSpawned++;
                Wait(10);
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
                    Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Starting...", debugTime, 1);
                }
                else
                {
                    Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Over. Total spawned: " + totalSpawned, debugTime, 1);
                    if (debug) Wait(debugTime);

                    int pedsKilled = 0;

                    //Do some clean up

                    //kill all pedestrians 
                    foreach (Ped ped in peds)
                    {
                        if (ped.Exists())
                        {
                            ped.Health = 0;
                            ped.AlwaysDiesOnLowHealth = true;
                            ped.Die();
                            pedsKilled++;

                        }

                        //remove all blips on living pedestrians
                        if (blips.ContainsKey(ped))
                        {
                            if (blips[ped].Exists())
                            {
                                blips[ped].Delete();
                                blips.Remove(ped);
                            }
                        }
                    }

                    //remove any dead blips if there are any
                    foreach(Blip blip in blips.Values)
                    {
                        if (blip.Exists())
                        {
                            blip.Delete();
                        }
                        
                    }

                    if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Killed off " + pedsKilled + " peds", debugTime, 1);
                    peds = new PedCollection();
                    blips = new Dictionary<Ped, Blip>();
                    currentWave = 0;
                    //reinit lists and reset counters
                }
            }
        }

        public void SpawnPoliceWave()
        { 
            for (int i = 0; i < cops; i++)
            {
                SpawnPedestrian(true);
            }
            if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Spawned " + cops + " police. Total: " + peds.Count, debugTime, 1);
        }

        public void SpawnRobberWave()
        {

            int amountOfRobbers = RandomNumber(robbers / 2, robbers);
            int currentSize = peds.Count;

            for (int i = 0; i < RandomNumber(cops, robbers); i++)
            {
                SpawnPedestrian(false);
            }
            RandomWeaponTier();

            if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Spawned " + (peds.Count - currentSize) + " " + weaponTier.ToString() + " Total: " + peds.Count, debugTime, 1);

        }

        public void RandomPedEvents_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isAlive == false)
            {
                RandomEventsPedestrians();

                return;
            }
            if (scripton)
            {
                if (revent)
                {
                    //cull dead pedestrians
                    if (peds.Count >= MaxPeds)
                    {
                        if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Hit max peds " + peds.Count, debugTime, 1);
                        return;
                    }

                    else if (Player.Character.isInVehicle() == false)
                    {
                        //spawn wave of police
                        if (currentWave >= wavesUntilCops)
                        {
                            currentWave = 0;

                            SpawnPoliceWave();
                        }
                        else
                        {
                            SpawnRobberWave();
                            currentWave++;
                        }

                        Cull();
                    }
                }
            }

        }

        public void Cull()
        {
            int currentSize = peds.Count;
            List<Ped> toRemove = new List<Ped>();

            foreach (Ped ped in peds)
            {
                if (ped.Exists() == false)
                {
                    toRemove.Add(ped);
                }
                else if (ped.isDead)
                {
                    toRemove.Add(ped);
                }

            }

            foreach (Ped ped in toRemove)
            {
                peds.Remove(ped);

                if (blips.ContainsKey(ped))
                {
                    if (blips[ped].Exists())
                    {
                        blips[ped].Delete();
                        blips.Remove(ped);
                    }
                }
            }

            foreach(Ped ped in peds)
            {
                if(ped.Exists() == false)
                {
                    blips[ped].Delete();
                }
            }

            if((currentSize - peds.Count) > 0)
            {
                if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Removed " + (currentSize - peds.Count) + " dead peds - Total: " + peds.Count, debugTime, 1);
            }
            
        }

        public void RandomWeaponTier()
        {
            int tier = RandomNumber(0, 3);

            if (tier == 0)
            {
                weaponTier = WeaponTier.Melee;
            }
            else if (tier == 1)
            {
                weaponTier = WeaponTier.Pistols;
            }
            else if (tier == 2)
            {
                weaponTier = WeaponTier.Full;
            }
        }

        public void RandomWeapons(Ped ped, bool cop)
        {
            if (cop)
            {
                int weapon = RandomNumber(0, 5);

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
                else if (weapon == 4)
                {
                    ped.Weapons.SniperRifle_M40A1.Ammo = 999999;
                }
            }
            else
            {
                if (weaponTier == WeaponTier.Melee)
                {
                    int weapon = RandomNumber(0, 10);

                    if (weapon < 9 && weapon > 3)
                    {
                        ped.Weapons.Knife.Ammo = 1;
                    }
                    else if (weapon < 9 && weapon > 0)
                    {
                        ped.Weapons.BaseballBat.Ammo = 1;
                    }
                    else
                    {
                        ped.Weapons.MolotovCocktails.Ammo = 4;
                    }

                }
                else if (weaponTier == WeaponTier.Pistols)
                {
                    int weapon = RandomNumber(0, 2);

                    if (weapon == 0)
                    {
                        ped.Weapons.Glock.Ammo = 999999;
                    }
                    else if (weapon == 1)
                    {
                        ped.Weapons.DesertEagle.Ammo = 999999;
                    }
                }
                else if (weaponTier == WeaponTier.Full)
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
                        ped.Weapons.MolotovCocktails.Ammo = 999999;

                    }
                    else if (weapon == 5)
                    {
                        ped.Weapons.AnySniperRifle.Ammo = 99999;

                    }
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
