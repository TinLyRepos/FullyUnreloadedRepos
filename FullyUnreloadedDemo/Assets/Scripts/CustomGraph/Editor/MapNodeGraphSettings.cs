
namespace MapNodeGraph
{
    public static class MapGraph
    {
        // Grid spacing
        public const float GRID_OPACITY = 0.2f;
        public const ushort GRID_LARGE = 100;
        public const ushort GRID_SMALL = 25;

        // Define node layout styles with less repetition
        public enum NODEKEY : int
        {
            Default,
            DefaultOn,
            Entrance,
            EntranceOn,
            Boss,
            BossOn,
        }

        public static readonly string[] NODE_TEXTURES = 
        {
            "node1",
            "node1 on",
            "node3",
            "node3 on",
            "node6",
            "node6 on"
        };

        // Node style values
        public const ushort NODE_WIDTH = 160;
        public const ushort NODE_HEIGHT = 72;
        public const ushort NODE_PADDING = 24;
        public const ushort NODE_BORDER = 12;

        // Node connect line values
        public const ushort CONNECT_LINE_THICKNESS = 3;
        public const ushort CONNECT_LINE_ARROW_SIZE = 6;
    }
}
