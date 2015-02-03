using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Grikwa.Controllers;
using System.Net.Mime;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Grikwa.Models
{

    public enum ProductStatus
    {
        NEW,
        AUCTION,
        SOLD,
        REQUESTED
    }

    public enum ProductIntention
    {
        SELL,
        NOTIFY
    }

    public enum MessageStatus
    {
        READ,
        UNREAD
    }

    public enum MessageType
    {
        NORMAL,
        SALEREQUEST,
        SYSTEM
    }

    public class Product
    {

        public virtual int ProductID { get; set; }

        public virtual string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        [MaxLength(50)]
        public virtual string Name { get; set; }

        [MaxLength(100)]
        public virtual string ShortDescription { get; set; }

        [MaxLength(300)]
        public virtual string LongDescription { get; set; }

        [DataType(DataType.Currency)]
        public virtual decimal Price { get; set; }

        public virtual DateTime DatePosted { get; set; }

        public virtual ProductStatus ProductStatus { get; set; }

        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public virtual string ContactEmail { get; set; }

        [MaxLength(15)]
        [DataType(DataType.PhoneNumber)]
        public virtual string ContactNumber { get; set; }

        [MaxLength(200)]
        public virtual string WebsiteLink { get; set; }

        [MaxLength(250)]
        public virtual string KeyWords { get; set; }

        public virtual ProductIntention ProductIntention { get; set; }

        public virtual bool AcceptedTerms { get; set; }

        public virtual byte[] FullSizeImage { get; set; }

        public virtual byte[] ThumbnailImage { get; set; }

        public virtual bool Visible { get; set; }

    }

    public class Sale
    {
        public virtual int SaleID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public virtual string CustomerID { get; set; }
        public virtual ApplicationUser Customer { get; set; }
        public virtual DateTime SaleDate { get; set; }

    }

    public class Category
    {
        public virtual int CategoryID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string IconName { get; set; }
    }

    public class ProductCategory
    {

        public virtual int ProductCategoryID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual Category Category { get; set; }

    }

    public class Message
    {
        public virtual int MessageID { get; set; }
        public virtual string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual string Text { get; set; }
        public virtual int Frequency { get; set; }
    }

    public class Conversation
    {
        public virtual int ConversationID { get; set; }
        public virtual string FromUserID { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
        public virtual string ToUserID { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual MessageStatus MessageStatus { get; set; }
        public virtual MessageType MessageType { get; set; }
    }

    public class ConversationRoom
    {
        public virtual int ConversationRoomID { get; set; }
        public virtual string Name { get; set; }
        public virtual string User1ID { get; set; }
        public virtual ApplicationUser User1 { get; set; }
        public virtual string User2ID { get; set; }
        public virtual ApplicationUser User2 { get; set; }
    }

    public class ConversationRoomProduct
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int ConversationRoomID { get; set; }
        public virtual ConversationRoom ConversationRoom { get; set; }
        public virtual int ProductID { get; set; }
        public virtual Product Product { get; set; }
    }

    //public class UserConnection
    //{
    //    public int UserConnectionID { get; set; }
    //    public int 
    //}

    /// <summary>
    /// The static chat rooms class to maintain the list of rooms between two users
    /// </summary>
    public class ChatRooms
    {
        #region Properties
        /// <summary>
        /// List of active rooms
        /// </summary>
        static HashSet<Room> Rooms = new HashSet<Room>(new RoomEqualityComparer());
        #endregion

        #region Methods
        /// <summary>
        /// Add a room to the room list
        /// </summary>
        /// <param name="room">The room to add</param>
        public static void Add(Room room)
        {
            Rooms.Add(room);
        }

        /// <summary>
        /// Check if room already exits in room list
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <returns>True if room exist, false otherwise</returns>
        public static bool Exists(string roomName)
        {
            var rooms = (from r in Rooms
                         where r.Name == roomName
                         select r).Count();

            return rooms > 0;
        }

        /// <summary>
        /// Check if the room list is empty
        /// </summary>
        /// <returns>True if list is empty, false otherwise</returns>
        public static bool IsEmpty()
        {
            return Rooms.Count < 1;
        }

        /// <summary>
        /// Get a specific room given its name
        /// </summary>
        /// <param name="name">The name of the room</param>
        /// <returns>A room</returns>
        public static Room getRoom(string name)
        {
            var room = (from r in Rooms
                        where r.Name.Equals(name)
                        select r).First();
            return room;
        }

        /// <summary>
        /// Get all the rooms in the list
        /// </summary>
        /// <returns>List of rooms</returns>
        public static IEnumerable<Room> GetAll()
        {
            return Rooms;
        }

        /// <summary>
        /// Get all the rooms that contain a specific user
        /// </summary>
        /// <param name="userID">The username of the user</param>
        /// <returns>A list of rooms</returns>
        public static IEnumerable<Room> GetAll(string userID)
        {
            var userRooms = (from r in Rooms
                             where (r.User1ID.Equals(userID) || r.User2ID.Equals(userID))
                             select r).ToList();

            return userRooms;
        }

        /// <summary>
        /// Restore all the active rooms in the application
        /// </summary>
        /// <returns>The number of rooms</returns>
        public static int RestoreRooms()
        {
            int count = 0;

            //using (var db = new ApplicationDbContext())
            //{
            //    var cRooms = (from cr in db.ConversationRooms
            //                  select cr).ToList();
            //    var rooms = (from r in cRooms
            //                 select new Room
            //                 {
            //                     Name = r.Name,
            //                     User1DatabaseID = r.User1ID,
            //                     User1ID = r.User1.UserName,
            //                     User1Name = r.User1.TitleID + " " + r.User1.Intials + " " + r.User1.Surname,
            //                     User1LastSeen = r.User1.LastSeen,
            //                     User2DatabaseID = r.User2ID,
            //                     User2ID = r.User2.UserName,
            //                     User2LastSeen = r.User2.LastSeen,
            //                     User2Name = r.User2.TitleID + " " + r.User2.Intials + " " + r.User2.Surname
            //                 }).ToList();
            //    Rooms.UnionWith(rooms);
            //    count = rooms.Count();
            //}

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierEmail"></param>
        /// <param name="supplierName"></param>
        /// <param name="productName"></param>
        public static void SendSaleRequestEmail(string supplierEmail, string customerEmail, string productName, string customerMessage)
        {
            try
            {
                MailMessage mail = new MailMessage();

                string from = customerEmail;
                string subject = "Grikwa - Reply To Advert";
                string bodyHTML = "<h1><strong>Dear Grikwa User</strong>.</h1>"
                                  + "<h4>You have a reply on Grikwa regarding product: " + productName + ".</h4>"
                                  + "<p>You can go to the <a href='http://www.grikwa.co.za'>Grikwa Notice Board</a> to see the transaction.</p>"
                                  + "<p>Below is the message from the customer:</p>"
                                  + "<p>"+customerMessage+"</p>"
                                  + "<p>You can reply to this email to start communicating with the customer.</p>"
                                  + " <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Grikwa User. "
                                  + "You have a sale request on Grikwa regarding product: " + productName + ". "
                                  + "You can go to the Grikwa Notice Board at http://www.grikwa.co.za to see the transaction."
                                  + "The following is the message from the customer:"
                                  + customerMessage + ". You can reply to this email to start communicating with the customer."
                                  + "Grikwa Team";

                // To
                mail.To.Add(supplierEmail);

                // From
                mail.From = new MailAddress(from);

                // Subject and multipart/alternative Body
                mail.Subject = subject;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_a34c075f62bba74426624b9a65795a59@azure.com", "pqx33rsp"); // Enter senders User name and password
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                Trace.WriteLine(smtpFailedRecipientException.Message, "Sale Request Email To: " + supplierEmail);
            }
            catch (SmtpException smtpException)
            {
                Trace.WriteLine(smtpException.Message, "Sale Request Email To: " + supplierEmail);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "Sale Request Email To: " + supplierEmail);
            }
        }

        public static void SendChatRequestEmail(string supplierEmail, string customerEmail, string productName, string customerMessage)
        {
            try
            {
                MailMessage mail = new MailMessage();

                string from = "sales@grikwa.co.za";
                string subject = "Grikwa - Reply To Advert";
                string bodyHTML = "<h1><strong>Dear Grikwa User</strong>.</h1>"
                                  + "<h4>Someone is interested in your product:"+productName+".</h4>"
                                  + "<p>Go to the <a href='http://www.grikwa.co.za'>Grikwa Notice Board</a> to have a chat with the person.</p>"
                                  + " <br/> <h5>Grikwa Team</h5>";
                string bodyText = "Dear Dear Grikwa User. "
                                  + "Someone is interested in your product:"+productName+"."
                                  + "Go to the Grikwa Notice Board at http://www.grikwa.co.za to have a chat with the person. Grikwa Team";

                // To
                mail.To.Add(customerEmail);

                // From
                mail.From = new MailAddress(from);

                // Subject and multipart/alternative Body
                mail.Subject = subject;
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_a34c075f62bba74426624b9a65795a59@azure.com", "pqx33rsp"); // Enter senders User name and password
                smtp.Credentials = credentials;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientException smtpFailedRecipientException)
            {
                Trace.WriteLine(smtpFailedRecipientException.Message, "Chat Request Email To: " + customerEmail);
            }
            catch (SmtpException smtpException)
            {
                Trace.WriteLine(smtpException.Message, "Chat Request Email To: " + customerEmail);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "Chat Request Email To: " + customerEmail);
            }
        }
        #endregion

    }

    /// <summary>
    /// The RoomEqualityComparer to compare two rooms
    /// </summary>
    public class RoomEqualityComparer : IEqualityComparer<Room>
    {
        public bool Equals(Room r1, Room r2)
        {
            if (r1.User1ID.Equals(r2.User1ID) && r1.User2ID.Equals(r2.User2ID) ||
                r1.User1ID.Equals(r2.User2ID) && r1.User2ID.Equals(r2.User1ID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Room r)
        {
            return r.Name.GetHashCode();
        }
    }

    /// <summary>
    /// The Room class to store a room
    /// </summary>
    public class Room
    {
        public string Name { get; set; }
        public string User1ID { get; set; }
        public virtual DateTime User1LastSeen { get; set; }
        public string User1Name { get; set; }
        public string User1DatabaseID { get; set; }
        public string User2ID { get; set; }
        public virtual DateTime User2LastSeen { get; set; }
        public string User2Name { get; set; }
        public string User2DatabaseID { get; set; }
    }

    /// <summary>
    /// The ChatMessage class to store a message
    /// </summary>
    public class ChatMessage
    {
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public MessageStatus Read { get; set; }
        public MessageType MessageType { get; set; }
    }

    /// <summary>
    /// The Chat Hub class to manage all the chatting
    /// </summary>
    [Authorize]
    public class Chat : Hub
    {
        #region Properties

        /// <summary>
        /// The database context
        /// </summary>
        private ApplicationDbContext db = new ApplicationDbContext();

        #endregion

        #region Methods
        /// <summary>
        /// (awaitable) Join a room
        /// </summary>
        /// <param name="roomName">The name of the room to join</param>
        /// <returns>void</returns>
        public async Task Join(string roomName)
        {
            await Groups.Add(Context.ConnectionId, roomName);
        }

        /// <summary>
        /// (awaitable) Create a private chat room for customer and supplier for negotiation purposes.
        /// </summary>
        /// <param name="requestMessage">The initial negotiation message.</param>
        /// <param name="customerID">The ID of the customer.</param>
        /// <param name="customerName">The name of the customer.</param>
        /// <param name="supplierID">The ID of the supplier</param>
        /// <param name="productId">The ID of the product been bought</param>
        /// <returns>void</returns>
        public async Task CreateChatRoom(string requestMessage, string customerID, string customerName, string supplierID, int productId)
        {

            // (Task) Get more necessary supplier details
            var supplierDetails = await (from u in db.Users
                                         where u.Id.Equals(supplierID)
                                         select new { id = u.UserName, name = u.TitleID + " " + u.Intials + " " + u.Surname, Email = u.Email }).FirstAsync();

            // (Task) Get more details of product involved
            var product = await db.Products.FindAsync(productId);

            // (Task) Get the number of sale requests of the product
            var currentSaleRequestNumber = await (from sr in db.ConversationRoomProducts
                                                  where sr.ProductID == productId
                                                  select sr).CountAsync();

            // Get possible room names
            string roomName = customerID + "_" + supplierID;
            string roomName2 = supplierID + "_" + customerID;
            var user1 = db.Users.Find(customerID);
            string customerEmail = user1.Email;
            /*Create new room or add product to existing room*/
            if (ChatRooms.Exists(roomName)) // room exists
            {
                // Get room
                var room = ChatRooms.getRoom(roomName);
                var conRoom = await (from cr in db.ConversationRooms
                                     where cr.Name.Equals(roomName)
                                     select cr).FirstAsync();

                // Add product to room
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName, requestMessage, customerID, customerName, 1);
            }
            else if (ChatRooms.Exists(roomName2)) // room exists
            {
                // Get room
                var room = ChatRooms.getRoom(roomName2);
                var conRoom = await (from cr in db.ConversationRooms
                                     where cr.Name.Equals(roomName2)
                                     select cr).FirstAsync();

                // Add product to room
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName2, requestMessage, customerID, customerName, 1);
            }
            else // room does not exist
            {
                // Get users involved
                var user2 = db.Users.Find(supplierID);

                // Create new room
                var conRoom = new ConversationRoom() { User1 = user1, User2 = user2, Name = roomName };
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });
                Room room = new Room()
                {
                    Name = roomName,
                    User1ID = Context.User.Identity.Name,
                    User1Name = customerName,
                    User1LastSeen = user1.LastSeen,
                    User1DatabaseID = customerID,
                    User2ID = supplierDetails.id,
                    User2Name = supplierDetails.name,
                    User2LastSeen = user2.LastSeen,
                    User2DatabaseID = supplierID,
                };

                // Add room to list of rooms
                ChatRooms.Add(room);

                // Join room
                await Join(room.Name);

                // Send room details to client (browser)
                await Clients.User(supplierDetails.id).addChatRoom(new
                {
                    Name = room.Name,
                    OtherUserID = room.User2DatabaseID,
                    OtherUserName = room.User2Name,
                    LastSeen = room.User2LastSeen,
                    Messages = new { },
                    Products = new { }
                });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName, requestMessage, customerID, customerName, 1);
            }

            // Send a sale request email to the supplier
            ChatRooms.SendSaleRequestEmail(product.ContactEmail, customerEmail , product.Name, requestMessage);
        }

        public async Task CreateChatRoomForLiveChat(string requestMessage, string customerID, string customerName, string supplierID, int productId)
        {

            // (Task) Get more necessary supplier details
            var supplierDetails = await (from u in db.Users
                                         where u.Id.Equals(supplierID)
                                         select new { id = u.UserName, name = u.TitleID + " " + u.Intials + " " + u.Surname, Email = u.Email }).FirstAsync();

            // (Task) Get more details of product involved
            var product = await db.Products.FindAsync(productId);

            // (Task) Get the number of sale requests of the product
            var currentSaleRequestNumber = await (from sr in db.ConversationRoomProducts
                                                  where sr.ProductID == productId
                                                  select sr).CountAsync();

            // Get possible room names
            string roomName = customerID + "_" + supplierID;
            string roomName2 = supplierID + "_" + customerID;
            var user1 = db.Users.Find(customerID);
            string customerEmail = user1.Email;
            /*Create new room or add product to existing room*/
            if (ChatRooms.Exists(roomName)) // room exists
            {
                // Get room
                var room = ChatRooms.getRoom(roomName);
                var conRoom = await (from cr in db.ConversationRooms
                                     where cr.Name.Equals(roomName)
                                     select cr).FirstAsync();

                // Add product to room
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName, requestMessage, customerID, customerName, 1);
            }
            else if (ChatRooms.Exists(roomName2)) // room exists
            {
                // Get room
                var room = ChatRooms.getRoom(roomName2);
                var conRoom = await (from cr in db.ConversationRooms
                                     where cr.Name.Equals(roomName2)
                                     select cr).FirstAsync();

                // Add product to room
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName2, requestMessage, customerID, customerName, 1);
            }
            else // room does not exist
            {
                // Get users involved
                var user2 = db.Users.Find(supplierID);

                // Create new room
                var conRoom = new ConversationRoom() { User1 = user1, User2 = user2, Name = roomName };
                db.ConversationRoomProducts.Add(new ConversationRoomProduct() { ConversationRoom = conRoom, Product = product });
                Room room = new Room()
                {
                    Name = roomName,
                    User1ID = Context.User.Identity.Name,
                    User1Name = customerName,
                    User1LastSeen = user1.LastSeen,
                    User1DatabaseID = customerID,
                    User2ID = supplierDetails.id,
                    User2Name = supplierDetails.name,
                    User2LastSeen = user2.LastSeen,
                    User2DatabaseID = supplierID,
                };

                // Add room to list of rooms
                ChatRooms.Add(room);

                // Join room
                await Join(room.Name);

                // Send room details to client (browser)
                await Clients.User(supplierDetails.id).addChatRoom(new
                {
                    Name = room.Name,
                    OtherUserID = room.User2DatabaseID,
                    OtherUserName = room.User2Name,
                    LastSeen = room.User2LastSeen,
                    Messages = new { },
                    Products = new { }
                });

                // save changes
                await db.SaveChangesAsync();

                // Send sale request message
                await Send(roomName, requestMessage, customerID, customerName, 1);
            }

            // Send a sale request email to the supplier
            var supplierEmail = (string.IsNullOrEmpty(product.ContactEmail) || string.IsNullOrWhiteSpace(product.ContactEmail)) ? supplierDetails.Email : product.ContactEmail;
            ChatRooms.SendChatRequestEmail(supplierEmail, customerEmail, product.Name, requestMessage);
        }

        /// <summary>
        /// Send message to the chat room
        /// </summary>
        /// <param name="roomName">the name of the room</param>
        /// <param name="message">the message to send</param>
        /// <param name="senderID">The ID of the sender</param>
        /// <param name="senderName">The name of the sender</param>
        public async Task Send(string roomName, string message, string senderID, string senderName, int messageType)
        {

            // Get the time now (on server)
            DateTime time = DateTime.Now;

            // Get the sender details
            var sender = await (from u in db.Users
                                where u.Id.Equals(senderID)
                                select u).FirstAsync();

            // Get the room
            var room = ChatRooms.getRoom(roomName);

            // Get the receiver details
            var receiverID = room.User1DatabaseID.Equals(senderID) ? room.User2ID : room.User1ID;
            var receiver = await (from u in db.Users
                                  where u.UserName.Equals(receiverID)
                                  select u).FirstAsync();

            // Get message type
            var mType = messageType == 0 ? MessageType.NORMAL :
                        messageType == 1 ? MessageType.SALEREQUEST : MessageType.SYSTEM;

            // get last message sent by receiver to sender and check if it was a sale request
            //var sendReplyEmail = false;
            //var receiverName = "";
            //var receiverEmail = "";
            //if (mType == MessageType.NORMAL)
            //{
            //    var conv = from c in db.Conversations
            //               where c.FromUser.UserName.Equals(receiverID) && c.ToUser.UserName.Equals(sender.UserName)
            //               orderby c.Time descending
            //               select c;

            //    var count = await conv.CountAsync();
            //    if (count > 0)
            //    {
            //        var con = await conv.FirstAsync();
            //        sendReplyEmail = con.MessageType == MessageType.SALEREQUEST;

            //        if (sendReplyEmail)
            //        {
            //            receiverName = receiver.TitleID + " " + receiver.Intials + " " + receiver.Surname;
            //            receiverEmail = receiver.Email;
            //        }
            //    }
            //}

            // Add conversation
            db.Conversations.Add(new Conversation()
            {
                FromUser = sender,
                ToUser = receiver,
                MessageStatus = MessageStatus.UNREAD,
                Text = message,
                Time = time,
                MessageType = mType
            });

            // save changes
            await db.SaveChangesAsync();

            // Send message to the room
            time = time.AddHours(2);

            // send message to receiver
            await Clients.OthersInGroup(roomName).addMessage(roomName, new ChatMessage()
            {
                SenderID = senderID,
                ReceiverID = receiver.Id,
                SenderName = senderName,
                Text = message,
                Time = time.ToShortDateString() + " @ " + time.ToShortTimeString(),
                Read = MessageStatus.UNREAD,
                MessageType = mType
            });

            // update last seen
            UpdateLastSeen();

            // send reply email to receiver
            //if (sendReplyEmail)
            //{
            //    ChatRooms.SendSaleRequestReplyEmail(receiverEmail, receiverName, senderName);
            //}

        }

        /// <summary>
        /// Get the current user details
        /// </summary>
        public void GetUserDetails()
        {

            try
            {
                var user = (from u in db.Users
                            where u.UserName.Equals(Context.User.Identity.Name)
                            select new { Id = u.Id, Name = u.TitleID + " " + u.Intials + " " + u.Surname }).First();
                Clients.Caller.setUserDetails(user);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message, "Getting User Details Using SignalR");
            }

        }

        /// <summary>
        /// Update the status of read conversations
        /// </summary>
        /// <param name="receiverID">The ID of the receiver</param>
        /// <param name="senderID">The ID of the sender</param>
        public void UpdateReadMessages(string receiverID, string senderID)
        {
            var messages = from c in db.Conversations
                           where c.ToUserID.Equals(receiverID) && c.FromUserID.Equals(senderID)
                           select c;

            foreach (var m in messages)
            {
                m.MessageStatus = MessageStatus.READ;
                db.Entry(m).State = EntityState.Modified;
            }

            db.SaveChangesAsync();

        }

        public override System.Threading.Tasks.Task OnConnected()
        {

            /* Get all the chat rooms and their conversations */
            foreach (var room in ChatRooms.GetAll(Context.User.Identity.Name))
            {

                // Get all the messages in this room
                var mis = (from c in db.Conversations
                           where (c.FromUser.UserName.Equals(room.User1ID) && c.ToUser.UserName.Equals(room.User2ID) ||
                                 (c.FromUser.UserName.Equals(room.User2ID) && c.ToUser.UserName.Equals(room.User1ID)))
                           orderby c.Time ascending
                           select new
                           {
                               SenderID = c.FromUser.Id,
                               ReceiverID = c.ToUser.Id,
                               SenderName = c.FromUser.TitleID + " " + c.FromUser.Intials + " " + c.FromUser.Surname,
                               Text = c.Text,
                               Time = c.Time,
                               Read = c.MessageStatus,
                               MessageType = c.MessageType
                           }).ToList();
                var messages = (from m in mis
                                select new ChatMessage
                                {
                                    SenderID = m.SenderID,
                                    ReceiverID = (room.User1ID.Equals(Context.User.Identity.Name) ? room.User1DatabaseID : room.User2DatabaseID),
                                    SenderName = m.SenderName,
                                    Text = m.Text,
                                    Time = m.Time.AddHours(2).ToShortDateString() + " @ " + m.Time.AddHours(2).ToShortTimeString(),
                                    Read = m.Read,
                                    MessageType = m.MessageType
                                }).ToArray();

                // Get all products in this room
                var products = (from p in db.ConversationRoomProducts
                                where p.ConversationRoom.Name.Equals(room.Name)
                                select new NegotiationProductModel()
                                {
                                    ProductID = p.ProductID,
                                    Name = p.Product.Name
                                }).ToArray();

                // Send everything to the user
                if (room.User1ID.Equals(Context.User.Identity.Name))
                {
                    Clients.Caller.addChatRoom(new
                    {
                        Name = room.Name,
                        OtherUserID = room.User2DatabaseID,
                        OtherUserName = room.User2Name,
                        LastSeen = room.User2LastSeen,
                        Messages = messages,
                        Products = products
                    });
                }
                else
                {
                    Clients.Caller.addChatRoom(new
                    {
                        Name = room.Name,
                        OtherUserID = room.User1DatabaseID,
                        OtherUserName = room.User1Name,
                        LastSeen = room.User1LastSeen,
                        Messages = messages,
                        Products = products
                    });
                }
            }

            // Update last seen time
            UpdateLastSeen();

            return base.OnConnected();
        }

        private void UpdateLastSeen()
        {
            using (var context = new ApplicationDbContext())
            {
                // get user name
                var userName = Context.User.Identity.Name;

                // get time now
                var lastSeen = DateTime.Now;

                // update chat rooms in memory
                var rooms = from r in ChatRooms.GetAll()
                            where r.User1ID.Equals(userName) || r.User2ID.Equals(userName)
                            select r;
                foreach (var room in rooms)
                {
                    if (room.User1ID.Equals(userName))
                    {
                        room.User1LastSeen = lastSeen;
                    }
                    else
                    {
                        room.User2LastSeen = lastSeen;
                    }
                    Clients.OthersInGroup(room.Name).updateLastSeen(new { Name = room.Name, LastSeen = lastSeen });
                }

                // update user last seen time in database
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var user = userManager.FindByName(userName);
                user.LastSeen = lastSeen;
                userManager.Update(user);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            return base.OnDisconnected();
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion
    }

}