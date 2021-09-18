using System;
namespace ScrapingApp.Models
{
    public interface ISystemDate
    {
        DateTime GetSystemDate();
    }

    public class SystemDate : ISystemDate
    {
        public DateTime GetSystemDate() => DateTime.Now;
    }
}
