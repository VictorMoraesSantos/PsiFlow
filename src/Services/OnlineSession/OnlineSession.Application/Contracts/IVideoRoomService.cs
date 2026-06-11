using Core.Application.Interfaces;
using OnlineSession.Application.DTOs.VideoRoom;

namespace OnlineSession.Application.Contracts
{
    public interface IVideoRoomService :
        IReadService<VideoRoomDTO, int, VideoRoomFilterDTO>,
        ICreateService<CreateVideoRoomDTO>,
        IUpdateService<UpdateVideoRoomDTO>,
        IDeleteService<int>
    {
    }
}
