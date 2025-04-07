using FC.CodeFlix.Catalog.API.Extensions.String;
using System.Text.Json;

namespace FC.CodeFlix.Catalog.API.Configurations.Policies
{
    public class JsonSnakeCasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToSnakeCase();
    }
}
