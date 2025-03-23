
namespace LostKit
{
    internal class WorldData
    {
        public string WorldName { get; set; }
        public int PlayerCount { get; set; }
        public string FlagUrl { get; set; }
        public int WorldId { get; set; }
        public string Country { get; internal set; } = "Not found";
        public bool Offline { get; internal set; }
        public Bitmap FlagImage { get; internal set; }
    }
}