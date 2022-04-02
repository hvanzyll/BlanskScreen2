namespace BlankScreen2.Model
{
	public sealed class AudioModel : NotifyPropertyChanged
	{
		private int _Volume;
		private string? _DeviceName;

		public int Volume { get => _Volume; set => SetField(ref _Volume, value); }
		public string? DeviceName { get => _DeviceName; set => SetField(ref _DeviceName , value); }
	}
}