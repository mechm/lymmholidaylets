using LymmHolidayLets.Api.GraphQL.Mutations;
using LymmHolidayLets.Api.GraphQL.Queries;

namespace LymmHolidayLets.Api.GraphQL
{
    public static class GraphQLServiceExtensions
    {
        public static IServiceCollection AddApiGraphQl(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                    .AddQueryType<CalendarQuery>()
                   // .AddMutationType<CalendarMutation>()
                    .AddFiltering()
                    .AddSorting()
                    .AddProjections();

            return services;
        }
    }
}
