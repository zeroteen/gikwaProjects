//////////////////////////////////// CHAT CODE START    /////////////////////
var currentChatRoomName = "";
var myUserID = "";
var myUserName = "";
var currentChatRooms = [];

function setCurrentUserDetails(id, name) {
    myUserID = id;
    myUserName = name;
}

// get the chat room object given the chat room name
function getChatRoom(chatRoomName) {
    for (var room = 0; room < currentChatRooms.length; room++) {
        if (currentChatRooms[room].Name == chatRoomName) {
            return currentChatRooms[room];
        }
    }
    return null;
}

// get all the messages in the chat room given the chat room name
function getMessages(chatRoomName) {
    var room = getChatRoom(chatRoomName);
    if (room != null) {
        return room.Messages;
    }

    return [];
}

function addChatRoomToList(chatRoom) {

    var lastSeen = userLastSeen(chatRoom);

    if (lastSeen != "online") {
        lastSeen = "last seen " + lastSeen
    }

    $("#chatUserList").append($("<li>"
                                    + "<a href='' class = 'changeUser'>"
                                        + "<i class='icon-double-angle-right'></i>"
                                        + "<div class='username'>"
                                        + "<span id='userStatus' class='user-status status-" + userStatus(lastSeen) + "'></span> "
                                        + chatRoom.UserName
                                        + "</div>"
                                        + "<span class='badge badge-info'>0</span>"
                                        + "<div class='time'>"
                                        + "<span id='lastSeen' class='lastseen grey'>" + lastSeen + "</span>"
                                        + "</div>"
                                    + "</a>"
                                    + "<input type='hidden'  value = '" + chatRoom.Name + "'/>"
                                    + "<input class='sender' type='hidden'  value = '" + chatRoom.id + "'/>"
                                    + "</li>"));
}

function userStatus(lastSeen) {
    if (lastSeen == "online") {
        return "online";
    }

    return "offline";
}

function userLastSeen(room) {

    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    var lastSeen = "";

    // get last seen time
    var time = new Date(room.LastSeen);
    //time.setHours(time.getHours() + 2);

    // get time now
    var timeNow = new Date();

    // get time difference
    var yearDif = timeNow.getFullYear() - time.getFullYear();
    var monthDif = timeNow.getMonth() - time.getMonth();
    var dateDif = timeNow.getDate() - time.getDate();
    var minuteDif = timeNow.getMinutes() - time.getMinutes();

    // if its today
    if (yearDif == 0 && monthDif == 0 && dateDif == 0) {
        if (minuteDif == 0) // user is online
        {
            return "online";
        }

        lastSeen = lastSeen + "today at " + getTime(time.getHours(), time.getMinutes());

    }
    else if (yearDif == 0 && monthDif == 0 && dateDif == 1) // its yeaterday
    {
        lastSeen = lastSeen + "yesterday at " + getTime(time.getHours(), time.getMinutes());
    }
    else if (yearDif == 0 && monthDif == 0 && dateDif > 1 && dateDif <= 7) {
        lastSeen = lastSeen + days[time.getDay()] + " at " + getTime(time.getHours(), time.getMinutes());
    }
    else if (yearDif == 0 && monthDif == 0 && dateDif > 7) {
        lastSeen = lastSeen + time.getDate() + " " + months[time.getMonth()] + " at " + getTime(time.getHours(), time.getMinutes());
    }
    else if (yearDif == 0 && monthDif > 0) {
        lastSeen = lastSeen + time.getDate() + " " + months[time.getMonth()];
    }
    else {
        lastSeen = lastSeen + time.getDate() + " " + months[time.getMonth()] + " " + time.getFullYear();
    }

    return lastSeen;
}

function getTime(hour, minute) {
    var time = "";

    // determine hour
    if (hour < 10) {
        time = "0"+hour+":";
    }
    else {
        time = hour + ":";
    }

    // determine minute
    if (minute < 10) {
        time = time + "0"+minute;
    }
    else {
        time = time + minute;
    }

    return time;
}

function updateUserListMessagesCount() {

    $(".badge-info").each(function () {

        if (!$(this).parent().parent().hasClass("active")) {
            var count = 0;

            var roomName = $(this).parent().next("input").attr("value");

            var room = getChatRoom(roomName);
            var sender = $(this).parent().next("input").next().attr("value");

            for (var i = 0; i < room.Messages.length; i++) {
                if (room.Messages[i].SenderID == sender && room.Messages[i].Read == 1) {
                    count = count + 1;
                }
            }

            $(this).html(count);
        }

    });
}

function addMessageToChatWindow(message) {

    var name = "Me";
    if (message.ReceiverID != message.SenderID) {
        name = message.SenderName;
    }

    $("#chatWindow").append("<div class='itemdiv dialogdiv'>"
                                + "<div class='body'>"
                                    + "<div class='time'>"
                                        + "<i class='icon-time'></i>"
                                        + "<span class='green'>"
                                        + message.Time
                                        + "</span>"
                                    + "</div>"
                                    + "<div class='name'>"
                                        + "<a href='/NoticeBoard/BusinessCard/" + message.SenderID + "' >"
                                        + name
                                        + "</a>"
                                    + "</div>"
                                    + "<div class='text'>"
                                        + message.Text
                                    + "</div>"
                                + "</div>"
                            + "</div>");

}

function clearChatRoomForChange(chatRoomName) {

    // check if in chat page
    var chatPage = $("#inChat").val();

    if (chatPage == "true") {
        $("#chatWindow").html("");

        currentChatRoomName = chatRoomName;

        var chatRoom = getChatRoom(chatRoomName);

        // add products to chat window header
        var productList = "Products: "
        for (var product = 0; product < chatRoom.Products.length; product++) {
            if (product == 0) {
                productList += "<a href='/NoticeBoard/Details?id="
                                + chatRoom.Products[product].ProductID + "'>"
                                + chatRoom.Products[product].Name + "</a>";
            }
            else {
                productList += ", <a href='/NoticeBoard/Details?id="
                                + chatRoom.Products[product].ProductID + "'>"
                                + chatRoom.Products[product].Name + "</a>";
            }
        }
        $("#roomProductList").html(productList);



        // add messages to chat window
        for (var message = 0; message < chatRoom.Messages.length; message++) {
            chatRoom.Messages[message].Read = 2;
            addMessageToChatWindow(chatRoom.Messages[message]);
        }
    }
    return false;
}

function getRoomIndex(roomName) {
    for (i = 0; i < currentChatRooms.length; i++) {
        if (currentChatRooms[i].Name == roomName) {
            return i;
        }
    }
    return -1;
}

function getCurrentDateTime() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();
    var h = today.getHours();
    var m = today.getMinutes();
    var daytime = "AM";

    if (h > 11) {
        h = h % 12;
        daytime = "PM";
    }
    if (h == 0) { h = 12 }
    if (m < 10) { m = "0" + m }

    return mm + "/" + dd + "/" + yyyy + " @ " + h + ":" + m + " " + daytime;

}

function scrollToBottom(slimObjectID) {
    var scrollTo_val = $(slimObjectID).prop('scrollHeight') + 'px';
    $(slimObjectID).slimScroll({ scrollTo: scrollTo_val });
}

function updateLastSeenStatus() {

    $(".lastseen").each(function () {

        var roomName = $(this).parent().parent().next("input").attr("value");

        var room = getChatRoom(roomName);

        var lastSeen = userLastSeen(room);

        var element = $(this).parent().parent().children("div[class='username']").children("span");

        if (lastSeen != "online") {
            lastSeen = "last seen " + lastSeen;

            if (element.hasClass("status-online")) {
                element.toggleClass("status-online");
                element.toggleClass("status-offline");
            }

            $(this).html(lastSeen);
            return;
        }

        if (element.hasClass("status-offline")) {
            element.toggleClass("status-offline");
            element.toggleClass("status-online");
        }

        $(this).html(lastSeen);


    });

}

$(function () {
    var chat = $.connection.chat;

    chat.client.addChatRoom = function (chatRoom) {
        addChatRoomToList({ Name: chatRoom.Name, UserName: chatRoom.OtherUserName, id: chatRoom.OtherUserID, LastSeen: chatRoom.LastSeen });
        currentChatRooms.push(chatRoom);

        if (currentChatRoomName == "" && currentChatRooms.length > 0) {
            currentChatRoomName = currentChatRooms[0].Name;
        }

        chat.server.join(chatRoom.Name);
        if (chatRoom.Name == currentChatRoomName) {
            var activeUser = $("#chatUserList").children("li[class='changeUser active']").first();
            clearChatRoomForChange(currentChatRoomName);
            //chat.server.updateReadMessages(myUserID, senderID);
        }
    }

    chat.client.addMessage = function (roomName, message) {
        var index = getRoomIndex(roomName);
        currentChatRooms[index].Messages.push(message);

        if (roomName == currentChatRoomName) {
            addMessageToChatWindow(message);
            // scroll down
            scrollToBottom('#chatWindow');
        }
        else {
            // show notification
            updateUserListMessagesCount();
            //updateMessagesCount();
            var count = Number($("#messageCount").html());
            count = count + 1;
            $("#messageCount").html(count);
        }

    }

    chat.client.updateLastSeen = function (room) {

        // get room and update last seen status
        var index = getRoomIndex(room.Name);
        currentChatRooms[index].LastSeen = room.LastSeen;

        updateLastSeenStatus();

    }

    chat.client.setUserDetails = function (user) {
        setCurrentUserDetails(user.Id, user.Name);
    }

    $.connection.hub.start().done(function () {

        chat.server.getUserDetails();

        window.setInterval(function () {

            // check if in chat page
            var chatPage = $("#inChat").val();

            if (chatPage == "true") {
                updateLastSeenStatus();
            }

        }, 60000);


        $(function () {
            updateMessagesCount();
            updateUserListMessagesCount();
            // scroll down
            scrollToBottom("#chatWindow");
        });

        $("#chatForm").submit(function () {
            var message = $("#messageTextBox").val();
            if (message != "") {
                $("#sendButton").click();
                // scroll down
                scrollToBottom('#chatWindow');
            }
            return false;
        })

        $("#sendButton").click(function () {
            var message = $("#messageTextBox").val();
            if (message != "") {
                if (currentChatRoomName != "") {
                    var index = getRoomIndex(currentChatRoomName);
                    var receiverID = currentChatRooms[index].OtherUserID;

                    // DateTime might conflict with the one created by the system
                    var m = {
                        SenderID: myUserID,
                        ReceiverID: myUserID,
                        SenderName: myUserName,
                        Text: message,
                        Time: getCurrentDateTime(),
                        Read: 1
                    };
                    addMessageToChatWindow(m);
                    m.ReceiverID = receiverID;
                    currentChatRooms[index].Messages.push(m);
                    chat.server.send(currentChatRoomName, message, myUserID, myUserName,0);
                }

                // scroll down
                scrollToBottom('#chatWindow');

                $("#messageTextBox").val("");
            }
        });

        $(".changeUser").click(function () {
            var roomName = $(this).parent().children("input").attr("value");
            var senderID = $(this).parent().children("input[class='sender']").attr("value");

            // activate current users
            $(this).parent().parent().children("li").removeClass("active");
            $(this).parent().addClass("active");

            // clear chat window
            clearChatRoomForChange(roomName);

            // update message status
            updateMessagesCount();
            $(this).children("span").html("0");
            chat.server.updateReadMessages(myUserID, senderID);

            // update scroll bar
            // by scroll down
            scrollToBottom('#chatWindow');

            return false;
        })

        $("#sendSaleRequest").click(function () {
            var productName = $("#saleProductName").attr("value");
            var productID = $("#saleProductID").attr("value");
            var supplierID = $("#saleProductOwnerID").attr("value");
            var requestMessage = $("#saleRequestMessage").val();

            if (requestMessage != "") {

                if (supplierID != myUserID) { // check if room already exist also
                    chat.server.createChatRoom("(Regarding product: "
                                    + productName + ") "
                                    + requestMessage, myUserID, myUserName, supplierID, productID).done(function () {

                                    });
                    $("#saleRequestForm").submit();
                }
                else {
                    // Something was wrong
                    requestMessage = $("#saleRequestMessage").val("You cannot buy from yourself");
                }
            }
            else {
                $("#requestError").html("Please enter a sale request messgae")
                return false;
            }
        });

    });

});

function updateMessagesCount() {
    var id = $("#uid").attr("value");
    var count = 0;
    for (var i = 0; i < currentChatRooms.length; i++) {
        for (var j = 0; j < currentChatRooms[i].Messages.length; j++) {
            if (currentChatRooms[i].Messages[j].Read == 1 && currentChatRooms[i].Messages[j].SenderID != id) {
                count = count + 1;
            }
        }
    }
    $("#messageCount").html(count);
}

//////////////////////////////////// CHAT CODE END   ////////////////////////