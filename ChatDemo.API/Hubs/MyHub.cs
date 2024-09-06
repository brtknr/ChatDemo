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
        static List<string> clients = new List<string>();
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

            if (user.ConnectionId != "") clients.RemoveAll(x => x.Equals(user.ConnectionId));

            user.ConnectionId = connId;
            await _db.SaveChangesAsync();
        }

        public async Task ClientToClientSendMessage(string message,string targetConnId)
        {
            //await Clients.Client(targetConnId).SendAsync("recieveMessage",message);
            await Clients.Client(targetConnId).RecieveMessage(message);
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

            var messageList = _db.Messages.Where(x=>x.GroupId == Convert.ToInt32(groupId)).ToList();
            
            await Clients.Group(groupId).RecieveMessagesByGroupId(messageList);
        }

        public async Task GetMessagesByGroupId(string oldGroupId,int groupId,string username)
        {
            // son girdigi gruptan kullanciyi cikarip yeni gruba ekle.
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,oldGroupId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

            var messageList = await _db.Messages.Where(x => x.GroupId == groupId).ToListAsync();

            var groupName = groupId.ToString();

            await Clients.Group(groupName).RecieveMessagesByGroupId(messageList);
        }

        public override async Task OnConnectedAsync()
        {
            clients.Add(Context.ConnectionId);

            var groupList = _db.Groups.ToList();
            var groupListVM = new List<GroupResponseModel>();
            
            foreach (var group in groupList)
            {
                groupListVM.Add(new()
                {
                    AdminId = group.AdminId,
                    AdminUsername = group.AdminUsername,
                    AvatarUrl = group.AvatarUrl,
                    CreatedDate = group.CreatedDate,
                    GroupId = group.Id,
                    LastMessage = group.Messages != null ? group.Messages.LastOrDefault().Text : "testsonmesaj",
                    Title = group.Title
                });
            }

            await Clients.All.RecieveGroups(groupListVM);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            clients.Remove(Context.ConnectionId);
            await Clients.All.RecieveClients(clients);
            await Clients.All.UserLeft(Context.ConnectionId);
        }
        
    }
}
