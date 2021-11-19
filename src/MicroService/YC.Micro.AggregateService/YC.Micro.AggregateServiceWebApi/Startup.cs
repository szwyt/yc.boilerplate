using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YC.Micro.AggregateServiceWebApi.AopModule;
using YC.Micro.AggregateServiceWebApi.ServiceCollectionExtensions;
using YC.Micro.Configuration;
using YC.Micro.Consul.ServiceRegister.Extentions;

namespace YC.Micro.AggregateServiceWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultConfig.JsonConfig = DefaultConfig.GetConfigJson(DefaultConfig.dbConfigFilePath);
          
            services.AddControllers();

            services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", options =>
               {
                   options.Authority = "https://localhost:5001";
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateAudience = false
                   };
               });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", builder =>
                {
                    builder.RequireAuthenticatedUser();//��Ҫodic �����֤
                    builder.RequireClaim("scope", "test_api");//��Ҫ��Ӧ����
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YC.Micro.AggregateServiceWebApi", Version = "v1" });
                //���Jwt��֤����
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "Bearer",
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                new List<string>()
                            }
                        });
                //swagger �Ǳߵ�ֱֵ����д tokenֵ����ҪдBearer token������
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Value: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // TODO:һ��Ҫ����true��
                c.DocInclusionPredicate((docName, description) => true);
            });

            #region Autofac IOC ע��

            var builder = new ContainerBuilder();

            //�Զ���ע��
            builder.RegisterModule(new CustomAutofacModule());

            //automapper ע��
            builder.RegisterModule(new AutoMapperAutofacModule());

            //freesql ע��
            builder.RegisterModule(new FreesqlAutofacModule());

            //es ע��
            builder.RegisterModule(new ElasticSearchAutofacModule());

            //GRpc ����ע��
            services.AddGrpcModule();
            services.AddServiceRegister(); //consul ע�����
            var idle = services.AddTenantDb();//�⻧ע��
            builder.RegisterInstance(idle).SingleInstance();//����ע��
            //����ע�봦��
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());//ע�뵱ǰ�̳���
            builder.Populate(services);// ������ص�
            var container = builder.Build();

            #endregion Autofac IOC ע��

            return new AutofacServiceProvider(container);//�Ǿͷ���Ĭ�ϵ�ע��ģʽ
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YC.Micro.AggregateServiceWebApi v1"));
            }

            app.UseRouting();

            //�����֤
            app.UseAuthentication();
            app.UseAuthorization();
            // ��ӽ������·�ɵ�ַ
            app.Map("/HealthCheck", HealthMap);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void HealthMap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("OK");
            });
        }
    }
}