using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace StepDX
{
    class GameSounds
    {
        private Device SoundDevice = null;

        //private SecondaryBuffer[] clank = new SecondaryBuffer[10];
        //int clankToUse = 0;

        private SecondaryBuffer jump = null;
        private SecondaryBuffer coin = null;
        private SecondaryBuffer death = null;
        private SecondaryBuffer background = null;

        public GameSounds(Form form)
        {
            SoundDevice = new Device();
            SoundDevice.SetCooperativeLevel(form, CooperativeLevel.Priority);

            Load(ref jump, "../../jump.wav");
            Load(ref coin, "../../coin.wav");
            Load(ref death, "../../death.wav");
            Load(ref background, "../../msufight.wav");

            if (background != null && !background.Status.Playing)
                background.Play(0, BufferPlayFlags.Default);

            //for (int i = 0; i < clank.Length; i++)
            //    Load(ref clank[i], "../../clank.wav");
        }

        private void Load(ref SecondaryBuffer buffer, string filename)
        {
            try
            {
                buffer = new SecondaryBuffer(filename, SoundDevice);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load " + filename,
                                "Danger, Will Robinson", MessageBoxButtons.OK);
                buffer = null;
            }
        }

        //public void Clank()
        //{
        //    clankToUse = (clankToUse + 1) % clank.Length;

        //    if (clank[clankToUse] == null)
        //        return;

        //    if (!clank[clankToUse].Status.Playing)
        //        clank[clankToUse].Play(0, BufferPlayFlags.Default);
        //}

        public void Jump()
        {
            if (jump == null)
                return;

            if (!jump.Status.Playing)
                jump.Play(0, BufferPlayFlags.Default);
        }

        public void Coin()
        {
            if (coin == null)
                return;

            if (!coin.Status.Playing)
                coin.Play(0, BufferPlayFlags.Default);
        }

        public void Death()
        {
            if (death == null)
                return;

            if (!death.Status.Playing)
                death.Play(0, BufferPlayFlags.Default);
        }

        //public void Arms()
        //{
        //    if (arms == null)
        //        return;

        //    if (!arms.Status.Playing)
        //    {
        //        arms.SetCurrentPosition(0);
        //        arms.Play(0, BufferPlayFlags.Default);
        //    }
        //}

        //public void ArmsEnd()
        //{
        //    if (arms == null)
        //        return;

        //    if (arms.Status.Playing)
        //        arms.Stop();
        //}

    }
}
