"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/MessagerieHub").build();
var connected = false;
//Disable send button until connection is established
document.getElementsByClassName("new").disabled = true;
timer();

connection.on("ReceiveSignal", (message) => {
    connection.invoke("SendMessageToGroup", message).catch(function (err) {
        return console.error(err.toString());
    });
});

connection.on("ReceiverMessage", function (message, dateMessage, dateEnvoi, imgAvatar) {
    if (parseInt($("#conversation").val()) === message.conversationId) {
        if ($(".messagerie-discussion-date-separator").last().text().trim() !== dateEnvoi) {
            $('.messagerie-discussion-content').append('<p class="messagerie-discussion-date-separator color-grey">' + dateEnvoi + '</p>');
        }

        var newMessage = "";
        if (message.text !== null) {
            newMessage = message.text;
        }

        var messagerieDiscussionContent = $('.messagerie-discussion-content');
        messagerieDiscussionContent.append(
            '<div class="messagerie-discussion-message">' +
                '<input type="hidden" value="' + message.id + '" />' +
                '<img src="' + imgAvatar + '" alt="" class="messagerie-contact-image" />' +
                '<div class="messagerie-contact-text">' +
                    '<div class="messagerie-contact-info">' +
                        '<p class="messagerie-contact-name">' +
                            '<b>' + message.utilisateur.prenom + ' ' + message.utilisateur.nom + '</b >' +
                        '</p>' +
                        '<p class="messagerie-contact-date color-grey">' + dateMessage + '</p>' +
                    '</div>' +
                    '<p class="messagerie-contact-message">' + newMessage + '</p>' +
                '</div>' +
            '</div>');

        if (message.files !== null) {
           
            var file = FormatDoc(message, "Receiver");
            var messagerieContactText = messagerieDiscussionContent.children().find('.messagerie-contact-text').last();
            messagerieContactText.append('<div class="messagerie-contact-attachment">' + file + '</div>');
        }
    }
});

connection.on("SenderMessage", function (message, dateMessage, dateEnvoi, imgAvatar) {
    if (parseInt($("#conversation").val()) === message.conversationId)
    {
        //$(".overlay").remove();
        if ($(".messagerie-discussion-date-separator").last().text().trim() !== dateEnvoi) {
            $('.messagerie-discussion-content').append('<p class="messagerie-discussion-date-separator color-grey">' + dateEnvoi + '</p>');
        }

        var newMessage = "";
        if (message.text !== null) {
            newMessage = message.text;
        }

        var messagerieDiscussionContent = $('.messagerie-discussion-content');
        messagerieDiscussionContent.append(
            '<div class="messagerie-discussion-message messagerie-discussion-message-right">' +
                '<input type="hidden" value="' + message.id + '" />' +
                '<img src="' + imgAvatar + '" alt="" class="messagerie-contact-image" />' +
                '<div class="messagerie-contact-text">' +
                    '<div class="messagerie-contact-info">' +
                        '<p class="messagerie-contact-name">' +
                            '<b>' + message.utilisateur.prenom + ' ' + message.utilisateur.nom + '</b >' +
                        '</p>' +
                        '<p class="messagerie-contact-date color-grey">' + dateMessage + '</p>' +
                    '</div>' +
                    '<p class="messagerie-contact-message">' + newMessage + '</p>' +
                '</div>' +
            '</div>');

        if (message.files !== null) {
            var file = FormatDoc(message, "Sender");

            var messagerieContactText = messagerieDiscussionContent.children().find('.messagerie-contact-text').last();
            messagerieContactText.append('<div class="messagerie-contact-attachment">' + file + '</div>');
        }
    }
});

connection.on("UpdateContactSender", function (message, dateLast, conversation, user, img) {
    var exist = false;
    var newMessage = "";
    if (message !== null) {
        newMessage = message;
    }

    $(".messagerie-contact").each(function () {
        var conversationId = parseInt($(this).find('input').val());
        if (conversationId === conversation) {
            exist = true;
            $(this).children().children().find('i').text(newMessage);
            $(this).children().children().find('.color-grey').text(dateLast);
        }
    });
    if (exist === false) {
        $('.mesagerie-contact-list').prepend(
            '<li class="messagerie-contact active">' +
                '<input type="hidden" value="' + conversation + '" />' +
                '<img src="' + img + '" alt="" class="messagerie-contact-image" />' +
                '<div class="messagerie-contact-text">' +
                    '<div class="messagerie-contact-info">' +
                        '<p class="messagerie-contact-name">' +
                           ' <b>'+ user.prenom + ' ' + user.nom + '</b>' +
                        '</p>' +
                        '<p class="color-grey">' + dateLast + '</p>' +
                    '</div>' + 
                    '<p class="messagerie-contact-message color-grey">' +
                        '<i>' + message + '</i>' +
                    '</p>' +
                '</div>' +
            '</li>');
    }
});

connection.on("UpdateContactReceiver", function (message, dateLast, img) {
    var exist = false;
    var newMessage = "";
    if (message.text !== null) {
        newMessage = message.text;
    }
    $(".messagerie-contact").each(function () {
        var conversationId = parseInt($(this).find('input').val());
        if (conversationId === message.conversationId) {
            exist = true;
            $(this).addClass('new-message');
            $(this).children().children().find('i').text(newMessage);
            $(this).children().children().find('.color-grey').text(dateLast);
        }
    });
    if (exist === false) {
        $('.mesagerie-contact-list').prepend(
            '<li class="messagerie-contact new-message">' +
                '<input type="hidden" value="' + message.conversationId + '" />' +
                '<img src="' + img + '" alt="" class="messagerie-contact-image" />' +
                '<div class="messagerie-contact-text">' +
                    '<div class="messagerie-contact-info">' +
                        '<p class="messagerie-contact-name">' +
                            ' <b>' + message.utilisateur.prenom + ' ' + message.utilisateur.nom + '</b>' +
                        '</p>' +
                        '<p class="color-grey">' + dateLast + '</p>' +
                    '</div>' +
                    '<p class="messagerie-contact-message color-grey">' +
                        '<i>' + message.text + '</i>' +
                    '</p>' +
                '</div>' +
            '</li>');
    }
});

connection.on("ReceiveUpdateContact", function (conversation) {
    $(".messagerie-contact").each(function () {
        var contact = $(this);
        var conversationId = parseInt(contact.find('input').val());
        $.each(conversation, function (index, value) {
            if (conversationId === value.conversationId) {
                if (value.utilisateur.presence === 1) {
                    contact.removeClass('contact-disconnected');
                    contact.removeClass('contact-away');
                    contact.addClass('contact-connected');
                }
                else if (value.utilisateur.presence === 2) {
                    contact.removeClass('contact-connected');
                    contact.removeClass('contact-away');
                    contact.addClass('contact-disconnected');
                }
                else {
                    contact.removeClass('contact-connected');
                    contact.removeClass('contact-disconnected');
                    contact.addClass('contact-away');
                }
            }
        });
    });
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveRemove", function (message) {
    if (parseInt($("#conversation").val()) === message.conversationId) {
        $('.messagerie-discussion-message').each(function () {
            var id = $(this).find('input').val();
            if (parseInt(id) === message.id) {
                $(this).find('.messagerie-contact-attachment').remove();
                $(this).find('.messagerie-contact-text').append('<p class="color-grey">pièce-jointe supprimée</p>');
            }
        });
    }
});

connection.on('displayNotificationMessage', () => {
    $.ajax({
        url: $("#notification-message").data('request-url'),
        method: "GET",
        success: function (result) {
            console.log(result.count);
            $(".notification-drop.messages").html(result.count);
            $(".notification-drop.notification-drop-big").html(result.count + "+");
        },
        error: function (error) {

        }
    });
});

connection.on('displayNotification', () => {
    $.ajax({
        url: $("#notification").data('request-url'),
        method: "GET",
        success: function (result) {
            if (result.role === "Fondateur") {
                $(".notification-drop.selection").html(result.count2);
                $(".notification-drop.candidatureR").html(result.count1);
            }
            else {
                $(".notification-drop.candidature").html(result.count2);
                $(".notification-drop.candidatureT").html(result.count1);
            }
        },
        error: function (error) {

        }
    });
});

function FormatDoc(message,user) {
    var file = '<div class="messagerie-contact-attachment-actions">' +
                    '<a href="'+ message.files +'" class="messagerie-contact-attachment-button" target="_blank" download>' +
                        '<i class="icon-download"></i>' +
                    '</a>';
    var extension = message.files.split('.').pop();
    if (extension === "jpg" || extension === "png") {
        file = file +
                    '<button class="messagerie-contact-attachment-button messagerie-attachment-reduce">' +
                        '<i class="icon-reduce"></i>' +
                    '</button>' +
                    '<button class="messagerie-contact-attachment-button messagerie-attachment-expand">' +
                        '<i class="icon-expand"></i>' +
                    '</button>';
                        
    }
    if (user === "Sender") {
        file = file +
            '<button class="messagerie-contact-attachment-button messagerie-attachment-close" data-message-id="' + message.id + '">' +
                '<i class="icon-close"></i>' +
            '</button>';
    }
    file = file + '</div>';

    if (extension === "jpg" || extension === "png" || extension === "bmp") {
        file = file + '<img src="' + message.files + '" class="messagerie-contact-attachment-preview" alt="" />';

    }
    else if (extension === "pdf") {
        file = file + '<img src="https://res.cloudinary.com/sanchez-consultant/image/upload/v1551086069/pdf.jpg" class="messagerie-contact-attachment-preview" alt="" />';
    }
    else {
        file = file + '<img src="https://res.cloudinary.com/sanchez-consultant/image/upload/v1551256889/document.png" class="messagerie-contact-attachment-preview" alt="" />';
    }

    return file;
}

function timer() {
    setTimeout(function () {
        connection.invoke("SendUpdateContact").catch(function (err) {
            return console.error(err.toString());
        });
        timer();
    }, 30000);
}
