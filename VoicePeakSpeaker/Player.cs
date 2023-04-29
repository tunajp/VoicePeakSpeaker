using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoicePeakSpeaker
{
    public class Player
    {
        // NOTE: nugetでMicrosoft.Windows.Compatibilityを追加する必要があります
        private static System.Media.SoundPlayer? player = null;

        public Player() { }
        public void PlaySound(string wavFile)
        {
            if (OperatingSystem.IsWindows())
            {
                //再生されているときは止める
                if (player != null)
                    StopSound();

                //読み込む
                player = new System.Media.SoundPlayer(wavFile);

                //非同期再生する
                //player.Play();

                //次のようにすると、ループ再生される
                //player.PlayLooping();

                //次のようにすると、最後まで再生し終えるまで待機する
                player.PlaySync();
            }
        }
        private void StopSound()
        {
            if (OperatingSystem.IsWindows())
            {
                if (player != null)
                {
                    player.Stop();
                    player.Dispose();
                    player = null;
                }
            }
        }

    }
}
