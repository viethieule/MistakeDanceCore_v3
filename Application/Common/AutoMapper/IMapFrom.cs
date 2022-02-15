using AutoMapper;

namespace Application.Common.AutoMapper
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile);
    }
}