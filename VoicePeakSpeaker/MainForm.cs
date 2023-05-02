using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Xml.Serialization;

namespace VoicePeakSpeaker
{
    public partial class MainForm : Form
    {
        // スタートアップに登録するには「Win+R」で「shell:startup」を入れてショートカットを作成する

        public static string outputDir = "";

        Setting? setting;
        VoicePeak? vp;
        Player? player;

        private ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void RemoveClipboardFormatListener(IntPtr hwnd);

        private const int WM_CLIPBOARDUPDATE = 0x31D;

        public MainForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            ControlBox = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Opacity = 1;
            this.TopMost = true;
            this.TopMost = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            notifyIcon1.Text = "Ready - VoicePeakSpeaker";

            //setting = new Setting();
            string settingFile = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + ".setting.xml";

            if (File.Exists(settingFile))
            {
                XmlSerializer se = new XmlSerializer(typeof(Setting));
                StreamReader sr = new StreamReader(settingFile, new System.Text.UTF8Encoding(false));
                setting = (Setting?)se.Deserialize(sr);
                sr.Close();
            } else
            {
                setting = new Setting();
            }

            vp = new VoicePeak(setting);
            player = new Player();

            VoicePeakTextBox.Text = vp.VoicePeakProgram;

            outputDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            try
            {
                NarratorComboBox.Items.Clear();
                List<string> narrators = vp.getNarrators();
                foreach (string narrator in narrators)
                {
                    NarratorComboBox.Items.Add(narrator);
                }
                if (setting.CurrentNarrator < narrators.Count())
                {
                    NarratorComboBox.SelectedIndex = setting.CurrentNarrator;
                } else
                {
                    NarratorComboBox.SelectedIndex = 0;
                }
                vp.currentNarrator = NarratorComboBox.SelectedItem.ToString();
            } catch(Exception ex)
            {

            }

            AddClipboardFormatListener(Handle);
        }

        private void showMainFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
            this.TopMost = true;
            this.TopMost = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Windows.Forms.Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 不要になったファイルを削除する
            string[] files = Directory.GetFiles(outputDir, "*.wav");
            foreach (string name in files)
            {
                System.IO.File.Delete(name);
            }

            this.Opacity = 0;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardUpdate();
                m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }

        void OnClipboardUpdate()
        {
            // クリップボードのデータが変更された
            if (Clipboard.ContainsText())
            {
                notifyIcon1.Text = "Ready - VoicePeakSpeaker";
                var msg = Clipboard.GetText();
                if (msg.StartsWith("http"))
                {
                    return;
                }
                else
                {
                    // 日本語を含まないメッセージを読み上げない
                    var isJapanese = Regex.IsMatch(msg, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+");
                    if (!isJapanese)
                    {
                        // 半角カタカナ
                        if (Regex.IsMatch(msg, @"[\uFF61-\uFF9F]")) isJapanese = true;
                    }
                    if (!isJapanese) return;

                    // 140文字以内の複数ブロックにわける
                    string[] messages = MySplit(msg, 120);

                    Thread t = new Thread(new ParameterizedThreadStart(this.generateThread));
                    t.Start(messages);

                    // 一個目の生成だけ待つ(二個目以降は生成のほうが先に終わる)
                    _waitHandle.Wait();
                    _waitHandle.Reset();

                    int count = 0;
                    foreach (string message in messages)
                    {
                        notifyIcon1.Text = $"Speaking({count + 1}/{messages.Count()}) - VoicePeakSpeaker";
                        try
                        {
                            player.PlaySound($"{outputDir}output{count}.wav");
                        }
                        catch (Exception e)
                        {

                        }
                        count++;
                    }

                    t.Join();
                    _waitHandle.Reset(); // 溜まってるかもしれないので開放しておく
                    notifyIcon1.Text = "Ready - VoicePeakSpeaker";
                }
            }
        }

        private void generateThread(Object obj)
        {
            string[] messages = (string[])obj;
            int count = 0;
            foreach (string message in messages)
            {
                notifyIcon1.Text = $"Generating({count + 1}/{messages.Count()}) - VoicePeakSpeaker";
                try
                {
                    bool ret = vp.generateWav(message, count);
                    _waitHandle.Set();
                    if (!ret)
                    {
                        notifyIcon1.Text = "Ready - VoicePeakSpeaker";
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
                count++;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveClipboardFormatListener(Handle);
        }

        // 拡張メソッド
        private string[] MySplit(string str, int count)
        {
            var list = new List<string>();
            int length = (int)Math.Ceiling((double)str.Length / count);

            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (str.Length <= start)
                {
                    break;
                }
                if (str.Length < start + count)
                {
                    list.Add(str.Substring(start));
                }
                else
                {
                    list.Add(str.Substring(start, count));
                }
            }

            return list.ToArray();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void NarratorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            setting.CurrentNarrator = NarratorComboBox.SelectedIndex;
            XmlSerializer se = new XmlSerializer(typeof(Setting));
            try
            {
                string settingFile = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + ".setting.xml";
                StreamWriter sw = new StreamWriter(settingFile, false, new System.Text.UTF8Encoding(false));
                se.Serialize(sw, setting);
                sw.Close();
            }
            catch (Exception ex)
            {

            }

            vp.currentNarrator = NarratorComboBox.SelectedItem.ToString();
        }

        private void VoicePeakTextBox_TextChanged(object sender, EventArgs e)
        {
            this.setting.VoicePeakProgram = VoicePeakTextBox.Text;
            XmlSerializer se = new XmlSerializer(typeof(Setting));
            try
            {
                string settingFile = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + ".setting.xml";
                StreamWriter sw = new StreamWriter(settingFile, false, new System.Text.UTF8Encoding(false));
                se.Serialize(sw, setting);
                sw.Close();
            } catch(Exception ex)
            {

            }

            vp.VoicePeakProgram = VoicePeakTextBox.Text;

            NarratorComboBox.Items.Clear();

            try
            {
                List<string> narrators = vp.getNarrators();
                if (narrators.Count == 0) return;

                foreach (string narrator in narrators)
                {
                    NarratorComboBox.Items.Add(narrator);
                }
                NarratorComboBox.SelectedIndex = setting.CurrentNarrator;
                vp.currentNarrator = NarratorComboBox.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
            }
        }

        private void VoicePeakReferenceButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.FileName = "";
                if (VoicePeakTextBox.Text.Length != 0)
                {
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(VoicePeakTextBox.Text);
                }
                dlg.Filter = "実行ファイル(*.exe;*.dll)|*.exe;*.dll";
                dlg.Title = "VoicePeakプログラム";     //タイトルを設定する
                dlg.RestoreDirectory = true;        //カレントディレクトリ復元
                dlg.Multiselect = false;            //複数選択可否

                //ダイアログを開く
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    //OKが返ってこない場合は、キャンセルされたとする。
                    return;
                }

                //選択したファイル名を取得する
                VoicePeakTextBox.Text = dlg.FileName;
                vp.VoicePeakProgram = dlg.FileName;

            }
        }
    }
}