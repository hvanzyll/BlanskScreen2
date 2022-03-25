using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankScreen2.Model
{
	public sealed class AudioModel : NotifyPropertyChanged
	{
		private int _Volume;

		public int Volume { get => _Volume; set => SetField(ref _Volume, value); }
	}
}