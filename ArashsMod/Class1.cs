using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace Mod
{
    public class Mod : Script
    {

        public Mod()
        {
            //set interval
            Interval = Settings.GetValueInteger("INTERVAL", "SETTINGS", 10000);

            //bind tick event
            this.Tick += new EventHandler(ModTick);

            //bind keydown event.
            this.KeyDown += new GTA.KeyEventHandler(ModKeyDown);


        }

        //tick method, ran every 20 secs
        public void ModTick(object sender, EventArgs e)
        {
            if (Player.Character.isInjured)
            {
                Player.Character.Health = 1000;
            }
        }

        //key down handler
        //A lot of your script will go here
        public void ModKeyDown(object sender, GTA.KeyEventArgs e)
        {

        }
    }
} 
