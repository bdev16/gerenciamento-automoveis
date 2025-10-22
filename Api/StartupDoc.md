
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
* 
