using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace VoicePeakSpeaker
{
    public partial class MainForm : Form
    {
        // �X�^�[�g�A�b�v�ɓo�^����ɂ́uWin+R�v�Łushell:startup�v�����ăV���[�g�J�b�g���쐬����

        public static string outputDir = "";

        private bool closeFromStripMenu = false;
        VoicePeak vp = new VoicePeak();
        Player player = new Player();

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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            notifyIcon1.Text = "Ready - VoicePeakSpeaker";

            VoicePeakTextBox.Text = vp.VoicePeakProgram;

            outputDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            List<string> narrators = vp.getNarrators();
            foreach (string narrator in narrators)
            {
                NarratorComboBox.Items.Add(narrator);
            }
            NarratorComboBox.SelectedIndex = 0;
            vp.currentNarrator = NarratorComboBox.SelectedItem.ToString();

            AddClipboardFormatListener(Handle);
        }

        private void showMainFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Windows.Forms.Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // �s�v�ɂȂ����t�@�C�����폜����
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
            // �N���b�v�{�[�h�̃f�[�^���ύX���ꂽ
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
                    // ���{����܂܂Ȃ����b�Z�[�W��ǂݏグ�Ȃ�
                    var isJapanese = Regex.IsMatch(msg, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+");
                    if (!isJapanese)
                    {
                        // ���p�J�^�J�i
                        if (Regex.IsMatch(msg, @"[\uFF61-\uFF9F]")) isJapanese = true;
                    }
                    if (!isJapanese) return;

                    // 140�����ȓ��̕����u���b�N�ɂ킯��
                    string[] messages = MySplit(msg, 120);
                    int count = 0;
                    foreach (string message in messages)
                    {
                        notifyIcon1.Text = $"Generating({count+1}/{messages.Count()}) - VoicePeakSpeaker";
                        try
                        {
                            bool ret = vp.generateWav(message, count);
                            if (!ret)
                            {
                                notifyIcon1.Text = "Ready - VoicePeakSpeaker";
                                return;
                            }
                        }
                        catch (Exception ex) {
                        }
                        count++;
                    }
                    count = 0;
                    foreach (string message in messages)
                    {
                        notifyIcon1.Text = $"Speaking({count + 1}/{messages.Count()}) - VoicePeakSpeaker";
                        try
                        {
                            player.PlaySound($"{outputDir}output{count}.wav");
                        } catch(Exception e)
                        {

                        }
                        count++;
                    }
                    notifyIcon1.Text = "Ready - VoicePeakSpeaker";
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveClipboardFormatListener(Handle);
        }

        // �g�����\�b�h
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
            vp.currentNarrator = NarratorComboBox.SelectedItem.ToString();
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
                dlg.Filter = "���s�t�@�C��(*.exe;*.dll)|*.exe;*.dll";
                dlg.Title = "VoicePeak�v���O����";     //�^�C�g����ݒ肷��
                dlg.RestoreDirectory = true;        //�J�����g�f�B���N�g������
                dlg.Multiselect = false;            //�����I����

                //�_�C�A���O���J��
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    //OK���Ԃ��Ă��Ȃ��ꍇ�́A�L�����Z�����ꂽ�Ƃ���B
                    return;
                }

                //�I�������t�@�C�������擾����
                VoicePeakTextBox.Text = dlg.FileName;
                vp.VoicePeakProgram = dlg.FileName;

            }
        }
    }
}