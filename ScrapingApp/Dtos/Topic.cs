using System;
namespace ScrapingApp.Dtos
{
    public sealed class Topic : IEquatable<Topic>
    {
        public string Title { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Url { get; set; }

        public bool Equals(Topic other) => Title == other.Title;

        public override bool Equals(object obj) => Equals(obj as Topic);

        public override int GetHashCode() => Title.GetHashCode();
    }
}
