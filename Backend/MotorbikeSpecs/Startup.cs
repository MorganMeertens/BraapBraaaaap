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


namespace MotorbikeSpecs
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
            services.AddPooledDbContextFactory<BraapDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
