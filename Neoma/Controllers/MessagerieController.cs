using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neoma.Models;
using Neoma.Data;
using Neoma.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Neoma.Models.MessagerieViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Neoma.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Neoma.Controllers
{
    public class MessagerieController : BaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IHubContext<MessagerieHub> _hub;

        public MessagerieController(ApplicationDbContext db, IConfiguration configuration, IHostingEnvironment hostingEnvironment, IHubContext<MessagerieHub> hub) : base(db)
        {
            _db = db;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _hub = hub;
        }

        public async Task<IActionResult> Index(int? conversationId, string user = null)
        {
            var ConversationCurrentUser = await _db.ConversationUtilisateur.Where(cu => cu.UtilisateurId == User.getUserId()).ToListAsync();
            var ConversationContactUser = await _db.ConversationUtilisateur.Where(c => ConversationCurrentUser.Exists(x => x.ConversationId == c.ConversationId && x.UtilisateurId != c.UtilisateurId)).ToListAsync();
            foreach(var conversation in ConversationContactUser)
            {
                conversation.Utilisateur = await _db.Users.Where(u => u.Id == conversation.UtilisateurId).FirstOrDefaultAsync();
                conversation.Utilisateur.Presence = conversation.Utilisateur.LastAction.AddSeconds(30) >= DateTime.Now ? 1 : conversation.Utilisateur.LastAction.AddMinutes(5) <= DateTime.Now ? 2 : 3;
                conversation.Conversation = await _db.Conversation.Where(c => c.Id == conversation.ConversationId).FirstOrDefaultAsync();
                var Message = await _db.Message.Where(m => m.ConversationId == conversation.ConversationId).OrderByDescending(d => d.DateEnvoi).FirstOrDefaultAsync();
                conversation.LastMessage = Message;
            }
            ConversationContactUser = ConversationContactUser.OrderByDescending(x => x.Conversation.DateLastMessage).ToList();
            switch (conversationId)
            {
                case -1:
                    ViewBag.Conversation = -1;
                    break;

                case null:
                    if(ConversationContactUser.Count > 0)
                    {
                        ViewBag.Conversation = ConversationContactUser.First().ConversationId;
                    }             
                    break;

                default:
                    ViewBag.Conversation = conversationId;
                    break;
            }
            ViewBag.User = user != null ? user : null;
            ViewBag.Url = Request.Headers["Referer"].ToString();
            return View(ConversationContactUser);
        }

        public IActionResult UpdateDiscussion(int ConversationId)
        {
            var countMessage = HttpContext.Session.GetInt32("NbrMessages");
            HttpContext.Session.SetInt32("NbrMessages", Convert.ToInt32(countMessage - 1));
            return ViewComponent("Conversation", new { ConversationId = ConversationId });
        }

        [HttpPost]
        public async Task<IActionResult> NewMessage(MessagerieViewModel message)
        {
            var Message = new Message();
            var cloudinary = new Cloudinary(
                new Account(
                    _configuration["Cloudinary:Name"],
                    _configuration["Cloudinary:Key"],
                    _configuration["Cloudinary:Secret"]
                ));

            string webRootPath = _hostingEnvironment.WebRootPath;
            var file = Request.Form.Files;

            var currentUser = await _db.Users.Where(u => u.Id == User.getUserId()).FirstOrDefaultAsync();
            if (message.Conversation == -1)
            {
                var Conversation = new Conversation
                {
                    DateCreation = DateTime.Now,
                    DateLastMessage = DateTime.Now
                };

                _db.Conversation.Add(Conversation);

                _db.ConversationUtilisateur.Add(new ConversationUtilisateur
                {
                    ConversationId = Conversation.Id,
                    Conversation = Conversation,
                    UtilisateurId = User.getUserId()
                });

                _db.ConversationUtilisateur.Add(new ConversationUtilisateur
                {
                    ConversationId = Conversation.Id,
                    Conversation = Conversation,
                    UtilisateurId = message.User
                });
                Message.ConversationId = Conversation.Id;
            }
            else
            {
                var Conversation = await _db.Conversation.Where(c => c.Id == message.Conversation).FirstOrDefaultAsync();
                Conversation.DateLastMessage = DateTime.Now;
                _db.Conversation.Update(Conversation);
                Message.ConversationId = Conversation.Id;
            }

            if (file.Count > 0)
            {
                
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = file[0].FileName.Substring(file[0].FileName.LastIndexOf("."), file[0].FileName.Length - file[0].FileName.LastIndexOf("."));

                var imagePath = Path.Combine(uploads, file[0].FileName);
                using (var filestream = new FileStream(imagePath, FileMode.Create))
                {
                    file[0].CopyTo(filestream);
                }

                if (extension == ".png" || extension == ".jpg" || extension == ".bmp")
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imagePath),
                        PublicId = "Conversation_" + Message.ConversationId + "/" + file[0].FileName
                    };

                    var uploadResult = await Task.Run(() => cloudinary.Upload(uploadParams));
                    Message.Files = uploadResult.SecureUri.AbsoluteUri;
                }
                else
                {
                    var uploadParams = new RawUploadParams()
                    {
                        File = new FileDescription(imagePath),
                        PublicId = "Conversation_" + Message.ConversationId + "/" + file[0].FileName
                    };
                    var uploadResult = await Task.Run(() => cloudinary.Upload(uploadParams));
                    Message.Files = uploadResult.SecureUri.AbsoluteUri;
                }

                Message.FileName = file[0].FileName;
                
                if (System.IO.File.Exists(imagePath))
                {
                    await Task.Run(() => System.IO.File.Delete(imagePath));
                }
            }
            Message.Text = message.Text;
            Message.DateEnvoi = DateTime.Now;
            Message.Utilisateur = currentUser;
            Message.MessageLu = false;
            await _db.Message.AddAsync(Message);
            await _db.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("ReceiveSignal", Message);
            await _hub.Clients.All.SendAsync("displayNotificationMessage");
            return Json(new { success = true, result = Message });
        }

        public async Task<IActionResult> ReadMessage(int conversation)
        {
            var userId = User.getUserId();
            var Messages = await _db.Message.Where(m => m.ConversationId == conversation && m.UtilisateurId != userId && !m.MessageLu).ToListAsync();
            foreach(var message in Messages)
            {
                message.MessageLu = true;
                _db.Message.Update(message);
            }

            await _db.SaveChangesAsync();

            var conversationsId = await _db.ConversationUtilisateur.Where(cu => cu.UtilisateurId == userId && cu.ConversationId != conversation).Select(x => x.ConversationId).ToListAsync();
            var conversations = await _db.Conversation.Where(c => conversationsId.Contains(c.Id)).ToListAsync();
            int count = 0;
            foreach (var conv in conversations)
            {
                var message = await _db.Message.Where(m => !m.MessageLu && m.UtilisateurId != userId && m.ConversationId == conv.Id).ToListAsync();
                if (message.Count > 0)
                {
                    count++;
                }
            }

            return Ok(new { Count = count });
        }

        public async Task<IActionResult> RemoveFile(int message)
        {
            var cloudinary = new Cloudinary(
                new Account(
                    _configuration["Cloudinary:Name"],
                    _configuration["Cloudinary:Key"],
                    _configuration["Cloudinary:Secret"]
                ));

            var Message = await _db.Message.Where(m => m.Id == message).FirstOrDefaultAsync();
            var deleteParams = new DeletionParams("Conversation_" + Message.ConversationId + "/" + Message.FileName);

            var deleteResult = await Task.Run(() => cloudinary.Destroy(deleteParams));

            
            Message.Files = "deleted";
            Message.FileName = null;
            _db.Message.Update(Message);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("ReceiveRemove", Message);
            return Ok(Message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
