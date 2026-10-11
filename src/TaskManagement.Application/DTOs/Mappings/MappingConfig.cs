using Mapster;
using TaskManagement.Application.Common.Extensions;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaskCreateDto, TaskItem>()
            .Map(dest => dest.Title, src => src.Title.Trim())
            .Map(dest => dest.Date, src => src.Date.ToUtc());

        config.NewConfig<TaskUpdateDto, TaskItem>()
            .Map(dest => dest.Title, src => src.Title.Trim())
            .Map(dest => dest.Date, src => src.Date.ToUtc());

        config.NewConfig<TaskItem, TaskDto>();
    }
}