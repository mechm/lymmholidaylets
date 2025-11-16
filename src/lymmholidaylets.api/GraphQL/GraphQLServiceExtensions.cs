using lymmholidaylets.api.GraphQL.Mutations;
using lymmholidaylets.api.GraphQL.Queries;

namespace lymmholidaylets.api.GraphQL
{
    public static class GraphQLServiceExtensions
    {
        public static IServiceCollection AddApiGraphQL(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                    .AddQueryType<CalendarQuery>()
                  //  .AddMutationType<CalendarMutation>()
                    .AddFiltering()
                    .AddSorting()
                    .AddProjections();

            return services;
        }
    }
}
