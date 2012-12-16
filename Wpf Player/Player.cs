using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;

namespace Wpf_Player
{
    class Player
    {
        int stream;
        bool playing, paused,fxChanged;
        private int _fxChorusHandle = 0;
        private BASS_DX8_CHORUS _chorus = new BASS_DX8_CHORUS(0f, 25f, 90f, 5f, 1, 0f, BASSFXPhase.BASS_FX_PHASE_NEG_90);
        private int _fxEchoHandle = 0;
        private BASS_DX8_ECHO _echo = new BASS_DX8_ECHO(90f, 50f, 500f, 500f, false);
        private int[] _fxEQ = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] _fxEQcopy = null;
        public Player()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero);
            
            playing = false;
            paused = false;
            fxChanged = false;
        }
        #region accessors
        public bool Playing
        {
            set { playing = value; }
            get { return playing; }
        }
        public bool Paused
        {
            set { paused = value; }
            get { return paused; }
        }
        public int Stream
        {
            get { return stream; }
        }
        public int CurrentPos
        {
            get { return CurrentPossition(); }
        }
        #endregion
        #region methods
        public void LoadSong(string location)
        {
            stream = Bass.BASS_StreamCreateFile(location, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
            SetFXParameters();
        }
        private void SetFXParameters()
        {
            _fxChorusHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_CHORUS, 1);
            _chorus.fWetDryMix = 0f;
            //trackBarChorus.Value = 0;
            Bass.BASS_FXSetParameters(_fxChorusHandle, _chorus);

            _fxEchoHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_ECHO, 2);
            _echo.fWetDryMix = 0f;
            //trackBarEcho.Value = 0;
            Bass.BASS_FXSetParameters(_fxEchoHandle, _echo);
            // tu gdzies jest blad :P 
            // 3-band EQ
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            _fxEQ[0] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[1] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[2] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[3] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[4] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[5] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[6] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[7] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[8] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[9] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            
            eq.fBandwidth = 18f;

            eq.fCenter = 50f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[0], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 100f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[1], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 200f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[2], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 400f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[3], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 700f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[4], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 1000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[5], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 2000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[6], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 4000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[7], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 6000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[8], eq);
            eq.fBandwidth = 18f;
            eq.fCenter = 8000f;
            eq.fGain = 0 / 10f;
            Bass.BASS_FXSetParameters(_fxEQ[9], eq);
            fxChanged = true;
            
        }
        public void SetEchoAndChorus(float value)
        {
            _echo.fWetDryMix = value;
            Bass.BASS_FXSetParameters(_fxEchoHandle, _echo);
            fxChanged = true;
        }
        public void PlaySong()
        {

  
            
            
            Bass.BASS_ChannelPlay(stream, false);
            SetVolume(0);
            SetVolume(100);
        }
        public void UpdateEQ(int band, float gain)
        {
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            if (Bass.BASS_FXGetParameters(_fxEQ[band], eq))
            {
                eq.fGain = gain;
                Bass.BASS_FXSetParameters(_fxEQ[band], eq);
            }
        }
        public void StopSong()
        {
            Bass.BASS_ChannelStop(stream);
        }

        public void PauseSong()
        {
            Bass.BASS_ChannelPause(stream);
        }
        public void SeekSong(double seconds)
        {
            Bass.BASS_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, seconds));
           
        }
        public int CurrentPossition()
        {
            return  (int)Bass.BASS_ChannelBytes2Seconds(stream,Bass.BASS_ChannelGetPosition(stream));
        }
        
        public void SetVolume(float value)
        {
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, value/100);
        }
        public void SetBalance(float value)
        {
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_PAN, value / 100);
        }
        public void Mute(bool trigger,float volume)
        {
            
            if (trigger==true)
            {
         
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volume);
                
            }
            else
            {
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volume/100);
            }

        }
        ~Player()
        {
            Bass.BASS_Free();
        }
        #endregion 
    }
}
