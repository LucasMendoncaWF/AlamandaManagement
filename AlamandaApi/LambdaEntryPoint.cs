using Amazon.Lambda.AspNetCoreServer;

namespace AlamandaApi {
  public class LambdaEntryPoint : APIGatewayProxyFunction {
    protected override void Init(IWebHostBuilder builder) {
      var envFileName = Environment.GetEnvironmentVariable("DOTNET_ENV_FILE") ?? ".env";
      DotNetEnv.Env.Load(envFileName);
      builder.UseStartup<Startup>();
    }
  }
}
