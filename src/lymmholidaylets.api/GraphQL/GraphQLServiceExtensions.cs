using LymmHolidayLets.Api.GraphQL.Queries;

namespace LymmHolidayLets.Api.GraphQL
{
    public static class GraphQLServiceExtensions
    {
        public static void AddApiGraphQl(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                    .AddQueryType<Query>()
                    .AddType<CalendarQuery>()
                    .AddType<PropertyQuery>()
                    .AddType<PageQuery>()
                    .AddFiltering()
                    .AddSorting()
                    .AddProjections();
        }
    }
}
