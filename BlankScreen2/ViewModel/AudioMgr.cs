using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BlankScreen2.Model;
using CoreAudioApi;

namespace BlankScreen2.ViewModel
{
	internal class AudioMgr
	{
		private MMDevice _MMDevice;
		private readonly AudioModel _AudioModel;

		public AudioModel AudioModel => _AudioModel;

		public AudioMgr(AudioModel audioModel)
		{
			_AudioModel = audioModel;
			UpdateVolume();
		}

		public void UpdateVolume()
		{
			AudioModel.Volume = GetVolume();
		}

		private void InitMMDevice()
		{
			if (_MMDevice == null)
			{
				MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
				_MMDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
				_MMDevice.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
			}
		}

		private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
		{
			UpdateVolume();
		}

		private int GetVolume()
		{
			try
			{
				if (_MMDevice == null)
					InitMMDevice();
				int nRet = (int)(_MMDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

				return nRet;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return 0;
			}
		}
	}
}