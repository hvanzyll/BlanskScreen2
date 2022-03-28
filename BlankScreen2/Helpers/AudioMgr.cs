using System;
using System.ComponentModel;
using System.Diagnostics;
using BlankScreen2.Model;
using CoreAudioApi;

namespace BlankScreen2.Helpers
{
	public delegate void VolumeUpdatedEventHandler(object? sender, VolumeUpdatedEventArgs e);

	internal class AudioMgr
	{
		private MMDevice _MMDevice;
		private HideWindowsVolume _HideWindowsVolume;

		public event VolumeUpdatedEventHandler? VolumeUpdatedEvent;

		public AudioMgr()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			_MMDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			_MMDevice.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);

			_HideWindowsVolume = new HideWindowsVolume();
		}

		public void UpdateVolume()
		{
			GetVolume();
		}

		public void HideWindowsVolume(bool hideWindowsVolume)
		{
			if (hideWindowsVolume)
				_HideWindowsVolume.HideOSD();
			else
				_HideWindowsVolume.ShowOSD();
		}

		private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
		{
			GetVolume();
		}

		private int GetVolume()
		{
			try
			{
				int volume = (int)(_MMDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);

				VolumeUpdatedEvent?.Invoke(this, new VolumeUpdatedEventArgs(volume));
				return volume;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return 0;
			}
		}
	}

	public class VolumeUpdatedEventArgs : EventArgs
	{
		public int Volume { get; private set; }

		public VolumeUpdatedEventArgs(int volume)
		{
			Volume = volume;
		}
	}
}