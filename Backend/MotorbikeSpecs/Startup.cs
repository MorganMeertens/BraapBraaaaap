using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.GraphQL.BraapUsers;
using MotorbikeSpecs.GraphQL.Companies;
using MotorbikeSpecs.GraphQL.Motorbikes;
using MotorbikeSpecs.GraphQL.Reviews;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace MotorbikeSpecs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; } = default!;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPooledDbContextFactory<BraapDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidIssuer = "BraapBraaaap",
                            ValidAudience = "BraapBraaaap-User",
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = signingKey
                        };
                });



            services.AddAuthorization();

            services.AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                    .AddTypeExtension<BraapUserQueries>()
                    .AddTypeExtension<MotorbikeQueries>()
                    .AddTypeExtension<CompanyQueries>()
                    .AddTypeExtension<ReviewQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddTypeExtension<MotorbikeMutations>()
                    .AddTypeExtension<CompanyMutations>()
                    .AddTypeExtension<BraapUserMutations>()
                    .AddTypeExtension<ReviewMutations>()
                .AddType<BraapUserType>()
                .AddType<ReviewType>()
                .AddType<CompanyType>()
                .AddType<MotorbikeType>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
