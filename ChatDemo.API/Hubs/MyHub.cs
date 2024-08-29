using ChatDemo.API.DbContext;
using ChatDemo.API.Hubs.HubInterface;
using ChatDemo.API.Models;
using ChatDemo.API.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ChatDemo.API.Hubs
{
    public class MyHub : Hub<IMessageClient>
    {
        static List<string> clients = new List<string>();
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

        public async Task GetMessagesByGroupId(int groupId,string username)
        {
            // son girdigi gruptan kullanciyi cikarip yeni gruba ekle. son girdigi grubu db ye kaydet ya da client tan 2 var tanimlayip yeni eski olarak gonder 
            //await Groups.AddToGroupAsync(groupId.ToString(), Context.ConnectionId);

            var messageList = await _db.Messages.Where(x => x.GroupId == groupId).ToListAsync();

            await Clients.Caller.RecieveMessagesByGroupId(messageList);
        }

        public async Task SendMessage(string message,int groupId)
        {
            //Message msg = new()
            //{
            //    Date = DateTime.Now,
            //    GroupId = groupId,
            //    Text = message,
            //    SenderId = ,
            //    SenderUsername = ,
            //}


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
