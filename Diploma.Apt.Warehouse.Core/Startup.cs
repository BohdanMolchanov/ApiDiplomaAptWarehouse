using System;
using System.Text;
using AutoMapper;
using Diploma.Apt.Warehouse.Core.Data;
using Diploma.Apt.Warehouse.Core.Data.Entities.MongoDB;
using Diploma.Apt.Warehouse.Core.Data.Helpers.MongoDbConnection;
using Diploma.Apt.Warehouse.Core.Enums;
using Diploma.Apt.Warehouse.Core.Extensions.Mapper;
using Diploma.Apt.Warehouse.Core.Interfaces;
using Diploma.Apt.Warehouse.Core.Models.DataModel;
using Diploma.Apt.Warehouse.Core.Repositories;
using Diploma.Apt.Warehouse.Core.Services;
using Diploma.Apt.Warehouse.Core.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Diploma.Apt.Warehouse.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WarehouseContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("warehouseContext")));
            
            var userDbSettings = Configuration.GetSection("userContext").Get<MongoDbContextOptions<UserContext>>();
            // general context
            services.AddDbContext<UserContext>(o =>
            {
                o.MongoDbConnectionString =
                    userDbSettings.MongoDbConnectionString;
                //o.Mapping = userDbSettings.Mapping;
            }); 
            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new WarehouseProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountsService, AccountsService>();
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Secret").Value);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Diploma.Apt.Warehouse.Core", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(WarehouseContext warehouseContext, UserContext userContext, IMapper mapper, IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Diploma.Apt.Warehouse.Core v1"));
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware<JwtMiddleware>();
            warehouseContext.Database.MigrateAsync().Wait();
            userContext.MigrateAsync().Wait();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            var usersRepository = new UsersRepository(userContext, mapper);
            
            //seed mongodb with admin if not exist
            if (!usersRepository.ExistsAsync<UserEntity>(Guid.Parse("d98aa779-c640-443f-a594-3f9548f59b17")).Result)
                userContext.Users.InsertOneAsync(new UserEntity
                {
                    Id = Guid.Parse("d98aa779-c640-443f-a594-3f9548f59b17"),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Password = "32fhqtNSApFrwZHrDmrC+IVjLraYpHvwT2iVh4ZrVbM=",
                    Data = new UserDataModel
                    {
                        Email = "molchanovbohdan@gmail.com",
                        RoleType = RoleTypes.Admin
                    }
                }).Wait();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}