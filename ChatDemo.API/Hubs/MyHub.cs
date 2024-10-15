using ChatDemo.API.DbContext;
using ChatDemo.API.Hubs.HubInterface;
using ChatDemo.API.Models;
using ChatDemo.API.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace ChatDemo.API.Hubs
{
    public class MyHub : Hub<IMessageClient>
    {
        static Dictionary<string,string> clients = new(); //username , connid
        static List<string> groups = new();

        AppDbContext _db;
        UserManager<AppUser> _userManager;

        public MyHub(AppDbContext dbContext,UserManager<AppUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        public async Task matchConnIdAndUser(string connId,string username)
        {

            var user = await _db.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(user.ConnectionId)) clients.Remove(username); /*clients.RemoveAll(x => x.Equals(user.ConnectionId));*/

            clients.Add(username,connId);
            user.ConnectionId = connId;

            await _db.SaveChangesAsync();

            await Clients.All.RecieveClients(clients.Keys.ToList());
            await Clients.Others.UserJoined(username);
        }

        public async Task SendMessageToGroup(string message, string groupId)
        {
            var user = _db.Users.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();

            await _db.Messages.AddAsync(new()
            {
                Date = DateTime.Now,
                GroupId = Convert.ToInt32(groupId),
                SenderId = user.Id,
                SenderUsername = user.UserName,
                Text = message,
            });

            await _db.SaveChangesAsync();

            var messageList = await _db.Messages.Where(x=>x.GroupId == Convert.ToInt32(groupId)).ToListAsync();

            //var groupList = GetAllGroups();

            //await Clients.All.RecieveGroups(groupList);
            await Clients.Group(groupId).RecieveMessagesByGroupId(messageList);
        }

        public async Task GetMessagesByGroupId(string oldGroupId,int groupId,string username)
        {
            // son girdigi gruptan kullanciyi cikarip yeni gruba ekle.
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,oldGroupId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

            var messageList = await _db.Messages.Where(x => x.GroupId == groupId).OrderBy(x=>x.Date).ToListAsync();

            var groupName = groupId.ToString();

            await Clients.Group(groupName).RecieveMessagesByGroupId(messageList);
        }

        public async Task CreateChatGroup(string groupId,string connId)
        {
            var user = _db.Users.Where(x => x.ConnectionId == connId).FirstOrDefault();
            

            if(user != null)
            {
                await Groups.AddToGroupAsync(connId, groupId);
                await _db.Groups.AddAsync(new()
                    {
                        AdminId = connId,
                        AvatarUrl = "./assets/GroupAvatars/football.jpg",
                        CreatedDate = DateTime.Now,
                        AdminUsername = user.UserName,
                        Title = groupId
                    });

                var groupList = await GetAllGroups();
                await Clients.All.RecieveGroups(groupList);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var groupList = await GetAllGroups();

            if(clients.Count() > 0) await Clients.All.RecieveClients(clients.Keys.ToList()); // usernames
            
            await Clients.All.RecieveGroups(groupList);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var user = _db.Users.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();

            var clientToRemove = clients.Where(x => x.Value == Context.ConnectionId).FirstOrDefault().Key;

            
            await Clients.Others.UserLeft(clientToRemove);

            clients.Remove(clientToRemove);

            await Clients.All.RecieveClients(clients.Keys.ToList());

            if (user != null)
            {
                user.ConnectionId = "";
                await _db.SaveChangesAsync();
            }
        }
        



        private async Task<List<GroupResponseModel>> GetAllGroups()
        {
            var groupList = await _db.Groups.ToListAsync();
            var groupListVM = new List<GroupResponseModel>();

            foreach (var group in groupList)
            {
                IOrderedQueryable<Message> lastMessageQuery = _db.Messages.Where(x => x.GroupId == group.Id).OrderBy(x => x.Date);
                string? lastMessage = lastMessageQuery.Count() != 0 ? lastMessageQuery.Last().Text : "";

                groupListVM.Add(new()
                {
                    AdminId = group.AdminId,
                    AdminUsername = group.AdminUsername,
                    AvatarUrl = group.AvatarUrl,
                    CreatedDate = group.CreatedDate,
                    GroupId = group.Id,
                    LastMessage = lastMessage,
                    Title = group.Title
                });
            }

            return groupListVM;
        }


    }
}
