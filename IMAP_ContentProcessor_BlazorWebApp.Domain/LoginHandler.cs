using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using IMAP_ContentProcessor_BlazorWebApp.Domain.Models;
using IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAP_ContentProcessor_BlazorWebApp.Domain
{
    public class LoginHandler : ILoginHandler
    {
        private readonly ZamowieniaImapContext context;
        private readonly IConfiguration configuration;
        private readonly string? hostMail;

        public LoginHandler(ZamowieniaImapContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
            this.hostMail = configuration["hostMail"];
        }

        public async Task GetOrdersInformation(AuthenticateResult authenticateResult)
        {
            if (!authenticateResult.Succeeded || authenticateResult.Properties?.GetTokenValue("access_token") == null)
            {
                return;
            }

            List<MailContent> EmailList = new List<MailContent>();
            var gmailService = await GetGmailService(authenticateResult);

            UsersResource.MessagesResource.ListRequest ListRequest = gmailService.Users.Messages.List(hostMail);
            ListRequest.LabelIds = "INBOX";
            ListRequest.IncludeSpamTrash = false;
            ListRequest.Q = "is:unread";

            ListMessagesResponse ListResponse = ListRequest.Execute();

            //MarkMessageAsRead(gmailService, ListResponse.Messages.Select(x => x.Id).ToList());

            foreach (var messageData in ListResponse.Messages)
            {
                UsersResource.MessagesResource.GetRequest messageGmail = gmailService.Users.Messages.Get(hostMail, messageData.Id);

                var messageDetail = messageGmail.Execute();

                if (!(messageDetail.Payload.Headers.FirstOrDefault(x => x.Name.Equals("From"))?.Value == "sklep@example.com")
                    && !messageDetail.Snippet.Contains("Simplelightcandle"))
                {
                    continue;
                }

                //await SaveAttachments(messageDetail, gmailService);
            }
        }

        private void MarkMessageAsRead(GmailService gmailService, List<string> messageIds)
        {
            BatchModifyMessagesRequest mods = new();
            mods.Ids = messageIds;
            mods.RemoveLabelIds = new List<string> { "UNREAD" };
            gmailService.Users.Messages.BatchModify(mods, hostMail).Execute();
        }

        private async Task SaveAttachments(Message message, GmailService gmailService)
        {
            try
            {
                IList<MessagePart> parts = message.Payload.Parts;

                foreach (MessagePart part in parts)
                {
                    if (!String.IsNullOrEmpty(part.Filename))
                    {
                        MessagePartBody attachPart = gmailService.Users.Messages.Attachments.Get(hostMail, message.Id, part.Body.AttachmentId).Execute();

                        byte[] data = Base64ToByte(attachPart.Data);

                        context.Set<Mail>().Add(new Mail
                        {
                            Eml = data
                        });

                        await context.SaveChangesAsync();
                    }
                    //File.WriteAllBytes(Path.Combine("C:\\Users\\admin\\Desktop\\gmail\\zamowienia", part.Filename), data);              
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        private static byte[] Base64ToByte(string base64Test)
        {
            string encodedText = string.Empty;
            encodedText = base64Test.Replace("-", "+");
            encodedText = encodedText.Replace("_", "/");
            encodedText = encodedText.Replace(" ", "+");

            return Convert.FromBase64String(encodedText);
        }

        private async Task<string> RegisterRefreshToken(AuthenticateResult authenticateResult)
        {
            var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");// ?? configuration["refreshtoken"];

            if (refreshToken == null)
            {
                refreshToken = await context.Set<Token>().Where(x => x.Email == hostMail)
                    .Select(x => x.RefreshToken)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var token = await context.Set<Token>().Where(x => x.Email == hostMail).FirstOrDefaultAsync();

                if (token is null)
                {
                    context.Set<Token>().Add(new Token
                    {
                        Email = hostMail,
                        RefreshToken = refreshToken,
                    });
                }
                else
                {
                    context.Set<Token>().Where(x => x.Email == hostMail)
                        .ExecuteUpdate(x => x.SetProperty(y => y.RefreshToken, refreshToken));
                }
                context.SaveChanges();
            }

            return refreshToken!;
        }

        private async Task<GmailService> GetGmailService(AuthenticateResult authenticateResult)
        {
            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
            var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");// ?? configuration["refreshtoken"];

            if (refreshToken == null)
            {
                refreshToken = await context.Set<Token>().Where(x => x.Email == hostMail)
                    .Select(x => x.RefreshToken)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var token = await context.Set<Token>().Where(x => x.Email == hostMail).FirstOrDefaultAsync();

                if (token is null)
                {
                    context.Set<Token>().Add(new Token
                    {
                        Email = hostMail,
                        RefreshToken = refreshToken,
                    });
                }
                else
                {
                    context.Set<Token>().Where(x => x.Email == hostMail)
                        .ExecuteUpdate(x => x.SetProperty(y => y.RefreshToken, refreshToken));
                }
                context.SaveChanges();
            }

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
                {
                    ClientId = configuration["Authentication:Google:ClientId"],
                    ClientSecret = configuration["Authentication:Google:ClientSecret"]
                },
                Scopes = new[] { GmailService.Scope.GmailCompose, GmailService.Scope.GmailReadonly, GmailService.Scope.GmailModify },
            });
            
            var credential = new Google.Apis.Auth.OAuth2.UserCredential(flow, authenticateResult.Principal.Identity.Name, new Google.Apis.Auth.OAuth2.Responses.TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer"
            });

            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Zamowienia",
            });
        }
    }
}
