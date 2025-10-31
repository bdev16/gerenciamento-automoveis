## Construtor
```
    IConfiguration configuration
```
* Essa classe carrega o appsettings.json, variaveis de ambientes, argumentos de linhas de comandos e outras coisas

## ConfigureServices
Método nativo da ASPNET.Core que tem o intuito de registrar todos os serviços do APP .NET que está sendo desenvolvido. Aluguns deles sendo, configurações de endpoints, conexão com banco de dados, autorização e authenticação e etc...

```
    IServiceCollection services
```
* É uma interface que vai armazenar as definições de uma lista de serviços. Usada para acessar os metodos que vai servir para registrar alguns dos serviços das aplicação.

### Authenticação
```
    services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
```
* O AddAuthentication vai definir o serviço de authenticação e as suas configurações e o AddJwtBearer vai definir que o serviço de authenticação vai utilizar o JwtBearer com as definições especificas.

### Authorization  
```
    services.AddAuthorization();
```
* O AddAuthorization vai definir o serviço de autorização com as configurações padrões de um App .Net.

### Injeção de dependências (services)
```
    services.AddScoped<IAdministratorService, AdministratorService>();
    services.AddScoped<IVehicleService, VehicleService>();
```
* O services vai registrar serviços dentro do container de injeção de dependencia e o AddScoped vai definir que vão ser criados novas instancias desse serviço a cada requisição feita. A tipagem define que quando dentro da aplicação for chamado a Interface IAdministratorService na verdade vai ser chamado e instanciado uma instancia de AdministratorService e assim sucessivamente.

### Swagger (Interface de gerenciamento de endpoints)
```
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT aqui:"
        });
    
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    }
```
* Os codigos acima são responsáveis para fazer a configuração e inserção da interface do Swagger na aplicação. 
* O primeiro código define que o swagger pode descobrir de maneira automatica todos os endpoints existentes na aplicação.
* O segundo código serve para definir a documentação do swagger que seria a interface do Swagger. Os codigos inseridos dentro dessa aplicação estão relacionados a definição do esquema de segurança da aplicação que vão servir para poder utilizar dos enpoints disponiveis e fazer as requisições necessarias.
* O codigo define que o esquema de segurança utilizado vai ser o JWT Bearer.
* O codigo options.AddSecurityRequiriment vai definir uma interface dentro do swagger responsável por receber a chave JWT Bearer e liberar o acesso aos endpoints da aplicação para o usuario.

### Contexto (DB)
```
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(
            Configuration.GetConnectionString("sqlserver")
        );
    });
```
* Registra o contexto no container de injeção de dependencia da aplicação e faz as configurações voltadas ao uso do banco de dados, nesse caso o SqlServer. O codigo pega a string de conexão dentro do arquivo appsettings e utiliza tanto a classe AppDbContext e a string de conexão para entrar no banco de dados e fazer as operações definidas
  
### IApplicationBuilder; IWebHostEnvironment
* O **IApplicationBuilder** é a interface que define métodos que são necessários para configurar o pipeline de requisições HTTP da aplicação, definindo como os serviços que estão em execução vai continuar funcionando. O Pipeline de requisições é onde os middlewares, que são os componentes que recebem as requisições, ficam organizados e estruturadas dentro da aplicação, ele vai definir como as requisições vão ser tratadas, permissões, barreiras, etc....
* O **IWebHostEnvironment** é a interface que define métodos voltados para a configuração e fornece as informações sobre o ambiente ao qual a aplicação está sendo executada.

### Midlewares
* Nesse contexto, os middlewares são métodos responsáveis por processar as requisições que são feitas a aplicação, toda requisição feita passa pelos middlewares e eles vão fazer a captura das informações da requisição.
<img width="1024" height="1536" alt="ChatGPT Image 31_10_2025, 18_53_00" src="https://github.com/user-attachments/assets/de0ea284-667a-4a4a-a2e0-74d1be020dbb" />

```
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
```
* O Swagger é o middleware responsável por verificar se a requisição está sendo feita pelo swagger ou não e por fazer a documentação no formato json da aplicação, essa documentação contém todos os endpoints criados e gera um novo endpoint que vai ser utilizado pelo SwaggerUI, além de conter registrado as possiveis respostas que podem ser feitas por cada endpoint.
* O SwaggerUI é o middleware responsável por gerar uma ladding page interativa que vai representar a aplicação de maneira visual, contendo enpoints e alguns outros componentes que podem ser definidos.
* O UseRouting é o middleware responsável por verificar e capturar as informações sobre qual o enpoint que está sendo solicitado, qual rota está sendo solicitada. Ele so faz o mapeamento, mas não faz o direcionamento.
* O Use authentication é o middleware responsável por verificar se a requisição está sendo feita por um usuario logado ou não, esse middleware vai adquirir as informações sobre os usuarios, como o token de permissão, caso essas informações existam.
* O UseAuthorization é o middleware responsável por verificar se o usuario que está fazendo a requisição tem ou não authorização para acessar aquele enpoint.

```
app.UseEndpoints(endpoints =>
{    
```
* O UseEndpoints é responsável por capturar a requisição e direcionar essa requisição o seu respectivo endpoint que esteja mapeado.

```
SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
```
*
