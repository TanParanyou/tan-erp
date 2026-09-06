using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TanErp.Api.OpenApi;

public static class OpenApiConfiguration
{
    public static void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Project ERP (tan-erp) API",
            Version = "v1",
            Description = "Foundation Login and Current User API"
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };

        options.AddSecurityDefinition("Bearer", securityScheme);

        options.AddSecurityRequirement(doc =>
        {
            var requirement = new OpenApiSecurityRequirement();
            requirement.Add(new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>());
            return requirement;
        });
    }
}
