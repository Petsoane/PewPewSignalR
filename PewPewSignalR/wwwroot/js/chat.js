// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var RecieveGroupMessages = true;
var OnlyMyReciever = false;
var reciever;

$(document).ready(async function () {
"use strict";
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var username = $('#Username').val();

    // Start connection
    connection.start().then(() => {
        connection.invoke("AddUser", username);
    });


    // Recieve a private message
    connection.on("PrivateReciever", (user, message)  =>{

        if (OnlyMyReciever) {
            if (reciever === user) {
                var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var encodedMsg = user + ": " + msg;

                $('#messagesList').append('<li class="alert alert-info>' + encodedMsg + '</li>');
                $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
            }
        }
        else {
                var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var encodedMsg = user + ": " + msg;

                $('#messagesList').append('<li class="list-group-item alert-primary">' + encodedMsg + '</li>');
                $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
        }
    });


    // This will add a username to the list of usernames on the page.
    connection.on("NewOnlineUser", (username) => {
        $('#onlineUsers').append('<li class="list-group-item">' + username + '</li>');
        $('#onlineUsers').scrollTop($('#onlineUsers').prop('scrollHeight'));
    });

    // Recive a group message.
    connection.on("ReceiveMessage", (user, message) => {
        if (RecieveGroupMessages && !OnlyMyReciever) {
            var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
            var encodedMsg = user + ": " + msg;

            $('#messagesList').append('<li class="list-group-item">' + encodedMsg + '</li>');
            $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
        }
    });

    var $messagecontent = $('#messageInput');
    $messagecontent.keyup(function (e) {
        //var user = document.getElementById("userInput").value;
        if (e.keyCode == 13) {

            var message = $messagecontent.val().trim();
            if (message.length == 0)
                return false;

            connection.invoke("Send", username, message).catch((err) => {
                return console.error(err.toString());
            });

            //    $('#messagesList').append('<li class="list-group-item text-muted">' + message + '</li>');
             //   $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));

            $messagecontent.val('');
        }
    });

    // Change the reciever the message sending.
    $messagecontent.keydown(function (e) {
        if (e.keyCode == 191) {
            var message = $messagecontent.val().trim()

            if (message.length ==  0)
                return false;

            connection.invoke("ChangeReciever", username, message).catch((err) => {
                return console.error(err.toString());
            });
            reciever = message;
            $messagecontent.val('');
            $messagecontent.addClass("border border-primary");
        }
    });

    // Toggle the recieve group messaages.
    $messagecontent.keydown(function (e) {
        if (e.key == "c" && e.ctrlKey)
            toggleRecieveMessages();
        else if (e.key == "x" && e.ctrlKey)
            toggleSilenceAllButPrivateOne();
    });

});

// This function is used to toggle the recieving of private messages
function toggleRecieveMessages() {
    RecieveGroupMessages = !RecieveGroupMessages; 
}

// This function is used to disable all the other user except the current users chat
function toggleSilenceAllButPrivateOne() {
    OnlyMyReciever = !OnlyMyReciever;
}
