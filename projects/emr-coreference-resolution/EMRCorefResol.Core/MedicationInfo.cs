using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMUT.EMRCorefResol
{
    using Utilities;
    public class MedicationInfo : IEquatable<MedicationInfo>
    {
        public int LineIndex { get; }
        public string Line { get; }
        public string Drug { get; }
        public string Form { get; }
        public string Strength { get; }
        public string DoseAmount { get; }
        public string Route { get; }
        public string Frequency { get; }
        public string Duration { get; }
        public string Neccessity { get; }

        public MedicationInfo(int index, string line, string drug, string form,
            string strength, string dose, string route, string freq, string duration, string nec)
        {
            LineIndex = index;
            Line = line;
            Drug = drug;
            Form = form;
            Strength = strength;
            DoseAmount = dose;
            Route = route;
            Frequency = freq;
            Duration = duration;
            Neccessity = nec;
        }

        public bool Equals(MedicationInfo other)
        {
            return  int.Equals(LineIndex, other.LineIndex) &&
                string.Equals(Line, other.Line) &&
                string.Equals(Drug, other.Drug) &&
                string.Equals(Form, other.Form) &&
                string.Equals(Strength, other.Strength) &&
                string.Equals(DoseAmount, other.DoseAmount) &&
                string.Equals(Route, other.Route) &&
                string.Equals(Frequency, other.Frequency) &&
                string.Equals(Duration, other.Duration) &&
                string.Equals(Neccessity, other.Neccessity);
        }

        public override bool Equals(object obj)
        {
            var c = obj as MedicationInfo;
            return (c != null) ? Equals(c) : false;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ComputeHashCode(Line, Drug);
        }
    }
}
