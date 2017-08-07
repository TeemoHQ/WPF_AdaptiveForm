using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.Common
{
    public class VoiceHelper
    {
        private static SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        public VoiceHelper(int volume,int rate)
        {
            _speechSynthesizer.Rate = rate;
            _speechSynthesizer.Rate = volume;
        }
        private void Speech(string str)
        {
           _speechSynthesizer.Speak(str);
        }
    }
}
