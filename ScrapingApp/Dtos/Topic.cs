using System;
namespace ScrapingApp.Dtos
{
    public sealed class Topic : IEquatable<Topic>
    {
        public string Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Url { get; set; }

        public bool Equals(Topic other)
        {
            if (other == null) return false;
            return Title == other.Title;
        }

        public override bool Equals(object obj)
        {
            if (obj is Topic sample) return Equals(sample);
            return false;
        }

        public override int GetHashCode() => 0;
    }
}
