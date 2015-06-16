using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.Shared
{
    public class Concept
    {
		public string Lexicon { get; private set; }
		public Position Begin { get; private set; }
		public Position End { get; private set; }
		public EntityType Type { get; private set; }

		public Concept(string lexicon, Position begin, Position end, EntityType type)
		{
			this.Lexicon = lexicon;
			this.Begin = begin;
			this.End = end;
			this.Type = type;
		}
    }
}
