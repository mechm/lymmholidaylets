using LymmHolidayLets.Api.Models.Property;
using LymmHolidayLets.Application.Model.Property;

namespace LymmHolidayLets.Api.Services;

public interface IPropertyDetailResponseBuilder
{
    PropertyDetailResponse Build(PropertyDetailResult detail);
}
