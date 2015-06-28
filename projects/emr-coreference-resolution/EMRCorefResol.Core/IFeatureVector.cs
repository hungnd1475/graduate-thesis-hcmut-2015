using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Core
{
	public interface IFeatureVector
	{
		double this[int index] { get; }
		int Size { get; }
		void Write(StreamWriter writer);
		void WriteLine(StreamWriter writer);
		double[] GetArray();
	}
}
