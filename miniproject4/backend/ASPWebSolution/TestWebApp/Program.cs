namespace TestWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // HttpClient ���
            builder.Services.AddHttpClient("MyWebClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:8000"); //Python uvcorn ���� URL
            
            });

            //CORS ��å ��� --> ���ϸ� �ٸ� ����Ʈ ���� ����
            builder.Services.AddCors(options =>
            // �׽�Ʈ �ÿ��� ���, � ȯ�濡���� Ư�� �����θ�(������) ���� ����ϴ� �޼��常 ����ϵ��� �����ؾ� ��
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.AddControllers(); // ��Ʈ�ѷ� ���� ���
            var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");
            app.UseCors(); // ������ ������ CORS ��å�� ����ϰڴ�
            app.UseDefaultFiles(); // index.html ������ �ڵ� ó���ϰڴ�.
            app.UseStaticFiles(); // wwwroot ������ ���� ������ �����ϰڴ�.
            app.MapControllers(); // ��Ʈ�ѷ��� �����ϰڴ�.
            
            app.Run();
        }
    }
}
