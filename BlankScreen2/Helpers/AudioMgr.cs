using System;
using System.Diagnostics;
using BlankScreen2.Model;
using CoreAudioApi;

namespace BlankScreen2.Helpers
{
	internal class AudioMgr
	{
		private MMDevice _MMDevice;
		private readonly AudioModel _AudioModel;
		private HideWindowsVolume _HideWindowsVolume;

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

		public void HideWindowsVolume(bool hideWindowsVolume)
		{
			if (_HideWindowsVolume == null)
				_HideWindowsVolume = new HideWindowsVolume();

			if (hideWindowsVolume)
				_HideWindowsVolume.HideOSD();
			else
				_HideWindowsVolume.ShowOSD();
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