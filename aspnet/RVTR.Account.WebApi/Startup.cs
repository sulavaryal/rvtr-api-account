using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RVTR.Account.DataContext;
using RVTR.Account.DataContext.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;
using zipkin4net.Middleware;

namespace RVTR.Account.WebApi
{
  /// <summary>
  ///
  /// </summary>
  public class Startup
  {
    /// <summary>
    ///
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddApiVersioning(options =>
      {
        options.ReportApiVersions = true;
      });

      services.AddControllers().AddNewtonsoftJson(options=>
      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

      services.AddCors(cors =>
      {
        cors.AddPolicy("Public", policy =>
        {
          policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
      });

      services.AddDbContext<AccountContext>(options =>
      {
        options.UseNpgsql(_configuration.GetConnectionString("pgsql"), options =>
        {
          options.EnableRetryOnFailure(3);
        });
      });

      services.AddScoped<ClientZipkinMiddleware>();
      services.AddScoped<UnitOfWork>();
      services.AddSwaggerGen();
      services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ClientSwaggerOptions>();
      services.AddVersionedApiExplorer(options =>
      {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
      });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="descriptionProvider"></param>
    /// <param name="applicationBuilder"></param>
    /// <param name="hostEnvironment"></param>
    public void Configure(IApiVersionDescriptionProvider descriptionProvider, IApplicationBuilder applicationBuilder, IWebHostEnvironment hostEnvironment)
    {
      if (hostEnvironment.IsDevelopment())
      {
        applicationBuilder.UseDeveloperExceptionPage();
      }

      applicationBuilder.UseZipkin();
      applicationBuilder.UseTracing("accountapi.rest");
      applicationBuilder.UseHttpsRedirection();
      applicationBuilder.UseRouting();
      applicationBuilder.UseSwagger();
      applicationBuilder.UseSwaggerUI(options =>
      {
        foreach (var description in descriptionProvider.ApiVersionDescriptions)
        {
          options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        }
      });

      applicationBuilder.UseCors();
      applicationBuilder.UseAuthorization();


      applicationBuilder.UseEndpoints(endpoints =>
      {
        endpoints.MapGet("/", context => {
          context.Response.Redirect("/swagger/");
          return Task.CompletedTask;
        });
        endpoints.MapControllers();
      });
    }
  }
}
