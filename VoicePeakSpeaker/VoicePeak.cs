using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicePeakSpeaker
{
    public class VoicePeak
    {
        public string VoicePeakProgram = @"D:\Program Files\VOICEPEAK\voicepeak.exe";
        public string currentNarrator = "";

        public VoicePeak() {
        }

        // 後でリストで選択できるようにする
        public List<string> getNarrators()
        {
            //Processオブジェクトを作成
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = VoicePeakProgram;
            //出力を読み取れるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            //コマンドラインを指定（"/c"は実行後閉じるために必要）
            p.StartInfo.Arguments = @"--list-narrator";

            //起動
            p.Start();

            //出力を読み取る
            string results = p.StandardOutput.ReadToEnd();

            //プロセス終了まで待機する
            //WaitForExitはReadToEndの後である必要がある
            //(親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();
            p.Close();

            //出力された結果を表示
            Console.WriteLine(results);
            string[] arr = results.Split(Environment.NewLine);
            arr = arr.Where(x => x.Length != 0).ToArray();
            return new List<string>(arr);
        }

        // ここではすでに140文字以内になっているものとする
        public bool generateWav(string msg, int n)
        {
            //string narrator = @"Japanese Female Child";

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = VoicePeakProgram;
            p.StartInfo.Arguments = $"-s \"{msg}\" -o \"{MainForm.outputDir}output{n}.wav\" -n \"{currentNarrator}\"";
            var result = p.Start();
            if (result == false)
            {
                return false;
            }

            p.WaitForExit(20000);
            bool ret;
            if (p.HasExited)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
            p.Close();
            return ret;
        }
    }
}
