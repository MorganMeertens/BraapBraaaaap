﻿
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Octokit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using HotChocolate.AspNetCore;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;
using HotChocolate.AspNetCore.Authorization;

namespace MotorbikeSpecs.GraphQL.BraapUsers
{
    [ExtendObjectType(name: "Mutation")]
    public class BraapUserMutations
    {
        /* [UseBraapDbContext]
         public async Task<BraapUser> AddBraapUserAsync(AddBraapUserInput input,
         [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
         {
             var braapuser = new BraapUser
             {
                 UserName = input.UserName,
                 GitHub = input.GitHub,
                 ImageURI = input.ImageURI,
             };

             context.BraapUsers.Add(braapuser);
             await context.SaveChangesAsync(cancellationToken);

             return braapuser;
         }

         [UseBraapDbContext]
         [Authorize]
         public async Task<BraapUser> EditBraapUserAsync(EditBraapUserInput input, ClaimsPrincipal claimsPrincipal,
                 [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
         {

             var braapuserIdStr = claimsPrincipal.Claims.First(c => c.Type == "braapuserId").Value;
             var braapuser = await context.BraapUsers.FindAsync(int.Parse(braapuserIdStr),cancellationToken);

             braapuser.UserName = input.UserName ?? braapuser.UserName;
             braapuser.ImageURI = input.ImageURI ?? braapuser.ImageURI;

             await context.SaveChangesAsync(cancellationToken);

             return braapuser;
         }*/

        [UseBraapDbContext]
        [Authorize]
        public async Task<BraapUser> EditSelfAsync(EditSelfInput input, ClaimsPrincipal claimsPrincipal,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var braapuserIdStr = claimsPrincipal.Claims.First(c => c.Type == "braapuserId").Value;
            var braapuser = await context.BraapUsers.FindAsync(int.Parse(braapuserIdStr), cancellationToken);

            braapuser.UserName = input.UserName ?? braapuser.UserName;
            braapuser.ImageURI = input.ImageURI ?? braapuser.ImageURI;

            context.BraapUsers.Add(braapuser);
            await context.SaveChangesAsync(cancellationToken);

            return braapuser;
        }





        [UseBraapDbContext]
        public async Task<LoginPayload> LoginAsync(LoginInput input, [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var client = new GitHubClient(new ProductHeaderValue("BraapBraaaap"));

            var request = new OauthTokenRequest(Startup.Configuration["Github:ClientId"], Startup.Configuration["Github:ClientSecret"], input.Code);
            var tokenInfo = await client.Oauth.CreateAccessToken(request);

            if (tokenInfo.AccessToken == null)
            {
                throw new GraphQLRequestException(ErrorBuilder.New()
                    .SetMessage("Bad code")
                    .SetCode("AUTH_NOT_AUTHENTICATED")
                    .Build());
            }

            client.Credentials = new Credentials(tokenInfo.AccessToken);
            var user = await client.User.Current();

            var braapuser = await context.BraapUsers.FirstOrDefaultAsync(s => s.GitHub == user.Login, cancellationToken);

            if (braapuser == null)
            {

                braapuser = new BraapUser
                {
                    UserName = user.Name ?? user.Login,
                    GitHub = user.Login,
                    ImageURI = user.AvatarUrl,
                };

                context.BraapUsers.Add(braapuser);
                await context.SaveChangesAsync(cancellationToken);
            }

            // authentication successful so generate jwt token
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>{
                new Claim("braapuserId", braapuser.Id.ToString()),
            };

            var jwtToken = new JwtSecurityToken(
                "BraapBraaaap",
                "BraapBraaaap-User",
                claims,
                expires: DateTime.Now.AddDays(90),
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new LoginPayload(braapuser, token);
        }



    }
}
