public class RoomNode
{
    public int _id { get; set; }
    public int _roomTypeId { get; set; }
    public int _placeOnPath { get; set; }
    public RoomNode(int id, int roomTypeId, int placeOnPath) 
    {
        _id = id;
        _roomTypeId = roomTypeId;
        _placeOnPath = placeOnPath;
    }
}
