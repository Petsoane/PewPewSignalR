// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
var RecieveGroupMessages = true;

$(document).ready(function () {
"use strict";
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = $('#Username').val();

    // Start connection
    connection.start().then(() => {
            connection.invoke("AddUser", user)
    });


    // Recieve a private message
    connection.on("PrivateReciever", (user, message) => {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + ": " + msg;

        $('#messagesList').append('<li class="list-group-item text-primary">' + encodedMsg + '</li>');
        $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
    });


    // Recive a group message.
    connection.on("ReceiveMessage", (user, message) => {
        if (RecieveGroupMessages) {
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

            connection.invoke("Send", user, message).catch((err) => {
                return console.error(err.toString());
            });
            $messagecontent.val('');
        }
    });

    // Change the reciever the message sending.
    $messagecontent.keydown(function (e) {
        if (e.keyCode == 191) {
            var message = $messagecontent.val().trim()

            if (message.length ==  0)
                return false;

            connection.invoke("ChangeReciever", user, message).catch((err) => {
                return console.error(err.toString());
            });
            $messagecontent.val('');
        }
    });

    // Toggle the recieve group messaages.
    $messagecontent.keydown(function (e) {
        if (e.key == "c" && e.ctrlKey)
            toggleRecieveMessages();
    });

});

// This function is used to toggle the recieving of private messages
function toggleRecieveMessages() {
    alert("Turning off group messages");
    RecieveGroupMessages = !RecieveGroupMessages; 
}

