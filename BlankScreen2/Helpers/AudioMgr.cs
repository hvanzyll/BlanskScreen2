using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CoreAudioApi;

namespace BlankScreen2.Helpers
{
	public delegate void VolumeUpdatedEventHandler(object? sender, VolumeUpdatedEventArgs e);

	internal sealed class AudioMgr
	{
		private MMDeviceCollection? _MMDeviceCollection;
		private HideWindowsVolume _HideWindowsVolume;
		private Thread _InitThread;
		private ConcurrentQueue<AudioVolumeNotificationData> _VolumeChangedQueue = new ConcurrentQueue<AudioVolumeNotificationData>();
		private EventWaitHandle _VolumeChangeEnqueued = new EventWaitHandle(false, EventResetMode.AutoReset);

		public event VolumeUpdatedEventHandler? VolumeUpdatedEvent;

		public AudioMgr()
		{
			_HideWindowsVolume = new HideWindowsVolume();

			_InitThread = new Thread(Init);
			_InitThread.IsBackground = true;
			_InitThread.Name = "AudioMgr";
			_InitThread.Start();
		}

		private void Init()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			_MMDeviceCollection = devEnum.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);
			for (int deviceIndex = 0; deviceIndex < _MMDeviceCollection.Count; deviceIndex++)
			{
				MMDevice device = _MMDeviceCollection[deviceIndex];
				device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
			}

			ProcessVolumeNotifications();
		}

		private void ProcessVolumeNotifications()
		{
			while (true)
			{
				_VolumeChangeEnqueued.WaitOne();
				while (_VolumeChangedQueue.TryDequeue(out AudioVolumeNotificationData? data) && data != null)
				{
					int volume = (int)(data.MMDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
					VolumeUpdatedEvent?.Invoke(this, new VolumeUpdatedEventArgs(volume, data.MMDevice.FriendlyName));
				}
			};
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
			_VolumeChangedQueue.Enqueue(data);
			_VolumeChangeEnqueued.Set();
		}
	}

	public class VolumeUpdatedEventArgs : EventArgs
	{
		public int Volume { get; private set; }
		public string DeviceName { get; private set; }

		public VolumeUpdatedEventArgs(int volume, string deviceName)
		{
			Volume = volume;
			DeviceName = deviceName;
		}
	}
}