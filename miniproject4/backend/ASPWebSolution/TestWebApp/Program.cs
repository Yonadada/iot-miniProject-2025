namespace TestWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // HttpClient 등록
            builder.Services.AddHttpClient("MyWebClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:8000"); //Python uvcorn 서비스 URL
            
            });

            //CORS 정책 등록 --> 안하면 다른 사이트 접속 못함
            builder.Services.AddCors(options =>
            // 테스트 시에만 허용, 운영 환경에서는 특정 도메인만(아이피) 실제 사용하는 메서드만 허용하도록 변경해야 함
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.AddControllers(); // 컨트롤러 서비스 등록
            var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");
            app.UseCors(); // 위에서 설정한 CORS 정책을 사용하겠다
            app.UseDefaultFiles(); // index.html 파일을 자동 처리하겠다.
            app.UseStaticFiles(); // wwwroot 폴더의 정적 파일을 제공하겠다.
            app.MapControllers(); // 컨트롤러를 매핑하겠다.
            
            app.Run();
        }
    }
}
