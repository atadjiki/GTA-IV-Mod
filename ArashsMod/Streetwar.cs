using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace Mod
{
    public class Streetwar : Script
    {
        private bool debug = true;
        private int debugTime = 4000;
        private bool scripton = false;
        private bool revent = false;

        private string[] CopSkins = { "M_Y_SWAT" };
        private string[] CriminalSkins = { "M_Y_GRU2_LO_01", "M_M_GRU2_LO_02", "M_M_GRU2_HI_02", "M_M_GRU2_HI_01",
                                           "M_Y_GRUS_HI_02", "M_Y_GRUS_LO_02", "M_Y_GRUS_LO_01", "M_O_GRUS_HI_01", "M_Y_GALB_LO_01", "M_Y_GALB_LO_02",
                                           "M_Y_GALB_LO_03", "M_Y_GALB_LO_04" };

        private const int interval = 10000;
        private const int spawnRadius = 25;
        private const int fightRadius = 200;

        private const int cops = 8;
        private const int robbers = 16;

        private const int copHealth = 100;
        private const int robberHealth = 50;

        private const int wavesUntilCops = 3;

        private int currentWave = 0;
        private const int MaxPeds = 64;
        private int totalSpawned = 0;

        PedCollection peds = new PedCollection();

        public enum WeaponTier { Melee, Pistols, Full };
        WeaponTier weaponTier = WeaponTier.Melee;

        public Streetwar()
        {
            //set interval
            Interval = Settings.GetValueInteger("INTERVAL", "SETTINGS", interval);
            BindKey(Settings.GetValueKey("Toggle Script", "SETTINGS", Keys.K), new KeyPressDelegate(ScriptOn));
            BindKey(Settings.GetValueKey("Toggle Pedestrian Events", "SETTINGS", Keys.L), new KeyPressDelegate(RandomEventsPedestrians));
            BindKey(Settings.GetValueKey("Toggle Debug", "SETTINGS", Keys.End), new KeyPressDelegate(ToggleDebug));

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
        private void SpawnPedestrian(bool cop)
        {

            int mod = 1;

            if (!cop) { mod *= -1; }

            Ped ped = World.CreatePed(RandomModel(cop), Player.Character.GetOffsetPosition(new Vector3(mod * RandomNumber(spawnRadius / 2, spawnRadius), mod * RandomNumber(spawnRadius / 2, spawnRadius), 0.0f)).ToGround());
            if (Exists(ped) && ped != null)
            {
                ped.BecomeMissionCharacter();

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
                }

                ped.Task.FightAgainstHatedTargets(fightRadius);

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
                    if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Starting...", debugTime, 1);
                }
                else
                {
                    if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Streetwar Over. Total spawned: " + totalSpawned, debugTime, 1);
                    Wait(1000);
                    if (Player.CanControlCharacter == false)
                    {
                        int pedsKilled = 0;
                        foreach (Ped ped in peds)
                        {
                            if (ped.Exists())
                            {
                                ped.Die();
                                pedsKilled++;

                            }
                        }
                        if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Killed off " + pedsKilled + " peds", debugTime, 1);
                        peds.Clear();
                    }
                    currentWave = 0;
                }
            }
        }

        public void RandomPedEvents_Tick(object sender, EventArgs e)
        {
            if (Player.CanControlCharacter == false)
            {
                if (peds.Count > 0)
                {
                    peds.Clear();
                }

                RandomEventsPedestrians();

                return;
            }
            if (scripton)
            {
                if (revent)
                {

                    if (peds.Count >= MaxPeds)
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
                        }

                        if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Hit max amount of peds, removed " + (currentSize - peds.Count) + " peds - Total: " + peds.Count, debugTime, 1);
                        return;
                    }

                    if (Player.Character.isInVehicle() == false)
                    {
                        if (currentWave >= wavesUntilCops)
                        {
                            currentWave = 0;

                            for (int i = 0; i < cops; i++)
                            {
                                SpawnPedestrian(true);
                            }
                            if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Spawned " + cops + " police. Total: " + peds.Count, debugTime, 1);
                        }
                        else
                        {
                            int amountOfRobbers = RandomNumber(robbers / 2, robbers);
                            int currentSize = peds.Count;

                            for (int i = 0; i < RandomNumber(cops, robbers); i++)
                            {
                                SpawnPedestrian(false);
                            }
                            RandomWeaponTier();

                            if (debug) Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", "Spawned " + (peds.Count - currentSize) + " " + weaponTier.ToString() + " criminals. Wave " + (currentWave + 1) + " Total: " + peds.Count, debugTime, 1);
                            currentWave++;
                        }
                    }
                }
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
