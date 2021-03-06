﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.Runtime.InteropServices;
using System.Windows.Input;
/*To update:
 * ctrl+alt+del works
 * ctrl_alt_tab works
 * media player controls - ie mute
 */

namespace ainsleyharriot
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            Cursor.Hide();
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.CenterToScreen();
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            player.enableContextMenu = false;
            
            player.Width = this.Width;
            player.Height = this.Height;
            player.Dock = DockStyle.Fill;
            player.Left = 0;
            player.Top = 0;
            player.settings.volume = 100;
            player.uiMode = "none";
            player.URL = "ainsleyharriott.mp4";
            player.ClickEvent += player_ClickEvent;
            
            

            
        }

        void player_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            Point p = new Point(0, 0);
            Cursor.Position = p; 
        }

       

        
        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            
            player.Ctlcontrols.play();
            this.player.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.player_PlayStateChange);
            
        }

        

        

        private delegate int LowLevelKeyboardProcDelegate(int nCode, int
           wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi)]
        private static extern int SetWindowsHookEx(
           int idHook,
           LowLevelKeyboardProcDelegate lpfn,
           int hMod,
           int dwThreadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(int hHook);

        [DllImport("user32.dll", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi)]
        private static extern int CallNextHookEx(
            int hHook, int nCode,
            int wParam, ref KBDLLHOOKSTRUCT lParam);

        const int WH_KEYBOARD_LL = 13;
        private int intLLKey;
        private KBDLLHOOKSTRUCT lParam;

        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            int scanCode;
            public int flags;
            int time;
            int dwExtraInfo;
        }

        private int LowLevelKeyboardProc(
            int nCode, int wParam,
            ref KBDLLHOOKSTRUCT lParam)
        {
            bool blnEat = false;
            switch (wParam)
            {
                case 256:
                case 257:
                case 260:
                case 261:
                    //Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key*
                    if (( (lParam.flags == 32)  && (lParam.vkCode == 0x09) ) ||      // Alt+Tab
         ( (lParam.flags == 32)  && (lParam.vkCode == 0x1B) ) ||      // Alt+Esc
         ( (lParam.flags == 0 )  && (lParam.vkCode == 0x1B) ) ||      // Ctrl+Esc
         ( (lParam.flags == 1 )  && (lParam.vkCode == 0x5B) ) ||      // Left Windows Key
         ( (lParam.flags == 1 )  && (lParam.vkCode == 0x5C) ) ||      // Right Windows Key
         ( (lParam.flags == 32)  && (lParam.vkCode == 0x73) ) ||      // Alt+F4              
         ( (lParam.flags == 32)  && (lParam.vkCode == 0x20))        // Alt+Space
         )        
                    {
                        blnEat = true;
                    }
                    break;
            }


            /*((lParam.vkCode == 9) && (lParam.flags == 32)) ||
                    ((lParam.vkCode == 27) && (lParam.flags == 32)) || ((lParam.vkCode ==
                    27) && (lParam.flags == 0)) || ((lParam.vkCode == 91) && (lParam.flags
                    == 1)) || ((lParam.vkCode == 92) && (lParam.flags == 1)) || ((true) &&
                    (lParam.flags == 32))*/





            if (blnEat)
                return 1;
            else return CallNextHookEx(0, nCode, wParam, ref lParam);

        }

        private void KeyboardHook(object sender, EventArgs e)
        {
            intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc),
                       System.Runtime.InteropServices.Marshal.GetHINSTANCE(
                       System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
        }

        private void ReleaseKeyboardHook()
        {
            intLLKey = UnhookWindowsHookEx(intLLKey);
        }
        /*
         * 
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            KeyboardHook(this, e);
            
            
        }

        private void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {

            if (player.playState != WMPPlayState.wmppsPlaying)
            {
                
                ReleaseKeyboardHook();
                Cursor.Show();
                Application.Exit();
            }
        }

    }
    


}
