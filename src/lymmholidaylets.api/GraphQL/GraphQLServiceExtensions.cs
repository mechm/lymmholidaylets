using LymmHolidayLets.Api.GraphQL.Queries;

namespace LymmHolidayLets.Api.GraphQL
{
    public static class GraphQLServiceExtensions
    {
        public static IServiceCollection AddApiGraphQl(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                    .AddQueryType<Query>()
                    .AddType<CalendarQuery>()
                    .AddType<PropertyQuery>()
                    .AddFiltering()
                    .AddSorting()
                    .AddProjections();

            return services;
        }
    }
}
